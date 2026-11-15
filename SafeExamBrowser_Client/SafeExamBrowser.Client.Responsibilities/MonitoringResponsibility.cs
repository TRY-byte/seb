using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using SafeExamBrowser.Client.Contracts;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Applications;
using SafeExamBrowser.Monitoring.Contracts.Display;
using SafeExamBrowser.Monitoring.Contracts.System;
using SafeExamBrowser.Monitoring.Contracts.System.Events;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Client.Responsibilities;

internal class MonitoringResponsibility : ClientResponsibility
{
	private readonly IActionCenter actionCenter;

	private readonly IApplicationMonitor applicationMonitor;

	private readonly ICoordinator coordinator;

	private readonly IDisplayMonitor displayMonitor;

	private readonly IExplorerShell explorerShell;

	private readonly ISystemSentinel sentinel;

	private readonly ITaskbar taskbar;

	private readonly IText text;

	public MonitoringResponsibility(IActionCenter actionCenter, IApplicationMonitor applicationMonitor, ClientContext context, ICoordinator coordinator, IDisplayMonitor displayMonitor, IExplorerShell explorerShell, ILogger logger, ISystemSentinel sentinel, ITaskbar taskbar, IText text)
		: base(context, logger)
	{
		this.actionCenter = actionCenter;
		this.applicationMonitor = applicationMonitor;
		this.coordinator = coordinator;
		this.displayMonitor = displayMonitor;
		this.explorerShell = explorerShell;
		this.sentinel = sentinel;
		this.taskbar = taskbar;
		this.text = text;
	}

	public override void Assume(ClientTask task)
	{
		switch (task)
		{
		case ClientTask.DeregisterEvents:
			DeregisterEvents();
			break;
		case ClientTask.PrepareShutdown_Wave2:
			StopMonitoring();
			break;
		case ClientTask.RegisterEvents:
			RegisterEvents();
			break;
		case ClientTask.StartMonitoring:
			StartMonitoring();
			break;
		case ClientTask.PrepareShutdown_Wave1:
		case ClientTask.ScheduleIntegrityVerification:
		case ClientTask.ShowShell:
			break;
		}
	}

	private void DeregisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected Obj, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected Obj, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected Obj, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected Obj, but got Unknown
		applicationMonitor.ExplorerStarted -= ApplicationMonitor_ExplorerStarted;
		applicationMonitor.TerminationFailed -= ApplicationMonitor_TerminationFailed;
		displayMonitor.DisplayChanged -= DisplayMonitor_DisplaySettingsChanged;
		sentinel.CursorChanged -= Sentinel_CursorChanged;
		sentinel.EaseOfAccessChanged -= Sentinel_EaseOfAccessChanged;
		sentinel.SessionChanged -= Sentinel_SessionChanged;
		sentinel.StickyKeysChanged -= Sentinel_StickyKeysChanged;
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
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected Obj, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected Obj, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected Obj, but got Unknown
		applicationMonitor.ExplorerStarted += ApplicationMonitor_ExplorerStarted;
		applicationMonitor.TerminationFailed += ApplicationMonitor_TerminationFailed;
		displayMonitor.DisplayChanged += DisplayMonitor_DisplaySettingsChanged;
		sentinel.CursorChanged += Sentinel_CursorChanged;
		sentinel.EaseOfAccessChanged += Sentinel_EaseOfAccessChanged;
		sentinel.SessionChanged += Sentinel_SessionChanged;
		sentinel.StickyKeysChanged += Sentinel_StickyKeysChanged;
	}

	private void StartMonitoring()
	{
		sentinel.StartMonitoringSystemEvents();
		if (!Settings.Security.AllowStickyKeys)
		{
			sentinel.StartMonitoringStickyKeys();
		}
		if (Settings.Security.VerifyCursorConfiguration)
		{
			sentinel.StartMonitoringCursors();
		}
		if (Settings.Service.IgnoreService)
		{
			sentinel.StartMonitoringEaseOfAccess();
		}
	}

	private void StopMonitoring()
	{
		sentinel.StopMonitoring();
	}

	private void ApplicationMonitor_ExplorerStarted()
	{
		Logger.Info("Trying to terminate Windows explorer...");
		explorerShell.Terminate();
		Logger.Info("Re-initializing working area...");
		displayMonitor.InitializePrimaryDisplay(Settings.UserInterface.Taskbar.EnableTaskbar ? taskbar.GetAbsoluteHeight() : 0);
		Logger.Info("Re-initializing shell...");
		actionCenter.InitializeBounds();
		taskbar.InitializeBounds();
		Logger.Info("Desktop successfully restored.");
	}

	private void ApplicationMonitor_TerminationFailed(IEnumerable<RunningApplication> applications)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected Obj, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected Obj, but got Unknown
		string text = string.Join(Environment.NewLine, applications.Select((RunningApplication a) => "- " + a.Name));
		string message = this.text.Get((TextKey)57) + Environment.NewLine + Environment.NewLine + text;
		string title = this.text.Get((TextKey)73);
		LockScreenOption val = new LockScreenOption
		{
			Text = this.text.Get((TextKey)56)
		};
		LockScreenOption val2 = new LockScreenOption
		{
			Text = this.text.Get((TextKey)58)
		};
		Logger.Warn("Detected termination failure of blacklisted application(s)!");
		LockScreenResult val3 = ShowLockScreen(message, title, new LockScreenOption[2] { val, val2 });
		if (val3.OptionId == val.Id)
		{
			Logger.Info("The blacklisted application(s) " + string.Join(", ", applications.Select((RunningApplication a) => "'" + a.Name + "'")) + " will be temporarily allowed.");
		}
		else if (val3.OptionId == val2.Id)
		{
			Logger.Info("Attempting to shutdown as requested by the user...");
			TryRequestShutdown();
		}
	}

	private void DisplayMonitor_DisplaySettingsChanged()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected Obj, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected Obj, but got Unknown
		Logger.Info("Re-initializing working area...");
		displayMonitor.InitializePrimaryDisplay(Settings.UserInterface.Taskbar.EnableTaskbar ? taskbar.GetAbsoluteHeight() : 0);
		Logger.Info("Re-initializing shell...");
		actionCenter.InitializeBounds();
		ILockScreen lockScreen = Context.LockScreen;
		if (lockScreen != null)
		{
			lockScreen.InitializeBounds();
		}
		taskbar.InitializeBounds();
		Logger.Info("Desktop successfully restored.");
		bool isAllowed = displayMonitor.ValidateConfiguration(Settings.Display).IsAllowed;
		if (!isAllowed && coordinator.RequestSessionLock())
		{
			LockScreenOption val = new LockScreenOption
			{
				Text = text.Get((TextKey)62)
			};
			LockScreenOption val2 = new LockScreenOption
			{
				Text = text.Get((TextKey)64)
			};
			string message = text.Get((TextKey)63);
			string title = text.Get((TextKey)73);
			if (ShowLockScreen(message, title, new LockScreenOption[2] { val, val2 }).OptionId == val2.Id)
			{
				Logger.Info("Attempting to shutdown as requested by the user...");
				TryRequestShutdown();
			}
			coordinator.ReleaseSessionLock();
		}
		else if (!isAllowed)
		{
			Logger.Info("Display configuration is not allowed but lock screen is already active.");
		}
	}

	private void Sentinel_CursorChanged(SentinelEventArgs args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected Obj, but got Unknown
		if (coordinator.RequestSessionLock())
		{
			string message = text.Get((TextKey)60);
			string title = text.Get((TextKey)73);
			LockScreenOption val = new LockScreenOption
			{
				Text = text.Get((TextKey)59)
			};
			LockScreenOption val2 = new LockScreenOption
			{
				Text = text.Get((TextKey)61)
			};
			args.Allow = true;
			Logger.Info("Cursor changed! Attempting to show lock screen...");
			LockScreenResult val3 = ShowLockScreen(message, title, new LockScreenOption[2] { val, val2 });
			if (val3.OptionId == val.Id)
			{
				Logger.Info("The session will be allowed to resume as requested by the user...");
			}
			else if (val3.OptionId == val2.Id)
			{
				Logger.Info("Attempting to shutdown as requested by the user...");
				TryRequestShutdown();
			}
			coordinator.ReleaseSessionLock();
		}
		else
		{
			Logger.Info("Cursor changed but lock screen is already active.");
		}
	}

	private void Sentinel_EaseOfAccessChanged(SentinelEventArgs args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected Obj, but got Unknown
		if (coordinator.RequestSessionLock())
		{
			string message = text.Get((TextKey)66);
			string title = text.Get((TextKey)73);
			LockScreenOption val = new LockScreenOption
			{
				Text = text.Get((TextKey)65)
			};
			LockScreenOption val2 = new LockScreenOption
			{
				Text = text.Get((TextKey)67)
			};
			args.Allow = true;
			Logger.Info("Ease of access changed! Attempting to show lock screen...");
			LockScreenResult val3 = ShowLockScreen(message, title, new LockScreenOption[2] { val, val2 });
			if (val3.OptionId == val.Id)
			{
				Logger.Info("The session will be allowed to resume as requested by the user...");
			}
			else if (val3.OptionId == val2.Id)
			{
				Logger.Info("Attempting to shutdown as requested by the user...");
				TryRequestShutdown();
			}
			coordinator.ReleaseSessionLock();
		}
		else
		{
			Logger.Info("Ease of access changed but lock screen is already active.");
		}
	}

	private void Sentinel_SessionChanged(SessionSwitchReason reason)
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected Obj, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected Obj, but got Unknown
		bool flag = !Settings.Service.IgnoreService && (!Settings.Service.DisableUserLock || !Settings.Service.DisableUserSwitch);
		bool disableSessionChangeLockScreen = Settings.Security.DisableSessionChangeLockScreen;
		bool flag2 = Settings.Service.IgnoreService && (reason == SessionSwitchReason.SessionLock || reason == SessionSwitchReason.SessionUnlock);
		if (flag | disableSessionChangeLockScreen)
		{
			Logger.Info(string.Format("Detected user session change ({0}), but {1}.", reason, flag ? "session locking and/or switching is allowed" : "lock screen is disabled"));
			return;
		}
		if (flag2)
		{
			Logger.Info($"Ignoring user session change ({reason}).");
			return;
		}
		string message = text.Get((TextKey)(Settings.Service.IgnoreService ? 78 : 76));
		string title = text.Get((TextKey)73);
		LockScreenOption val = new LockScreenOption
		{
			Text = text.Get((TextKey)75)
		};
		LockScreenOption val2 = new LockScreenOption
		{
			Text = text.Get((TextKey)77)
		};
		Logger.Warn($"User session changed ({reason})! Attempting to show lock screen...");
		if (coordinator.RequestSessionLock())
		{
			LockScreenResult val3 = ShowLockScreen(message, title, new LockScreenOption[2] { val, val2 });
			if (val3.OptionId == val.Id)
			{
				Logger.Info("The session will be allowed to resume as requested by the user...");
			}
			else if (val3.OptionId == val2.Id)
			{
				Logger.Info("Attempting to shutdown as requested by the user...");
				TryRequestShutdown();
			}
			coordinator.ReleaseSessionLock();
		}
		else
		{
			Logger.Warn("User session changed but lock screen is already active.");
		}
	}

	private void Sentinel_StickyKeysChanged(SentinelEventArgs args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected Obj, but got Unknown
		if (coordinator.RequestSessionLock())
		{
			string message = text.Get((TextKey)70);
			string title = text.Get((TextKey)73);
			LockScreenOption val = new LockScreenOption
			{
				Text = text.Get((TextKey)71)
			};
			LockScreenOption val2 = new LockScreenOption
			{
				Text = text.Get((TextKey)72)
			};
			args.Allow = true;
			Logger.Info("Sticky keys changed! Attempting to show lock screen...");
			LockScreenResult val3 = ShowLockScreen(message, title, new LockScreenOption[2] { val, val2 });
			if (val3.OptionId == val.Id)
			{
				Logger.Info("The session will be allowed to resume as requested by the user...");
			}
			else if (val3.OptionId == val2.Id)
			{
				Logger.Info("Attempting to shutdown as requested by the user...");
				TryRequestShutdown();
			}
			coordinator.ReleaseSessionLock();
		}
		else
		{
			Logger.Info("Sticky keys changed but lock screen is already active.");
		}
	}
}
