using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.System.Events;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Monitoring.System.Components;

internal class StickyKeys
{
	private readonly ILogger logger;

	private readonly INativeMethods nativeMethods;

	private readonly global::System.Timers.Timer timer;

	private IStickyKeysState original;

	[CompilerGenerated]
	private SentinelEventHandler m_Changed;

	internal event SentinelEventHandler Changed
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_Changed;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_Changed, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_Changed;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_Changed, value2, val2);
			}
			while (val != val2);
		}
	}

	internal StickyKeys(ILogger logger, INativeMethods nativeMethods)
	{
		this.logger = logger;
		this.nativeMethods = nativeMethods;
		timer = new global::System.Timers.Timer();
	}

	internal bool Disable()
	{
		IStickyKeysState val = default;
		bool flag = nativeMethods.TryGetStickyKeys(ref val);
		if (flag)
		{
			flag = nativeMethods.DisableStickyKeys();
			if (flag)
			{
				original = val;
				logger.Info("Disabled sticky keys (original state: " + ToString(val) + ").");
			}
			else
			{
				logger.Error("Failed to disable sticky keys (original state: " + ToString(val) + ")!");
			}
		}
		else
		{
			logger.Error("Failed to retrieve sticky keys state!");
		}
		return flag;
	}

	internal bool Enable()
	{
		IStickyKeysState val = default;
		bool flag = nativeMethods.TryGetStickyKeys(ref val);
		if (flag)
		{
			flag = nativeMethods.EnableStickyKeys();
			if (flag)
			{
				original = val;
				logger.Info("Enabled sticky keys (original state: " + ToString(val) + ").");
			}
			else
			{
				logger.Error("Failed to enable sticky keys (original state: " + ToString(val) + ")!");
			}
		}
		else
		{
			logger.Error("Failed to retrieve sticky keys state!");
		}
		return flag;
	}

	internal bool Revert()
	{
		bool flag = true;
		if (original != null)
		{
			flag = nativeMethods.TrySetStickyKeys(original);
			if (flag)
			{
				logger.Info("Successfully reverted sticky keys (original state: " + ToString(original) + ").");
			}
			else
			{
				logger.Error("Failed to revert sticky keys (original state: " + ToString(original) + ")!");
			}
		}
		original = null;
		return flag;
	}

	internal void StartMonitoring()
	{
		timer.AutoReset = true;
		timer.Elapsed += Timer_Elapsed;
		timer.Interval = 1000.0;
		timer.Start();
		logger.Info("Started monitoring sticky keys.");
	}

	internal void StopMonitoring()
	{
		timer.Elapsed -= Timer_Elapsed;
		timer.Stop();
		logger.Info("Stopped monitoring sticky keys.");
	}

	private void Timer_Elapsed(object sender, ElapsedEventArgs e)
	{
		IStickyKeysState val = default;
		if (nativeMethods.TryGetStickyKeys(ref val))
		{
			if (val.IsEnabled || val.IsHotkeyActive)
			{
				HandleStickyKeysChange(val);
			}
		}
		else
		{
			logger.Error("Failed to monitor sticky keys state!");
		}
	}

	private void HandleStickyKeysChange(IStickyKeysState state)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected Obj, but got Unknown
		SentinelEventArgs args = new SentinelEventArgs();
		logger.Warn("The sticky keys state has changed: " + ToString(state) + ".");
		Task.Run(() =>
		{
			SentinelEventHandler val = this.m_Changed;
			if (val != null)
			{
				val.Invoke(args);
			}
		}).ContinueWith((Task _) =>
		{
			if (args.Allow)
			{
				StopMonitoring();
			}
		});
		if (nativeMethods.DisableStickyKeys())
		{
			logger.Info("Disabled sticky keys.");
		}
		else
		{
			logger.Error("Failed to disable sticky keys!");
		}
	}

	private string ToString(IStickyKeysState state)
	{
		string text = (state.IsAvailable ? "available" : "unavailable");
		string text2 = (state.IsEnabled ? "enabled" : "disabled");
		string text3 = (state.IsHotkeyActive ? "active" : "inactive");
		return "functionality " + text + " and " + text2 + ", hotkey " + text3;
	}
}
