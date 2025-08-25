using System;

namespace YagizEraslan.Gemini.Unity
{
    [Serializable]
    public class ChatCompletionResponse
    {
        public Candidate[] candidates;
        public UsageMetadata usageMetadata;
        public string modelVersion;
        
        [Serializable]
        public class Candidate
        {
            public Content content;
            public string finishReason;
            public int index;
            public SafetyRating[] safetyRatings;
        }
        
        [Serializable]
        public class Content
        {
            public Part[] parts;
            public string role;
        }
        
        [Serializable]
        public class Part
        {
            public string text;
        }
        
        [Serializable]
        public class UsageMetadata
        {
            public int promptTokenCount;
            public int candidatesTokenCount;
            public int totalTokenCount;
        }
        
        [Serializable]
        public class SafetyRating
        {
            public string category;
            public string probability;
        }

        // Compatibility properties for easier migration
        public Choice[] choices => candidates != null ? System.Array.ConvertAll(candidates, c => new Choice 
        {
            index = c.index,
            message = c.content?.parts?.Length > 0 ? new ChatMessage("assistant", c.content.parts[0].text) : new ChatMessage("assistant", ""),
            finish_reason = c.finishReason
        }) : new Choice[0];

        public Usage usage => usageMetadata != null ? new Usage
        {
            prompt_tokens = usageMetadata.promptTokenCount,
            completion_tokens = usageMetadata.candidatesTokenCount,
            total_tokens = usageMetadata.totalTokenCount
        } : new Usage();
        
        // Legacy compatibility classes
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
