using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class RenderProcessMessageHandlerSwitch : IRenderProcessMessageHandler
{
	public void OnContextCreated(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnContextCreated(chromiumWebBrowser, browser, frame);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnContextCreated(chromiumWebBrowser, browser, frame);
		}
	}

	public void OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnContextReleased(chromiumWebBrowser, browser, frame);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnContextReleased(chromiumWebBrowser, browser, frame);
		}
	}

	public void OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnFocusedNodeChanged(chromiumWebBrowser, browser, frame, node);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnFocusedNodeChanged(chromiumWebBrowser, browser, frame, node);
		}
	}

	public void OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, JavascriptException exception)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnUncaughtException(chromiumWebBrowser, browser, frame, exception);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnUncaughtException(chromiumWebBrowser, browser, frame, exception);
		}
	}
}
