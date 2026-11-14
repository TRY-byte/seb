using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void OpenUrlFromTabEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture, GenericEventArgs args);
