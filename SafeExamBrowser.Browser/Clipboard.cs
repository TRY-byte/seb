using System;
using System.Threading.Tasks;
using CefSharp;
using SafeExamBrowser.Browser.Events;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings.Browser;

namespace SafeExamBrowser.Browser;

internal class Clipboard
{
	private class Data
	{
		public string Content { get; set; }

		public string Id { get; set; }

		public string Type { get; set; }
	}

	private readonly ILogger logger;

	private readonly BrowserSettings settings;

	internal string Content { get; private set; }

	internal event ClipboardChangedEventHandler Changed;

	internal Clipboard(ILogger logger, BrowserSettings settings)
	{
		this.logger = logger;
		this.settings = settings;
	}

	internal void Update(JavascriptMessageReceivedEventArgs message)
	{
		if (!settings.UseIsolatedClipboard)
		{
			return;
		}
		try
		{
			Data data = message.ConvertMessageTo<Data>();
			if (data != null && data.Type == "Clipboard" && TrySetContent(data.Content))
			{
				Task.Run(() =>
				{
					Changed?.Invoke(data.Id);
				});
			}
		}
		catch (Exception ex)
		{
			logger.Error($"Failed to process browser message '{message}'!", ex);
		}
	}

	private bool TrySetContent(object value)
	{
		string text = value as string;
		if (text != null)
		{
			Content = text;
		}
		return text != null;
	}
}
