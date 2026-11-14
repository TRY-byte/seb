using System.Collections.Generic;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class DialogHandlerSwitch : IDialogHandler
{
	public bool OnFileDialog(IWebBrowser webBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, IReadOnlyCollection<string> acceptFilters, IReadOnlyCollection<string> acceptExtensions, IReadOnlyCollection<string> acceptDescriptions, IFileDialogCallback callback)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnFileDialog(webBrowser, browser, mode, title, defaultFilePath, acceptFilters, acceptExtensions, acceptDescriptions, callback);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnFileDialog(webBrowser, browser, mode, title, defaultFilePath, acceptFilters, acceptExtensions, acceptDescriptions, callback);
		}
		return true;
	}
}
