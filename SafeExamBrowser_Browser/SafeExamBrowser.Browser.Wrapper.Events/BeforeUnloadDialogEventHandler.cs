using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void BeforeUnloadDialogEventHandler(IWebBrowser webBrowser, IBrowser browser, string message, bool isReload, IJsDialogCallback callback, GenericEventArgs args);
