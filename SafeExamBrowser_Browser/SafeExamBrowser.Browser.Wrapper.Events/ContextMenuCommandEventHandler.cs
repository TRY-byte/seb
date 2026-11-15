using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void ContextMenuCommandEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags, GenericEventArgs args);
