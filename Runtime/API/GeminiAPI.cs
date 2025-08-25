using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Gemini.Unity
{
    public class GeminiAPI : IGeminiAPI
    {
        private readonly GeminiSettings settings;
        private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/";

        public GeminiAPI(GeminiSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request)
        {
            string jsonResponse = null;
            
            try
            {
                jsonResponse = await CreateChatCompletionRaw(request);
                
                if (string.IsNullOrEmpty(jsonResponse))
                {
                    throw new Exception("Received empty response from Gemini API");
                }
                
                var response = JsonUtility.FromJson<ChatCompletionResponse>(jsonResponse);
                if (response == null)
                {
                    throw new Exception("Failed to deserialize API response");
                }
                
                return response;
            }
            catch (Exception e)
            {
                var errorContext = string.IsNullOrEmpty(jsonResponse) ? "No response data" : $"Response: {jsonResponse.Substring(0, Math.Min(200, jsonResponse.Length))}...";
                Debug.LogError($"Failed to parse Gemini API response: {e.Message}. {errorContext}");
                throw new Exception($"Failed to parse API response: {e.Message}");
            }
        }

        public async Task<string> CreateChatCompletionRaw(ChatCompletionRequest request)
        {
            ValidateRequest(request);

            var geminiRequest = ConvertToGeminiRequest(request);
            var jsonPayload = JsonUtility.ToJson(geminiRequest);
            
            var url = $"{BASE_URL}{request.model}:generateContent?key={settings.ApiKey}";
            
            using var www = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload)),
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = 30 // 30 second timeout
            };
            
            www.SetRequestHeader("Content-Type", "application/json");
            
            try
            {
                await www.SendWebRequest();
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    var errorResponse = www.downloadHandler?.text ?? "No error details available";
                    var httpCode = www.responseCode;
                    var errorMsg = $"Gemini API Error (HTTP {httpCode}): {www.error}";
                    
                    if (!string.IsNullOrEmpty(errorResponse))
                    {
                        errorMsg += $" - Response: {errorResponse}";
                    }
                    
                    Debug.LogError(errorMsg);
                    throw new Exception(GetUserFriendlyError(www.result, httpCode, errorResponse));
                }
                
                var responseText = www.downloadHandler.text;
                if (string.IsNullOrEmpty(responseText))
                {
                    throw new Exception("Received empty response from Gemini API");
                }
                
                return responseText;
            }
            catch (Exception e) when (!(e is System.Exception && e.Message.Contains("Gemini API Error")))
            {
                Debug.LogError($"Gemini API request failed: {e.Message}");
                throw new Exception($"Network error: {e.Message}");
            }
        }

        private void ValidateRequest(ChatCompletionRequest request)
        {
            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                throw new Exception("API key is not set in GeminiSettings");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrEmpty(request.model))
            {
                throw new Exception("Model is required for API request");
            }

            if (request.messages == null || request.messages.Length == 0)
            {
                throw new Exception("At least one message is required for API request");
            }
        }

        private string GetUserFriendlyError(UnityWebRequest.Result result, long httpCode, string errorResponse)
        {
            return result switch
            {
                UnityWebRequest.Result.ConnectionError => "Unable to connect to Gemini API. Please check your internet connection.",
                UnityWebRequest.Result.DataProcessingError => "Error processing response from Gemini API.",
                UnityWebRequest.Result.ProtocolError => httpCode switch
                {
                    400 => "Invalid request format.",
                    401 => "Invalid API key. Please check your Gemini API key.",
                    403 => "Access forbidden. Please verify your API key permissions.",
                    429 => "Rate limit exceeded. Please wait before making another request.",
                    500 => "Gemini API server error. Please try again later.",
                    _ => $"API error (HTTP {httpCode}): {errorResponse}"
                },
                _ => $"Unexpected error: {result}"
            };
        }

        private GeminiApiRequest ConvertToGeminiRequest(ChatCompletionRequest request)
        {
            var contents = new Content[request.messages.Length];
            
            for (int i = 0; i < request.messages.Length; i++)
            {
                var msg = request.messages[i];
                contents[i] = new Content
                {
                    role = msg.role == "assistant" ? "model" : "user",
                    parts = new Part[] { new Part { text = msg.content } }
                };
            }

            return new GeminiApiRequest
            {
                contents = contents,
                generationConfig = new GenerationConfig
                {
                    temperature = request.temperature,
                    maxOutputTokens = request.max_tokens
                }
            };
        }

        [Serializable]
        private class GeminiApiRequest
        {
            public Content[] contents;
            public GenerationConfig generationConfig;
        }

        [Serializable]
        private class Content
        {
            public string role;
            public Part[] parts;
        }

        [Serializable]
        private class Part
        {
            public string text;
        }

        [Serializable]
        private class GenerationConfig
        {
            public float temperature;
            public int maxOutputTokens;
        }
    }
}
