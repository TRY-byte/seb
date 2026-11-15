using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void KeyEventHandler(IWebBrowser webBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey);
