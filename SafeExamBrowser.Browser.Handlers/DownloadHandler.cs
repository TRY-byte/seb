using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using SafeExamBrowser.Browser.Contracts.Events;
using SafeExamBrowser.Browser.Events;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings.Browser;
using SafeExamBrowser.UserInterface.Contracts.Browser.Data;
using Syroot.Windows.IO;

namespace SafeExamBrowser.Browser.Handlers;

internal class DownloadHandler : IDownloadHandler
{
	private readonly AppConfig appConfig;

	private readonly ConcurrentDictionary<int, DownloadFinishedCallback> callbacks;

	private readonly ConcurrentDictionary<int, Guid> downloads;

	private readonly ILogger logger;

	private readonly BrowserSettings settings;

	private readonly WindowSettings windowSettings;

	[CompilerGenerated]
	private DownloadRequestedEventHandler m_ConfigurationDownloadRequested;

	internal event DownloadRequestedEventHandler ConfigurationDownloadRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			DownloadRequestedEventHandler val = this.m_ConfigurationDownloadRequested;
			DownloadRequestedEventHandler val2;
			do
			{
				val2 = val;
				DownloadRequestedEventHandler value2 = (DownloadRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ConfigurationDownloadRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			DownloadRequestedEventHandler val = this.m_ConfigurationDownloadRequested;
			DownloadRequestedEventHandler val2;
			do
			{
				val2 = val;
				DownloadRequestedEventHandler value2 = (DownloadRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ConfigurationDownloadRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event DownloadAbortedEventHandler DownloadAborted;

	internal event DownloadUpdatedEventHandler DownloadUpdated;

	internal DownloadHandler(AppConfig appConfig, ILogger logger, BrowserSettings settings, WindowSettings windowSettings)
	{
		this.appConfig = appConfig;
		callbacks = new ConcurrentDictionary<int, DownloadFinishedCallback>();
		downloads = new ConcurrentDictionary<int, Guid>();
		this.logger = logger;
		this.settings = settings;
		this.windowSettings = windowSettings;
	}

	public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
	{
		return true;
	}

	public bool OnBeforeDownload(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		string extension = Path.GetExtension(downloadItem.SuggestedFileName);
		string text = downloadItem.Url;
		string b = null;
		if (downloadItem.Url.StartsWith("data:"))
		{
			text = ((downloadItem.Url.Length <= 100) ? downloadItem.Url : (downloadItem.Url.Substring(0, 100) + "..."));
		}
		if (Uri.TryCreate(downloadItem.Url, UriKind.RelativeOrAbsolute, out var result))
		{
			b = Path.GetExtension(result.AbsolutePath);
		}
		int num = (int)(0u | (string.Equals(appConfig.ConfigurationFileExtension, extension, StringComparison.OrdinalIgnoreCase) ? 1u : 0u) | (string.Equals(appConfig.ConfigurationFileExtension, b, StringComparison.OrdinalIgnoreCase) ? 1u : 0u)) | (string.Equals(appConfig.ConfigurationFileMimeType, downloadItem.MimeType, StringComparison.OrdinalIgnoreCase) ? 1 : 0);
		logger.Debug("Detected download request" + (UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" for '" + text + "'") : "") + ".");
		if (num != 0)
		{
			Task.Run(() =>
			{
				RequestConfigurationFileDownload(downloadItem, callback);
			});
		}
		else if (settings.AllowDownloads)
		{
			Task.Run(() =>
			{
				HandleFileDownload(downloadItem, callback);
			});
		}
		else
		{
			logger.Info("Aborted download request" + (UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" for '" + text + "'") : "") + ", as downloading is not allowed.");
			Task.Run(() =>
			{
				DownloadAborted?.Invoke();
			});
		}
		return true;
	}

	public void OnDownloadUpdated(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected Obj, but got Unknown
		bool flag = downloads.TryGetValue(downloadItem.Id, out var value);
		if (flag)
		{
			DownloadItemState state = new DownloadItemState(value)
			{
				Completion = (double)downloadItem.PercentComplete / 100.0,
				FullPath = downloadItem.FullPath,
				IsCancelled = downloadItem.IsCancelled,
				IsComplete = downloadItem.IsComplete,
				IsIndeterminate = (downloadItem.PercentComplete < 0),
				Size = downloadItem.ReceivedBytes,
				Url = downloadItem.Url
			};
			Task.Run(() =>
			{
				DownloadUpdated?.Invoke(state);
			});
		}
		if (!downloadItem.IsComplete && !downloadItem.IsCancelled)
		{
			return;
		}
		logger.Debug("Download of '" + downloadItem.FullPath + "' " + (downloadItem.IsComplete ? "is complete" : "was cancelled") + ".");
		if (callbacks.TryRemove(downloadItem.Id, out var finished) && finished != null)
		{
			Task.Run(() =>
			{
				finished.Invoke(downloadItem.IsComplete, downloadItem.Url, downloadItem.FullPath);
			});
		}
		if (flag)
		{
			downloads.TryRemove(downloadItem.Id, out var _);
		}
	}

	private void HandleFileDownload(DownloadItem downloadItem, IBeforeDownloadCallback callback)
	{
		string text = null;
		bool allowCustomDownAndUploadLocation = settings.AllowCustomDownAndUploadLocation;
		logger.Debug("Handling download of file '" + downloadItem.SuggestedFileName + "'.");
		text = (string.IsNullOrEmpty(settings.DownAndUploadDirectory) ? Path.Combine(KnownFolders.Downloads.ExpandedPath, downloadItem.SuggestedFileName) : Path.Combine(Environment.ExpandEnvironmentVariables(settings.DownAndUploadDirectory), downloadItem.SuggestedFileName));
		if (File.Exists(text))
		{
			text = AppendIndexSuffixTo(text);
		}
		if (allowCustomDownAndUploadLocation)
		{
			logger.Debug("Allowing user to select custom download location, with '" + text + "' as suggestion.");
		}
		else
		{
			logger.Debug("Automatically downloading file as '" + text + "'.");
		}
		downloads[downloadItem.Id] = Guid.NewGuid();
		try
		{
			callback.Continue(text, allowCustomDownAndUploadLocation);
		}
		finally
		{
			((IDisposable)callback)?.Dispose();
		}
	}

	private string AppendIndexSuffixTo(string filePath)
	{
		string directoryName = Path.GetDirectoryName(filePath);
		string extension = Path.GetExtension(filePath);
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
		string text = null;
		for (int i = 1; i < int.MaxValue; i++)
		{
			text = Path.Combine(directoryName, $"{fileNameWithoutExtension}({i}){extension}");
			if (!File.Exists(text))
			{
				break;
			}
		}
		return text;
	}

	private void RequestConfigurationFileDownload(DownloadItem downloadItem, IBeforeDownloadCallback callback)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		DownloadEventArgs e = new DownloadEventArgs
		{
			Url = downloadItem.Url
		};
		logger.Debug("Handling download of configuration file '" + downloadItem.SuggestedFileName + "'.");
		DownloadRequestedEventHandler val = this.m_ConfigurationDownloadRequested;
		if (val != null)
		{
			val.Invoke(downloadItem.SuggestedFileName, e);
		}
		if (e.AllowDownload)
		{
			if (e.Callback != null)
			{
				callbacks[downloadItem.Id] = e.Callback;
			}
			logger.Debug("Starting download of configuration file '" + downloadItem.SuggestedFileName + "'...");
			try
			{
				callback.Continue(e.DownloadPath, false);
				return;
			}
			finally
			{
				((IDisposable)callback)?.Dispose();
			}
		}
		logger.Debug("Download of configuration file '" + downloadItem.SuggestedFileName + "' was cancelled.");
	}
}
