using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;
using SafeExamBrowser.Browser.Wrapper.Events;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class ContextMenuHandlerSwitch : IContextMenuHandler
{
	public void OnBeforeContextMenu(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
	{
		ICefSharpControl cefSharpControl = null;
		((!browser.IsPopup) ? ((ICefSharpControl)(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)) : ((ICefSharpControl)(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)))?.OnBeforeContextMenu(webBrowser, browser, frame, parameters, model);
	}

	public bool OnContextMenuCommand(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		GenericEventArgs e = new GenericEventArgs();
		ICefSharpControl cefSharpControl = null;
		((!browser.IsPopup) ? ((ICefSharpControl)(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)) : ((ICefSharpControl)(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)))?.OnContextMenuCommand(webBrowser, browser, frame, parameters, commandId, eventFlags, e);
		return e.Value;
	}

	public void OnContextMenuDismissed(IWebBrowser webBrowser, IBrowser browser, IFrame frame)
	{
		ICefSharpControl cefSharpControl = null;
		((!browser.IsPopup) ? ((ICefSharpControl)(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)) : ((ICefSharpControl)(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)))?.OnContextMenuDismissed(webBrowser, browser, frame);
	}

	public bool RunContextMenu(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
	{
		GenericEventArgs e = new GenericEventArgs();
		ICefSharpControl cefSharpControl = null;
		((!browser.IsPopup) ? ((ICefSharpControl)(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)) : ((ICefSharpControl)(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)))?.OnRunContextMenu(webBrowser, browser, frame, parameters, model, callback, e);
		return e.Value;
	}
}
