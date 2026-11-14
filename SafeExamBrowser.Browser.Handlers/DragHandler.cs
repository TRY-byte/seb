using System;
using System.Collections.Generic;
using CefSharp;
using CefSharp.Enums;

namespace SafeExamBrowser.Browser.Handlers;

internal class DragHandler : IDragHandler
{
	public bool OnDragEnter(IWebBrowser chromiumWebBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (dragData.IsFragment)
		{
			return !((Enum)mask).HasFlag((Enum)(object)(DragOperationsMask)16);
		}
		return true;
	}

	public void OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
	{
	}
}
