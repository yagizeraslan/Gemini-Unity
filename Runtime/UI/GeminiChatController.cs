using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace YagizEraslan.Gemini.Unity
{
    public class GeminiChatController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GeminiSettings settings;
        
        [Header("Chat Settings")]
        [SerializeField] private GeminiModel model = GeminiModel.Gemini_25_Pro;
        [SerializeField] private float temperature = 0.7f;
        [SerializeField] private int maxTokens = 1000;
        [SerializeField] private bool useStreaming = true;

        [Header("Memory Management")]
        [SerializeField] private int maxConversationHistory = 50;
        [SerializeField] private bool autoCleanupHistory = true;

        private List<ChatMessage> conversationHistory = new List<ChatMessage>();
        private GeminiAPI geminiApi;
        private GeminiStreamingApi streamingApi;
        private string currentStreamContent = "";
        private bool isProcessing = false;
        private UnityWebRequest activeStreamRequest;

        public event Action<string> OnMessageReceived;
        public event Action<string> OnStreamingUpdate;
        public event Action<string> OnError;
        public event Action OnStreamingComplete;

        private void Awake()
        {
            if (settings == null)
            {
                Debug.LogError("GeminiSettings not assigned to GeminiChatController!");
                return;
            }

            geminiApi = new GeminiAPI(settings);
            streamingApi = new GeminiStreamingApi();
        }

        public async Task<string> SendMessage(string userMessage)
        {
            if (isProcessing)
            {
                Debug.LogWarning("Already processing a message. Please wait.");
                return null;
            }

            if (string.IsNullOrEmpty(userMessage))
            {
                Debug.LogWarning("Cannot send empty message.");
                return null;
            }

            isProcessing = true;
            conversationHistory.Add(new ChatMessage("user", userMessage));

            try
            {
                var request = new ChatCompletionRequest
                {
                    model = model.ToModelString(),
                    messages = conversationHistory.ToArray(),
                    temperature = temperature,
                    max_tokens = maxTokens,
                    stream = useStreaming
                };

                if (useStreaming)
                {
                    return await HandleStreamingResponse(request);
                }
                else
                {
                    return await HandleNonStreamingResponse(request);
                }
            }
            catch (Exception e)
            {
                await HandleError(e, "Error sending message");
                return null;
            }
            finally
            {
                CleanupAfterRequest();
            }
        }

        private async Task<string> HandleNonStreamingResponse(ChatCompletionRequest request)
        {
            var response = await geminiApi.CreateChatCompletion(request);
            
            if (response?.choices != null && response.choices.Length > 0)
            {
                var assistantMessage = response.choices[0].message.content;
                conversationHistory.Add(new ChatMessage("assistant", assistantMessage));
                ManageConversationHistory();
                OnMessageReceived?.Invoke(assistantMessage);
                return assistantMessage;
            }

            throw new Exception("Invalid response from Gemini API");
        }

        private Task<string> HandleStreamingResponse(ChatCompletionRequest request)
        {
            var tcs = new TaskCompletionSource<string>();
            currentStreamContent = "";

            activeStreamRequest = streamingApi.CreateChatCompletionStream(
                request,
                settings.ApiKey,
                onTokenReceived: (token) =>
                {
                    currentStreamContent += token;
                    OnStreamingUpdate?.Invoke(token);
                },
                onComplete: () =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(currentStreamContent))
                        {
                            conversationHistory.Add(new ChatMessage("assistant", currentStreamContent));
                            ManageConversationHistory();
                            OnMessageReceived?.Invoke(currentStreamContent);
                            OnStreamingComplete?.Invoke();
                            tcs.SetResult(currentStreamContent);
                        }
                        else
                        {
                            tcs.SetException(new Exception("Received empty response from streaming"));
                        }
                    }
                    finally
                    {
                        activeStreamRequest = null;
                    }
                },
                onError: (error) =>
                {
                    try
                    {
                        OnError?.Invoke(error);
                        tcs.SetException(new Exception(error));
                    }
                    finally
                    {
                        activeStreamRequest = null;
                    }
                }
            );

            return tcs.Task;
        }

        public void ClearConversation()
        {
            StopActiveStream();
            conversationHistory.Clear();
            currentStreamContent = "";
            
            // Force garbage collection after clearing large conversation history
            if (autoCleanupHistory)
            {
                System.GC.Collect();
            }
        }

        public List<ChatMessage> GetConversationHistory()
        {
            return new List<ChatMessage>(conversationHistory);
        }

        private void ManageConversationHistory()
        {
            if (autoCleanupHistory && conversationHistory.Count > maxConversationHistory)
            {
                var messagesToRemove = conversationHistory.Count - maxConversationHistory;
                conversationHistory.RemoveRange(0, messagesToRemove);
                
                Debug.Log($"Cleaned up {messagesToRemove} old messages from conversation history");
            }
        }

        private async Task HandleError(Exception e, string context)
        {
            var errorMsg = $"{context}: {e.Message}";
            Debug.LogError(errorMsg);
            
            // Stop any active streaming
            StopActiveStream();
            
            // Clear current stream content on error
            currentStreamContent = "";
            
            // Add error message to conversation for user visibility
            conversationHistory.Add(new ChatMessage("assistant", $"❌ Error: {e.Message}"));
            ManageConversationHistory();
            
            OnError?.Invoke(errorMsg);
            
            // Small delay to allow UI to update before cleanup
            await Task.Yield();
        }

        private void CleanupAfterRequest()
        {
            isProcessing = false;
            
            // Clean up stream content if no longer needed
            if (string.IsNullOrEmpty(currentStreamContent) || !useStreaming)
            {
                currentStreamContent = "";
            }
        }

        private void StopActiveStream()
        {
            if (activeStreamRequest != null)
            {
                activeStreamRequest.Dispose();
                activeStreamRequest = null;
            }
        }

        public void SetModel(GeminiModel newModel)
        {
            model = newModel;
        }

        public void SetTemperature(float newTemperature)
        {
            temperature = Mathf.Clamp01(newTemperature);
        }

        public void SetMaxTokens(int newMaxTokens)
        {
            maxTokens = Mathf.Max(1, newMaxTokens);
        }

        public void SetUseStreaming(bool streaming)
        {
            if (!streaming && isProcessing && useStreaming)
            {
                StopActiveStream();
            }
            useStreaming = streaming;
        }

        private void OnDestroy()
        {
            StopActiveStream();
            streamingApi?.Dispose();
            
            // Clear events to prevent memory leaks
            OnMessageReceived = null;
            OnStreamingUpdate = null;
            OnError = null;
            OnStreamingComplete = null;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                StopActiveStream();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                StopActiveStream();
            }
        }

        public bool IsProcessing => isProcessing;
        public string CurrentStreamContent => currentStreamContent;
    }
}
