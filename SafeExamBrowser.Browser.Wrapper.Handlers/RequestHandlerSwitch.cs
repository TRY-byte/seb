using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;
using SafeExamBrowser.Browser.Wrapper.Events;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class RequestHandlerSwitch : RequestHandler
{
	protected override bool GetAuthCredentials(IWebBrowser webBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
	{
		GenericEventArgs e = new GenericEventArgs
		{
			Value = false
		};
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.GetAuthCredentials(webBrowser, browser, originUrl, isProxy, host, port, realm, scheme, callback, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.GetAuthCredentials(webBrowser, browser, originUrl, isProxy, host, port, realm, scheme, callback, e);
		}
		return e.Value;
	}

	protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
	{
		ResourceRequestEventArgs e = new ResourceRequestEventArgs();
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.GetResourceRequestHandler(chromiumWebBrowser, browser, frame, request, isNavigation, isDownload, requestInitiator, ref disableDefaultHandling, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.GetResourceRequestHandler(chromiumWebBrowser, browser, frame, request, isNavigation, isDownload, requestInitiator, ref disableDefaultHandling, e);
		}
		return e.Handler;
	}

	protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
	{
		GenericEventArgs e = new GenericEventArgs
		{
			Value = false
		};
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect, e);
		}
		return e.Value;
	}

	protected override bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		GenericEventArgs e = new GenericEventArgs
		{
			Value = false
		};
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnOpenUrlFromTab(chromiumWebBrowser, browser, frame, targetUrl, targetDisposition, userGesture, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnOpenUrlFromTab(chromiumWebBrowser, browser, frame, targetUrl, targetDisposition, userGesture, e);
		}
		return e.Value;
	}
}
