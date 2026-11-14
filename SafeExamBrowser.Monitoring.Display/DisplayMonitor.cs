using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Display;
using SafeExamBrowser.Monitoring.Contracts.Display.Events;
using SafeExamBrowser.Settings.Monitoring;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Monitoring.Display;

public class DisplayMonitor : IDisplayMonitor
{
	private IBounds originalWorkingArea;

	private readonly ILogger logger;

	private readonly INativeMethods nativeMethods;

	private readonly ISystemInfo systemInfo;

	private string wallpaper;

	[CompilerGenerated]
	private DisplayChangedEventHandler m_DisplayChanged;

	public event DisplayChangedEventHandler DisplayChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			DisplayChangedEventHandler val = this.m_DisplayChanged;
			DisplayChangedEventHandler val2;
			do
			{
				val2 = val;
				DisplayChangedEventHandler value2 = (DisplayChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_DisplayChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			DisplayChangedEventHandler val = this.m_DisplayChanged;
			DisplayChangedEventHandler val2;
			do
			{
				val2 = val;
				DisplayChangedEventHandler value2 = (DisplayChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_DisplayChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public DisplayMonitor(ILogger logger, INativeMethods nativeMethods, ISystemInfo systemInfo)
	{
		this.logger = logger;
		this.nativeMethods = nativeMethods;
		this.systemInfo = systemInfo;
	}

	public void InitializePrimaryDisplay(int taskbarHeight)
	{
		InitializeWorkingArea(taskbarHeight);
		InitializeWallpaper();
	}

	public void ResetPrimaryDisplay()
	{
		ResetWorkingArea();
		ResetWallpaper();
	}

	public void StartMonitoringDisplayChanges()
	{
		SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
		logger.Info("Started monitoring display changes.");
	}

	public void StopMonitoringDisplayChanges()
	{
		SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
		logger.Info("Stopped monitoring display changes.");
	}

	public ValidationResult ValidateConfiguration(DisplaySettings settings)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		ValidationResult val = new ValidationResult();
		if (TryLoadDisplays(out var displays))
		{
			IEnumerable<Display> source = displays.Where((Display d) => d.IsActive);
			int num = source.Count();
			bool flag = num <= settings.AllowedDisplays;
			bool flag2 = !settings.InternalDisplayOnly || source.All((Display d) => d.IsInternal);
			val.ExternalDisplays = source.Count((Display d) => !d.IsInternal);
			val.InternalDisplays = source.Count((Display d) => d.IsInternal);
			val.IsAllowed = flag & flag2;
			if (val.IsAllowed)
			{
				logger.Info($"Detected {num} active displays and {settings.AllowedDisplays} are allowed, display configuration is valid.");
			}
			else
			{
				if (!flag)
				{
					logger.Warn($"Detected {num} active displays but only {settings.AllowedDisplays} are allowed!");
				}
				if (!flag2)
				{
					logger.Warn("Detected external display(s) but only internal displays are allowed!");
				}
				logger.Warn("Display configuration is not valid!");
			}
		}
		else
		{
			val.IsAllowed = settings.IgnoreError;
		}
		return val;
	}

	private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
	{
		logger.Info("Display change detected!");
		Task.Run(() =>
		{
			DisplayChangedEventHandler val = this.m_DisplayChanged;
			if (val != null)
			{
				val.Invoke();
			}
		});
	}

	private void InitializeWorkingArea(int taskbarHeight)
	{
		string identifierForPrimaryDisplay = GetIdentifierForPrimaryDisplay();
		if (originalWorkingArea == null)
		{
			originalWorkingArea = nativeMethods.GetWorkingArea();
			LogWorkingArea("Saved original working area for " + identifierForPrimaryDisplay, originalWorkingArea);
		}
		Bounds bounds = new Bounds
		{
			Left = 0,
			Top = 0,
			Right = Screen.PrimaryScreen.Bounds.Width,
			Bottom = Screen.PrimaryScreen.Bounds.Height - taskbarHeight
		};
		LogWorkingArea("Trying to set new working area for " + identifierForPrimaryDisplay, (IBounds)(object)bounds);
		nativeMethods.SetWorkingArea((IBounds)(object)bounds);
		LogWorkingArea("Working area of " + identifierForPrimaryDisplay + " is now set to", nativeMethods.GetWorkingArea());
	}

	private void InitializeWallpaper()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)systemInfo.OperatingSystem == 1)
		{
			string wallpaperPath = nativeMethods.GetWallpaperPath();
			if (!string.IsNullOrEmpty(wallpaperPath))
			{
				wallpaper = wallpaperPath;
				logger.Info("Saved wallpaper image: " + wallpaper + ".");
			}
			nativeMethods.RemoveWallpaper();
			logger.Info("Removed current wallpaper.");
		}
	}

	private bool TryLoadDisplays(out IList<Display> displays)
	{
		bool result = true;
		displays = new List<Display>();
		try
		{
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Root\\WMI", "SELECT * FROM WmiMonitorBasicDisplayParams"))
			{
				using ManagementObjectCollection source = managementObjectSearcher.Get();
				foreach (ManagementObject item in source.Cast<ManagementObject>())
				{
					displays.Add(new Display
					{
						Identifier = Convert.ToString(item["InstanceName"]),
						IsActive = Convert.ToBoolean(item["Active"])
					});
				}
			}
			using ManagementObjectSearcher managementObjectSearcher2 = new ManagementObjectSearcher("Root\\WMI", "SELECT * FROM WmiMonitorConnectionParams");
			using ManagementObjectCollection source2 = managementObjectSearcher2.Get();
			foreach (ManagementObject item2 in source2.Cast<ManagementObject>())
			{
				string identifier = Convert.ToString(item2["InstanceName"]);
				bool flag = Convert.ToBoolean(item2["Active"]);
				long num = Convert.ToInt64(item2["VideoOutputTechnology"]);
				VideoOutputTechnology videoOutputTechnology = (VideoOutputTechnology)num;
				Display display = displays.FirstOrDefault((Display d) => d.Identifier?.Equals(identifier, StringComparison.OrdinalIgnoreCase) ?? false);
				if (!Enum.IsDefined(typeof(VideoOutputTechnology), videoOutputTechnology))
				{
					logger.Warn($"Detected undefined video output technology '{num}' for display '{identifier}'!");
				}
				if (display != null)
				{
					display.IsActive &= flag;
					display.Technology = videoOutputTechnology;
				}
			}
		}
		catch (Exception)
		{
			result = false;
		}
		foreach (Display display2 in displays)
		{
			logger.Info(string.Format("Detected {0}, {1} display '{2}' connected via '{3}'.", display2.IsActive ? "active" : "inactive", display2.IsInternal ? "internal" : "external", display2.Identifier, display2.Technology));
		}
		return result;
	}

	private void ResetWorkingArea()
	{
		string identifierForPrimaryDisplay = GetIdentifierForPrimaryDisplay();
		if (originalWorkingArea != null)
		{
			nativeMethods.SetWorkingArea(originalWorkingArea);
			LogWorkingArea("Restored original working area for " + identifierForPrimaryDisplay, originalWorkingArea);
		}
		else
		{
			logger.Warn("Could not restore original working area for " + identifierForPrimaryDisplay + "!");
		}
	}

	private void ResetWallpaper()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)systemInfo.OperatingSystem == 1 && !string.IsNullOrEmpty(wallpaper))
		{
			nativeMethods.SetWallpaper(wallpaper);
			logger.Info("Restored wallpaper image: " + wallpaper + ".");
		}
	}

	private string GetIdentifierForPrimaryDisplay()
	{
		string text = Screen.PrimaryScreen.DeviceName?.Replace("\\\\.\\", string.Empty);
		string text2 = $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";
		return text + " (" + text2 + ")";
	}

	private void LogWorkingArea(string message, IBounds area)
	{
		logger.Info($"{message}: Left = {area.Left}, Top = {area.Top}, Right = {area.Right}, Bottom = {area.Bottom}.");
	}
}
