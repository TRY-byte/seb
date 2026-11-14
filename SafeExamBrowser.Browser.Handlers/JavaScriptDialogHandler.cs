using System;
using System.Threading.Tasks;
using CefSharp;
using SafeExamBrowser.Browser.Events;

namespace SafeExamBrowser.Browser.Handlers;

internal class JavaScriptDialogHandler : IJsDialogHandler
{
	internal event JavaScriptDialogRequestedEventHandler DialogRequested;

	public bool OnBeforeUnloadDialog(IWebBrowser webBrowser, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
	{
		JavaScriptDialogRequestedEventArgs args = new JavaScriptDialogRequestedEventArgs
		{
			Type = (isReload ? JavaScriptDialogType.Reload : JavaScriptDialogType.LeavePage)
		};
		Task.Run(() =>
		{
			DialogRequested?.Invoke(args);
			IJsDialogCallback val = callback;
			try
			{
				callback.Continue(args.Success);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		});
		return true;
	}

	public void OnDialogClosed(IWebBrowser webBrowser, IBrowser browser)
	{
	}

	public bool OnJSDialog(IWebBrowser webBrowser, IBrowser browser, string originUrl, CefJsDialogType type, string message, string promptText, IJsDialogCallback callback, ref bool suppress)
	{
		return false;
	}

	public void OnResetDialogState(IWebBrowser webBrowser, IBrowser browser)
	{
	}
}
