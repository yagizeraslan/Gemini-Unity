using System;

namespace YagizEraslan.Gemini.Unity
{
    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
        
        public ChatMessage() { }
        
        public ChatMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}
