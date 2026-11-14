using CefSharp;

namespace SafeExamBrowser.Browser.Handlers;

internal class ContextMenuHandler : IContextMenuHandler
{
	public void OnBeforeContextMenu(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
	{
	}

	public bool OnContextMenuCommand(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
	{
		return false;
	}

	public void OnContextMenuDismissed(IWebBrowser webBrowser, IBrowser browser, IFrame frame)
	{
	}

	public bool RunContextMenu(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
	{
		return false;
	}
}
