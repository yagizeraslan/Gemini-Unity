<p align="center">
  <!-- Top small clean badges -->
  <a href="https://unity.com/releases/editor/whats-new/2020.3.0">
    <img alt="Unity 2020.3+" src="https://img.shields.io/badge/Unity-2020.3%2B-green?logo=unity&logoColor=white">
  </a>
  <img alt="UPM Compatible" src="https://img.shields.io/badge/UPM-Compatible-brightgreen">
  <a href="https://opensource.org/licenses/MIT">
    <img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-blue.svg">
  </a>
  <a href="https://github.com/sponsors/yagizeraslan">
    <img alt="Sponsor" src="https://img.shields.io/badge/Sponsor-❤-ff69b4">
  </a>
  <a href="https://github.com/yagizeraslan/Gemini-Unity/commits/main">
    <img alt="Last Commit" src="https://img.shields.io/github/last-commit/yagizeraslan/Gemini-Unity">
  </a>
  <a href="https://github.com/yagizeraslan/Gemini-Unity">
    <img alt="Code Size" src="https://img.shields.io/github/languages/code-size/yagizeraslan/Gemini-Unity">
  </a>
  <br/>
  
  <!-- Bottom bigger social + platform badges -->
  <img alt="Maintenance" src="https://img.shields.io/badge/Maintained-Yes-brightgreen">
  <a href="https://github.com/yagizeraslan/Gemini-Unity/stargazers">
    <img alt="GitHub stars" src="https://img.shields.io/github/stars/yagizeraslan/Gemini-Unity?style=social">
  </a>
  <a href="https://github.com/yagizeraslan/Gemini-Unity/network/members">
    <img alt="GitHub forks" src="https://img.shields.io/github/forks/yagizeraslan/Gemini-Unity?style=social">
  </a>
  <a href="https://github.com/yagizeraslan/Gemini-Unity/issues">
    <img alt="GitHub issues" src="https://img.shields.io/github/issues/yagizeraslan/Gemini-Unity?style=social">
  </a>
  <img alt="Windows" src="https://img.shields.io/badge/Platform-Windows-blue">
  <img alt="WebGL" src="https://img.shields.io/badge/Platform-WebGL-orange">
  <img alt="Android" src="https://img.shields.io/badge/Platform-Android-green">
</p>

# 🤖 Gemini API for Unity

> 💬 A clean, modular Unity integration for Google Gemini's powerful AI models — chat, reasoning, and multimodal capabilities made easy.
> 

⚠️ **Note**: This is an unofficial integration not affiliated with or endorsed by Google.

---

## ✨ Features

- ✅ Clean, reusable SDK for Google Gemini API
- 🔄 Supports true SSE-based streaming and non-streaming chat completions
- 🧠 Compatible with latest Gemini models (2.5 Pro, 2.5 Flash, 2.0 Flash series)
- 🎨 Modular & customizable UI chat component with advanced memory management
- 🔐 Secure API key storage (runtime-safe)
- ⚙️ Built with Unity Package Manager (UPM)
- 🧪 Includes sample scene & prefabs
- 🛡️ Comprehensive error handling with user-friendly messages
- 🔧 Editor tools for easy API key management

---

### 🧩 Supported Platforms & Unity Versions

| Platform | Unity 2020.3 | Unity 2021 | Unity 2022 | Unity 6 | Notes |
| --- | --- | --- | --- | --- | --- |
| **Windows** | ✅ | ✅ | ✅ | ✅ | Fully supported (tested with IL2CPP & Mono) |
| **Android** | ✅ | ✅ | ✅ | ✅ | Requires internet permission in manifest |
| **WebGL** | ⚠️ *Partial* | ⚠️ *Partial* | ✅ | ✅ | Streaming unsupported; add CORS headers on server |
| **Linux** | ❓ | ❓ | ❓ | ❓ | Likely works, but not yet tested |
| **macOS** | ❓ | ❓ | ❓ | ❓ | Not tested, expected to work |
| **iOS** | ❓ | ❓ | ❓ | ❓ | Not tested, expected to work (HTTPS required) |
| **Consoles** | ❌ | ❌ | ❌ | ❌ | Not supported (Unity license + network limitations) |

> ❓ = Not tested yet — expected to work but needs verification
> 
> 
> ⚠️ = Partial support (some limitations)
>

---

## 🧰 Requirements

- Unity 2020.3 LTS or newer
- TextMeshPro (via Package Manager)
- Google Gemini API key from [Google AI Studio](https://aistudio.google.com/)

---

## 📦 Installation

### Option 1: Via Git URL (Unity Package Manager)

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click `+` → **Add package from Git URL**
4. Paste:
    
    ```csharp
    https://github.com/yagizeraslan/Gemini-Unity.git
    
    ```
    
5. ✅ Done

---

## 🚀 Getting Started

### 🔧 Setup

1. After installation, download Sample scene from Package Manager
2. Go to **Gemini/Settings** menu to configure your API key
3. Or create a **GeminiSettings.asset** and assign your API key
4. Hit Play — and chat with Gemini AI in seconds 💬

---

## 🧪 Sample Scene

To test everything:

1. In **Package Manager**, under **Gemini API for Unity**, click **Samples**
2. Click **Import** on `Gemini Chat Example`
3. Open:
    
    ```csharp
    Assets/Samples/Gemini API for Unity/1.0.0/Gemini Chat Example/Scenes/Gemini-Chat.unity

    ```
    
4. Press Play — you're live.

- You can change model type and streaming mode during play — the SDK picks up changes automatically for each new message.
- You can also press **Enter** instead of clicking Send button — handy for fast testing.

---

## 🔐 API Key Handling

- During dev: Store key via `EditorPrefs` using the **Gemini/Settings** editor window
- In production builds: Use the `GeminiSettings` ScriptableObject (recommended)

**DO NOT** hardcode your key in scripts or prefabs — Unity will reject the package.

---

## 🧱 Architecture Overview

| Layer | Folder | Role |
| --- | --- | --- |
| API Logic | `Runtime/API/` | HTTP & model logic |
| Data Models | `Runtime/Data/` | DTOs for requests/responses |
| UI Component | `Runtime/UI/` | MonoBehaviour & Controller |
| Config Logic | `Runtime/Common/` | Secure key storage |
| Editor Tools | `Editor/` | Editor-only settings UI |
| Example Scene | `Samples~/` | Demo prefab, scene, assets |

---

## 🧩 Example Integration

### 🕐 Non-Streaming (Full Response)

```csharp
[SerializeField] private GeminiSettings config;

private async void Start()
{
    var api = new GeminiAPI(config);
    var request = new ChatCompletionRequest
    {
        model = GeminiModel.Gemini_25_Pro.ToModelString(),
        messages = new ChatMessage[]
        {
            new ChatMessage { role = "user", content = "Tell me something cool about space." }
        }
    };

    var response = await api.CreateChatCompletion(request);
    Debug.Log("[Gemini] " + response.choices[0].message.content);
}

```

### 🔄 Streaming (Real-Time Updates)
```csharp
[SerializeField] private GeminiSettings config;

private void Start()
{
    var request = new ChatCompletionRequest
    {
        model = GeminiModel.Gemini_25_Flash.ToModelString(),
        messages = new ChatMessage[]
        {
            new ChatMessage { role = "user", content = "Stream a fun fact about AI." }
        },
        stream = true
    };

    var streamingApi = new GeminiStreamingApi();
    streamingApi.CreateChatCompletionStream(
        request,
        config.ApiKey,
        token => Debug.Log("[Gemini Streaming] " + token),
        () => Debug.Log("Streaming completed"),
        error => Debug.LogError("Error: " + error)
    );
}

```

---

## 🛠 Advanced Usage

### 🔄 Streaming Support

Gemini-Unity supports **real-time streaming** using Google's Gemini streaming API with Server-Sent Events (SSE).

✅ Uses Unity's `DownloadHandlerScript` for chunked response handling  
✅ UI updates per-token (no simulated typewriter effect)  
✅ No coroutines, no external libraries — works natively in Unity
✅ Advanced memory management with buffer limits and cleanup

To enable:
- Check `Use Streaming` in the chat prefab or component
- Partial responses will automatically stream into the UI

📌 You can toggle streaming on/off at runtime.

### 💬 Current Gemini Models

```csharp
// Latest Models (2024-2025)
GeminiModel.Gemini_25_Pro        // State-of-the-art (Default)
GeminiModel.Gemini_25_Flash      // Best price-performance (Recommended)
GeminiModel.Gemini_25_Flash_Lite // Lightweight and fast
GeminiModel.Gemini_20_Flash      // Advanced features
GeminiModel.Gemini_20_Flash_Lite // Efficient processing

// Legacy Models (Deprecated)
GeminiModel.Gemini_15_Pro        // Will be unavailable April 2025
GeminiModel.Gemini_15_Flash      // Will be unavailable April 2025
GeminiModel.Gemini_15_Flash_8B   // Will be unavailable April 2025

```

---

## 🐞 Troubleshooting

**Can't add component?**

→ Make sure you created `GeminiSettings.asset` and assigned it to the GeminiChatController's Settings field.

**Streaming not working?**

→ Make sure you're on a platform that supports `DownloadHandlerScript` (Standalone or Editor).  
→ WebGL may have platform limitations for live SSE streams.

**Seeing JSON parse warnings in streaming mode?**  

→ These are normal during SSE — they occur when the parser receives partial chunks. They're automatically handled and won't affect the final output.

**Getting API errors?**

→ Check your API key in **Gemini/Settings** menu
→ Ensure you have a valid Google Gemini API key from [Google AI Studio](https://aistudio.google.com/)

---

## 💖 Support This Project

If you find **Gemini-Unity** useful, please consider supporting its development!

- [Become a sponsor on GitHub Sponsors](https://github.com/sponsors/yagizeraslan)
- [Buy me a coffee on Ko-fi](https://ko-fi.com/yagizeraslan)

Your support helps me continue maintaining and improving this project. Thank you! 🚀

---

## 📄 License

Unofficial Google Gemini integration. Gemini™ is a trademark of Google LLC.

This project is licensed under the MIT License.

---

## 🤝 Contact & Support

**Author**: [Yağız ERASLAN](https://www.linkedin.com/in/yagizeraslan/)

📬 yagizeraslan@gmail.com

💬 GitHub Issues welcome!
