using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void LoadingProgressChangedEventHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress);
