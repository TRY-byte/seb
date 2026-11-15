using System.Collections.Generic;
using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal delegate void FaviconUrlChangedEventHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<string> urls);
