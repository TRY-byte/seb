using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp;
using SafeExamBrowser.Browser.Events;
using SafeExamBrowser.Browser.Wrapper;

namespace SafeExamBrowser.Browser.Handlers;

internal class DialogHandler : IDialogHandler
{
	internal event DialogRequestedEventHandler DialogRequested;

	public bool OnFileDialog(IWebBrowser webBrowser, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, IReadOnlyCollection<string> acceptFilters, IReadOnlyCollection<string> acceptExtensions, IReadOnlyCollection<string> acceptDescriptions, IFileDialogCallback callback)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DialogRequestedEventArgs args = new DialogRequestedEventArgs
		{
			Element = mode.ToElement(),
			InitialPath = defaultFilePath,
			Operation = mode.ToOperation(),
			Title = title
		};
		Task.Run(() =>
		{
			DialogRequested?.Invoke(args);
			IFileDialogCallback val = callback;
			try
			{
				if (args.Success)
				{
					callback.Continue(new List<string> { args.FullPath });
				}
				else
				{
					callback.Cancel();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		});
		return true;
	}
}
