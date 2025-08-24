namespace YagizEraslan.Gemini.Unity
{
    public enum GeminiModel
    {
        // Current Stable Models (2024-2025)
        Gemini_25_Pro,
        Gemini_25_Flash,
        Gemini_25_Flash_Lite,
        Gemini_20_Flash,
        Gemini_20_Flash_Lite,
        
        // Legacy Models (Deprecated - backward compatibility only)
        [System.Obsolete("Gemini 1.5 Pro is deprecated. Use Gemini_25_Pro instead.", false)]
        Gemini_15_Pro,
        [System.Obsolete("Gemini 1.5 Flash is deprecated. Use Gemini_25_Flash instead.", false)]
        Gemini_15_Flash,
        [System.Obsolete("Gemini 1.5 Flash 8B is deprecated. Use Gemini_25_Flash_Lite instead.", false)]
        Gemini_15_Flash_8B
    }

    public static class GeminiModelExtensions
    {
        public static string ToModelString(this GeminiModel model)
        {
            return model switch
            {
                // Current Stable Models
                GeminiModel.Gemini_25_Pro => "gemini-2.5-pro",
                GeminiModel.Gemini_25_Flash => "gemini-2.5-flash",
                GeminiModel.Gemini_25_Flash_Lite => "gemini-2.5-flash-lite",
                GeminiModel.Gemini_20_Flash => "gemini-2.0-flash",
                GeminiModel.Gemini_20_Flash_Lite => "gemini-2.0-flash-lite",
                
                // Legacy Models (Deprecated)
                GeminiModel.Gemini_15_Pro => "gemini-1.5-pro",
                GeminiModel.Gemini_15_Flash => "gemini-1.5-flash",
                GeminiModel.Gemini_15_Flash_8B => "gemini-1.5-flash-8b",
                
                // Default to latest stable model
                _ => "gemini-2.5-pro"
            };
        }

        public static GeminiModel FromString(string modelString)
        {
            return modelString switch
            {
                // Current Stable Models
                "gemini-2.5-pro" => GeminiModel.Gemini_25_Pro,
                "gemini-2.5-flash" => GeminiModel.Gemini_25_Flash,
                "gemini-2.5-flash-lite" => GeminiModel.Gemini_25_Flash_Lite,
                "gemini-2.0-flash" => GeminiModel.Gemini_20_Flash,
                "gemini-2.0-flash-lite" => GeminiModel.Gemini_20_Flash_Lite,
                
                // Legacy Models (Deprecated)
                "gemini-1.5-pro" => GeminiModel.Gemini_15_Pro,
                "gemini-1.5-flash" => GeminiModel.Gemini_15_Flash,
                "gemini-1.5-flash-8b" => GeminiModel.Gemini_15_Flash_8B,
                
                // Default to latest stable model
                _ => GeminiModel.Gemini_25_Pro
            };
        }

        /// <summary>
        /// Gets the display name for the model with deprecation warnings
        /// </summary>
        public static string GetDisplayName(this GeminiModel model)
        {
            return model switch
            {
                // Current Stable Models
                GeminiModel.Gemini_25_Pro => "Gemini 2.5 Pro (Latest)",
                GeminiModel.Gemini_25_Flash => "Gemini 2.5 Flash (Recommended)",
                GeminiModel.Gemini_25_Flash_Lite => "Gemini 2.5 Flash Lite (Fast)",
                GeminiModel.Gemini_20_Flash => "Gemini 2.0 Flash",
                GeminiModel.Gemini_20_Flash_Lite => "Gemini 2.0 Flash Lite",
                
                // Legacy Models (Deprecated)
                GeminiModel.Gemini_15_Pro => "Gemini 1.5 Pro (Deprecated)",
                GeminiModel.Gemini_15_Flash => "Gemini 1.5 Flash (Deprecated)",
                GeminiModel.Gemini_15_Flash_8B => "Gemini 1.5 Flash 8B (Deprecated)",
                
                _ => "Unknown Model"
            };
        }

        /// <summary>
        /// Checks if the model is deprecated
        /// </summary>
        public static bool IsDeprecated(this GeminiModel model)
        {
            return model switch
            {
                GeminiModel.Gemini_15_Pro => true,
                GeminiModel.Gemini_15_Flash => true,
                GeminiModel.Gemini_15_Flash_8B => true,
                _ => false
            };
        }

        /// <summary>
        /// Gets a recommended current model as replacement for deprecated models
        /// </summary>
        public static GeminiModel GetRecommendedReplacement(this GeminiModel model)
        {
            return model switch
            {
                GeminiModel.Gemini_15_Pro => GeminiModel.Gemini_25_Pro,
                GeminiModel.Gemini_15_Flash => GeminiModel.Gemini_25_Flash,
                GeminiModel.Gemini_15_Flash_8B => GeminiModel.Gemini_25_Flash_Lite,
                _ => model // Return the same model if not deprecated
            };
        }
    }
}
