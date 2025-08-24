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
  <a href="https://github.com/yagizeraslan/Claude-Unity/commits/main">
    <img alt="Last Commit" src="https://img.shields.io/github/last-commit/yagizeraslan/Claude-Unity">
  </a>
  <a href="https://github.com/yagizeraslan/Claude-Unity">
    <img alt="Code Size" src="https://img.shields.io/github/languages/code-size/yagizeraslan/Claude-Unity">
  </a>
  <br/>
  
  <!-- Bottom bigger social + platform badges -->
  <img alt="Maintenance" src="https://img.shields.io/badge/Maintained-Yes-brightgreen">
  <a href="https://github.com/yagizeraslan/Claude-Unity/stargazers">
    <img alt="GitHub stars" src="https://img.shields.io/github/stars/yagizeraslan/Claude-Unity?style=social">
  </a>
  <a href="https://github.com/yagizeraslan/Claude-Unity/network/members">
    <img alt="GitHub forks" src="https://img.shields.io/github/forks/yagizeraslan/Claude-Unity?style=social">
  </a>
  <a href="https://github.com/yagizeraslan/Claude-Unity/issues">
    <img alt="GitHub issues" src="https://img.shields.io/github/issues/yagizeraslan/Claude-Unity?style=social">
  </a>
  <img alt="Windows" src="https://img.shields.io/badge/Platform-Windows-blue">
  <img alt="WebGL" src="https://img.shields.io/badge/Platform-WebGL-orange">
  <img alt="Android" src="https://img.shields.io/badge/Platform-Android-green">
</p>

# 🧠 Claude API for Unity

> 💬 A clean, modular Unity integration for Claude's powerful LLMs — chat, reasoning, and task automation made easy.
> 

⚠️ **Note**: This is an unofficial integration not affiliated with or endorsed by Claude.

---

## ✨ Features

- ✅ Clean, reusable SDK for Claude API
- 🔄 Supports true SSE-based streaming and non-streaming chat completions
- 🧠 Compatible with multiple models (Sonnet, Opus, Haiku)
- 🎨 Modular & customizable UI chat component
- 🔐 Secure API key storage (runtime-safe)
- ⚙️ Built with Unity Package Manager (UPM)
- 🧪 Includes sample scene & prefabs

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
- Claude API key from [console.anthropic.com](https://console.anthropic.com/)

---

## 📦 Installation

### Option 1: Via Git URL (Unity Package Manager)

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click `+` → **Add package from Git URL**
4. Paste:
    
    ```csharp
    https://github.com/yagizeraslan/Claude-Unity.git
    
    ```
    
5. ✅ Done

---

## 🚀 Getting Started

### 🔧 Setup

1. After installation, download Sample scene from Package Manager
2. Paste your **API key** into the ClaudeSettings.asset.
3. Hit Play — and chat with Claude AI in seconds 💬

---

## 🧪 Sample Scene

To test everything:

1. In **Package Manager**, under **Claude API for Unity**, click **Samples**
2. Click **Import** on `Claude Chat Example`
3. Open:
    
    ```csharp
    Assets/Samples/Claude API for Unity/1.0.0/Claude Chat Example/Scenes/Claude-Chat.unity

    ```
    
4. Press Play — you're live.

- You can change model type and streaming mode during play — the SDK picks up changes automatically for each new message.
- You can also press **Enter** instead of clicking Send button — handy for fast testing.

---

## 🔐 API Key Handling

- During dev: Store key via `EditorPrefs` using the Claude Editor Window
- In production builds: Use the `ClaudeSettings` ScriptableObject (recommended)

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
[SerializeField] private ClaudeSettings config;

private async void Start()
{
    var api = new ClaudeApi(config);
    var request = new ChatCompletionRequest
    {
        model = ClaudeModel.Claude_3_Sonnet.ToModelString(),
        messages = new ChatMessage[]
        {
            new ChatMessage { role = "user", content = "Tell me something cool." }
        }
    };

    var response = await api.CreateChatCompletion(request);
    Debug.Log("[Claude] " + response.content[0].text);
}

```

### 🔄 Streaming (Real-Time Updates)
```csharp
[SerializeField] private ClaudeSettings config;

private void Start()
{
    var request = new ChatCompletionRequest
    {
        model = ClaudeModel.Claude_3_Sonnet.ToModelString(),
        messages = new ChatMessage[]
        {
            new ChatMessage { role = "user", content = "Stream a fun fact about space." }
        },
        stream = true
    };

    var streamingApi = new ClaudeStreamingApi();
    streamingApi.CreateChatCompletionStream(
        request,
        config.apiKey,
        token => Debug.Log("[Claude Streaming] " + token)
    );
}

```

---

## 🛠 Advanced Usage

### 🔄 Streaming Support

Claude-Unity supports **real-time streaming** using Claude's official `stream: true` Server-Sent Events (SSE) endpoint.

✅ Uses Unity's `DownloadHandlerScript` for chunked response handling  
✅ UI updates per-token (no simulated typewriter effect)  
✅ No coroutines, no external libraries — works natively in Unity

To enable:
- Check `Use Streaming` in the chat prefab or component
- Partial responses will automatically stream into the UI

📌 You can toggle streaming on/off at runtime.

### 💬 Multiple Models

```csharp
ClaudeModel.Claude_3_Opus
ClaudeModel.Claude_3_Sonnet
ClaudeModel.Claude_3_Haiku

```

---

## 🐞 Troubleshooting

**Can't add component?**

→ Make sure you dragged `ClaudeSettings.asset` into the ClaudeChat.cs's Config field.

**Streaming not working?**

→ Make sure you're on a platform that supports `DownloadHandlerScript` (Standalone or Editor).  
→ WebGL and iOS may have platform limitations for live SSE streams.

**Seeing JSON parse warnings in streaming mode?**  

→ These are normal during SSE — they occur when the parser receives partial chunks. They're automatically skipped and won't affect the final output.

---

## 💖 Support This Project

If you find **Claude-Unity** useful, please consider supporting its development!

- [Become a sponsor on GitHub Sponsors](https://github.com/sponsors/yagizeraslan)
- [Buy me a coffee on Ko-fi](https://ko-fi.com/yagizeraslan)

Your support helps me continue maintaining and improving this project. Thank you! 🚀

---

## 📄 License

Unofficial Claude integration. Claude™ is a trademark of Anthropic PBC.

This project is licensed under the MIT License.

---

## 🤝 Contact & Support

**Author**: [Yağız ERASLAN](https://www.linkedin.com/in/yagizeraslan/)

📬 yagizeraslan@gmail.com

💬 GitHub Issues welcome!
