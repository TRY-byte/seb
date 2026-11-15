using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Applications;
using SafeExamBrowser.Monitoring.Contracts.Applications.Events;
using SafeExamBrowser.Settings.Applications;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Monitoring.Applications;

public class ApplicationMonitor : IApplicationMonitor
{
	private readonly IList<BlacklistApplication> blacklist;

	private readonly ILogger logger;

	private readonly INativeMethods nativeMethods;

	private readonly IProcessFactory processFactory;

	private readonly global::System.Timers.Timer timer;

	private readonly IList<WhitelistApplication> whitelist;

	private Guid? captureHookId;

	private Guid? foregroundHookId;

	private IList<IProcess> processes;

	private Window activeWindow;

	[CompilerGenerated]
	private ExplorerStartedEventHandler m_ExplorerStarted;

	[CompilerGenerated]
	private InstanceStartedEventHandler m_InstanceStarted;

	[CompilerGenerated]
	private TerminationFailedEventHandler m_TerminationFailed;

	public event ExplorerStartedEventHandler ExplorerStarted
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ExplorerStartedEventHandler val = this.m_ExplorerStarted;
			ExplorerStartedEventHandler val2;
			do
			{
				val2 = val;
				ExplorerStartedEventHandler value2 = (ExplorerStartedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ExplorerStarted, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ExplorerStartedEventHandler val = this.m_ExplorerStarted;
			ExplorerStartedEventHandler val2;
			do
			{
				val2 = val;
				ExplorerStartedEventHandler value2 = (ExplorerStartedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ExplorerStarted, value2, val2);
			}
			while (val != val2);
		}
	}

	public event InstanceStartedEventHandler InstanceStarted
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			InstanceStartedEventHandler val = this.m_InstanceStarted;
			InstanceStartedEventHandler val2;
			do
			{
				val2 = val;
				InstanceStartedEventHandler value2 = (InstanceStartedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_InstanceStarted, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			InstanceStartedEventHandler val = this.m_InstanceStarted;
			InstanceStartedEventHandler val2;
			do
			{
				val2 = val;
				InstanceStartedEventHandler value2 = (InstanceStartedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_InstanceStarted, value2, val2);
			}
			while (val != val2);
		}
	}

	public event TerminationFailedEventHandler TerminationFailed
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TerminationFailedEventHandler val = this.m_TerminationFailed;
			TerminationFailedEventHandler val2;
			do
			{
				val2 = val;
				TerminationFailedEventHandler value2 = (TerminationFailedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TerminationFailed, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TerminationFailedEventHandler val = this.m_TerminationFailed;
			TerminationFailedEventHandler val2;
			do
			{
				val2 = val;
				TerminationFailedEventHandler value2 = (TerminationFailedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TerminationFailed, value2, val2);
			}
			while (val != val2);
		}
	}

	public ApplicationMonitor(int interval_ms, ILogger logger, INativeMethods nativeMethods, IProcessFactory processFactory)
	{
		blacklist = new List<BlacklistApplication>();
		this.logger = logger;
		this.nativeMethods = nativeMethods;
		processes = new List<IProcess>();
		this.processFactory = processFactory;
		timer = new global::System.Timers.Timer(interval_ms);
		whitelist = new List<WhitelistApplication>();
	}

	public InitializationResult Initialize(ApplicationSettings settings)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		InitializationResult val = new InitializationResult();
		InitializeProcesses();
		return val;
	}

	public void Start()
	{
		logger.Info("Started monitoring applications.");
	}

	public void Stop()
	{
		logger.Info("Stopped monitoring applications.");
	}

	public bool TryGetActiveApplication(out ActiveApplication application)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected Obj, but got Unknown
		application = null;
		if (activeWindow != null && TryGetProcessFor(activeWindow, out var process))
		{
			Window window = new Window
			{
				Handle = activeWindow.Handle,
				Title = nativeMethods.GetWindowTitle(activeWindow.Handle)
			};
			application = new ActiveApplication(process, (IWindow)(object)window);
		}
		return application != null;
	}

	public bool TryTerminate(RunningApplication application)
	{
		bool flag = true;
		foreach (IProcess process in application.Processes)
		{
			flag &= TryTerminate(process);
		}
		return flag;
	}

	private void SystemEvent_WindowChanged(IntPtr handle)
	{
		if (!(handle != IntPtr.Zero))
		{
			return;
		}
		Window window = activeWindow;
		if (window != null && !(window.Handle != handle))
		{
			return;
		}
		Window window2 = CreateWindowFor(handle);
		logger.Debug($"Window has changed from {activeWindow} to {window2}.");
		activeWindow = window2;
		Task.Run(() =>
		{
			if (!IsAllowed(window2) && !TryHide(window2))
			{
				Close(window2);
			}
		});
	}

	private void Timer_Elapsed(object sender, ElapsedEventArgs args)
	{
		MonitorProcesses();
		MonitorWindows();
		timer.Start();
	}

	private void AddFailed(IProcess process, List<RunningApplication> failed)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		string name = blacklist.First((BlacklistApplication a) => BelongsToApplication(process, a)).ExecutableName;
		RunningApplication val = failed.FirstOrDefault((RunningApplication a) => a.Name == name);
		if (val == null)
		{
			val = new RunningApplication(name);
			failed.Add(val);
		}
		val.Processes.Add(process);
	}

	private void AddFailed(string name, IProcess process, InitializationResult result)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected Obj, but got Unknown
		RunningApplication val = result.FailedAutoTerminations.FirstOrDefault((RunningApplication a) => a.Name == name);
		if (val == null)
		{
			val = new RunningApplication(name);
			result.FailedAutoTerminations.Add(val);
		}
		val.Processes.Add(process);
		logger.Error($"Process {process} belongs to application '{val.Name}' and could not be terminated automatically!");
	}

	private void AddForTermination(string name, IProcess process, InitializationResult result)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected Obj, but got Unknown
		RunningApplication val = result.RunningApplications.FirstOrDefault((RunningApplication a) => a.Name == name);
		if (val == null)
		{
			val = new RunningApplication(name);
			result.RunningApplications.Add(val);
		}
		val.Processes.Add(process);
		logger.Debug($"Process {process} belongs to application '{val.Name}' and needs to be terminated.");
	}

	private bool BelongsToApplication(IProcess process, BlacklistApplication application)
	{
		bool flag = process.Name.Equals(application.ExecutableName, StringComparison.OrdinalIgnoreCase);
		bool flag2 = process.OriginalName?.Equals(application.OriginalName, StringComparison.OrdinalIgnoreCase) ?? false;
		return flag | flag2;
	}

	private bool BelongsToApplication(IProcess process, WhitelistApplication application)
	{
		bool flag = string.IsNullOrWhiteSpace(application.OriginalName);
		bool flag2 = string.IsNullOrWhiteSpace(application.Signature);
		bool flag3 = process.Name.Equals(application.ExecutableName, StringComparison.OrdinalIgnoreCase);
		bool flag4 = process.OriginalName?.Equals(application.OriginalName, StringComparison.OrdinalIgnoreCase) ?? false;
		bool flag5 = process.Signature?.Equals(application.Signature, StringComparison.OrdinalIgnoreCase) ?? false;
		if (flag3 && (flag | flag4))
		{
			return flag2 | flag5;
		}
		return false;
	}

	private bool BelongsToSafeExamBrowser(IProcess process)
	{
		bool flag = true;
		int num = (int)(1u & ((process.Name == "SafeExamBrowser.Client.exe") ? 1u : 0u)) & ((process.OriginalName == "SafeExamBrowser.Client.exe") ? 1 : 0);
		flag &= process.Name == "SafeExamBrowser.exe";
		flag &= process.OriginalName == "SafeExamBrowser.exe";
		return (byte)((uint)num | (flag ? 1u : 0u)) != 0;
	}

	private void Close(Window window)
	{
		int processId = Convert.ToInt32(nativeMethods.GetProcessIdFor(window.Handle));
		nativeMethods.SendCloseMessageTo(window.Handle);
		logger.Info($"Sent close message to window {window}.");
		Task.Delay(1000).ContinueWith((Task _) =>
		{
			if (!nativeMethods.IsExistingWindow(window.Handle) || processId != nativeMethods.GetProcessIdFor(window.Handle))
			{
				logger.Info($"Successfully closed window {window}.");
			}
			else
			{
				logger.Warn($"Failed to close window {window}! Attempting to terminate its process...");
				if (TryGetProcessFor(window, out var process))
				{
					TryTerminate(process, gracefully: false);
				}
			}
		});
	}

	private Window CreateWindowFor(IntPtr handle)
	{
		IWindowStyle windowStyle = nativeMethods.GetWindowStyle(handle);
		return new Window
		{
			Handle = handle,
			IsMinimized = nativeMethods.IsMinimizedWindow(handle),
			IsOverlay = (windowStyle.IsDisabled || windowStyle.IsNotActivatable || windowStyle.IsTopmost),
			IsVisible = windowStyle.IsVisible,
			Title = nativeMethods.GetWindowTitle(handle)
		};
	}

	private void HandleExplorerStart(IProcess process)
	{
		logger.Warn($"A new instance of Windows Explorer {process} has been started!");
		Task.Run(() =>
		{
			ExplorerStartedEventHandler val = this.m_ExplorerStarted;
			if (val != null)
			{
				val.Invoke();
			}
		});
	}

	private void HandleInstanceStart(Guid applicationId, IProcess process)
	{
		logger.Debug($"Detected start of whitelisted application instance {process}.");
		Task.Run(() =>
		{
			InstanceStartedEventHandler val = this.m_InstanceStarted;
			if (val != null)
			{
				val.Invoke(applicationId, process);
			}
		});
	}

	private void InitializeProcesses()
	{
		processes = processFactory.GetAllRunning();
		logger.Debug($"Initialized {processes.Count} currently running processes.");
	}

	private void InitializeBlacklist(ApplicationSettings settings, InitializationResult result)
	{
		foreach (BlacklistApplication item in settings.Blacklist)
		{
			blacklist.Add(item);
		}
		logger.Debug(string.Format("Initialized blacklist with {0} applications{1}", blacklist.Count, blacklist.Any() ? (": " + string.Join(", ", blacklist.Select((BlacklistApplication a) => a.ExecutableName))) : "."));
		foreach (IProcess process in processes)
		{
			foreach (BlacklistApplication item2 in blacklist)
			{
				if (BelongsToApplication(process, item2))
				{
					if (!item2.AutoTerminate)
					{
						AddForTermination(item2.ExecutableName, process, result);
					}
					else if (item2.AutoTerminate && !TryTerminate(process))
					{
						AddFailed(item2.ExecutableName, process, result);
					}
					break;
				}
			}
		}
	}

	private void InitializeWhitelist(ApplicationSettings settings, InitializationResult result)
	{
		foreach (WhitelistApplication item in settings.Whitelist)
		{
			whitelist.Add(item);
		}
		logger.Debug(string.Format("Initialized whitelist with {0} applications{1}", whitelist.Count, whitelist.Any() ? (": " + string.Join(", ", whitelist.Select((WhitelistApplication a) => a.ExecutableName))) : "."));
		foreach (IProcess process in processes)
		{
			foreach (WhitelistApplication item2 in whitelist)
			{
				if (BelongsToApplication(process, item2))
				{
					if (!item2.AllowRunning && !item2.AutoTerminate)
					{
						AddForTermination(item2.ExecutableName, process, result);
					}
					else if (!item2.AllowRunning && item2.AutoTerminate && !TryTerminate(process))
					{
						AddFailed(item2.ExecutableName, process, result);
					}
					break;
				}
			}
		}
	}

	private bool IsAllowed(IProcess process)
	{
		foreach (BlacklistApplication item in blacklist)
		{
			if (BelongsToApplication(process, item))
			{
				logger.Warn($"Process {process} belongs to blacklisted application '{item.ExecutableName}'!");
				return false;
			}
		}
		return true;
	}

	private bool IsAllowed(Window window)
	{
		bool flag = false;
		if (TryGetProcessFor(window, out var process))
		{
			flag = BelongsToSafeExamBrowser(process) || IsWhitelisted(process, out var _);
		}
		if (!flag)
		{
			logger.Warn(string.Format("Window {0} belongs to not whitelisted process '{1}'!", window, ((process != null) ? process.Name : null) ?? "n/a"));
		}
		return flag;
	}

	private bool IsWhitelisted(IProcess process, out Guid? applicationId)
	{
		applicationId = null;
		foreach (WhitelistApplication item in whitelist)
		{
			if (BelongsToApplication(process, item))
			{
				applicationId = item.Id;
				return true;
			}
		}
		return false;
	}

	private void MonitorProcesses()
	{
		try
		{
			List<RunningApplication> list = new List<RunningApplication>();
			IList<IProcess> running = processFactory.GetAllRunning();
			List<IProcess> list2 = running.Where((IProcess r) => processes.All((IProcess p) => p.Id != r.Id)).ToList();
			List<IProcess> list3 = processes.Where((IProcess p) => running.All((IProcess r) => r.Id != p.Id)).ToList();
			foreach (IProcess item in list2)
			{
				logger.Debug($"Process {item} has been started [{item.GetAdditionalInfo()}].");
				processes.Add(item);
				Guid? applicationId;
				if (item.Name == "explorer.exe")
				{
					HandleExplorerStart(item);
				}
				else if (!IsAllowed(item) && !TryTerminate(item))
				{
					AddFailed(item, list);
				}
				else if (IsWhitelisted(item, out applicationId))
				{
					HandleInstanceStart(applicationId.Value, item);
				}
			}
			foreach (IProcess item2 in list3)
			{
				logger.Debug($"Process {item2} has been terminated.");
				processes.Remove(item2);
			}
			if (list.Any())
			{
				logger.Warn("Failed to terminate these blacklisted applications: " + string.Join(", ", list.Select((RunningApplication a) => a.Name)) + ".");
				TerminationFailedEventHandler val = this.m_TerminationFailed;
				if (val != null)
				{
					val.Invoke((IEnumerable<RunningApplication>)list);
				}
			}
		}
		catch (Exception ex)
		{
			logger.Error("Failed to monitor processes!", ex);
		}
	}

	private void MonitorWindows()
	{
		try
		{
			foreach (IntPtr allWindow in nativeMethods.GetAllWindows())
			{
				Window window = CreateWindowFor(allWindow);
				if (window.IsVisible && !window.IsMinimized && !IsAllowed(window) && !TryHide(window))
				{
					Close(window);
				}
			}
		}
		catch (Exception ex)
		{
			logger.Error("Failed to monitor windows!", ex);
		}
	}

	private bool TryGetProcessFor(Window window, out IProcess process)
	{
		int num = Convert.ToInt32(nativeMethods.GetProcessIdFor(window.Handle));
		if (!processFactory.TryGetById(num, ref process))
		{
			logger.Error($"Could not find process for window {window} and process ID = {num}!");
		}
		return process != null;
	}

	private bool TryHide(Window window)
	{
		bool flag = nativeMethods.HideWindow(window.Handle);
		if (flag)
		{
			logger.Info($"Successfully hid window {window}.");
			return flag;
		}
		logger.Warn($"Failed to hide window {window}!");
		return flag;
	}

	private bool TryTerminate(IProcess process, bool gracefully = true, bool kill = true)
	{
		if (gracefully)
		{
			for (int i = 0; i < 5; i++)
			{
				if (process.TryClose(500))
				{
					break;
				}
			}
		}
		if (!process.HasTerminated & kill)
		{
			for (int j = 0; j < 5; j++)
			{
				if (process.TryKill(500))
				{
					break;
				}
			}
		}
		if (process.HasTerminated)
		{
			logger.Info($"Successfully terminated process {process}.");
		}
		else
		{
			logger.Warn($"Failed to terminate process {process}!");
		}
		return process.HasTerminated;
	}
}
