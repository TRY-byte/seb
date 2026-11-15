using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.System.Events;

namespace SafeExamBrowser.Monitoring.System.Components;

internal class SystemEvents
{
	private readonly ILogger logger;

	[CompilerGenerated]
	private SessionChangedEventHandler m_SessionChanged;

	internal event SessionChangedEventHandler SessionChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SessionChangedEventHandler val = this.m_SessionChanged;
			SessionChangedEventHandler val2;
			do
			{
				val2 = val;
				SessionChangedEventHandler value2 = (SessionChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_SessionChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SessionChangedEventHandler val = this.m_SessionChanged;
			SessionChangedEventHandler val2;
			do
			{
				val2 = val;
				SessionChangedEventHandler value2 = (SessionChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_SessionChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	internal SystemEvents(ILogger logger)
	{
		this.logger = logger;
	}

	internal void StartMonitoring()
	{
		Microsoft.Win32.SystemEvents.EventsThreadShutdown += SystemEvents_EventsThreadShutdown;
		Microsoft.Win32.SystemEvents.InstalledFontsChanged += SystemEvents_InstalledFontsChanged;
		Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
		Microsoft.Win32.SystemEvents.SessionEnded += SystemEvents_SessionEnded;
		Microsoft.Win32.SystemEvents.SessionEnding += SystemEvents_SessionEnding;
		Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionChanged;
		Microsoft.Win32.SystemEvents.TimeChanged += SystemEvents_TimeChanged;
		Microsoft.Win32.SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
		logger.Info("Started monitoring system events.");
	}

	internal void StopMonitoring()
	{
		Microsoft.Win32.SystemEvents.EventsThreadShutdown -= SystemEvents_EventsThreadShutdown;
		Microsoft.Win32.SystemEvents.InstalledFontsChanged -= SystemEvents_InstalledFontsChanged;
		Microsoft.Win32.SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
		Microsoft.Win32.SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
		Microsoft.Win32.SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
		Microsoft.Win32.SystemEvents.SessionSwitch -= SystemEvents_SessionChanged;
		Microsoft.Win32.SystemEvents.TimeChanged -= SystemEvents_TimeChanged;
		Microsoft.Win32.SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
		logger.Info("Stopped monitoring system events.");
	}

	private void SystemEvents_EventsThreadShutdown(object sender, EventArgs e)
	{
		logger.Warn("System event thread is about to be terminated!");
	}

	private void SystemEvents_InstalledFontsChanged(object sender, EventArgs e)
	{
		logger.Info("Installed fonts changed.");
	}

	private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
	{
		logger.Info($"Power mode changed: {e.Mode}.");
	}

	private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
	{
		logger.Warn($"User session ended! Reason: {e.Reason}.");
	}

	private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
	{
		logger.Warn($"User session is ending! Reason: {e.Reason}.");
	}

	private void SystemEvents_SessionChanged(object sender, SessionSwitchEventArgs e)
	{
		logger.Info($"User session change detected: {e.Reason}.");
		Task.Run(() =>
		{
			SessionChangedEventHandler val = this.m_SessionChanged;
			if (val != null)
			{
				val.Invoke(e.Reason);
			}
		});
	}

	private void SystemEvents_TimeChanged(object sender, EventArgs e)
	{
		logger.Info("Time changed.");
	}

	private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
	{
		logger.Info($"User preference changed. Category: {e.Category}.");
	}
}
