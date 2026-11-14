using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void PreKeyEventHandler(IWebBrowser webBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut, GenericEventArgs args);
