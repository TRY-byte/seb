using CefSharp;

namespace SafeExamBrowser.Browser.Handlers;

internal class FocusHandler : IFocusHandler
{
	internal FocusHandler()
	{
	}

	public void OnGotFocus(IWebBrowser webBrowser, IBrowser browser)
	{
	}

	public bool OnSetFocus(IWebBrowser webBrowser, IBrowser browser, CefFocusSource source)
	{
		return false;
	}

	public void OnTakeFocus(IWebBrowser webBrowser, IBrowser browser, bool next)
	{
	}
}
