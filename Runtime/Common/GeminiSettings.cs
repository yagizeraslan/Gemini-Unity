using UnityEngine;

namespace YagizEraslan.Gemini.Unity
{
    [CreateAssetMenu(fileName = "GeminiSettings", menuName = "Gemini/Settings")]
    public class GeminiSettings : ScriptableObject
    {
        [Header("API Configuration")]
        [SerializeField] private string apiKey = "";
        
        [Header("Default Parameters")]
        [SerializeField] private GeminiModel defaultModel = GeminiModel.Gemini_25_Pro;
        [SerializeField] private float defaultTemperature = 0.7f;
        [SerializeField] private int defaultMaxTokens = 1000;
        [SerializeField] private bool useStreaming = true;

        public string ApiKey 
        { 
            get => apiKey; 
            set => apiKey = value; 
        }
        
        public GeminiModel DefaultModel 
        { 
            get => defaultModel; 
            set => defaultModel = value; 
        }
        
        public float DefaultTemperature 
        { 
            get => defaultTemperature; 
            set => defaultTemperature = value; 
        }
        
        public int DefaultMaxTokens 
        { 
            get => defaultMaxTokens; 
            set => defaultMaxTokens = value; 
        }
        
        public bool UseStreaming 
        { 
            get => useStreaming; 
            set => useStreaming = value; 
        }
    }
}
