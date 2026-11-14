using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate bool BeforeDownloadEventHandler(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback, GenericEventArgs args);
