using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace YagizEraslan.Gemini.Unity
{
    public class GeminiChat : MonoBehaviour
    {
        [Header("Gemini Configuration")]
        [SerializeField] private GeminiSettings config;
        [SerializeField] private GeminiModel modelType = GeminiModel.Gemini_25_Flash;
        [SerializeField] private bool useStreaming = true;

        [Header("UI Elements")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private RectTransform sentMessagePrefab;
        [SerializeField] private RectTransform receivedMessagePrefab;
        [SerializeField] private Transform messageContainer;

        [Header("Memory Management")]
        [SerializeField] private int maxUIMessages = 100;
        [SerializeField] private int trimToMessages = 70;

        private TMP_Text activeStreamingText;
        private readonly List<GameObject> messageGameObjects = new();

        private void Start()
        {
            sendButton.onClick.AddListener(SendMessage);

            // Allow Enter key to send message
            inputField.onSubmit.AddListener(text =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    SendMessage();
                }
            });
        }

        private string GetSelectedModelName()
        {
            return modelType.ToModelString();
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(inputField.text)) return;

            // Validate configuration
            if (config == null)
            {
                HandleError("GeminiSettings configuration is not assigned. Please assign a GeminiSettings asset in the inspector.");
                return;
            }

            // Add user message to UI first
            var userMessage = new ChatMessage("user", inputField.text);
            AddFullMessageToUI(userMessage, true);

            // Send message to Gemini API
            SendMessageToGemini(inputField.text);
            
            inputField.text = ""; // Clear input
            inputField.ActivateInputField(); // Focus input again
        }

        private async void SendMessageToGemini(string message)
        {
            try
            {
                var request = new ChatCompletionRequest
                {
                    model = GetSelectedModelName(),
                    messages = new[] { new ChatMessage("user", message) },
                    temperature = 0.7f,
                    max_tokens = 1000,
                    stream = useStreaming
                };

                if (useStreaming)
                {
                    // Prepare for streaming response
                    AddEmptyAssistantMessage();
                    
                    var streamingApi = new GeminiStreamingApi();
                    streamingApi.CreateChatCompletionStream(
                        request,
                        config.ApiKey, // API key parameter
                        OnStreamToken,  // onTokenReceived
                        OnStreamComplete, // onComplete
                        OnStreamError   // onError
                    );
                }
                else
                {
                    var geminiApi = new GeminiAPI(config); // Pass GeminiSettings
                    var response = await geminiApi.CreateChatCompletion(request);
                    
                    if (response != null && response.choices != null && response.choices.Length > 0)
                    {
                        var assistantMessage = new ChatMessage("assistant", response.choices[0].message.content);
                        AddFullMessageToUI(assistantMessage, false);
                    }
                }
            }
            catch (System.Exception e)
            {
                HandleError($"Failed to send message: {e.Message}");
            }
        }

        private void AddEmptyAssistantMessage()
        {
            var instance = Instantiate(receivedMessagePrefab, messageContainer);
            var textComponent = instance.GetComponentInChildren<TMP_Text>();
            
            messageGameObjects.Add(instance.gameObject);
            TrimUIMessagesIfNeeded();

            if (textComponent != null)
            {
                textComponent.text = "";
                activeStreamingText = textComponent;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)messageContainer);
        }

        private void OnStreamToken(string token)
        {
            Debug.Log($"[GeminiChat] OnStreamToken received: '{token}'");
            AppendStreamingCharacter(token);
        }

        private void OnStreamComplete()
        {
            activeStreamingText = null;
        }

        private void OnStreamError(string error)
        {
            HandleError(error);
            activeStreamingText = null;
        }

        private void AddFullMessageToUI(ChatMessage message, bool isUser)
        {
            var prefab = isUser ? sentMessagePrefab : receivedMessagePrefab;
            var instance = Instantiate(prefab, messageContainer);
            var textComponent = instance.GetComponentInChildren<TMP_Text>();

            // Add to tracking list
            messageGameObjects.Add(instance.gameObject);
            
            // Trim old UI messages if needed
            TrimUIMessagesIfNeeded();

            if (textComponent != null)
            {
                if (!isUser && useStreaming)
                {
                    textComponent.text = "";
                    activeStreamingText = textComponent;
                }
                else
                {
                    textComponent.text = message.content;
                    activeStreamingText = null;
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)messageContainer);
        }

        private void AppendStreamingCharacter(string partialContent)
        {
            Debug.Log($"[GeminiChat] AppendStreamingCharacter called with: '{partialContent}'");
            if (activeStreamingText != null)
            {
                activeStreamingText.text = partialContent;
                Debug.Log($"[GeminiChat] Updated UI text to: '{partialContent}'");
            }
            else
            {
                Debug.LogWarning("[UI] activeStreamingText is null — cannot update streaming content.");
            }
        }

        private void HandleError(string errorMessage)
        {
            Debug.LogError($"Gemini Chat Error: {errorMessage}");
            
            // Add error message to UI
            var errorMsg = new ChatMessage("assistant", $"❌ Error: {errorMessage}");
            AddFullMessageToUI(errorMsg, false);
        }

        private void OnStreamingComplete()
        {
            activeStreamingText = null;
        }

        private void TrimUIMessagesIfNeeded()
        {
            if (messageGameObjects.Count > maxUIMessages)
            {
                int messagesToRemove = messageGameObjects.Count - trimToMessages;
                
                for (int i = 0; i < messagesToRemove; i++)
                {
                    if (messageGameObjects[i] != null)
                    {
                        DestroyImmediate(messageGameObjects[i]);
                    }
                }
                
                messageGameObjects.RemoveRange(0, messagesToRemove);
                Debug.Log($"UI messages trimmed. Removed {messagesToRemove} old message GameObjects. Current count: {messageGameObjects.Count}");
            }
        }

        public void ClearChat()
        {
            // Clear UI messages using tracked GameObjects
            for (int i = 0; i < messageGameObjects.Count; i++)
            {
                if (messageGameObjects[i] != null)
                {
                    DestroyImmediate(messageGameObjects[i]);
                }
            }
            messageGameObjects.Clear();
            
            activeStreamingText = null;
        }

        private void OnDestroy()
        {
            // Clean up event listeners
            sendButton?.onClick.RemoveListener(SendMessage);
            if (inputField != null)
            {
                inputField.onSubmit.RemoveAllListeners();
            }
        }
    }
}
