using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;
using SafeExamBrowser.Browser.Wrapper.Events;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class FocusHandlerSwitch : IFocusHandler
{
	public void OnGotFocus(IWebBrowser webBrowser, IBrowser browser)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnGotFocus(webBrowser, browser);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnGotFocus(webBrowser, browser);
		}
	}

	public bool OnSetFocus(IWebBrowser webBrowser, IBrowser browser, CefFocusSource source)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		GenericEventArgs e = new GenericEventArgs
		{
			Value = false
		};
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnSetFocus(webBrowser, browser, source, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnSetFocus(webBrowser, browser, source, e);
		}
		return e.Value;
	}

	public void OnTakeFocus(IWebBrowser webBrowser, IBrowser browser, bool next)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnTakeFocus(webBrowser, browser, next);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnTakeFocus(webBrowser, browser, next);
		}
	}
}
