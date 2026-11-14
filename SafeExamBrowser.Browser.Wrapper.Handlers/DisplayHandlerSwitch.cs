using System.Collections.Generic;
using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using CefSharp.WinForms.Handler;
using CefSharp.WinForms.Host;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class DisplayHandlerSwitch : DisplayHandler
{
	protected override void OnFaviconUrlChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<string> urls)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnFaviconUrlChange(chromiumWebBrowser, browser, urls);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnFaviconUrlChange(chromiumWebBrowser, browser, urls);
		}
		((DisplayHandler)this).OnFaviconUrlChange(chromiumWebBrowser, browser, urls);
	}

	protected override void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnLoadingProgressChange(chromiumWebBrowser, browser, progress);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnLoadingProgressChange(chromiumWebBrowser, browser, progress);
		}
		((DisplayHandler)this).OnLoadingProgressChange(chromiumWebBrowser, browser, progress);
	}
}
