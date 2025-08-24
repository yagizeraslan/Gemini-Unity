using System.Threading.Tasks;

namespace YagizEraslan.Gemini.Unity
{
    public interface IGeminiAPI
    {
        Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request);
        Task<string> CreateChatCompletionRaw(ChatCompletionRequest request);
    }
}
