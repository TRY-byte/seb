using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void RunContextMenuEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback, GenericEventArgs args);
