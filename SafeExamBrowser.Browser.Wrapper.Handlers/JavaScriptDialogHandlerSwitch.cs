using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;
using SafeExamBrowser.Browser.Wrapper.Events;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class JavaScriptDialogHandlerSwitch : IJsDialogHandler
{
	public bool OnBeforeUnloadDialog(IWebBrowser webBrowser, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
	{
		GenericEventArgs e = new GenericEventArgs
		{
			Value = false
		};
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnBeforeUnloadDialog(webBrowser, browser, message, isReload, callback, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnBeforeUnloadDialog(webBrowser, browser, message, isReload, callback, e);
		}
		return e.Value;
	}

	public void OnDialogClosed(IWebBrowser webBrowser, IBrowser browser)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnDialogClosed(webBrowser, browser);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnDialogClosed(webBrowser, browser);
		}
	}

	public bool OnJSDialog(IWebBrowser webBrowser, IBrowser browser, string originUrl, CefJsDialogType type, string message, string promptText, IJsDialogCallback callback, ref bool suppress)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		GenericEventArgs e = new GenericEventArgs
		{
			Value = false
		};
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnJavaScriptDialog(webBrowser, browser, originUrl, type, message, promptText, callback, ref suppress, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnJavaScriptDialog(webBrowser, browser, originUrl, type, message, promptText, callback, ref suppress, e);
		}
		return e.Value;
	}

	public void OnResetDialogState(IWebBrowser webBrowser, IBrowser browser)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnResetDialogState(webBrowser, browser);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnResetDialogState(webBrowser, browser);
		}
	}
}
