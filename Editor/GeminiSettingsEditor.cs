using UnityEngine;
using UnityEditor;

namespace YagizEraslan.Gemini.Unity
{
    public class GeminiSettingsEditor : EditorWindow
    {
        private const string API_KEY_PREF = "GeminiUnity_ApiKey";
        private const string TEMP_PREF = "GeminiUnity_Temperature";
        private const string TOKENS_PREF = "GeminiUnity_MaxTokens";
        private const string MODEL_PREF = "GeminiUnity_Model";
        private const string STREAMING_PREF = "GeminiUnity_UseStreaming";

        private string apiKey = "";
        private float temperature = 0.7f;
        private int maxTokens = 1000;
        private GeminiModel selectedModel = GeminiModel.Gemini_25_Pro;
        private bool useStreaming = true;

        [MenuItem("Gemini/Settings")]
        public static void ShowWindow()
        {
            GetWindow<GeminiSettingsEditor>("Gemini Settings");
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void OnGUI()
        {
            GUILayout.Label("Gemini API Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("API Configuration", EditorStyles.boldLabel);
            apiKey = EditorGUILayout.PasswordField("API Key:", apiKey);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Default Parameters", EditorStyles.boldLabel);
            
            selectedModel = (GeminiModel)EditorGUILayout.EnumPopup("Model:", selectedModel);
            
            // Show deprecation warning for legacy models
            if (selectedModel.IsDeprecated())
            {
                EditorGUILayout.HelpBox(
                    $"⚠️ {selectedModel.GetDisplayName()} is deprecated and will be unavailable for new projects starting April 29, 2025.\n" +
                    $"Recommended replacement: {selectedModel.GetRecommendedReplacement().GetDisplayName()}",
                    MessageType.Warning);
                
                if (GUILayout.Button($"Switch to {selectedModel.GetRecommendedReplacement().GetDisplayName()}"))
                {
                    selectedModel = selectedModel.GetRecommendedReplacement();
                }
            }
            
            temperature = EditorGUILayout.Slider("Temperature:", temperature, 0f, 1f);
            maxTokens = EditorGUILayout.IntSlider("Max Tokens:", maxTokens, 1, 4000);
            useStreaming = EditorGUILayout.Toggle("Use Streaming:", useStreaming);

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Settings"))
            {
                SaveSettings();
            }

            if (GUILayout.Button("Reset to Defaults"))
            {
                ResetToDefaults();
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "API Key is stored securely in EditorPrefs and will not be included in builds.\n" +
                "For runtime builds, create a GeminiSettings ScriptableObject asset instead.",
                MessageType.Info);

            if (GUILayout.Button("Create GeminiSettings Asset"))
            {
                CreateGeminiSettingsAsset();
            }
        }

        private void LoadSettings()
        {
            apiKey = EditorPrefs.GetString(API_KEY_PREF, "");
            temperature = EditorPrefs.GetFloat(TEMP_PREF, 0.7f);
            maxTokens = EditorPrefs.GetInt(TOKENS_PREF, 1000);
            selectedModel = (GeminiModel)EditorPrefs.GetInt(MODEL_PREF, 0);
            useStreaming = EditorPrefs.GetBool(STREAMING_PREF, true);
        }

        private void SaveSettings()
        {
            EditorPrefs.SetString(API_KEY_PREF, apiKey);
            EditorPrefs.SetFloat(TEMP_PREF, temperature);
            EditorPrefs.SetInt(TOKENS_PREF, maxTokens);
            EditorPrefs.SetInt(MODEL_PREF, (int)selectedModel);
            EditorPrefs.SetBool(STREAMING_PREF, useStreaming);

            Debug.Log("Gemini settings saved successfully!");
        }

        private void ResetToDefaults()
        {
            apiKey = "";
            temperature = 0.7f;
            maxTokens = 1000;
            selectedModel = GeminiModel.Gemini_25_Pro;
            useStreaming = true;
        }

        private void CreateGeminiSettingsAsset()
        {
            var asset = CreateInstance<GeminiSettings>();
            asset.ApiKey = apiKey;
            asset.DefaultModel = selectedModel;
            asset.DefaultTemperature = temperature;
            asset.DefaultMaxTokens = maxTokens;
            asset.UseStreaming = useStreaming;

            var path = EditorUtility.SaveFilePanelInProject(
                "Save GeminiSettings",
                "GeminiSettings",
                "asset",
                "Choose location for GeminiSettings asset");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;

                Debug.Log($"GeminiSettings asset created at: {path}");
            }
        }
    }
}
