using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void SetFocusEventHandler(IWebBrowser webBrowser, IBrowser browser, CefFocusSource source, GenericEventArgs args);
