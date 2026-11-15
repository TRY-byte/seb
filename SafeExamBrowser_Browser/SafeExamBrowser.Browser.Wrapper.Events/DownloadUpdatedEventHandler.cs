using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void DownloadUpdatedEventHandler(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback);
