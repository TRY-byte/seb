using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void UncaughtExceptionEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, JavascriptException exception);
