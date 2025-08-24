using System;

namespace YagizEraslan.Gemini.Unity
{
    [Serializable]
    public class ChatCompletionRequest
    {
        public string model;
        public ChatMessage[] messages;
        public float temperature = 0.7f;
        public int max_tokens = 1000;
        public bool stream = false;
        
        public ChatCompletionRequest() { }
    }
}
