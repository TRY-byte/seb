using System.Collections.Generic;
using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void DraggableRegionsChangedEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions);
