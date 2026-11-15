using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void BeforeBrowseEventHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect, GenericEventArgs args);
