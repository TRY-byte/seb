using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void AuthCredentialsEventHandler(IWebBrowser webBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback, GenericEventArgs args);
