using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Browser.Contracts;
using SafeExamBrowser.Browser.Contracts.Events;
using SafeExamBrowser.Client.Contracts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Server.Contracts.Data;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client.Responsibilities;

internal class BrowserResponsibility : ClientResponsibility
{
	private readonly ICoordinator coordinator;

	private readonly IMessageBox messageBox;

	private readonly IRuntimeProxy runtime;

	private readonly ISplashScreen splashScreen;

	private readonly ITaskbar taskbar;

	private IBrowserApplication Browser => Context.Browser;

	public BrowserResponsibility(ClientContext context, ICoordinator coordinator, ILogger logger, IMessageBox messageBox, IRuntimeProxy runtime, ISplashScreen splashScreen, ITaskbar taskbar)
		: base(context, logger)
	{
		this.coordinator = coordinator;
		this.messageBox = messageBox;
		this.runtime = runtime;
		this.splashScreen = splashScreen;
		this.taskbar = taskbar;
	}

	public override void Assume(ClientTask task)
	{
		switch (task)
		{
		case ClientTask.AutoStartApplications:
			AutoStartBrowser();
			break;
		case ClientTask.DeregisterEvents:
			DeregisterEvents();
			break;
		case ClientTask.RegisterEvents:
			RegisterEvents();
			break;
		}
	}

	private void AutoStartBrowser()
	{
		if (Settings.Browser.EnableBrowser && ((IApplication<IBrowserWindow>)(object)Browser).AutoStart)
		{
			Logger.Info("Auto-starting browser...");
			((IApplication<IBrowserWindow>)(object)Browser).Start();
		}
	}

	private void DeregisterEvents()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected Obj, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected Obj, but got Unknown
		if (Browser != null)
		{
			Browser.ConfigurationDownloadRequested -= Browser_ConfigurationDownloadRequested;
			Browser.LoseFocusRequested -= Browser_LoseFocusRequested;
			Browser.TerminationRequested -= Browser_TerminationRequested;
			Browser.UserIdentifierDetected -= Browser_UserIdentifierDetected;
		}
	}

	private void RegisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected Obj, but got Unknown
		Browser.ConfigurationDownloadRequested += Browser_ConfigurationDownloadRequested;
		Browser.LoseFocusRequested += Browser_LoseFocusRequested;
		Browser.TerminationRequested += Browser_TerminationRequested;
		Browser.UserIdentifierDetected += Browser_UserIdentifierDetected;
	}

	private void Browser_ConfigurationDownloadRequested(string fileName, DownloadEventArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected Obj, but got Unknown
		args.AllowDownload = false;
		if (IsAllowedToReconfigure(args.Url))
		{
			if (coordinator.RequestReconfigurationLock())
			{
				args.AllowDownload = true;
				args.Callback = Browser_ConfigurationDownloadFinished;
				args.DownloadPath = Path.Combine(Context.AppConfig.TemporaryDirectory, fileName);
				((IWindow)splashScreen).Show();
				((IWindow)splashScreen).BringToForeground();
				((IProgressIndicator)splashScreen).SetIndeterminate();
				((IProgressIndicator)splashScreen).UpdateStatus((TextKey)190, true);
				Logger.Info("Allowed download request for configuration file '" + fileName + "'.");
			}
			else
			{
				Logger.Warn("A reconfiguration is already in progress, denied download request for configuration file '" + fileName + "'!");
			}
		}
		else
		{
			Logger.Info("Reconfiguration is not allowed, denied download request for configuration file '" + fileName + "'.");
		}
	}

	private bool IsAllowedToReconfigure(string url)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		bool flag = !string.IsNullOrWhiteSpace(Settings.Security.QuitPasswordHash);
		bool flag2 = !string.IsNullOrWhiteSpace(Settings.Security.ReconfigurationUrl);
		if (flag)
		{
			if (flag2)
			{
				string text = Regex.Escape(Settings.Security.ReconfigurationUrl).Replace("\\*", ".*");
				Regex regex = new Regex("^" + text + "$", RegexOptions.IgnoreCase);
				string input = url.Replace(Uri.UriSchemeHttps, Context.AppConfig.SebUriSchemeSecure).Replace(Uri.UriSchemeHttp, Context.AppConfig.SebUriScheme);
				result = Settings.Security.AllowReconfiguration && (regex.IsMatch(url) || regex.IsMatch(input));
			}
			else
			{
				Logger.Warn("The active configuration does not contain a valid reconfiguration URL!");
			}
		}
		else
		{
			result = (int)Settings.ConfigurationMode == 0 || Settings.Security.AllowReconfiguration;
		}
		return result;
	}

	private void Browser_ConfigurationDownloadFinished(bool success, string url, string filePath = null)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (success)
		{
			PrepareShutdown();
			if (runtime.RequestReconfiguration(filePath, url).Success)
			{
				Logger.Info("Sent reconfiguration request for '" + filePath + "' to the runtime.");
				return;
			}
			Logger.Error("Failed to communicate reconfiguration request for '" + filePath + "'!");
			messageBox.Show((TextKey)136, (TextKey)137, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)splashScreen);
			((IWindow)splashScreen).Hide();
			coordinator.ReleaseReconfigurationLock();
		}
		else
		{
			Logger.Error("Failed to download configuration file '" + filePath + "'!");
			messageBox.Show((TextKey)104, (TextKey)105, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)splashScreen);
			((IWindow)splashScreen).Hide();
			coordinator.ReleaseReconfigurationLock();
		}
	}

	private void Browser_LoseFocusRequested(bool forward)
	{
		taskbar.Focus(forward);
	}

	private void Browser_UserIdentifierDetected(string identifier)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)Settings.SessionMode == 1)
		{
			ServerResponse val = Context.Server.SendUserIdentifier(identifier);
			while (!val.Success)
			{
				Logger.Error("Failed to communicate user identifier with server! " + val.Message);
				Thread.Sleep(Settings.Server.RequestAttemptInterval);
				val = Context.Server.SendUserIdentifier(identifier);
			}
		}
	}

	private void Browser_TerminationRequested()
	{
		Logger.Info("Attempting to shutdown as requested by the browser...");
		TryRequestShutdown();
	}
}
