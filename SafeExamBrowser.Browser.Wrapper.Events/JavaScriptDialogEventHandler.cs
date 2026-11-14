using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void JavaScriptDialogEventHandler(IWebBrowser webBrowser, IBrowser browser, string originUrl, CefJsDialogType type, string message, string promptText, IJsDialogCallback callback, ref bool suppress, GenericEventArgs args);
