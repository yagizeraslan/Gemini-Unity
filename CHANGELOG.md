# 📋 Changelog

All notable changes to **Gemini API for Unity** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [1.0.0] - 2024-08-24

### 🎉 Added
- **Initial Release**: Complete Gemini AI API integration for Unity projects
- **Current Gemini Model Support**: Full support for Google's latest Gemini model family
  - Gemini 2.5 Pro: Latest state-of-the-art thinking model for complex reasoning (Default)
  - Gemini 2.5 Flash: Best price-performance model for large scale processing (Recommended)
  - Gemini 2.5 Flash Lite: Lightweight model for fast, efficient responses
  - Gemini 2.0 Flash: Next-gen features with superior speed and 1M token context
  - Gemini 2.0 Flash Lite: Efficient version of 2.0 Flash
- **Legacy Model Support**: Backward compatibility with deprecated models
  - Gemini 1.5 Pro/Flash/Flash-8B: Marked as deprecated (unavailable for new projects April 2025)
- **Dual API Architecture**:
  - `GeminiAPI`: Non-streaming HTTP implementation with async/await patterns
  - `GeminiStreamingApi`: Real-time SSE streaming with `IDisposable` pattern
  - `IGeminiAPI`: Clean interface contract for both implementations
- **Advanced Memory Management System**:
  - Configurable conversation history limits via `GeminiSettings` (default: 50 messages)
  - Automatic trimming of oldest messages to prevent unbounded memory growth
  - StringBuilder buffer management with 64KB size limits
  - Proper resource disposal patterns throughout the codebase
- **Enhanced Resource Management**:
  - `IDisposable` implementation for `GeminiStreamingApi` and `GeminiChatController`
  - Automatic `UnityWebRequest` disposal with proper cleanup
  - Event handler cleanup in `OnDestroy()`, `OnApplicationPause()`, `OnApplicationFocus()`
  - Active request tracking and cancellation
- **Robust Error Handling**:
  - User-friendly error messages with HTTP status code interpretation
  - Request validation with comprehensive parameter checking
  - 30-second request timeout with graceful failure handling
  - Automatic cleanup on API failures with resource disposal
- **Unity Integration Features**:
  - UPM (Unity Package Manager) compatible structure
  - Runtime-safe API key storage using `GeminiSettings` ScriptableObject
  - Editor tools with `Gemini/Settings` menu for API key management
  - Complete sample scene with prefabs and demo UI
  - Support for Unity 2020.3+ (tested up to Unity 6.0)
- **Platform Support**:
  - Full support: Windows, Android
  - Partial support: WebGL (streaming limitations)
  - Expected to work: macOS, iOS, Linux (not yet tested)
- **Developer Experience**:
  - Comprehensive CLAUDE.md documentation
  - Clean namespace organization (`YagizEraslan.Gemini.Unity`)
  - Event-driven architecture with proper callbacks
  - Configurable parameters (temperature, max tokens, streaming toggle)

### 🛠 Technical Implementation
- **Streaming Architecture**:
  - Custom `StreamingDownloadHandler` with efficient SSE parsing
  - Real-time token processing without blocking main thread
  - Proper buffer management with automatic cleanup
  - Line-by-line processing to minimize memory allocations
- **API Integration**:
  - Native integration with Google's Gemini API endpoints
  - Proper request format conversion for Gemini's expected structure
  - Role mapping ("assistant" → "model", "user" → "user")
  - Support for both `generateContent` and `streamGenerateContent` endpoints
- **Memory Optimization**:
  - Eliminated memory churn during token streaming
  - Efficient conversation history management
  - Automatic garbage collection triggers (configurable)
  - Resource disposal in error scenarios

### 🔧 Architecture Decisions
- **Modular Design**: Separate API, Data, UI, Common, and Editor layers
- **Async/Await Pattern**: Native Unity integration with `UnityWebRequestAwaiter`
- **Event-Driven UI**: Decoupled architecture with proper event management
- **Security First**: API keys stored securely with EditorPrefs for development
- **Performance Focus**: Memory-efficient streaming with configurable limits

---