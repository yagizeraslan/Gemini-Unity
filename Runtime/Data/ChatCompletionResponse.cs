using System;

namespace YagizEraslan.Gemini.Unity
{
    [Serializable]
    public class ChatCompletionResponse
    {
        public string id;
        public string model;
        public Choice[] choices;
        public Usage usage;
        
        [Serializable]
        public class Choice
        {
            public int index;
            public ChatMessage message;
            public string finish_reason;
        }
        
        [Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }
}
