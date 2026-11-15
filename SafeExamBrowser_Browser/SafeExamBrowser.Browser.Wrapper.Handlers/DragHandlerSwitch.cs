using System.Collections.Generic;
using CefSharp;
using CefSharp.Enums;
using CefSharp.WinForms;
using CefSharp.WinForms.Host;
using SafeExamBrowser.Browser.Wrapper.Events;

namespace SafeExamBrowser.Browser.Wrapper.Handlers;

internal class DragHandlerSwitch : IDragHandler
{
	public bool OnDragEnter(IWebBrowser webBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		GenericEventArgs e = new GenericEventArgs();
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnDragEnter(webBrowser, browser, dragData, mask, e);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnDragEnter(webBrowser, browser, dragData, mask, e);
		}
		return e.Value;
	}

	public void OnDraggableRegionsChanged(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
	{
		if (browser.IsPopup)
		{
			(ChromiumHostControl.FromBrowser(browser) as CefSharpPopupControl)?.OnDraggableRegionsChanged(webBrowser, browser, frame, regions);
		}
		else
		{
			(ChromiumWebBrowser.FromBrowser(browser) as CefSharpBrowserControl)?.OnDraggableRegionsChanged(webBrowser, browser, frame, regions);
		}
	}
}
