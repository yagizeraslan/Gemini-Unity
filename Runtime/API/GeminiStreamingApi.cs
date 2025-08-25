using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Gemini.Unity
{
    public class GeminiStreamingApi : IDisposable
    {
        private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/";
        private UnityWebRequest activeRequest;
        private bool disposed = false;

        public UnityWebRequest CreateChatCompletionStream(ChatCompletionRequest request, string apiKey, Action<string> onTokenReceived, Action onComplete = null, Action<string> onError = null)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(GeminiStreamingApi));
            }

            DisposeActiveRequest();

            try
            {
                var geminiRequest = ConvertToGeminiRequest(request);
                var jsonPayload = JsonUtility.ToJson(geminiRequest);
                
                var url = $"{BASE_URL}{request.model}:streamGenerateContent?key={apiKey}";
                
                activeRequest = new UnityWebRequest(url, "POST")
                {
                    uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload)),
                    downloadHandler = new StreamingDownloadHandler(onTokenReceived, 
                        () => {
                            onComplete?.Invoke();
                            DisposeActiveRequest();
                        }, 
                        (error) => {
                            onError?.Invoke(error);
                            DisposeActiveRequest();
                        })
                };
                
                activeRequest.SetRequestHeader("Content-Type", "application/json");
                activeRequest.SendWebRequest();
                
                return activeRequest;
            }
            catch (Exception e)
            {
                DisposeActiveRequest();
                Debug.LogError($"Failed to create streaming request: {e.Message}");
                onError?.Invoke($"Failed to create streaming request: {e.Message}");
                throw;
            }
        }

        private void DisposeActiveRequest()
        {
            if (activeRequest != null)
            {
                activeRequest.Dispose();
                activeRequest = null;
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                DisposeActiveRequest();
                disposed = true;
            }
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

        private class StreamingDownloadHandler : DownloadHandlerScript, IDisposable
        {
            private readonly Action<string> onTokenReceived;
            private readonly Action onComplete;
            private readonly Action<string> onError;
            private readonly StringBuilder buffer = new StringBuilder(1024);
            private const int MAX_BUFFER_SIZE = 65536; // 64KB limit to prevent memory issues
            private bool isDisposed = false;

            public StreamingDownloadHandler(Action<string> onTokenReceived, Action onComplete, Action<string> onError)
            {
                this.onTokenReceived = onTokenReceived;
                this.onComplete = onComplete;
                this.onError = onError;
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (isDisposed)
                {
                    return false;
                }

                try
                {
                    // Prevent memory overflow by limiting buffer size
                    if (buffer.Length > MAX_BUFFER_SIZE)
                    {
                        Debug.LogWarning("Streaming buffer size exceeded limit, clearing buffer");
                        buffer.Clear();
                    }

                    var chunk = Encoding.UTF8.GetString(data, 0, dataLength);
                    buffer.Append(chunk);
                    
                    ProcessBuffer();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error processing streaming data: {e.Message}");
                    onError?.Invoke($"Streaming error: {e.Message}");
                    CleanupBuffer();
                    return false;
                }
            }

            private void ProcessBuffer()
            {
                if (isDisposed) return;

                var content = buffer.ToString();
                var lines = content.Split('\n');
                
                // Process all complete lines
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;
                    
                    ProcessSingleLine(line);
                }
                
                // Keep only the last incomplete line in buffer
                buffer.Clear();
                if (lines.Length > 0 && !string.IsNullOrEmpty(lines[lines.Length - 1]))
                {
                    buffer.Append(lines[lines.Length - 1]);
                }
            }

            private void ProcessSingleLine(string line)
            {
                if (!line.StartsWith("data: ")) return;

                var jsonData = line.Substring(6);
                if (jsonData == "[DONE]")
                {
                    CleanupBuffer();
                    onComplete?.Invoke();
                    return;
                }
                
                try
                {
                    var streamResponse = JsonUtility.FromJson<GeminiStreamResponse>(jsonData);
                    if (streamResponse?.candidates != null && streamResponse.candidates.Length > 0)
                    {
                        var candidate = streamResponse.candidates[0];
                        if (candidate?.content?.parts != null && candidate.content.parts.Length > 0)
                        {
                            var text = candidate.content.parts[0].text;
                            if (!string.IsNullOrEmpty(text))
                            {
                                onTokenReceived?.Invoke(text);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to parse streaming chunk: {e.Message}");
                }
            }

            private void CleanupBuffer()
            {
                buffer?.Clear();
            }

            protected override void CompleteContent()
            {
                CleanupBuffer();
                onComplete?.Invoke();
            }

            public new void Dispose()
            {
                if (!isDisposed)
                {
                    CleanupBuffer();
                    isDisposed = true;
                }
                base.Dispose();
            }
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

        [Serializable]
        private class GeminiStreamResponse
        {
            public Candidate[] candidates;
        }

        [Serializable]
        private class Candidate
        {
            public Content content;
            public string finishReason;
        }
    }
}
