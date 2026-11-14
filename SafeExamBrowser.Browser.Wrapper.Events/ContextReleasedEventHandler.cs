using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void ContextReleasedEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame);
