using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Applications;
using SafeExamBrowser.Settings.Applications;
using SafeExamBrowser.UserInterface.Contracts.FileSystemDialog;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client.Operations;

internal class ApplicationOperation : ClientOperation
{
	private readonly IApplicationFactory factory;

	private readonly IFileSystemDialog fileSystemDialog;

	private readonly ILogger logger;

	private readonly IMessageBox messageBox;

	private readonly IApplicationMonitor monitor;

	private readonly ISplashScreen splashScreen;

	private readonly IText text;

	[CompilerGenerated]
	private new StatusChangedEventHandler m_StatusChanged;

	public override event StatusChangedEventHandler StatusChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public ApplicationOperation(ClientContext context, IApplicationFactory factory, IFileSystemDialog fileSystemDialog, ILogger logger, IMessageBox messageBox, IApplicationMonitor monitor, ISplashScreen splashScreen, IText text)
		: base(context)
	{
		this.factory = factory;
		this.fileSystemDialog = fileSystemDialog;
		this.logger = logger;
		this.messageBox = messageBox;
		this.monitor = monitor;
		this.splashScreen = splashScreen;
		this.text = text;
	}

	public override OperationResult Perform()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		logger.Info("Initializing applications...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)180);
		}
		OperationResult val2 = InitializeApplications();
		if ((int)val2 == 3)
		{
			StartMonitor();
		}
		return val2;
	}

	public override OperationResult Revert()
	{
		logger.Info("Finalizing applications...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)175);
		}
		FinalizeApplications();
		StopMonitor();
		return (OperationResult)3;
	}

	private OperationResult InitializeApplications()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		InitializationResult val = monitor.Initialize(Context.Settings.Applications);
		OperationResult val2 = (OperationResult)3;
		if (val.FailedAutoTerminations.Any())
		{
			val2 = HandleAutoTerminationFailure(val.FailedAutoTerminations);
		}
		else if (val.RunningApplications.Any())
		{
			val2 = TryTerminate(val.RunningApplications);
		}
		if ((int)val2 == 3)
		{
			foreach (WhitelistApplication item in Context.Settings.Applications.Whitelist)
			{
				Initialize(item);
			}
		}
		return val2;
	}

	private void Initialize(WhitelistApplication settings)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		IApplication<IApplicationWindow> item = default;
		FactoryResult val = factory.TryCreate(settings, ref item);
		string customPath;
		while ((int)val == 2 && settings.AllowCustomPath && TryAskForApplicationPath(settings.DisplayName, settings.ExecutableName, out customPath))
		{
			settings.ExecutablePath = customPath;
			val = factory.TryCreate(settings, ref item);
		}
		if ((int)val == 3)
		{
			Context.Applications.Add(item);
			return;
		}
		logger.Error($"Failed to initialize application '{settings.DisplayName}' ({settings.ExecutableName}). Reason: {val}.");
		InformAboutFailedApplicationInitialization(settings.DisplayName, settings.ExecutableName, val);
	}

	private void FinalizeApplications()
	{
		foreach (IApplication<IApplicationWindow> application in Context.Applications)
		{
			application.Terminate();
		}
	}

	private OperationResult HandleAutoTerminationFailure(IList<RunningApplication> applications)
	{
		logger.Error(string.Format("{0} application(s) could not be automatically terminated: {1}", applications.Count, string.Join(", ", applications.Select((RunningApplication a) => a.Name))));
		InformAboutFailedApplicationTermination(applications);
		return (OperationResult)2;
	}

	private void StartMonitor()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Context.Settings.Security.KioskMode != 0)
		{
			monitor.Start();
		}
	}

	private void StopMonitor()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Context.Settings.Security.KioskMode != 0)
		{
			monitor.Stop();
		}
	}

	private OperationResult TryTerminate(IEnumerable<RunningApplication> applications)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		List<RunningApplication> list = new List<RunningApplication>();
		OperationResult result = (OperationResult)3;
		logger.Info("The following applications need to be terminated: " + string.Join(", ", applications.Select((RunningApplication a) => a.Name)) + ".");
		if (TryAskForAutomaticApplicationTermination(applications))
		{
			logger.Info("The user chose to automatically terminate all running applications.");
			foreach (RunningApplication application in applications)
			{
				if (monitor.TryTerminate(application))
				{
					logger.Info("Successfully terminated application '" + application.Name + "'.");
					continue;
				}
				result = (OperationResult)2;
				list.Add(application);
				logger.Error("Failed to automatically terminate application '" + application.Name + "'!");
			}
		}
		else
		{
			logger.Info("The user chose not to automatically terminate all running applications. Aborting...");
			result = (OperationResult)1;
		}
		if (list.Any())
		{
			InformAboutFailedApplicationTermination(list);
		}
		return result;
	}

	private void InformAboutFailedApplicationInitialization(string displayName, string executableName, FactoryResult result)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		TextKey val = (TextKey)87;
		TextKey val2 = (TextKey)88;
		if ((int)result == 2)
		{
			val = (TextKey)89;
			val2 = (TextKey)90;
		}
		string text = this.text.Get(val).Replace("%%NAME%%", "'" + displayName + "' (" + executableName + ")");
		string text2 = this.text.Get(val2);
		messageBox.Show(text, text2, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)splashScreen);
	}

	private void InformAboutFailedApplicationTermination(IEnumerable<RunningApplication> applications)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Join(Environment.NewLine, applications.Select((RunningApplication a) => a.Name));
		string text2 = this.text.Get((TextKey)91) + Environment.NewLine + Environment.NewLine + text;
		string text3 = this.text.Get((TextKey)92);
		messageBox.Show(text2, text3, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)splashScreen);
	}

	private bool TryAskForApplicationPath(string displayName, string executableName, out string customPath)
	{
		string text = this.text.Get((TextKey)54).Replace("%%NAME%%", displayName).Replace("%%EXECUTABLE%%", executableName);
		FileSystemDialogResult val = fileSystemDialog.Show((FileSystemElement)1, (FileSystemOperation)0, (string)null, text, (string)null, (IWindow)(object)splashScreen, false, true);
		customPath = val.FullPath;
		return val.Success;
	}

	private bool TryAskForAutomaticApplicationTermination(IEnumerable<RunningApplication> applications)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Invalid comparison between Unknown and I4
		string newLine = Environment.NewLine;
		string text = string.Join(Environment.NewLine, applications.Select((RunningApplication a) => a.Name));
		string text2 = this.text.Get((TextKey)82);
		string text3 = this.text.Get((TextKey)83) + newLine + newLine + text2 + newLine + newLine + text;
		string text4 = this.text.Get((TextKey)84);
		return (int)messageBox.Show(text3, text4, (MessageBoxAction)2, (MessageBoxIcon)2, (IWindow)(object)splashScreen) == 4;
	}
}
