using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Applications.Contracts.Events;
using SafeExamBrowser.Browser.Contracts;
using SafeExamBrowser.Browser.Contracts.Events;
using SafeExamBrowser.Browser.Events;
using SafeExamBrowser.Browser.Integrations;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Core.Contracts.Resources.Icons;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Browser;
using SafeExamBrowser.Settings.Browser.Proxy;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.FileSystemDialog;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Browser;

public class BrowserApplication : IBrowserApplication, IApplication<IBrowserWindow>
{
	private int windowIdCounter;

	private readonly AppConfig appConfig;

	private readonly Clipboard clipboard;

	private readonly IFileSystemDialog fileSystemDialog;

	private readonly IHashAlgorithm hashAlgorithm;

	private readonly IKeyGenerator keyGenerator;

	private readonly IModuleLogger logger;

	private readonly IMessageBox messageBox;

	private readonly INativeMethods nativeMethods;

	private readonly SessionMode sessionMode;

	private readonly BrowserSettings settings;

	private readonly IText text;

	private readonly IUserInterfaceFactory uiFactory;

	private readonly List<BrowserWindow> windows;

	[CompilerGenerated]
	private DownloadRequestedEventHandler m_ConfigurationDownloadRequested;

	[CompilerGenerated]
	private LoseFocusRequestedEventHandler m_LoseFocusRequested;

	[CompilerGenerated]
	private TerminationRequestedEventHandler m_TerminationRequested;

	[CompilerGenerated]
	private UserIdentifierDetectedEventHandler m_UserIdentifierDetected;

	[CompilerGenerated]
	private WindowsChangedEventHandler m_WindowsChanged;

	public bool AutoStart { get; private set; }

	public IconResource Icon { get; private set; }

	public Guid Id { get; private set; }

	public string Name { get; private set; }

	public string Tooltip { get; private set; }

	public event DownloadRequestedEventHandler ConfigurationDownloadRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			DownloadRequestedEventHandler val = this.m_ConfigurationDownloadRequested;
			DownloadRequestedEventHandler val2;
			do
			{
				val2 = val;
				DownloadRequestedEventHandler value2 = (DownloadRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ConfigurationDownloadRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			DownloadRequestedEventHandler val = this.m_ConfigurationDownloadRequested;
			DownloadRequestedEventHandler val2;
			do
			{
				val2 = val;
				DownloadRequestedEventHandler value2 = (DownloadRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ConfigurationDownloadRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	public event LoseFocusRequestedEventHandler LoseFocusRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LoseFocusRequestedEventHandler val = this.m_LoseFocusRequested;
			LoseFocusRequestedEventHandler val2;
			do
			{
				val2 = val;
				LoseFocusRequestedEventHandler value2 = (LoseFocusRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LoseFocusRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LoseFocusRequestedEventHandler val = this.m_LoseFocusRequested;
			LoseFocusRequestedEventHandler val2;
			do
			{
				val2 = val;
				LoseFocusRequestedEventHandler value2 = (LoseFocusRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LoseFocusRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	public event TerminationRequestedEventHandler TerminationRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TerminationRequestedEventHandler val = this.m_TerminationRequested;
			TerminationRequestedEventHandler val2;
			do
			{
				val2 = val;
				TerminationRequestedEventHandler value2 = (TerminationRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TerminationRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TerminationRequestedEventHandler val = this.m_TerminationRequested;
			TerminationRequestedEventHandler val2;
			do
			{
				val2 = val;
				TerminationRequestedEventHandler value2 = (TerminationRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TerminationRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	public event UserIdentifierDetectedEventHandler UserIdentifierDetected
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			UserIdentifierDetectedEventHandler val = this.m_UserIdentifierDetected;
			UserIdentifierDetectedEventHandler val2;
			do
			{
				val2 = val;
				UserIdentifierDetectedEventHandler value2 = (UserIdentifierDetectedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_UserIdentifierDetected, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			UserIdentifierDetectedEventHandler val = this.m_UserIdentifierDetected;
			UserIdentifierDetectedEventHandler val2;
			do
			{
				val2 = val;
				UserIdentifierDetectedEventHandler value2 = (UserIdentifierDetectedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_UserIdentifierDetected, value2, val2);
			}
			while (val != val2);
		}
	}

	public event WindowsChangedEventHandler WindowsChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			WindowsChangedEventHandler val = this.m_WindowsChanged;
			WindowsChangedEventHandler val2;
			do
			{
				val2 = val;
				WindowsChangedEventHandler value2 = (WindowsChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_WindowsChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			WindowsChangedEventHandler val = this.m_WindowsChanged;
			WindowsChangedEventHandler val2;
			do
			{
				val2 = val;
				WindowsChangedEventHandler value2 = (WindowsChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_WindowsChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public BrowserApplication(AppConfig appConfig, BrowserSettings settings, IFileSystemDialog fileSystemDialog, IHashAlgorithm hashAlgorithm, IKeyGenerator keyGenerator, IMessageBox messageBox, IModuleLogger logger, INativeMethods nativeMethods, SessionMode sessionMode, IText text, IUserInterfaceFactory uiFactory)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		this.appConfig = appConfig;
		clipboard = new Clipboard((ILogger)(object)logger.CloneFor("Clipboard"), settings);
		this.fileSystemDialog = fileSystemDialog;
		this.hashAlgorithm = hashAlgorithm;
		this.keyGenerator = keyGenerator;
		this.logger = logger;
		this.messageBox = messageBox;
		this.nativeMethods = nativeMethods;
		this.sessionMode = sessionMode;
		this.settings = settings;
		this.text = text;
		this.uiFactory = uiFactory;
		windows = new List<BrowserWindow>();
	}

	public void Focus(bool forward)
	{
		windows.ForEach((BrowserWindow window) =>
		{
			window.Focus(forward);
		});
	}

	public IEnumerable<IBrowserWindow> GetWindows()
	{
		return new List<IBrowserWindow>((IEnumerable<IBrowserWindow>)windows);
	}

	public void Initialize()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		((ILogger)logger).Info("Starting initialization...");
		bool flag = Cef.Initialize((CefSettingsBase)(object)InitializeCefSettings(), true, (IApp)null);
		InitializeApplicationInfo();
		if (flag)
		{
			InitializeCookies();
			InitializeDownAndUploadDirectory();
			InitializeIntegrityKeys();
			InitializePreferences();
			((ILogger)logger).Info("Initialized browser.");
			return;
		}
		throw new Exception($"Failed to initialize browser (exit code: {Cef.GetExitCode()})!");
	}

	public void Start()
	{
		CreateNewWindow();
	}

	public void Terminate()
	{
		((ILogger)logger).Info("Initiating termination...");
		AwaitReady();
		foreach (BrowserWindow window in windows)
		{
			window.Closed -= Window_Closed;
			window.Close();
			((ILogger)logger).Info($"Closed browser window #{window.Id}.");
		}
		FinalizeCookies();
		FinalizeDownAndUploadDirectory();
		Cef.Shutdown();
		FinalizeCache();
		((ILogger)logger).Info("Terminated browser.");
	}

	private void AwaitReady()
	{
		Thread.Sleep(500);
	}

	private void CreateNewWindow(PopupRequestedEventArgs args = null)
	{
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected Obj, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected Obj, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected Obj, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected Obj, but got Unknown
		int num = ++windowIdCounter;
		Integration[] integrations = new Integration[3]
		{
			new GenericIntegration((ILogger)(object)logger.CloneFor(string.Format("{0} #{1}", "GenericIntegration", num))),
			new EdxIntegration((ILogger)(object)logger.CloneFor(string.Format("{0} #{1}", "EdxIntegration", num))),
			new MoodleIntegration((ILogger)(object)logger.CloneFor(string.Format("{0} #{1}", "MoodleIntegration", num)))
		};
		bool isMainWindow = windows.Count == 0;
		string startUrl = GenerateStartUrl();
		IModuleLogger val = logger.CloneFor($"Browser Window #{num}");
		BrowserWindow browserWindow = new BrowserWindow(appConfig, clipboard, fileSystemDialog, hashAlgorithm, num, integrations, isMainWindow, keyGenerator, val, messageBox, sessionMode, settings, startUrl, text, uiFactory);
		browserWindow.Closed += Window_Closed;
		browserWindow.ConfigurationDownloadRequested += (string f, DownloadEventArgs a) =>
		{
			DownloadRequestedEventHandler val3 = this.m_ConfigurationDownloadRequested;
			if (val3 != null)
			{
				val3.Invoke(f, a);
			}
		};
		browserWindow.PopupRequested += Window_PopupRequested;
		browserWindow.ResetRequested += Window_ResetRequested;
		browserWindow.UserIdentifierDetected += (string i) =>
		{
			UserIdentifierDetectedEventHandler val3 = this.m_UserIdentifierDetected;
			if (val3 != null)
			{
				val3.Invoke(i);
			}
		};
		browserWindow.TerminationRequested += () =>
		{
			TerminationRequestedEventHandler val3 = this.m_TerminationRequested;
			if (val3 != null)
			{
				val3.Invoke();
			}
		};
		browserWindow.LoseFocusRequested += (bool forward) =>
		{
			LoseFocusRequestedEventHandler val3 = this.m_LoseFocusRequested;
			if (val3 != null)
			{
				val3.Invoke(forward);
			}
		};
		browserWindow.InitializeControl();
		windows.Add(browserWindow);
		if (args != null)
		{
			args.Window = browserWindow;
		}
		else
		{
			browserWindow.InitializeWindow();
		}
		((ILogger)logger).Info($"Created browser window #{browserWindow.Id}.");
		WindowsChangedEventHandler val2 = this.m_WindowsChanged;
		if (val2 != null)
		{
			val2.Invoke();
		}
	}

	private void DeleteCookies()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		TaskDeleteCookiesCallback val = new TaskDeleteCookiesCallback();
		ICookieManager globalCookieManager = Cef.GetGlobalCookieManager((ICompletionCallback)null);
		val.Task.ContinueWith((Task<int> task) =>
		{
			if (!task.IsCompleted || task.Result == -1)
			{
				((ILogger)logger).Warn("Failed to delete cookies!");
			}
			else
			{
				((ILogger)logger).Debug($"Deleted {task.Result} cookies.");
			}
		});
		if (globalCookieManager != null && globalCookieManager.DeleteCookies((string)null, (string)null, (IDeleteCookiesCallback)(object)val))
		{
			((ILogger)logger).Debug("Successfully initiated cookie deletion.");
		}
		else
		{
			((ILogger)logger).Warn("Failed to initiate cookie deletion!");
		}
	}

	private void FinalizeCache()
	{
		if (settings.DeleteCacheOnShutdown && settings.DeleteCookiesOnShutdown)
		{
			try
			{
				Directory.Delete(appConfig.BrowserCachePath, recursive: true);
				((ILogger)logger).Info("Deleted browser cache.");
				return;
			}
			catch (Exception ex)
			{
				((ILogger)logger).Error("Failed to delete browser cache!", ex);
				return;
			}
		}
		((ILogger)logger).Info("Retained browser cache.");
	}

	private void FinalizeCookies()
	{
		if (settings.DeleteCookiesOnShutdown)
		{
			DeleteCookies();
		}
	}

	private void FinalizeDownAndUploadDirectory()
	{
		if (settings.UseTemporaryDownAndUploadDirectory)
		{
			try
			{
				Directory.Delete(settings.DownAndUploadDirectory, recursive: true);
				((ILogger)logger).Info("Deleted temporary down- and upload directory.");
			}
			catch (Exception ex)
			{
				((ILogger)logger).Error("Failed to delete temporary down- and upload directory!", ex);
			}
		}
	}

	private string GenerateStartUrl()
	{
		string text = settings.StartUrl;
		if (settings.UseQueryParameter)
		{
			if (text.Contains("?"))
			{
				string startUrlQuery = settings.StartUrlQuery;
				if (startUrlQuery != null && startUrlQuery.Length > 1 && Uri.TryCreate(text, UriKind.Absolute, out var result))
				{
					text = text.Replace(result.Query, result.Query + "&" + settings.StartUrlQuery.Substring(1));
					goto IL_008f;
				}
			}
			text += settings.StartUrlQuery;
		}
		goto IL_008f;
		IL_008f:
		return text;
	}

	private void InitializeApplicationInfo()
	{
		AutoStart = true;
		Icon = (IconResource)(object)new BrowserIconResource();
		Id = Guid.NewGuid();
		Name = text.Get((TextKey)8);
		Tooltip = text.Get((TextKey)10);
	}

	private CefSettings InitializeCefSettings()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (int)((ILogger)logger).LogLevel == 2;
		bool flag2 = (int)((ILogger)logger).LogLevel == 3;
		CefSettings val = new CefSettings();
		((CefSettingsBase)val).AcceptLanguageList = CultureInfo.CurrentUICulture.Name;
		((CefSettingsBase)val).CachePath = appConfig.BrowserCachePath;
		((CefSettingsBase)val).LogFile = appConfig.BrowserLogFilePath;
		((CefSettingsBase)val).LogSeverity = (LogSeverity)(flag2 ? 4 : (flag ? 3 : 2));
		((CefSettingsBase)val).PersistSessionCookies = !settings.DeleteCookiesOnStartup || !settings.DeleteCookiesOnShutdown;
		((CefSettingsBase)val).UserAgent = InitializeUserAgent();
		if (!settings.AllowPageZoom)
		{
			((CefSettingsBase)val).CefCommandLineArgs.Add("disable-pinch");
		}
		if (!settings.AllowPdfReader)
		{
			((CefSettingsBase)val).CefCommandLineArgs.Add("disable-pdf-extension");
		}
		if (!settings.AllowSpellChecking)
		{
			((CefSettingsBase)val).CefCommandLineArgs.Add("disable-spell-checking");
		}
		((CefSettingsBase)val).CefCommandLineArgs.Add("do-not-de-elevate");
		((CefSettingsBase)val).CefCommandLineArgs.Add("enable-media-stream");
		((CefSettingsBase)val).CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
		((Dictionary<string, string>)(object)((CefSettingsBase)val).CefCommandLineArgs).Add("touch-events", "enabled");
		((CefSettingsBase)val).CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
		InitializeProxySettings(val);
		((ILogger)logger).Debug("Accept Language: " + ((CefSettingsBase)val).AcceptLanguageList);
		((ILogger)logger).Debug("Cache Path: " + ((CefSettingsBase)val).CachePath);
		((ILogger)logger).Debug("Engine Version: Chromium " + Cef.ChromiumVersion + ", CEF " + Cef.CefVersion + ", CefSharp " + Cef.CefSharpVersion);
		((ILogger)logger).Debug("Log File: " + ((CefSettingsBase)val).LogFile);
		((ILogger)logger).Debug($"Log Severity: {((CefSettingsBase)val).LogSeverity}.");
		((ILogger)logger).Debug("PDF Reader: " + (settings.AllowPdfReader ? "Enabled" : "Disabled") + ".");
		((ILogger)logger).Debug("Session Persistence: " + (((CefSettingsBase)val).PersistSessionCookies ? "Enabled" : "Disabled") + ".");
		return val;
	}

	private void InitializeCookies()
	{
		if (settings.DeleteCookiesOnStartup)
		{
			DeleteCookies();
		}
	}

	private void InitializeDownAndUploadDirectory()
	{
		if (settings.UseTemporaryDownAndUploadDirectory)
		{
			InitializeTemporaryDownAndUploadDirectory();
		}
		else if (!string.IsNullOrEmpty(settings.DownAndUploadDirectory))
		{
			InitializeCustomDownAndUploadDirectory();
		}
	}

	private void InitializeCustomDownAndUploadDirectory()
	{
		if (!Directory.Exists(Environment.ExpandEnvironmentVariables(settings.DownAndUploadDirectory)))
		{
			((ILogger)logger).Warn("The configured down- and upload directory does not exist! Falling back to the default directory...");
			settings.DownAndUploadDirectory = null;
		}
		else
		{
			((ILogger)logger).Debug("Using custom down- and upload directory as defined in the active configuration.");
		}
	}

	private void InitializeTemporaryDownAndUploadDirectory()
	{
		try
		{
			settings.DownAndUploadDirectory = Path.Combine(appConfig.TemporaryDirectory, Path.GetRandomFileName());
			Directory.CreateDirectory(settings.DownAndUploadDirectory);
			((ILogger)logger).Info("Created temporary down- and upload directory.");
		}
		catch (Exception ex)
		{
			((ILogger)logger).Error("Failed to create temporary down- and upload directory!", ex);
		}
	}

	private void InitializeIntegrityKeys()
	{
		((ILogger)logger).Debug("Browser Exam Key (BEK) transmission is " + (settings.SendBrowserExamKey ? "enabled" : "disabled") + ".");
		((ILogger)logger).Debug("Configuration Key (CK) transmission is " + (settings.SendConfigurationKey ? "enabled" : "disabled") + ".");
		if (settings.CustomBrowserExamKey != null)
		{
			keyGenerator.UseCustomBrowserExamKey(settings.CustomBrowserExamKey);
			((ILogger)logger).Debug("The browser application will be using a custom browser exam key.");
		}
		else
		{
			((ILogger)logger).Debug("The browser application will be using the default browser exam key.");
		}
	}

	private void InitializePreferences()
	{
		Cef.UIThreadTaskFactory.StartNew(() =>
		{
			IRequestContext globalRequestContext = Cef.GetGlobalRequestContext();
			try
			{
				string text = default;
				globalRequestContext.SetPreference("autofill.credit_card_enabled", (object)false, ref text);
				globalRequestContext.SetPreference("autofill.profile_enabled", (object)false, ref text);
			}
			finally
			{
				((IDisposable)globalRequestContext)?.Dispose();
			}
		});
	}

	private void InitializeProxySettings(CefSettings cefSettings)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if ((int)settings.Proxy.Policy != 0)
		{
			return;
		}
		if (settings.Proxy.AutoConfigure)
		{
			((Dictionary<string, string>)(object)((CefSettingsBase)cefSettings).CefCommandLineArgs).Add("proxy-pac-url", settings.Proxy.AutoConfigureUrl);
		}
		if (settings.Proxy.AutoDetect)
		{
			((Dictionary<string, string>)(object)((CefSettingsBase)cefSettings).CefCommandLineArgs).Add("proxy-auto-detect", "");
		}
		if (settings.Proxy.BypassList.Any())
		{
			((Dictionary<string, string>)(object)((CefSettingsBase)cefSettings).CefCommandLineArgs).Add("proxy-bypass-list", string.Join(";", settings.Proxy.BypassList));
		}
		if (!settings.Proxy.Proxies.Any())
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (ProxyConfiguration proxy in settings.Proxy.Proxies)
		{
			list.Add($"{ToScheme(proxy.Protocol)}={proxy.Host}:{proxy.Port}");
		}
		((Dictionary<string, string>)(object)((CefSettingsBase)cefSettings).CefCommandLineArgs).Add("proxy-server", string.Join(";", list));
	}

	private string InitializeUserAgent()
	{
		string text = $"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}";
		string text2 = "SEB/" + appConfig.ProgramInformationalVersion;
		string text3 = null;
		text3 = ((!settings.UseCustomUserAgent) ? ("Mozilla/5.0 (Windows NT " + text + ") AppleWebKit/537.36 (KHTML, like Gecko) Chrome/" + Cef.ChromiumVersion + " " + text2) : (settings.CustomUserAgent + " " + text2));
		if (!string.IsNullOrWhiteSpace(settings.UserAgentSuffix))
		{
			text3 = text3 + " " + settings.UserAgentSuffix;
		}
		return text3;
	}

	private string ToScheme(ProxyProtocol protocol)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected I4, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return (int)protocol switch
		{
			0 => Uri.UriSchemeFtp, 
			1 => Uri.UriSchemeHttp, 
			2 => Uri.UriSchemeHttps, 
			3 => "socks", 
			_ => throw new NotImplementedException($"Mapping for proxy protocol '{protocol}' is not yet implemented!"), 
		};
	}

	private void Window_Closed(int id)
	{
		windows.Remove(windows.First((BrowserWindow i) => i.Id == id));
		WindowsChangedEventHandler val = this.m_WindowsChanged;
		if (val != null)
		{
			val.Invoke();
		}
		((ILogger)logger).Info($"Window #{id} has been closed.");
	}

	private void Window_PopupRequested(PopupRequestedEventArgs args)
	{
		((ILogger)logger).Info("Received request to create new window...");
		CreateNewWindow(args);
	}

	private void Window_ResetRequested()
	{
		((ILogger)logger).Info("Attempting to reset browser...");
		AwaitReady();
		foreach (BrowserWindow window in windows)
		{
			window.Closed -= Window_Closed;
			window.Close();
			((ILogger)logger).Info($"Closed browser window #{window.Id}.");
		}
		windows.Clear();
		WindowsChangedEventHandler val = this.m_WindowsChanged;
		if (val != null)
		{
			val.Invoke();
		}
		if (settings.DeleteCookiesOnStartup && settings.DeleteCookiesOnShutdown)
		{
			DeleteCookies();
		}
		nativeMethods.EmptyClipboard();
		CreateNewWindow();
		((ILogger)logger).Info("Successfully reset browser.");
	}
}
