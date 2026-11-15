using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms.Handler;
using CefSharp.WinForms.Host;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Applications.Contracts.Events;
using SafeExamBrowser.Browser.Contracts;
using SafeExamBrowser.Browser.Contracts.Events;
using SafeExamBrowser.Browser.Contracts.Filters;
using SafeExamBrowser.Browser.Events;
using SafeExamBrowser.Browser.Filters;
using SafeExamBrowser.Browser.Handlers;
using SafeExamBrowser.Browser.Integrations;
using SafeExamBrowser.Browser.Wrapper;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Core.Contracts.Resources.Icons;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Browser;
using SafeExamBrowser.Settings.Browser.Filter;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Browser;
using SafeExamBrowser.UserInterface.Contracts.Browser.Data;
using SafeExamBrowser.UserInterface.Contracts.FileSystemDialog;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;
using Syroot.Windows.IO;

namespace SafeExamBrowser.Browser;

internal class BrowserWindow : IBrowserWindow, IApplicationWindow
{
	private const string CLEAR_FIND_TERM = "thisisahacktoclearthesearchresultsasitappearsthatthereisnosuchfunctionalityincef";

	private const double ZOOM_FACTOR = 0.2;

	private readonly AppConfig appConfig;

	private readonly Clipboard clipboard;

	private readonly IFileSystemDialog fileSystemDialog;

	private readonly IHashAlgorithm hashAlgorithm;

	private readonly HttpClient httpClient;

	private readonly IEnumerable<Integration> integrations;

	private readonly IKeyGenerator keyGenerator;

	private readonly IModuleLogger logger;

	private readonly IMessageBox messageBox;

	private readonly SessionMode sessionMode;

	private readonly Dictionary<int, BrowserWindow> popups;

	private readonly BrowserSettings settings;

	private readonly string startUrl;

	private readonly IText text;

	private readonly IUserInterfaceFactory uiFactory;

	private (string term, bool isInitial, bool caseSensitive, bool forward) findParameters;

	private IBrowserWindow window;

	private double zoomLevel;

	[CompilerGenerated]
	private DownloadRequestedEventHandler m_ConfigurationDownloadRequested;

	[CompilerGenerated]
	private LoseFocusRequestedEventHandler m_LoseFocusRequested;

	[CompilerGenerated]
	private TerminationRequestedEventHandler m_TerminationRequested;

	[CompilerGenerated]
	private UserIdentifierDetectedEventHandler m_UserIdentifierDetected;

	[CompilerGenerated]
	private IconChangedEventHandler m_IconChanged;

	[CompilerGenerated]
	private TitleChangedEventHandler m_TitleChanged;

	private WindowSettings WindowSettings
	{
		get
		{
			if (!IsMainWindow)
			{
				return settings.AdditionalWindow;
			}
			return settings.MainWindow;
		}
	}

	internal IBrowserControl Control { get; private set; }

	internal int Id { get; }

	public IntPtr Handle { get; private set; }

	public IconResource Icon { get; private set; }

	public bool IsMainWindow { get; private set; }

	public string Title { get; private set; }

	public string Url { get; private set; }

	internal event WindowClosedEventHandler Closed;

	internal event DownloadRequestedEventHandler ConfigurationDownloadRequested
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

	internal event LoseFocusRequestedEventHandler LoseFocusRequested
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

	internal event PopupRequestedEventHandler PopupRequested;

	internal event ResetRequestedEventHandler ResetRequested;

	internal event TerminationRequestedEventHandler TerminationRequested
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

	internal event UserIdentifierDetectedEventHandler UserIdentifierDetected
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

	public event IconChangedEventHandler IconChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			IconChangedEventHandler val = this.m_IconChanged;
			IconChangedEventHandler val2;
			do
			{
				val2 = val;
				IconChangedEventHandler value2 = (IconChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_IconChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			IconChangedEventHandler val = this.m_IconChanged;
			IconChangedEventHandler val2;
			do
			{
				val2 = val;
				IconChangedEventHandler value2 = (IconChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_IconChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event TitleChangedEventHandler TitleChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TitleChangedEventHandler val = this.m_TitleChanged;
			TitleChangedEventHandler val2;
			do
			{
				val2 = val;
				TitleChangedEventHandler value2 = (TitleChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TitleChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TitleChangedEventHandler val = this.m_TitleChanged;
			TitleChangedEventHandler val2;
			do
			{
				val2 = val;
				TitleChangedEventHandler value2 = (TitleChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TitleChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public BrowserWindow(AppConfig appConfig, Clipboard clipboard, IFileSystemDialog fileSystemDialog, IHashAlgorithm hashAlgorithm, int id, IEnumerable<Integration> integrations, bool isMainWindow, IKeyGenerator keyGenerator, IModuleLogger logger, IMessageBox messageBox, SessionMode sessionMode, BrowserSettings settings, string startUrl, IText text, IUserInterfaceFactory uiFactory)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		this.appConfig = appConfig;
		this.clipboard = clipboard;
		this.fileSystemDialog = fileSystemDialog;
		this.hashAlgorithm = hashAlgorithm;
		httpClient = new HttpClient();
		Id = id;
		this.integrations = integrations;
		IsMainWindow = isMainWindow;
		this.keyGenerator = keyGenerator;
		this.logger = logger;
		this.messageBox = messageBox;
		popups = new Dictionary<int, BrowserWindow>();
		this.sessionMode = sessionMode;
		this.settings = settings;
		this.startUrl = startUrl;
		this.text = text;
		this.uiFactory = uiFactory;
	}

	public void Activate()
	{
		((IWindow)window).BringToForeground();
	}

	internal void Close()
	{
		((IWindow)window).Close();
		Control.Destroy();
	}

	internal void Focus(bool forward)
	{
		if (forward)
		{
			window.FocusToolbar(forward);
			return;
		}
		window.FocusBrowser();
		Activate();
	}

	internal void InitializeControl()
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected Obj, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected Obj, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Expected Obj, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected Obj, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected Obj, but got Unknown
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected Obj, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Expected Obj, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Expected Obj, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected Obj, but got Unknown
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Expected Obj, but got Unknown
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Expected Obj, but got Unknown
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Expected Obj, but got Unknown
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Expected Obj, but got Unknown
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Expected Obj, but got Unknown
		ICefSharpControl cefSharpControl = null;
		IModuleLogger val = logger.CloneFor(string.Format("{0} #{1}", "BrowserControl", Id));
		ContextMenuHandler contextMenuHandler = new ContextMenuHandler();
		DialogHandler dialogHandler = new DialogHandler();
		DisplayHandler displayHandler = new DisplayHandler();
		IModuleLogger val2 = logger.CloneFor(string.Format("{0} #{1}", "DownloadHandler", Id));
		DownloadHandler downloadHandler = new DownloadHandler(appConfig, (ILogger)(object)val2, settings, WindowSettings);
		DragHandler dragHandler = new DragHandler();
		FocusHandler focusHandler = new FocusHandler();
		JavaScriptDialogHandler javaScriptDialogHandler = new JavaScriptDialogHandler();
		KeyboardHandler keyboardHandler = new KeyboardHandler();
		RenderProcessMessageHandler renderProcessMessageHandler = new RenderProcessMessageHandler(appConfig, clipboard, keyGenerator, settings, text);
		RequestFilter requestFilter = new RequestFilter();
		IModuleLogger val3 = logger.CloneFor(string.Format("{0} #{1}", "RequestHandler", Id));
		ResourceHandler resourceHandler = new ResourceHandler(appConfig, (IRequestFilter)(object)requestFilter, integrations, keyGenerator, (ILogger)(object)logger, sessionMode, settings, WindowSettings, text);
		RequestHandler requestHandler = new RequestHandler(appConfig, (IRequestFilter)(object)requestFilter, (ILogger)(object)val3, resourceHandler, settings, WindowSettings);
		Icon = (IconResource)(object)new BrowserIconResource();
		cefSharpControl = ((!IsMainWindow) ? ((ICefSharpControl)new CefSharpPopupControl()) : ((ICefSharpControl)new CefSharpBrowserControl(CreateLifeSpanHandlerForMainWindow(), startUrl)));
		dialogHandler.DialogRequested += DialogHandler_DialogRequested;
		displayHandler.FaviconChanged += DisplayHandler_FaviconChanged;
		displayHandler.ProgressChanged += DisplayHandler_ProgressChanged;
		downloadHandler.ConfigurationDownloadRequested += DownloadHandler_ConfigurationDownloadRequested;
		downloadHandler.DownloadAborted += DownloadHandler_DownloadAborted;
		downloadHandler.DownloadUpdated += DownloadHandler_DownloadUpdated;
		javaScriptDialogHandler.DialogRequested += JavaScriptDialogHandler_DialogRequested;
		keyboardHandler.FindRequested += KeyboardHandler_FindRequested;
		keyboardHandler.FocusAddressBarRequested += KeyboardHandler_FocusAddressBarRequested;
		keyboardHandler.HomeNavigationRequested += HomeNavigationRequested;
		keyboardHandler.ReloadRequested += ReloadRequested;
		keyboardHandler.TabPressed += KeyboardHandler_TabPressed;
		keyboardHandler.ZoomInRequested += ZoomInRequested;
		keyboardHandler.ZoomOutRequested += ZoomOutRequested;
		keyboardHandler.ZoomResetRequested += ZoomResetRequested;
		requestHandler.QuitUrlVisited += RequestHandler_QuitUrlVisited;
		requestHandler.RequestBlocked += RequestHandler_RequestBlocked;
		resourceHandler.UserIdentifierDetected += (string id) =>
		{
			UserIdentifierDetectedEventHandler val4 = this.m_UserIdentifierDetected;
			if (val4 != null)
			{
				val4.Invoke(id);
			}
		};
		InitializeRequestFilter((IRequestFilter)(object)requestFilter);
		Control = (IBrowserControl)(object)new BrowserControl(clipboard, cefSharpControl, (IContextMenuHandler)(object)contextMenuHandler, (IDialogHandler)(object)dialogHandler, (IDisplayHandler)(object)displayHandler, (IDownloadHandler)(object)downloadHandler, (IDragHandler)(object)dragHandler, (IFocusHandler)(object)focusHandler, (IJsDialogHandler)(object)javaScriptDialogHandler, (IKeyboardHandler)(object)keyboardHandler, (ILogger)(object)val, (IRenderProcessMessageHandler)(object)renderProcessMessageHandler, (IRequestHandler)(object)requestHandler);
		Control.AddressChanged += Control_AddressChanged;
		Control.LoadFailed += Control_LoadFailed;
		Control.LoadingStateChanged += Control_LoadingStateChanged;
		Control.TitleChanged += Control_TitleChanged;
		Control.Initialize();
		((ILogger)logger).Debug("Initialized browser control.");
	}

	internal void InitializeWindow()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected Obj, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected Obj, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected Obj, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected Obj, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected Obj, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected Obj, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected Obj, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected Obj, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected Obj, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected Obj, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected Obj, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected Obj, but got Unknown
		window = uiFactory.CreateBrowserWindow(Control, settings, IsMainWindow, (ILogger)(object)logger);
		window.AddressChanged += Window_AddressChanged;
		window.BackwardNavigationRequested += Window_BackwardNavigationRequested;
		((IWindow)window).Closed += Window_Closed;
		((IWindow)window).Closing += Window_Closing;
		window.DeveloperConsoleRequested += Window_DeveloperConsoleRequested;
		window.FindRequested += Window_FindRequested;
		window.ForwardNavigationRequested += Window_ForwardNavigationRequested;
		window.HomeNavigationRequested += HomeNavigationRequested;
		window.LoseFocusRequested += Window_LoseFocusRequested;
		window.ReloadRequested += ReloadRequested;
		window.ZoomInRequested += ZoomInRequested;
		window.ZoomOutRequested += ZoomOutRequested;
		window.ZoomResetRequested += ZoomResetRequested;
		window.UpdateZoomLevel(CalculateZoomPercentage());
		((IWindow)window).Show();
		((IWindow)window).BringToForeground();
		Handle = window.Handle;
		InitiateCookieTraversal();
		((ILogger)logger).Debug("Initialized browser window.");
	}

	private ILifeSpanHandler CreateLifeSpanHandlerForMainWindow()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected Obj, but got Unknown
		return LifeSpanHandler.Create((CreatePopupChromiumHostControl)(() => LifeSpanHandler_CreatePopup())).OnBeforePopupCreated((OnBeforePopupCreatedDelegate)((IWebBrowser wb, IBrowser b, IFrame f, string u, string t, WindowOpenDisposition d, bool g, IBrowserSettings s) =>
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			return LifeSpanHandler_PopupRequested(u);
		})).OnPopupCreated((OnPopupCreatedDelegate)((ChromiumHostControl c, string u) =>
		{
			LifeSpanHandler_PopupCreated(c);
		}))
			.OnPopupDestroyed((OnPopupDestroyedDelegate)((ChromiumHostControl c, IBrowser b) =>
			{
				LifeSpanHandler_PopupDestroyed(c);
			}))
			.Build();
	}

	private void InitiateCookieTraversal()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected Obj, but got Unknown
		CookieVisitor cookieVisitor = new CookieVisitor(integrations);
		cookieVisitor.UserIdentifierDetected += (string id) =>
		{
			UserIdentifierDetectedEventHandler val = this.m_UserIdentifierDetected;
			if (val != null)
			{
				val.Invoke(id);
			}
		};
		if (Cef.GetGlobalCookieManager((ICompletionCallback)null).VisitAllCookies((ICookieVisitor)(object)cookieVisitor))
		{
			((ILogger)logger).Debug("Successfully initiated cookie traversal.");
		}
		else
		{
			((ILogger)logger).Warn("Failed to initiate cookie traversal!");
		}
	}

	private void InitializeRequestFilter(IRequestFilter requestFilter)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected Obj, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (!settings.Filter.ProcessContentRequests && !settings.Filter.ProcessMainRequests)
		{
			return;
		}
		RuleFactory ruleFactory = new RuleFactory();
		foreach (FilterRuleSettings rule in settings.Filter.Rules)
		{
			IRule val = ruleFactory.CreateRule(rule.Type);
			val.Initialize(rule);
			requestFilter.Load(val);
		}
		((ILogger)logger).Debug($"Initialized request filter with {settings.Filter.Rules.Count} rule(s).");
		if ((int)requestFilter.Process(new Request
		{
			Url = settings.StartUrl
		}) != 0)
		{
			IRule val2 = ruleFactory.CreateRule((FilterRuleType)1);
			val2.Initialize(new FilterRuleSettings
			{
				Expression = settings.StartUrl,
				Result = (FilterResult)0
			});
			requestFilter.Load(val2);
			((ILogger)logger).Debug("Automatically created filter rule to allow start URL" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" '" + settings.StartUrl + "'") : "") + ".");
		}
	}

	private void Control_AddressChanged(string address)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		((ILogger)logger).Info("Navigated" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" to '" + address + "'") : "") + ".");
		Url = address;
		window.UpdateAddress(address);
		if ((int)WindowSettings.UrlPolicy == 0 || (int)WindowSettings.UrlPolicy == 1)
		{
			Title = address;
			window.UpdateTitle(address);
			TitleChangedEventHandler val = this.m_TitleChanged;
			if (val != null)
			{
				val.Invoke(address);
			}
		}
	}

	private void Control_LoadFailed(int errorCode, string errorText, bool isMainRequest, string url)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		switch (errorCode)
		{
		case -3:
			((ILogger)logger).Info("Request" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" for '" + url + "'") : "") + " was aborted.");
			break;
		case -106:
			((ILogger)logger).Info("Request" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" for '" + url + "'") : "") + " has failed due to loss of internet connection.");
			break;
		case 0:
			((ILogger)logger).Info("Request" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" for '" + url + "'") : "") + " was successful.");
			break;
		case -302:
			((ILogger)logger).Info("Request" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" for '" + url + "'") : "") + " has an unknown URL scheme and will be handled by the OS.");
			break;
		default:
			HandleUnknownLoadFailure(errorCode, errorText, isMainRequest, url);
			break;
		}
	}

	private void HandleUnknownLoadFailure(int errorCode, string errorText, bool isMainRequest, string url)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Format("{0} ({1}, {2} request)", errorText, errorCode, isMainRequest ? "main" : "resource");
		((ILogger)logger).Warn("Request" + (UrlPolicyExtensions.CanLogError(WindowSettings.UrlPolicy) ? (" for '" + url + "'") : "") + " failed: " + text + ".");
		if (isMainRequest)
		{
			string title = this.text.Get((TextKey)7);
			string message = this.text.Get((TextKey)6).Replace("%%URL%%", UrlPolicyExtensions.CanLogError(WindowSettings.UrlPolicy) ? url : "") + " " + text;
			Task.Run(() =>
			{
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				return messageBox.Show(message, title, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)window);
			}).ContinueWith((Task<MessageBoxResult> _) =>
			{
				Control.NavigateBackwards();
			});
		}
	}

	private void Control_LoadingStateChanged(bool isLoading)
	{
		window.CanNavigateBackwards = WindowSettings.AllowBackwardNavigation && Control.CanNavigateBackwards;
		window.CanNavigateForwards = WindowSettings.AllowForwardNavigation && Control.CanNavigateForwards;
		window.UpdateLoadingState(isLoading);
	}

	private void Control_TitleChanged(string title)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)WindowSettings.UrlPolicy != 0)
		{
			Title = title;
			window.UpdateTitle(Title);
			TitleChangedEventHandler val = this.m_TitleChanged;
			if (val != null)
			{
				val.Invoke(Title);
			}
		}
	}

	private void DialogHandler_DialogRequested(DialogRequestedEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (int)args.Operation == 1;
		bool flag2 = (int)args.Operation == 0;
		bool num = (flag && settings.AllowDownloads) || (flag2 && settings.AllowUploads);
		string text = null;
		if (flag)
		{
			text = args.InitialPath;
		}
		else
		{
			text = ((!string.IsNullOrEmpty(settings.DownAndUploadDirectory)) ? Environment.ExpandEnvironmentVariables(settings.DownAndUploadDirectory) : KnownFolders.Downloads.ExpandedPath);
		}
		if (num)
		{
			FileSystemDialogResult val = fileSystemDialog.Show(args.Element, args.Operation, text, (string)null, args.Title, (IWindow)(object)window, !settings.AllowCustomDownAndUploadLocation, settings.ShowFileSystemElementPath);
			if (val.Success)
			{
				args.FullPath = val.FullPath;
				args.Success = val.Success;
				((ILogger)logger).Debug($"User selected path '{val.FullPath}' when asked to {args.Operation}->{args.Element}.");
			}
			else
			{
				((ILogger)logger).Debug($"User aborted file system dialog to {args.Operation}->{args.Element}.");
			}
		}
		else
		{
			((ILogger)logger).Info(string.Format("Blocked file system dialog to {0}->{1}, as {2} is not allowed.", args.Operation, args.Element, flag ? "downloading" : "uploading"));
			ShowDownUploadNotAllowedMessage(flag);
		}
	}

	private void DisplayHandler_FaviconChanged(string uri)
	{
		Task.Run(() =>
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, uri);
			httpClient.SendAsync(request).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.IsCompleted && task.Result.IsSuccessStatusCode)
				{
					Icon = (IconResource)(object)new BrowserIconResource(uri);
					IconChangedEventHandler val = this.m_IconChanged;
					if (val != null)
					{
						val.Invoke(Icon);
					}
					window.UpdateIcon(Icon);
				}
			});
		});
	}

	private void DisplayHandler_ProgressChanged(double value)
	{
		window.UpdateProgress(value);
	}

	private void DownloadHandler_ConfigurationDownloadRequested(string fileName, DownloadEventArgs args)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (settings.AllowConfigurationDownloads)
		{
			((ILogger)logger).Debug("Forwarding download request for configuration file '" + fileName + "'.");
			DownloadRequestedEventHandler val = this.m_ConfigurationDownloadRequested;
			if (val != null)
			{
				val.Invoke(fileName, args);
			}
			if (args.AllowDownload)
			{
				((ILogger)logger).Debug("Download request for configuration file '" + fileName + "' was granted.");
				return;
			}
			((ILogger)logger).Debug("Download request for configuration file '" + fileName + "' was denied.");
			messageBox.Show((TextKey)134, (TextKey)135, (MessageBoxAction)0, (MessageBoxIcon)1, (IWindow)(object)window);
		}
		else
		{
			((ILogger)logger).Debug("Discarded download request for configuration file '" + fileName + "'.");
		}
	}

	private void DownloadHandler_DownloadAborted()
	{
		ShowDownUploadNotAllowedMessage();
	}

	private void DownloadHandler_DownloadUpdated(DownloadItemState state)
	{
		window.UpdateDownloadState(state);
	}

	private void HomeNavigationRequested()
	{
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Invalid comparison between Unknown and I4
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (!IsMainWindow || (!settings.UseStartUrlAsHomeUrl && string.IsNullOrWhiteSpace(settings.HomeUrl)))
		{
			return;
		}
		bool flag = false;
		string text = (settings.UseStartUrlAsHomeUrl ? settings.StartUrl : settings.HomeUrl);
		if (settings.HomeNavigationRequiresPassword && !string.IsNullOrWhiteSpace(settings.HomePasswordHash))
		{
			string text2 = this.text.Get((TextKey)220);
			string text3 = ((!string.IsNullOrWhiteSpace(settings.HomeNavigationMessage)) ? settings.HomeNavigationMessage : this.text.Get((TextKey)221));
			PasswordDialogResult val = uiFactory.CreatePasswordDialog(text2, text3).Show((IWindow)(object)window);
			if (val.Success)
			{
				string value = hashAlgorithm.GenerateHashFor(val.Password);
				if (settings.HomePasswordHash.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
				}
				else
				{
					messageBox.Show((TextKey)114, (TextKey)115, (MessageBoxAction)0, (MessageBoxIcon)3, (IWindow)(object)window);
				}
			}
		}
		else
		{
			string text4 = this.text.Get((TextKey)93);
			string text5 = ((!string.IsNullOrWhiteSpace(settings.HomeNavigationMessage)) ? settings.HomeNavigationMessage : this.text.Get((TextKey)94));
			flag = (int)messageBox.Show(text4, text5, (MessageBoxAction)2, (MessageBoxIcon)2, (IWindow)(object)window) == 4;
		}
		if (flag)
		{
			Control.NavigateTo(text);
		}
	}

	private void JavaScriptDialogHandler_DialogRequested(JavaScriptDialogRequestedEventArgs args)
	{
		((ILogger)logger).Debug($"A JavaScript dialog of type '{args.Type}' has been requested...");
		if (args.Type == JavaScriptDialogType.LeavePage)
		{
			args.Success = RequestPageLeave();
		}
		else
		{
			args.Success = RequestPageReload();
		}
	}

	private void KeyboardHandler_FindRequested()
	{
		if (settings.AllowFind)
		{
			window.ShowFindbar();
		}
	}

	private void KeyboardHandler_FocusAddressBarRequested()
	{
		window.FocusAddressBar();
	}

	private void KeyboardHandler_TabPressed(bool shiftPressed)
	{
		Control.ExecuteJavaScript("document.activeElement.tagName", (Action<JavaScriptResult>)((JavaScriptResult result) =>
		{
			if (result.Result is string text && text?.ToUpper() == "BODY")
			{
				if (shiftPressed)
				{
					window.FocusToolbar(!shiftPressed);
				}
				else
				{
					LoseFocusRequestedEventHandler val = this.m_LoseFocusRequested;
					if (val != null)
					{
						val.Invoke(true);
					}
				}
			}
		}));
	}

	private ChromiumHostControl LifeSpanHandler_CreatePopup()
	{
		PopupRequestedEventArgs e = new PopupRequestedEventArgs();
		PopupRequested?.Invoke(e);
		object embeddableControl = e.Window.Control.EmbeddableControl;
		ChromiumHostControl val = (ChromiumHostControl)((embeddableControl is ChromiumHostControl) ? embeddableControl : null);
		int id = ((object)val).GetHashCode();
		BrowserWindow browserWindow = e.Window;
		popups[id] = browserWindow;
		browserWindow.Closed += (int _) =>
		{
			popups.Remove(id);
		};
		return val;
	}

	private void LifeSpanHandler_PopupCreated(ChromiumHostControl control)
	{
		int hashCode = ((object)control).GetHashCode();
		popups[hashCode].InitializeWindow();
	}

	private void LifeSpanHandler_PopupDestroyed(ChromiumHostControl control)
	{
		int hashCode = ((object)control).GetHashCode();
		popups[hashCode].Close();
	}

	private PopupCreation LifeSpanHandler_PopupRequested(string targetUrl)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected I4, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		PopupCreation result = (PopupCreation)0;
		bool flag = Uri.TryCreate(Control.Address, UriKind.Absolute, out var result2);
		bool flag2 = Uri.TryCreate(targetUrl, UriKind.Absolute, out var result3);
		bool flag3 = (flag & flag2) && string.Equals(result2.Host, result3.Host, StringComparison.OrdinalIgnoreCase);
		PopupPolicy popupPolicy = settings.PopupPolicy;
		switch ((int)popupPolicy)
		{
		case 1:
			if (flag3)
			{
				goto case 0;
			}
			if (!flag3)
			{
				goto IL_0116;
			}
			goto default;
		case 0:
			((ILogger)logger).Debug("Forwarding request to open new window" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" for '" + targetUrl + "'") : "") + "...");
			result = (PopupCreation)1;
			break;
		case 2:
			if (flag3)
			{
				goto case 3;
			}
			if (!flag3)
			{
				goto IL_0116;
			}
			goto default;
		case 3:
			((ILogger)logger).Info("Discarding request to open new window and loading" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" '" + targetUrl + "'") : "") + " directly...");
			Control.NavigateTo(targetUrl);
			break;
		default:
			{
				((ILogger)logger).Info("Blocked request to open new window" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" for '" + targetUrl + "'") : "") + ".");
				break;
			}
			IL_0116:
			((ILogger)logger).Info("Blocked request to open new window" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" for '" + targetUrl + "'") : "") + " as it targets a different host.");
			break;
		}
		return result;
	}

	private void RequestHandler_QuitUrlVisited(string url)
	{
		Task.Run(() =>
		{
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Invalid comparison between Unknown and I4
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			if (settings.ResetOnQuitUrl)
			{
				((ILogger)logger).Info("Forwarding request to reset browser...");
				ResetRequested?.Invoke();
			}
			else if (settings.ConfirmQuitUrl)
			{
				string text = this.text.Get((TextKey)97);
				string text2 = this.text.Get((TextKey)98);
				if ((int)messageBox.Show(text, text2, (MessageBoxAction)2, (MessageBoxIcon)2, (IWindow)(object)window) == 4)
				{
					((ILogger)logger).Info("User confirmed termination via quit URL" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" '" + url + "'") : "") + ", forwarding request...");
					TerminationRequestedEventHandler val = this.m_TerminationRequested;
					if (val != null)
					{
						val.Invoke();
					}
				}
				else
				{
					((ILogger)logger).Info("User aborted termination via quit URL" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" '" + url + "'") : "") + ".");
				}
			}
			else
			{
				((ILogger)logger).Info("Automatically requesting termination due to quit URL" + (UrlPolicyExtensions.CanLog(WindowSettings.UrlPolicy) ? (" '" + url + "'") : "") + "...");
				TerminationRequestedEventHandler val2 = this.m_TerminationRequested;
				if (val2 != null)
				{
					val2.Invoke();
				}
			}
		});
	}

	private void RequestHandler_RequestBlocked(string url)
	{
		Task.Run(() =>
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected Obj, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Expected Obj, but got Unknown
			string text = this.text.Get((TextKey)95).Replace("%%URL%%", UrlPolicyExtensions.CanLogError(WindowSettings.UrlPolicy) ? url : "");
			string text2 = this.text.Get((TextKey)96);
			Control.TitleChanged -= Control_TitleChanged;
			if (url.Equals(startUrl, StringComparison.OrdinalIgnoreCase))
			{
				window.UpdateTitle("*** " + text2 + " ***");
				TitleChangedEventHandler val = this.m_TitleChanged;
				if (val != null)
				{
					val.Invoke("*** " + text2 + " ***");
				}
			}
			messageBox.Show(text, text2, (MessageBoxAction)0, (MessageBoxIcon)1, (IWindow)(object)window);
			Control.TitleChanged += Control_TitleChanged;
		});
	}

	private void ReloadRequested()
	{
		((ILogger)logger).Debug("A reload of the current page has been requested...");
		if (RequestPageReload())
		{
			Control.Reload();
		}
	}

	private bool RequestPageLeave()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		bool result = false;
		if ((int)messageBox.Show((TextKey)126, (TextKey)127, (MessageBoxAction)2, (MessageBoxIcon)2, (IWindow)(object)window) == 4)
		{
			result = true;
			((ILogger)logger).Debug("The page leave has been granted by the user.");
		}
		else
		{
			((ILogger)logger).Debug("The page leave has been aborted by the user.");
		}
		return result;
	}

	private bool RequestPageReload()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		bool result = false;
		if (WindowSettings.AllowReloading && WindowSettings.ShowReloadWarning)
		{
			if ((int)messageBox.Show((TextKey)128, (TextKey)129, (MessageBoxAction)2, (MessageBoxIcon)2, (IWindow)(object)window) == 4)
			{
				result = true;
				((ILogger)logger).Debug("The page reload has been granted by the user.");
			}
			else
			{
				((ILogger)logger).Debug("The page reload has been aborted by the user.");
			}
		}
		else if (WindowSettings.AllowReloading)
		{
			result = true;
			((ILogger)logger).Debug("The page reload has been automatically granted.");
		}
		else
		{
			((ILogger)logger).Debug("The page reload has been blocked, as the user is not allowed to reload web pages.");
		}
		return result;
	}

	private void ShowDownUploadNotAllowedMessage(bool isDownload = true)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		TextKey val = (TextKey)(isDownload ? 110 : 154);
		TextKey val2 = (TextKey)(isDownload ? 111 : 155);
		messageBox.Show(val, val2, (MessageBoxAction)0, (MessageBoxIcon)3, (IWindow)(object)window);
	}

	private void Window_AddressChanged(string address)
	{
		if (Uri.TryCreate(address, UriKind.Absolute, out var result) || Uri.TryCreate("https://" + address, UriKind.Absolute, out result))
		{
			((ILogger)logger).Debug("The user requested to navigate to '" + address + "', the URI is valid.");
			Control.NavigateTo(address);
		}
		else
		{
			((ILogger)logger).Debug("The user requested to navigate to '" + address + "', but the URI is not valid.");
			window.UpdateAddress(string.Empty);
		}
	}

	private void Window_BackwardNavigationRequested()
	{
		((ILogger)logger).Debug("Navigating backwards...");
		Control.NavigateBackwards();
	}

	private void Window_Closing()
	{
		((ILogger)logger).Debug("Window is closing...");
	}

	private void Window_Closed()
	{
		((ILogger)logger).Debug("Window has been closed.");
		Control.Destroy();
		Closed?.Invoke(Id);
	}

	private void Window_DeveloperConsoleRequested()
	{
		((ILogger)logger).Debug("Showing developer console...");
		Control.ShowDeveloperConsole();
	}

	private void Window_FindRequested(string term, bool isInitial, bool caseSensitive, bool forward = true)
	{
		if (settings.AllowFind)
		{
			findParameters.caseSensitive = caseSensitive;
			findParameters.forward = forward;
			findParameters.isInitial = isInitial;
			findParameters.term = term;
			Control.Find(term, isInitial, caseSensitive, forward);
		}
	}

	private void Window_ForwardNavigationRequested()
	{
		((ILogger)logger).Debug("Navigating forwards...");
		Control.NavigateForwards();
	}

	private void Window_LoseFocusRequested(bool forward)
	{
		LoseFocusRequestedEventHandler val = this.m_LoseFocusRequested;
		if (val != null)
		{
			val.Invoke(forward);
		}
	}

	private void ZoomInRequested()
	{
		if (settings.AllowPageZoom && CalculateZoomPercentage() < 300.0)
		{
			zoomLevel += 0.2;
			Control.Zoom(zoomLevel);
			window.UpdateZoomLevel(CalculateZoomPercentage());
			((ILogger)logger).Debug($"Increased page zoom to {CalculateZoomPercentage()}%.");
		}
	}

	private void ZoomOutRequested()
	{
		if (settings.AllowPageZoom && CalculateZoomPercentage() > 25.0)
		{
			zoomLevel -= 0.2;
			Control.Zoom(zoomLevel);
			window.UpdateZoomLevel(CalculateZoomPercentage());
			((ILogger)logger).Debug($"Decreased page zoom to {CalculateZoomPercentage()}%.");
		}
	}

	private void ZoomResetRequested()
	{
		if (settings.AllowPageZoom)
		{
			zoomLevel = 0.0;
			Control.Zoom(0.0);
			window.UpdateZoomLevel(CalculateZoomPercentage());
			((ILogger)logger).Debug($"Reset page zoom to {CalculateZoomPercentage()}%.");
		}
	}

	private double CalculateZoomPercentage()
	{
		return zoomLevel * 25.0 + 100.0;
	}
}
