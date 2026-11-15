using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.System.Events;
using SafeExamBrowser.SystemComponents.Contracts.Registry;

namespace SafeExamBrowser.Monitoring.System.Components;

internal class Cursors
{
	private static readonly string SYSTEM_PATH = Environment.ExpandEnvironmentVariables("%SystemRoot%") + "\\Cursors\\";

	private static readonly string USER_PATH = Environment.ExpandEnvironmentVariables("%LocalAppData%") + "\\Microsoft\\Windows\\Cursors\\";

	private readonly ILogger logger;

	private readonly IRegistry registry;

	[CompilerGenerated]
	private SentinelEventHandler m_CursorChanged;

	internal event SentinelEventHandler CursorChanged
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

	internal Cursors(ILogger logger, IRegistry registry)
	{
		this.logger = logger;
		this.registry = registry;
	}

	internal void StartMonitoring()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		registry.ValueChanged += Registry_ValueChanged;
		IEnumerable<string> enumerable = default;
		if (registry.TryGetNames("HKEY_CURRENT_USER\\Control Panel\\Cursors", ref enumerable))
		{
			foreach (string item in enumerable)
			{
				registry.StartMonitoring("HKEY_CURRENT_USER\\Control Panel\\Cursors", item);
			}
			logger.Info("Started monitoring cursors.");
		}
		else
		{
			logger.Warn("Failed to start monitoring cursor registry values!");
		}
	}

	internal void StopMonitoring()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		registry.ValueChanged -= Registry_ValueChanged;
		IEnumerable<string> enumerable = default;
		if (registry.TryGetNames("HKEY_CURRENT_USER\\Control Panel\\Cursors", ref enumerable))
		{
			foreach (string item in enumerable)
			{
				registry.StopMonitoring("HKEY_CURRENT_USER\\Control Panel\\Cursors", item);
			}
			logger.Info("Stopped monitoring cursors.");
		}
		else
		{
			logger.Warn("Failed to stop monitoring cursor registry values!");
		}
	}

	internal bool Verify()
	{
		logger.Info("Starting cursor verification...");
		IEnumerable<string> source = default;
		bool flag = registry.TryGetNames("HKEY_CURRENT_USER\\Control Panel\\Cursors", ref source);
		if (flag)
		{
			foreach (string item in source.Where((string c) => !string.IsNullOrWhiteSpace(c)))
			{
				flag &= VerifyCursor(item);
			}
			if (flag)
			{
				logger.Info("Cursor configuration successfully verified.");
			}
			else
			{
				logger.Warn("Cursor configuration is compromised!");
			}
		}
		else
		{
			logger.Error("Failed to verify cursor configuration!");
		}
		return flag;
	}

	private void Registry_ValueChanged(string key, string name, object oldValue, object newValue)
	{
		if (key == "HKEY_CURRENT_USER\\Control Panel\\Cursors")
		{
			HandleCursorChange(key, name, oldValue, newValue);
		}
	}

	private void HandleCursorChange(string key, string name, object oldValue, object newValue)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected Obj, but got Unknown
		SentinelEventArgs args = new SentinelEventArgs();
		logger.Warn($"The cursor registry value '{key}\\{name}' has changed from '{oldValue}' to '{newValue}'!");
		Task.Run(() =>
		{
			SentinelEventHandler val = this.m_CursorChanged;
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

	private bool VerifyCursor(string cursor)
	{
		object obj = default;
		int num = (int)(1u & (registry.TryRead("HKEY_CURRENT_USER\\Control Panel\\Cursors", cursor, ref obj) ? 1u : 0u)) & ((!(obj is string) || (obj is string text && (string.IsNullOrWhiteSpace(text) || IsValidCursorPath(text)))) ? 1 : 0);
		if (num == 0)
		{
			if (obj != null)
			{
				logger.Warn($"Configuration of cursor '{cursor}' is compromised: '{obj}'!");
				return (byte)num != 0;
			}
			logger.Warn("Failed to verify configuration of cursor '" + cursor + "'!");
		}
		return (byte)num != 0;
	}

	private bool IsValidCursorPath(string path)
	{
		if (!path.StartsWith(USER_PATH, StringComparison.OrdinalIgnoreCase))
		{
			return path.StartsWith(SYSTEM_PATH, StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}
}
