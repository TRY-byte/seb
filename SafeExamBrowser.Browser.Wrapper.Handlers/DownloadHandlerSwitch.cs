using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;
using SafeExamBrowser.Browser.Wrapper.Events;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class DownloadHandlerSwitch : IDownloadHandler
{
	public bool CanDownload(IWebBrowser webBrowser, IBrowser browser, string url, string requestMethod)
	{
		GenericEventArgs e = new GenericEventArgs
		{
			Value = false
		};
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnCanDownload(webBrowser, browser, url, requestMethod, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnCanDownload(webBrowser, browser, url, requestMethod, e);
		}
		return e.Value;
	}

	public bool OnBeforeDownload(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
	{
		GenericEventArgs e = new GenericEventArgs();
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnBeforeDownload(webBrowser, browser, downloadItem, callback, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnBeforeDownload(webBrowser, browser, downloadItem, callback, e);
		}
		return e.Value;
	}

	public void OnDownloadUpdated(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnDownloadUpdated(webBrowser, browser, downloadItem, callback);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnDownloadUpdated(webBrowser, browser, downloadItem, callback);
		}
	}
}
