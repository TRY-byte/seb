using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.System.Events;
using SafeExamBrowser.SystemComponents.Contracts.Registry;

namespace SafeExamBrowser.Monitoring.System.Components;

internal class EaseOfAccess
{
	private readonly ILogger logger;

	private readonly IRegistry registry;

	[CompilerGenerated]
	private SentinelEventHandler m_EaseOfAccessChanged;

	internal event SentinelEventHandler EaseOfAccessChanged
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

	internal EaseOfAccess(ILogger logger, IRegistry registry)
	{
		this.logger = logger;
		this.registry = registry;
	}

	internal void StartMonitoring()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		registry.ValueChanged += Registry_ValueChanged;
		registry.StartMonitoring("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Utilman.exe", "Debugger");
		logger.Info("Started monitoring ease of access.");
	}

	internal void StopMonitoring()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		registry.ValueChanged -= Registry_ValueChanged;
		registry.StopMonitoring("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Utilman.exe", "Debugger");
		logger.Info("Stopped monitoring ease of access.");
	}

	internal bool Verify()
	{
		logger.Info("Starting ease of access verification...");
		object obj = default;
		bool flag = registry.TryRead("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Utilman.exe", "Debugger", ref obj);
		if (flag)
		{
			if (obj is string value && string.IsNullOrWhiteSpace(value))
			{
				logger.Info("Ease of access configuration successfully verified.");
			}
			else
			{
				logger.Warn($"Ease of access configuration is compromised: '{obj}'!");
				flag = false;
			}
		}
		else
		{
			flag = true;
			logger.Info("Ease of access configuration successfully verified (value does not exist).");
		}
		return flag;
	}

	private void Registry_ValueChanged(string key, string name, object oldValue, object newValue)
	{
		if (key == "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Utilman.exe")
		{
			HandleEaseOfAccessChange(key, name, oldValue, newValue);
		}
	}

	private void HandleEaseOfAccessChange(string key, string name, object oldValue, object newValue)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected Obj, but got Unknown
		SentinelEventArgs args = new SentinelEventArgs();
		logger.Warn($"The ease of access registry value '{key}\\{name}' has changed from '{oldValue}' to '{newValue}'!");
		Task.Run(() =>
		{
			SentinelEventHandler val = this.m_EaseOfAccessChanged;
			if (val != null)
			{
				val.Invoke(args);
			}
		}).ContinueWith((Task _) =>
		{
			if (args.Allow)
			{
				registry.StopMonitoring(key, name);
			}
		});
	}
}
