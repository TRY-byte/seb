using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void CanDownloadEventHandler(IWebBrowser webBrowser, IBrowser browser, string url, string requestMethod, GenericEventArgs args);
