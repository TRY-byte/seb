using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Win32;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.System;
using SafeExamBrowser.Monitoring.Contracts.System.Events;
using SafeExamBrowser.Monitoring.System.Components;
using SafeExamBrowser.SystemComponents.Contracts.Registry;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Monitoring.System;

public class SystemSentinel : ISystemSentinel
{
	private readonly Cursors cursors;

	private readonly EaseOfAccess easeOfAccess;

	private readonly StickyKeys stickyKeys;

	private readonly SafeExamBrowser.Monitoring.System.Components.SystemEvents systemEvents;

	[CompilerGenerated]
	private SentinelEventHandler m_CursorChanged;

	[CompilerGenerated]
	private SentinelEventHandler m_EaseOfAccessChanged;

	[CompilerGenerated]
	private SentinelEventHandler m_StickyKeysChanged;

	[CompilerGenerated]
	private SessionChangedEventHandler m_SessionChanged;

	public event SentinelEventHandler CursorChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_CursorChanged;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_CursorChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_CursorChanged;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_CursorChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event SentinelEventHandler EaseOfAccessChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_EaseOfAccessChanged;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_EaseOfAccessChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_EaseOfAccessChanged;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_EaseOfAccessChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event SentinelEventHandler StickyKeysChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_StickyKeysChanged;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_StickyKeysChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			SentinelEventHandler val = this.m_StickyKeysChanged;
			SentinelEventHandler val2;
			do
			{
				val2 = val;
				SentinelEventHandler value2 = (SentinelEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_StickyKeysChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event SessionChangedEventHandler SessionChanged
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

	public SystemSentinel(ILogger logger, INativeMethods nativeMethods, IRegistry registry)
	{
		cursors = new Cursors(logger, registry);
		easeOfAccess = new EaseOfAccess(logger, registry);
		stickyKeys = new StickyKeys(logger, nativeMethods);
		systemEvents = new SafeExamBrowser.Monitoring.System.Components.SystemEvents(logger);
	}

	public bool DisableStickyKeys()
	{
		return stickyKeys.Disable();
	}

	public bool EnableStickyKeys()
	{
		return stickyKeys.Enable();
	}

	public bool RevertStickyKeys()
	{
		return stickyKeys.Revert();
	}

	public void StartMonitoringCursors()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		cursors.CursorChanged += (SentinelEventArgs args) =>
		{
			SentinelEventHandler val = this.m_CursorChanged;
			if (val != null)
			{
				val.Invoke(args);
			}
		};
		cursors.StartMonitoring();
	}

	public void StartMonitoringEaseOfAccess()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		easeOfAccess.EaseOfAccessChanged += (SentinelEventArgs args) =>
		{
			SentinelEventHandler val = this.m_EaseOfAccessChanged;
			if (val != null)
			{
				val.Invoke(args);
			}
		};
		easeOfAccess.StartMonitoring();
	}

	public void StartMonitoringStickyKeys()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		stickyKeys.Changed += (SentinelEventArgs args) =>
		{
			SentinelEventHandler val = this.m_StickyKeysChanged;
			if (val != null)
			{
				val.Invoke(args);
			}
		};
		stickyKeys.StartMonitoring();
	}

	public void StartMonitoringSystemEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		systemEvents.SessionChanged += (SessionSwitchReason reason) =>
		{
			SessionChangedEventHandler val = this.m_SessionChanged;
			if (val != null)
			{
				val.Invoke(reason);
			}
		};
		systemEvents.StartMonitoring();
	}

	public void StopMonitoring()
	{
		cursors.StopMonitoring();
		easeOfAccess.StopMonitoring();
		stickyKeys.StopMonitoring();
		systemEvents.StopMonitoring();
	}

	public bool VerifyCursors()
	{
		return cursors.Verify();
	}

	public bool VerifyEaseOfAccess()
	{
		return easeOfAccess.Verify();
	}
}
