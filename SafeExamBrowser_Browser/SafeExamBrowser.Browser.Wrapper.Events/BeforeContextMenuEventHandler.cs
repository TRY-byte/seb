using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void BeforeContextMenuEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model);
