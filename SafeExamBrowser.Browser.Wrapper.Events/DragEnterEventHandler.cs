using CefSharp;
using CefSharp.Enums;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void DragEnterEventHandler(IWebBrowser webBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask, GenericEventArgs args);
