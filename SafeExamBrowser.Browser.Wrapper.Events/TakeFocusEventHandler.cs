using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void TakeFocusEventHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, bool next);
