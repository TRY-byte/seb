using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void ContextMenuDismissedEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame);
