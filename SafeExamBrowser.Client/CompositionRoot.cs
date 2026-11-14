using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SafeExamBrowser.Applications;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Browser;
using SafeExamBrowser.Browser.Contracts;
using SafeExamBrowser.Client.Communication;
using SafeExamBrowser.Client.Contracts;
using SafeExamBrowser.Client.Notifications;
using SafeExamBrowser.Client.Operations;
using SafeExamBrowser.Client.Responsibilities;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Communication.Hosts;
using SafeExamBrowser.Communication.Proxies;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Configuration.Cryptography;
using SafeExamBrowser.Configuration.Integrity;
using SafeExamBrowser.Core.Contracts.Notifications;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.Core.Operations;
using SafeExamBrowser.Core.ResponsibilityModel;
using SafeExamBrowser.I18n;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring;
using SafeExamBrowser.Monitoring.Applications;
using SafeExamBrowser.Monitoring.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Applications;
using SafeExamBrowser.Monitoring.Contracts.Display;
using SafeExamBrowser.Monitoring.Contracts.Keyboard;
using SafeExamBrowser.Monitoring.Contracts.Mouse;
using SafeExamBrowser.Monitoring.Contracts.System;
using SafeExamBrowser.Monitoring.Display;
using SafeExamBrowser.Monitoring.Keyboard;
using SafeExamBrowser.Monitoring.Mouse;
using SafeExamBrowser.Monitoring.System;
using SafeExamBrowser.Proctoring;
using SafeExamBrowser.Proctoring.Contracts;
using SafeExamBrowser.Server;
using SafeExamBrowser.Server.Contracts;
using SafeExamBrowser.Settings.Logging;
using SafeExamBrowser.Settings.UserInterface;
using SafeExamBrowser.SystemComponents;
using SafeExamBrowser.SystemComponents.Audio;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.SystemComponents.Contracts.Audio;
using SafeExamBrowser.SystemComponents.Contracts.Keyboard;
using SafeExamBrowser.SystemComponents.Contracts.Network;
using SafeExamBrowser.SystemComponents.Contracts.PowerSupply;
using SafeExamBrowser.SystemComponents.Contracts.Registry;
using SafeExamBrowser.SystemComponents.Keyboard;
using SafeExamBrowser.SystemComponents.Network;
using SafeExamBrowser.SystemComponents.PowerSupply;
using SafeExamBrowser.SystemComponents.Registry;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.FileSystemDialog;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Desktop;
using SafeExamBrowser.UserInterface.Mobile;
using SafeExamBrowser.UserInterface.Shared;
using SafeExamBrowser.UserInterface.Shared.Activators;
using SafeExamBrowser.WindowsApi;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Processes;

namespace SafeExamBrowser.Client;

internal class CompositionRoot
{
	private const int TWO_SECONDS = 2000;

	private const int FIVE_SECONDS = 5000;

	private Guid authenticationToken;

	private ClientContext context;

	private string logFilePath;

	private LogLevel logLevel;

	private string runtimeHostUri;

	private UserInterfaceMode uiMode;

	private IActionCenter actionCenter;

	private ApplicationMonitor applicationMonitor;

	private ILogger logger;

	private IMessageBox messageBox;

	private INativeMethods nativeMethods;

	private INetworkAdapter networkAdapter;

	private ISplashScreen splashScreen;

	private IPowerSupply powerSupply;

	private ISystemInfo systemInfo;

	private ITaskbar taskbar;

	private ITaskview taskview;

	private IUserInfo userInfo;

	private IText text;

	private WindowGuard windowGuard;

	private IUserInterfaceFactory uiFactory;

	internal ClientController ClientController { get; private set; }

	internal void BuildObjectGraph(Action shutdown)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected Obj, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected Obj, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected Obj, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected Obj, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected Obj, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected Obj, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected Obj, but got Unknown
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected Obj, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected Obj, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected Obj, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected Obj, but got Unknown
		ValidateCommandLineArguments();
		InitializeLogging();
		InitializeText();
		ProcessFactory val = new ProcessFactory(ModuleLogger("ProcessFactory"));
		windowGuard = new WindowGuard((ILogger)(object)ModuleLogger("WindowGuard"));
		uiFactory = BuildUserInterfaceFactory();
		actionCenter = uiFactory.CreateActionCenter();
		context = new ClientContext();
		messageBox = BuildMessageBox();
		nativeMethods = (INativeMethods)new NativeMethods();
		applicationMonitor = new ApplicationMonitor(2000, (ILogger)(object)ModuleLogger("ApplicationMonitor"), nativeMethods, (IProcessFactory)(object)val);
		networkAdapter = (INetworkAdapter)new NetworkAdapter((ILogger)(object)ModuleLogger("NetworkAdapter"), nativeMethods);
		splashScreen = uiFactory.CreateSplashScreen((AppConfig)null);
		systemInfo = (ISystemInfo)new SystemInfo((IRegistry)new Registry((ILogger)(object)ModuleLogger("Registry")));
		taskbar = uiFactory.CreateTaskbar((ILogger)(object)ModuleLogger("Taskbar"));
		taskview = uiFactory.CreateTaskview();
		userInfo = (IUserInfo)new UserInfo((ILogger)(object)ModuleLogger("UserInfo"));
		ApplicationFactory applicationFactory = new ApplicationFactory((IApplicationMonitor)(object)applicationMonitor, ModuleLogger("ApplicationFactory"), nativeMethods, (IProcessFactory)(object)val, (IRegistry)new Registry((ILogger)(object)ModuleLogger("Registry")));
		Clipboard clipboard = new Clipboard((ILogger)(object)ModuleLogger("Clipboard"), nativeMethods);
		Coordinator coordinator = new Coordinator();
		DisplayMonitor displayMonitor = new DisplayMonitor((ILogger)(object)ModuleLogger("DisplayMonitor"), nativeMethods, systemInfo);
		ExplorerShell explorerShell = new ExplorerShell((ILogger)(object)ModuleLogger("ExplorerShell"), nativeMethods);
		IFileSystemDialog fileSystemDialog = BuildFileSystemDialog();
		RuntimeProxy val2 = new RuntimeProxy(runtimeHostUri, (IProxyObjectFactory)new ProxyObjectFactory(), (ILogger)(object)ModuleLogger("RuntimeProxy"), (Interlocutor)1);
		SystemSentinel sentinel = new SystemSentinel((ILogger)(object)ModuleLogger("SystemSentinel"), nativeMethods, (IRegistry)new Registry((ILogger)(object)ModuleLogger("Registry")));
		ClientOperationSequence operations = BuildOperations((IApplicationFactory)(object)applicationFactory, (IClipboard)(object)clipboard, (IDisplayMonitor)(object)displayMonitor, fileSystemDialog, (IRuntimeProxy)(object)val2);
		ResponsibilityCollection<ClientTask> responsibilities = BuildResponsibilities(coordinator, (IDisplayMonitor)(object)displayMonitor, (IExplorerShell)(object)explorerShell, (IRuntimeProxy)(object)val2, (ISystemSentinel)(object)sentinel, shutdown);
		context.HashAlgorithm = (IHashAlgorithm)(object)new HashAlgorithm();
		context.MessageBox = messageBox;
		context.Responsibilities = (IResponsibilityCollection<ClientTask>)(object)responsibilities;
		context.Runtime = (IRuntimeProxy)(object)val2;
		context.UserInterfaceFactory = uiFactory;
		ClientController = new ClientController(context, logger, (IOperationSequence)(object)operations, (IResponsibilityCollection<ClientTask>)(object)responsibilities, (IRuntimeProxy)(object)val2, splashScreen);
	}

	internal void LogStartupInformation()
	{
		logger.Log($"# New client instance started at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
		logger.Log(string.Empty);
	}

	internal void LogShutdownInformation()
	{
		ILogger val = logger;
		if (val != null)
		{
			val.Log($"# Client instance terminated at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
		}
	}

	private ClientOperationSequence BuildOperations(IApplicationFactory applicationFactory, IClipboard clipboard, IDisplayMonitor displayMonitor, IFileSystemDialog fileSystemDialog, IRuntimeProxy runtimeProxy)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected Obj, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected Obj, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected Obj, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected Obj, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected Obj, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected Obj, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected Obj, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected Obj, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected Obj, but got Unknown
		Queue<IOperation> queue = new Queue<IOperation>();
		queue.Enqueue((IOperation)new I18nOperation(logger, text));
		queue.Enqueue((IOperation)(object)new RuntimeConnectionOperation(context, logger, runtimeProxy, authenticationToken));
		queue.Enqueue((IOperation)(object)new ConfigurationOperation(context, logger, runtimeProxy));
		queue.Enqueue((IOperation)(object)new WindowGuardOperation(context, logger, (IWindowGuard)(object)windowGuard));
		queue.Enqueue((IOperation)new DelegateOperation((Action)UpdateAppConfig, (Action)null, (Action)null));
		queue.Enqueue((IOperation)new DelegateOperation((Action)BuildIntegrityModule, (Action)null, (Action)null));
		queue.Enqueue((IOperation)new DelegateOperation((Action)BuildPowerSupply, (Action)null, (Action)null));
		queue.Enqueue((IOperation)new LazyInitializationOperation((Func<IOperation>)BuildClientHostOperation));
		queue.Enqueue((IOperation)(object)new ClientHostDisconnectionOperation(context, logger, 5000));
		queue.Enqueue((IOperation)new LazyInitializationOperation((Func<IOperation>)BuildKeyboardInterceptorOperation));
		queue.Enqueue((IOperation)new LazyInitializationOperation((Func<IOperation>)BuildMouseInterceptorOperation));
		queue.Enqueue((IOperation)(object)new PermissionOperation(context, logger, networkAdapter));
		queue.Enqueue((IOperation)(object)new ApplicationOperation(context, applicationFactory, fileSystemDialog, logger, messageBox, (IApplicationMonitor)(object)applicationMonitor, splashScreen, text));
		queue.Enqueue((IOperation)(object)new DisplayMonitorOperation(context, displayMonitor, logger, taskbar));
		queue.Enqueue((IOperation)new LazyInitializationOperation((Func<IOperation>)BuildShellOperation));
		queue.Enqueue((IOperation)new LazyInitializationOperation((Func<IOperation>)BuildBrowserOperation));
		queue.Enqueue((IOperation)new LazyInitializationOperation((Func<IOperation>)BuildServerOperation));
		queue.Enqueue((IOperation)new LazyInitializationOperation((Func<IOperation>)BuildProctoringOperation));
		queue.Enqueue((IOperation)(object)new ClipboardOperation(context, clipboard, logger));
		return new ClientOperationSequence(logger, queue, splashScreen);
	}

	private ResponsibilityCollection<ClientTask> BuildResponsibilities(ICoordinator coordinator, IDisplayMonitor displayMonitor, IExplorerShell explorerShell, IRuntimeProxy runtimeProxy, ISystemSentinel sentinel, Action shutdown)
	{
		Queue<ClientResponsibility> queue = new Queue<ClientResponsibility>();
		queue.Enqueue(new ApplicationsResponsibility(context, (ILogger)(object)ModuleLogger("ApplicationsResponsibility")));
		queue.Enqueue(new BrowserResponsibility(context, coordinator, (ILogger)(object)ModuleLogger("BrowserResponsibility"), messageBox, runtimeProxy, splashScreen, taskbar));
		queue.Enqueue(new CommunicationResponsibility(context, coordinator, (ILogger)(object)ModuleLogger("CommunicationResponsibility"), messageBox, runtimeProxy, shutdown, splashScreen, text, uiFactory));
		queue.Enqueue(new IntegrityResponsibility(context, coordinator, (ILogger)(object)ModuleLogger("IntegrityResponsibility"), text));
		queue.Enqueue(new MonitoringResponsibility(actionCenter, (IApplicationMonitor)(object)applicationMonitor, context, coordinator, displayMonitor, explorerShell, (ILogger)(object)ModuleLogger("MonitoringResponsibility"), sentinel, taskbar, text));
		queue.Enqueue(new NetworkResponsibility(context, (ILogger)(object)ModuleLogger("NetworkResponsibility"), networkAdapter, text, uiFactory));
		queue.Enqueue(new ProctoringResponsibility(context, (ILogger)(object)ModuleLogger("ProctoringResponsibility"), messageBox, uiFactory));
		queue.Enqueue(new ServerResponsibility(context, coordinator, (ILogger)(object)ModuleLogger("ServerResponsibility"), text));
		queue.Enqueue(new ShellResponsibility(actionCenter, context, (IHashAlgorithm)(object)new HashAlgorithm(), (ILogger)(object)ModuleLogger("ShellResponsibility"), messageBox, taskbar, uiFactory));
		return new ResponsibilityCollection<ClientTask>(logger, (IEnumerable<IResponsibility<ClientTask>>)queue);
	}

	private void ValidateCommandLineArguments()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		bool num = commandLineArgs != null && commandLineArgs.Length >= 5;
		bool flag = false;
		if (num)
		{
			string uriString = Encoding.UTF8.GetString(Convert.FromBase64String(commandLineArgs[1]));
			bool num2 = Uri.TryCreate(uriString, UriKind.Absolute, out var result) && result.IsFile;
			bool flag2 = Enum.TryParse<LogLevel>(commandLineArgs[2], out LogLevel result2);
			bool flag3 = Uri.TryCreate(commandLineArgs[3], UriKind.Absolute, out var result3) && result3.IsWellFormedOriginalString();
			bool flag4 = Guid.TryParse(commandLineArgs[4], out var result4);
			if (num2 & flag2 & flag3 & flag4)
			{
				authenticationToken = result4;
				logFilePath = uriString;
				logLevel = result2;
				runtimeHostUri = commandLineArgs[3];
				uiMode = (UserInterfaceMode)((commandLineArgs.Length == 6 && Enum.TryParse<UserInterfaceMode>(commandLineArgs[5], out uiMode)) ? ((int)uiMode) : 0);
				flag = true;
			}
		}
		if (!flag)
		{
			throw new ArgumentException("Invalid arguments! Required: SafeExamBrowser.Client.exe <logfile path> <log level> <host URI> <token>");
		}
	}

	private void InitializeLogging()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		LogFileWriter val = new LogFileWriter((ILogContentFormatter)new DefaultLogFormatter(), logFilePath);
		val.Initialize();
		logger = (ILogger)new Logger();
		logger.LogLevel = logLevel;
		logger.Subscribe((ILogObserver)(object)val);
	}

	private void InitializeText()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		text = (IText)new Text((ILogger)(object)ModuleLogger("Text"));
	}

	private IOperation BuildBrowserOperation()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		IFileSystemDialog fileSystemDialog = BuildFileSystemDialog();
		KeyGenerator keyGenerator = new KeyGenerator(context.AppConfig, context.IntegrityModule, (ILogger)(object)ModuleLogger("KeyGenerator"));
		IModuleLogger val = ModuleLogger("BrowserApplication");
		BrowserApplication browser = new BrowserApplication(context.AppConfig, context.Settings.Browser, fileSystemDialog, (IHashAlgorithm)(object)new HashAlgorithm(), (IKeyGenerator)(object)keyGenerator, messageBox, val, nativeMethods, context.Settings.SessionMode, text, uiFactory);
		BrowserOperation result = new BrowserOperation(actionCenter, context, logger, taskbar, taskview, uiFactory);
		context.Browser = (IBrowserApplication)(object)browser;
		return (IOperation)(object)result;
	}

	private IOperation BuildClientHostOperation()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected Obj, but got Unknown
		int id = Process.GetCurrentProcess().Id;
		HostObjectFactory factory = new HostObjectFactory();
		ClientHost clientHost = new ClientHost(context.AppConfig.ClientAddress, (IHostObjectFactory)(object)factory, (ILogger)(object)ModuleLogger("ClientHost"), id, 5000);
		CommunicationHostOperation val = new CommunicationHostOperation((ICommunicationHost)(object)clientHost, logger);
		context.ClientHost = (IClientHost)(object)clientHost;
		context.ClientHost.AuthenticationToken = authenticationToken;
		return (IOperation)val;
	}

	private void BuildIntegrityModule()
	{
		context.IntegrityModule = (IIntegrityModule)(object)new IntegrityModule(context.AppConfig, (ILogger)(object)ModuleLogger("IntegrityModule"));
	}

	private void BuildPowerSupply()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected Obj, but got Unknown
		powerSupply = (IPowerSupply)new PowerSupply((ILogger)(object)ModuleLogger("PowerSupply"), context.Settings.PowerSupply);
	}

	private IOperation BuildKeyboardInterceptorOperation()
	{
		KeyboardInterceptor keyboardInterceptor = new KeyboardInterceptor((ILogger)(object)ModuleLogger("KeyboardInterceptor"), nativeMethods, context.Settings.Keyboard);
		return (IOperation)(object)new KeyboardInterceptorOperation(context, (IKeyboardInterceptor)(object)keyboardInterceptor, logger);
	}

	private IOperation BuildMouseInterceptorOperation()
	{
		MouseInterceptor mouseInterceptor = new MouseInterceptor((ILogger)(object)ModuleLogger("MouseInterceptor"), nativeMethods, context.Settings.Mouse);
		return (IOperation)(object)new MouseInterceptorOperation(context, logger, (IMouseInterceptor)(object)mouseInterceptor);
	}

	private IOperation BuildProctoringOperation()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected Obj, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected Obj, but got Unknown
		ProctoringController val = new ProctoringController(context.AppConfig, (IApplicationMonitor)(object)applicationMonitor, context.Browser, (IFileSystem)new FileSystem(), ModuleLogger("ProctoringController"), nativeMethods, context.Server, text, uiFactory);
		ProctoringOperation result = new ProctoringOperation(actionCenter, context, (IProctoringController)(object)val, logger, taskbar, uiFactory);
		context.Proctoring = (IProctoringController)(object)val;
		return (IOperation)(object)result;
	}

	private IOperation BuildServerOperation()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected Obj, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected Obj, but got Unknown
		KeyGenerator keyGenerator = new KeyGenerator(context.AppConfig, context.IntegrityModule, (ILogger)(object)ModuleLogger("KeyGenerator"));
		ServerProxy val = new ServerProxy(context.AppConfig, (IKeyGenerator)(object)keyGenerator, (ILogger)(object)ModuleLogger("ServerProxy"), systemInfo, userInfo, powerSupply, networkAdapter);
		Invigilator invigilator = new Invigilator((ILogger)(object)ModuleLogger("Invigilator"), (IServerProxy)(object)val);
		ServerOperation result = new ServerOperation(actionCenter, context, (IInvigilator)(object)invigilator, logger, (IServerProxy)(object)val, taskbar, uiFactory);
		context.Server = (IServerProxy)(object)val;
		return (IOperation)(object)result;
	}

	private IOperation BuildShellOperation()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected Obj, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected Obj, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected Obj, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected Obj, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected Obj, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected Obj, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected Obj, but got Unknown
		AboutNotification aboutNotification = new AboutNotification(context.AppConfig, text, uiFactory);
		Audio audio = new Audio(context.Settings.Audio, (ILogger)(object)ModuleLogger("Audio"));
		Keyboard keyboard = new Keyboard((ILogger)(object)ModuleLogger("Keyboard"));
		LogNotification logNotification = new LogNotification(logger, text, uiFactory);
		ShellOperation result = new ShellOperation(actionCenter, (IAudio)(object)audio, (INotification)(object)aboutNotification, context, (IKeyboard)(object)keyboard, logger, (INotification)(object)logNotification, nativeMethods, networkAdapter, powerSupply, systemInfo, taskbar, taskview, text, uiFactory);
		context.Activators.Add((IActivator)new ActionCenterKeyboardActivator((ILogger)(object)ModuleLogger("ActionCenterKeyboardActivator"), nativeMethods));
		context.Activators.Add((IActivator)new ActionCenterTouchActivator((ILogger)(object)ModuleLogger("ActionCenterTouchActivator"), nativeMethods));
		context.Activators.Add((IActivator)new TaskbarKeyboardActivator((ILogger)(object)ModuleLogger("TaskbarKeyboardActivator"), nativeMethods));
		context.Activators.Add((IActivator)new TaskviewKeyboardActivator((ILogger)(object)ModuleLogger("TaskviewKeyboardActivator"), nativeMethods));
		context.Activators.Add((IActivator)new TerminationActivator((ILogger)(object)ModuleLogger("TerminationActivator"), nativeMethods));
		return (IOperation)(object)result;
	}

	private IFileSystemDialog BuildFileSystemDialog()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected Obj, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		if ((int)uiMode != 1)
		{
			return (IFileSystemDialog)new FileSystemDialogFactory(systemInfo, text, (IWindowGuard)(object)windowGuard);
		}
		return (IFileSystemDialog)new FileSystemDialogFactory(systemInfo, text, (IWindowGuard)(object)windowGuard);
	}

	private IMessageBox BuildMessageBox()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected Obj, but got Unknown
		if ((int)uiMode != 1)
		{
			return (IMessageBox)new MessageBoxFactory(text);
		}
		return (IMessageBox)new MessageBoxFactory(text);
	}

	private IUserInterfaceFactory BuildUserInterfaceFactory()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected Obj, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected Obj, but got Unknown
		if ((int)uiMode != 1)
		{
			return (IUserInterfaceFactory)new UserInterfaceFactory(text, (IWindowGuard)(object)windowGuard);
		}
		return (IUserInterfaceFactory)new UserInterfaceFactory(text, (IWindowGuard)(object)windowGuard);
	}

	private void UpdateAppConfig()
	{
		ClientController.UpdateAppConfig();
	}

	private IModuleLogger ModuleLogger(string moduleInfo)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		return (IModuleLogger)new ModuleLogger(logger, moduleInfo);
	}
}
