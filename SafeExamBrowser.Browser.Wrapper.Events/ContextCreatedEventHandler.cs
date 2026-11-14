using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void ContextCreatedEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame);
