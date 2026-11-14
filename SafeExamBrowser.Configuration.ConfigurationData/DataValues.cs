using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Browser;
using SafeExamBrowser.Settings.Browser.Proxy;
using SafeExamBrowser.Settings.Logging;
using SafeExamBrowser.Settings.Proctoring;
using SafeExamBrowser.Settings.Security;
using SafeExamBrowser.Settings.Service;
using SafeExamBrowser.Settings.UserInterface;

namespace SafeExamBrowser.Configuration.ConfigurationData;

internal class DataValues
{
	private const string DEFAULT_CONFIGURATION_NAME = "SebClientSettings.seb";

	private AppConfig appConfig;

	internal string GetAppDataFilePath()
	{
		return appConfig.AppDataFilePath;
	}

	internal AppConfig InitializeAppConfig()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected Obj, but got Unknown
		Assembly entryAssembly = Assembly.GetEntryAssembly();
		X509Certificate signerCertificate = entryAssembly.Modules.First().GetSignerCertificate();
		string fileVersion = FileVersionInfo.GetVersionInfo(entryAssembly.Location).FileVersion;
		string copyright = entryAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
		string title = entryAssembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
		string informationalVersion = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SafeExamBrowser");
		string path2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SafeExamBrowser");
		string path3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SafeExamBrowser");
		string text = Path.Combine(path, "Temp");
		DateTime now = DateTime.Now;
		string path4 = Path.Combine(path, "Logs");
		string text2 = now.ToString("yyyy-MM-dd\\_HH\\hmm\\mss\\s");
		appConfig = new AppConfig();
		appConfig.AppDataFilePath = Path.Combine(path2, "SebClientSettings.seb");
		appConfig.ApplicationStartTime = now;
		appConfig.BrowserCachePath = Path.Combine(path, "Cache");
		appConfig.BrowserLogFilePath = Path.Combine(path4, text2 + "_Browser.log");
		appConfig.ClientId = Guid.NewGuid();
		appConfig.ClientAddress = string.Format("{0}/client/{1}", "net.pipe://localhost/safeexambrowser", Guid.NewGuid());
		appConfig.ClientExecutablePath = Path.Combine(Path.GetDirectoryName(entryAssembly.Location), "SafeExamBrowser.Client.exe");
		appConfig.ClientLogFilePath = Path.Combine(path4, text2 + "_Client.log");
		appConfig.CodeSignatureHash = signerCertificate?.GetCertHashString();
		appConfig.ConfigurationFileExtension = ".seb";
		appConfig.ConfigurationFileMimeType = "application/seb";
		appConfig.ProgramBuildVersion = fileVersion;
		appConfig.ProgramCopyright = copyright;
		appConfig.ProgramDataFilePath = Path.Combine(path3, "SebClientSettings.seb");
		appConfig.ProgramTitle = title;
		appConfig.ProgramInformationalVersion = informationalVersion;
		appConfig.RuntimeId = Guid.NewGuid();
		appConfig.RuntimeAddress = string.Format("{0}/runtime/{1}", "net.pipe://localhost/safeexambrowser", Guid.NewGuid());
		appConfig.RuntimeLogFilePath = Path.Combine(path4, text2 + "_Runtime.log");
		appConfig.SebUriScheme = "seb";
		appConfig.SebUriSchemeSecure = "sebs";
		appConfig.ServiceAddress = "net.pipe://localhost/safeexambrowser/service";
		appConfig.ServiceEventName = string.Format("Global\\{0}-{1}", "SafeExamBrowser", Guid.NewGuid());
		appConfig.ServiceLogFilePath = Path.Combine(path4, text2 + "_Service.log");
		appConfig.SessionCacheFilePath = Path.Combine(text, "cache.bin");
		appConfig.TemporaryDirectory = text;
		return appConfig;
	}

	internal SessionConfiguration InitializeSessionConfiguration()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected Obj, but got Unknown
		SessionConfiguration val = new SessionConfiguration();
		appConfig.ClientId = Guid.NewGuid();
		appConfig.ClientAddress = string.Format("{0}/client/{1}", "net.pipe://localhost/safeexambrowser", Guid.NewGuid());
		appConfig.ServiceEventName = string.Format("Global\\{0}-{1}", "SafeExamBrowser", Guid.NewGuid());
		val.AppConfig = appConfig.Clone();
		val.ClientAuthenticationToken = Guid.NewGuid();
		val.SessionId = Guid.NewGuid();
		return val;
	}

	internal AppSettings LoadDefaultSettings()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Expected Obj, but got Unknown
		AppSettings val = new AppSettings();
		val.Browser.AdditionalWindow.AllowAddressBar = false;
		val.Browser.AdditionalWindow.AllowBackwardNavigation = true;
		val.Browser.AdditionalWindow.AllowDeveloperConsole = true;
		val.Browser.AdditionalWindow.AllowForwardNavigation = true;
		val.Browser.AdditionalWindow.AllowReloading = true;
		val.Browser.AdditionalWindow.FullScreenMode = false;
		val.Browser.AdditionalWindow.Position = (WindowPosition)2;
		val.Browser.AdditionalWindow.RelativeHeight = 100;
		val.Browser.AdditionalWindow.RelativeWidth = 50;
		val.Browser.AdditionalWindow.ShowHomeButton = true;
		val.Browser.AdditionalWindow.ShowReloadWarning = true;
		val.Browser.AdditionalWindow.ShowToolbar = true;
		val.Browser.AdditionalWindow.UrlPolicy = (UrlPolicy)3;
		val.Browser.AllowConfigurationDownloads = true;
		val.Browser.AllowCustomDownAndUploadLocation = true;
		val.Browser.AllowDownloads = true;
		val.Browser.AllowFind = true;
		val.Browser.AllowPageZoom = true;
		val.Browser.AllowPdfReader = true;
		val.Browser.AllowPdfReaderToolbar = false;
		val.Browser.AllowPrint = true;
		val.Browser.AllowUploads = true;
		val.Browser.DeleteCacheOnShutdown = true;
		val.Browser.DeleteCookiesOnShutdown = true;
		val.Browser.DeleteCookiesOnStartup = true;
		val.Browser.EnableBrowser = true;
		val.Browser.MainWindow.AllowAddressBar = false;
		val.Browser.MainWindow.AllowBackwardNavigation = true;
		val.Browser.MainWindow.AllowDeveloperConsole = true;
		val.Browser.MainWindow.AllowForwardNavigation = true;
		val.Browser.MainWindow.AllowReloading = true;
		val.Browser.MainWindow.FullScreenMode = false;
		val.Browser.MainWindow.RelativeHeight = 100;
		val.Browser.MainWindow.RelativeWidth = 100;
		val.Browser.MainWindow.ShowHomeButton = true;
		val.Browser.MainWindow.ShowReloadWarning = true;
		val.Browser.MainWindow.ShowToolbar = true;
		val.Browser.MainWindow.UrlPolicy = (UrlPolicy)0;
		val.Browser.PopupPolicy = (PopupPolicy)0;
		val.Browser.Proxy.Policy = (ProxyPolicy)1;
		val.Browser.ResetOnQuitUrl = false;
		val.Browser.SendBrowserExamKey = false;
		val.Browser.SendConfigurationKey = false;
		val.Browser.ShowFileSystemElementPath = true;
		val.Browser.StartUrl = "https://www.safeexambrowser.org/start";
		val.Browser.UseCustomUserAgent = false;
		val.Browser.UseIsolatedClipboard = false;
		val.Browser.UseQueryParameter = false;
		val.Browser.UseTemporaryDownAndUploadDirectory = false;
		val.ConfigurationMode = (ConfigurationMode)1;
		val.Display.AllowedDisplays = 500;
		val.Display.AlwaysOn = true;
		val.Display.IgnoreError = true;
		val.Display.InternalDisplayOnly = false;
		val.Keyboard.AllowAltEsc = true;
		val.Keyboard.AllowAltF4 = true;
		val.Keyboard.AllowAltTab = true;
		val.Keyboard.AllowCtrlC = true;
		val.Keyboard.AllowCtrlEsc = true;
		val.Keyboard.AllowCtrlV = true;
		val.Keyboard.AllowCtrlX = true;
		val.Keyboard.AllowEsc = true;
		val.Keyboard.AllowF1 = true;
		val.Keyboard.AllowF2 = true;
		val.Keyboard.AllowF3 = true;
		val.Keyboard.AllowF4 = true;
		val.Keyboard.AllowF5 = true;
		val.Keyboard.AllowF6 = true;
		val.Keyboard.AllowF7 = true;
		val.Keyboard.AllowF8 = true;
		val.Keyboard.AllowF9 = true;
		val.Keyboard.AllowF10 = true;
		val.Keyboard.AllowF11 = true;
		val.Keyboard.AllowF12 = true;
		val.Keyboard.AllowPrintScreen = true;
		val.Keyboard.AllowSystemKey = true;
		val.LogLevel = (LogLevel)3;
		val.Mouse.AllowMiddleButton = true;
		val.Mouse.AllowRightButton = true;
		val.PowerSupply.ChargeThresholdCritical = 0.1;
		val.PowerSupply.ChargeThresholdLow = 0.2;
		val.Proctoring.Enabled = false;
		val.Proctoring.ScreenProctoring.CacheSize = 500;
		val.Proctoring.ScreenProctoring.Enabled = false;
		val.Proctoring.ScreenProctoring.ImageDownscaling = 1.0;
		val.Proctoring.ScreenProctoring.ImageFormat = (ImageFormat)3;
		val.Proctoring.ScreenProctoring.ImageQuantization = (ImageQuantization)5;
		val.Proctoring.ScreenProctoring.IntervalMaximum = 5000;
		val.Proctoring.ScreenProctoring.IntervalMinimum = 1000;
		val.Proctoring.ScreenProctoring.MetaData.CaptureApplicationData = true;
		val.Proctoring.ScreenProctoring.MetaData.CaptureBrowserData = true;
		val.Proctoring.ScreenProctoring.MetaData.CaptureWindowTitle = true;
		val.Proctoring.ShowTaskbarNotification = true;
		val.Security.AllowApplicationLogAccess = false;
		val.Security.AllowTermination = true;
		val.Security.AllowReconfiguration = true;
		val.Security.AllowStickyKeys = true;
		val.Security.ClipboardPolicy = (ClipboardPolicy)0;
		val.Security.DisableSessionChangeLockScreen = false;
		val.Security.KioskMode = (KioskMode)2;
		val.Security.VerifyCursorConfiguration = false;
		val.Security.VerifySessionIntegrity = false;
		val.Security.VirtualMachinePolicy = (VirtualMachinePolicy)0;
		val.Security.AllowWindowCapture = true;
		val.Server.Invigilation.ForceRaiseHandMessage = false;
		val.Server.Invigilation.ShowRaiseHandNotification = true;
		val.Server.PerformFallback = false;
		val.Server.PingInterval = 1000;
		val.Server.RequestAttemptInterval = 2000;
		val.Server.RequestAttempts = 5;
		val.Server.RequestTimeout = 30000;
		val.Service.DisableChromeNotifications = false;
		val.Service.DisableEaseOfAccessOptions = false;
		val.Service.DisableFindPrinter = false;
		val.Service.DisableNetworkOptions = false;
		val.Service.DisablePasswordChange = false;
		val.Service.DisablePowerOptions = false;
		val.Service.DisableRemoteConnections = false;
		val.Service.DisableSignout = false;
		val.Service.DisableTaskManager = false;
		val.Service.DisableUserLock = false;
		val.Service.DisableUserSwitch = false;
		val.Service.DisableVmwareOverlay = false;
		val.Service.DisableWindowsUpdate = false;
		val.Service.IgnoreService = true;
		val.Service.Policy = (ServicePolicy)0;
		val.Service.SetVmwareConfiguration = false;
		val.SessionMode = (SessionMode)0;
		val.System.AlwaysOn = false;
		val.UserInterface.ActionCenter.EnableActionCenter = true;
		val.UserInterface.ActionCenter.ShowApplicationInfo = true;
		val.UserInterface.ActionCenter.ShowApplicationLog = false;
		val.UserInterface.ActionCenter.ShowClock = true;
		val.UserInterface.ActionCenter.ShowKeyboardLayout = true;
		val.UserInterface.ActionCenter.ShowNetwork = false;
		val.UserInterface.LockScreen.BackgroundColor = "#ff0000";
		val.UserInterface.Mode = (UserInterfaceMode)0;
		val.UserInterface.Taskbar.EnableTaskbar = true;
		val.UserInterface.Taskbar.ShowApplicationInfo = false;
		val.UserInterface.Taskbar.ShowApplicationLog = false;
		val.UserInterface.Taskbar.ShowClock = true;
		val.UserInterface.Taskbar.ShowKeyboardLayout = true;
		val.UserInterface.Taskbar.ShowNetwork = false;
		val.Browser.HomeUrl = "https://google.com";
		return val;
	}
}
