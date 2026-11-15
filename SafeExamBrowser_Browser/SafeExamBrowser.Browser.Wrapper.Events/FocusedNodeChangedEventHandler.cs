using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void FocusedNodeChangedEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IDomNode node);
