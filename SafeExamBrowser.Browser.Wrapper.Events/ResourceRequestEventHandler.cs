using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void ResourceRequestEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling, ResourceRequestEventArgs args);
