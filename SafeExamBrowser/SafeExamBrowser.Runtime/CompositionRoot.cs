using System;
using System.Collections.Generic;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Communication.Hosts;
using SafeExamBrowser.Communication.Proxies;
using SafeExamBrowser.Configuration;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.DataCompression;
using SafeExamBrowser.Configuration.Contracts.DataFormats;
using SafeExamBrowser.Configuration.Contracts.DataResources;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Configuration.Cryptography;
using SafeExamBrowser.Configuration.DataCompression;
using SafeExamBrowser.Configuration.DataFormats;
using SafeExamBrowser.Configuration.DataResources;
using SafeExamBrowser.Configuration.Integrity;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.Core.Operations;
using SafeExamBrowser.Core.ResponsibilityModel;
using SafeExamBrowser.I18n;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring;
using SafeExamBrowser.Monitoring.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Display;
using SafeExamBrowser.Monitoring.Contracts.System;
using SafeExamBrowser.Monitoring.Display;
using SafeExamBrowser.Monitoring.System;
using SafeExamBrowser.Runtime.Communication;
using SafeExamBrowser.Runtime.Operations.Bootstrap;
using SafeExamBrowser.Runtime.Operations.Session;
using SafeExamBrowser.Runtime.Responsibilities;
using SafeExamBrowser.Server;
using SafeExamBrowser.Server.Contracts;
using SafeExamBrowser.Settings.Logging;
using SafeExamBrowser.SystemComponents;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.SystemComponents.Contracts.Network;
using SafeExamBrowser.SystemComponents.Contracts.PowerSupply;
using SafeExamBrowser.SystemComponents.Contracts.Registry;
using SafeExamBrowser.SystemComponents.Registry;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Desktop;
using SafeExamBrowser.WindowsApi;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Desktops;
using SafeExamBrowser.WindowsApi.Processes;

namespace SafeExamBrowser.Runtime;

internal class CompositionRoot
{
	private const int FIVE_SECONDS = 5000;

	private const int THIRTY_SECONDS = 30000;

	private AppConfig appConfig;

	private IConfigurationRepository repository;

	private ILogger logger;

	private ISystemInfo systemInfo;

	private IText text;

	internal RuntimeController RuntimeController { get; private set; }

	internal void BuildObjectGraph(Action shutdown)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected Obj, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected Obj, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected Obj, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected Obj, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected Obj, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected Obj, but got Unknown
		logger = (ILogger)new Logger();
		InitializeConfiguration();
		InitializeLogging();
		InitializeText();
		UserInterfaceFactory val = new UserInterfaceFactory(text, (IWindowGuard)null);
		RuntimeContext runtimeContext = new RuntimeContext();
		IntegrityModule integrityModule = new IntegrityModule(appConfig, (ILogger)(object)ModuleLogger("IntegrityModule"));
		MessageBoxFactory messageBox = new MessageBoxFactory(text);
		Registry val2 = new Registry((ILogger)(object)ModuleLogger("Registry"));
		RuntimeHost runtimeHost = new RuntimeHost(appConfig.RuntimeAddress, (IHostObjectFactory)new HostObjectFactory(), (ILogger)(object)ModuleLogger("RuntimeHost"), 5000);
		IRuntimeWindow runtimeWindow = val.CreateRuntimeWindow(appConfig);
		ServiceProxy serviceProxy = new ServiceProxy(appConfig.ServiceAddress, (IProxyObjectFactory)new ProxyObjectFactory(), (ILogger)(object)ModuleLogger("ServiceProxy"), (Interlocutor)2);
		ISplashScreen splashScreen = val.CreateSplashScreen(appConfig);
		systemInfo = (ISystemInfo)new SystemInfo((IRegistry)(object)val2);
		BootstrapOperationSequence bootstrapSequence = BuildBootstrapOperations((IIntegrityModule)(object)integrityModule, (IRuntimeHost)(object)runtimeHost, splashScreen);
		SessionOperationSequence sessionSequence = BuildSessionOperations((IIntegrityModule)(object)integrityModule, (IMessageBox)(object)messageBox, (IRegistry)(object)val2, (IRuntimeHost)(object)runtimeHost, runtimeWindow, (IServiceProxy)(object)serviceProxy, runtimeContext, (IUserInterfaceFactory)(object)val);
		RuntimeController = new RuntimeController(responsibilities: runtimeContext.Responsibilities = (IResponsibilityCollection<RuntimeTask>)(object)BuildResponsibilities((IIntegrityModule)(object)integrityModule, (IMessageBox)(object)messageBox, (IRuntimeHost)(object)runtimeHost, runtimeWindow, (IServiceProxy)(object)serviceProxy, runtimeContext, sessionSequence, shutdown, splashScreen), logger: logger, bootstrapSequence: (IOperationSequence)(object)bootstrapSequence, runtimeContext: runtimeContext, runtimeWindow: runtimeWindow, splashScreen: splashScreen);
	}

	private BootstrapOperationSequence BuildBootstrapOperations(IIntegrityModule integrityModule, IRuntimeHost runtimeHost, ISplashScreen splashScreen)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected Obj, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected Obj, but got Unknown
		Queue<IOperation> queue = new Queue<IOperation>();
		queue.Enqueue((IOperation)new I18nOperation(logger, text));
		queue.Enqueue((IOperation)new CommunicationHostOperation((ICommunicationHost)(object)runtimeHost, logger));
		queue.Enqueue((IOperation)(object)new ApplicationIntegrityOperation(integrityModule, logger));
		return new BootstrapOperationSequence(logger, queue, splashScreen);
	}

	private ResponsibilityCollection<RuntimeTask> BuildResponsibilities(IIntegrityModule integrityModule, IMessageBox messageBox, IRuntimeHost runtimeHost, IRuntimeWindow runtimeWindow, IServiceProxy serviceProxy, RuntimeContext runtimeContext, SessionOperationSequence sessionSequence, Action shutdown, ISplashScreen splashScreen)
	{
		Queue<RuntimeResponsibility> queue = new Queue<RuntimeResponsibility>();
		queue.Enqueue(new ClientResponsibility((ILogger)(object)ModuleLogger("ClientResponsibility"), messageBox, runtimeContext, runtimeWindow, shutdown));
		queue.Enqueue(new CommunicationResponsibility((ILogger)(object)ModuleLogger("CommunicationResponsibility"), runtimeContext, runtimeHost, shutdown));
		queue.Enqueue(new ErrorMessageResponsibility(appConfig, (ILogger)(object)ModuleLogger("ErrorMessageResponsibility"), messageBox, runtimeContext, splashScreen, text));
		queue.Enqueue(new IntegrityResponsibility(integrityModule, (ILogger)(object)ModuleLogger("IntegrityResponsibility"), runtimeContext, shutdown));
		queue.Enqueue(new ServiceResponsibility((ILogger)(object)ModuleLogger("ServiceResponsibility"), messageBox, runtimeContext, runtimeWindow, serviceProxy, shutdown));
		queue.Enqueue(new SessionResponsibility(appConfig, (ILogger)(object)ModuleLogger("SessionResponsibility"), messageBox, runtimeContext, runtimeWindow, (IRepeatableOperationSequence)(object)sessionSequence, shutdown, text));
		return new ResponsibilityCollection<RuntimeTask>(logger, (IEnumerable<IResponsibility<RuntimeTask>>)queue);
	}

	private SessionOperationSequence BuildSessionOperations(IIntegrityModule integrityModule, IMessageBox messageBox, IRegistry registry, IRuntimeHost runtimeHost, IRuntimeWindow runtimeWindow, IServiceProxy serviceProxy, RuntimeContext runtimeContext, IUserInterfaceFactory uiFactory)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected Obj, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected Obj, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected Obj, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected Obj, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected Obj, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected Obj, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected Obj, but got Unknown
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		FileSystem fileSystem = new FileSystem();
		NativeMethods val = new NativeMethods();
		Queue<SessionOperation> queue = new Queue<SessionOperation>();
		UserInfo val2 = new UserInfo((ILogger)(object)ModuleLogger("UserInfo"));
		Dependencies dependencies = new Dependencies(new ClientBridge(runtimeHost, runtimeContext), logger, messageBox, runtimeWindow, runtimeContext, text);
		DesktopFactory desktopFactory = new DesktopFactory((ILogger)(object)ModuleLogger("DesktopFactory"));
		DesktopMonitor desktopMonitor = new DesktopMonitor((ILogger)(object)ModuleLogger("DesktopMonitor"));
		DisplayMonitor displayMonitor = new DisplayMonitor((ILogger)(object)ModuleLogger("DisplayMonitor"), (INativeMethods)(object)val, systemInfo);
		ExplorerShell explorerShell = new ExplorerShell((ILogger)(object)ModuleLogger("ExplorerShell"), (INativeMethods)(object)val);
		KeyGenerator keyGenerator = new KeyGenerator(appConfig, integrityModule, (ILogger)(object)ModuleLogger("KeyGenerator"));
		ProcessFactory processFactory = new ProcessFactory(ModuleLogger("ProcessFactory"));
		ProxyFactory proxyFactory = new ProxyFactory((IProxyObjectFactory)new ProxyObjectFactory(), ModuleLogger("ProxyFactory"));
		RemoteSessionDetector detector = new RemoteSessionDetector(integrityModule, (ILogger)(object)ModuleLogger("RemoteSessionDetector"));
		SystemSentinel sentinel = new SystemSentinel((ILogger)(object)ModuleLogger("SystemSentinel"), (INativeMethods)(object)val, registry);
		ServerProxy server = new ServerProxy(appConfig, (IKeyGenerator)(object)keyGenerator, (ILogger)(object)ModuleLogger("ServerProxy"), systemInfo, (IUserInfo)(object)val2, (IPowerSupply)null, (INetworkAdapter)null);
		VirtualMachineDetector detector2 = new VirtualMachineDetector(integrityModule, (ILogger)(object)ModuleLogger("VirtualMachineDetector"), registry, systemInfo);
		queue.Enqueue(new SessionInitializationOperation(dependencies, (IFileSystem)(object)fileSystem, repository));
		queue.Enqueue(new ConfigurationOperation(commandLineArgs, dependencies, (IFileSystem)new FileSystem(), (IHashAlgorithm)(object)new HashAlgorithm(), repository, uiFactory));
		queue.Enqueue(new ServerOperation(dependencies, (IFileSystem)(object)fileSystem, repository, (IServerProxy)(object)server, uiFactory));
		queue.Enqueue(new VersionRestrictionOperation(dependencies));
		queue.Enqueue(new DisclaimerOperation(dependencies));
		queue.Enqueue(new RemoteSessionOperation(dependencies, (IRemoteSessionDetector)(object)detector));
		queue.Enqueue(new SessionIntegrityOperation(dependencies, (ISystemSentinel)(object)sentinel));
		queue.Enqueue(new VirtualMachineOperation(dependencies, (IVirtualMachineDetector)(object)detector2));
		queue.Enqueue(new DisplayMonitorOperation(dependencies, (IDisplayMonitor)(object)displayMonitor));
		queue.Enqueue(new ServiceOperation(dependencies, runtimeHost, serviceProxy, 30000, (IUserInfo)(object)val2));
		queue.Enqueue(new ClientTerminationOperation(dependencies, (IProcessFactory)(object)processFactory, (IProxyFactory)(object)proxyFactory, runtimeHost, 30000));
		queue.Enqueue(new KioskModeOperation(dependencies, (IDesktopFactory)(object)desktopFactory, (IDesktopMonitor)(object)desktopMonitor, (IExplorerShell)(object)explorerShell, (IProcessFactory)(object)processFactory));
		queue.Enqueue(new ClientOperation(dependencies, (IProcessFactory)(object)processFactory, (IProxyFactory)(object)proxyFactory, runtimeHost, 30000));
		queue.Enqueue(new SessionActivationOperation(dependencies));
		return new SessionOperationSequence(logger, queue, runtimeWindow);
	}

	internal void LogStartupInformation()
	{
		logger.Log("/* " + appConfig.ProgramTitle + ", Version " + appConfig.ProgramInformationalVersion + ", Build " + appConfig.ProgramBuildVersion);
		logger.Log("/* " + appConfig.ProgramCopyright);
		logger.Log("/* ");
		logger.Log("/* Please visit https://www.github.com/SafeExamBrowser for more information.");
		logger.Log(string.Empty);
		logger.Log($"# Application started at {appConfig.ApplicationStartTime:yyyy-MM-dd HH:mm:ss.fff}");
		logger.Log("# Running on " + systemInfo.OperatingSystemInfo);
		logger.Log("# Computer '" + systemInfo.Name + "' is a " + systemInfo.Model + " manufactured by " + systemInfo.Manufacturer);
		logger.Log($"# Runtime-ID: {appConfig.RuntimeId}");
		logger.Log(string.Empty);
	}

	internal void LogShutdownInformation()
	{
		ILogger val = logger;
		if (val != null)
		{
			val.Log($"# Application terminated at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
		}
	}

	private void InitializeConfiguration()
	{
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected Obj, but got Unknown
		CertificateStore certificateStore = new CertificateStore((ILogger)(object)ModuleLogger("CertificateStore"));
		GZipCompressor compressor = new GZipCompressor((ILogger)(object)ModuleLogger("GZipCompressor"));
		PasswordEncryption passwordEncryption = new PasswordEncryption((ILogger)(object)ModuleLogger("PasswordEncryption"));
		PublicKeyEncryption publicKeyEncryption = new PublicKeyEncryption((ICertificateStore)(object)certificateStore, (ILogger)(object)ModuleLogger("PublicKeyEncryption"));
		PublicKeySymmetricEncryption publicKeySymmetricEncryption = new PublicKeySymmetricEncryption((ICertificateStore)(object)certificateStore, (ILogger)(object)ModuleLogger("PublicKeySymmetricEncryption"), passwordEncryption);
		IModuleLogger val = ModuleLogger("ConfigurationRepository");
		XmlParser xmlParser = new XmlParser((IDataCompressor)(object)compressor, (ILogger)(object)ModuleLogger("XmlParser"));
		XmlSerializer xmlSerializer = new XmlSerializer((ILogger)(object)ModuleLogger("XmlSerializer"));
		repository = (IConfigurationRepository)(object)new ConfigurationRepository((ICertificateStore)(object)certificateStore, val);
		appConfig = repository.InitializeAppConfig();
		repository.Register((IDataParser)(object)new BinaryParser((IDataCompressor)(object)compressor, (IHashAlgorithm)(object)new HashAlgorithm(), (ILogger)(object)ModuleLogger("BinaryParser"), (IPasswordEncryption)(object)passwordEncryption, (IPublicKeyEncryption)(object)publicKeyEncryption, (IPublicKeyEncryption)(object)publicKeySymmetricEncryption, (IDataParser)(object)xmlParser));
		repository.Register((IDataSerializer)(object)new BinarySerializer((IDataCompressor)(object)compressor, (ILogger)(object)ModuleLogger("BinarySerializer"), (IPasswordEncryption)(object)passwordEncryption, (IPublicKeyEncryption)(object)publicKeyEncryption, (IPublicKeyEncryption)(object)publicKeySymmetricEncryption, (IDataSerializer)(object)xmlSerializer));
		repository.Register((IDataParser)(object)new XmlParser((IDataCompressor)(object)compressor, (ILogger)(object)ModuleLogger("XmlParser")));
		repository.Register((IDataSerializer)(object)new XmlSerializer((ILogger)(object)ModuleLogger("XmlSerializer")));
		repository.Register((IResourceLoader)(object)new FileResourceLoader((ILogger)(object)ModuleLogger("FileResourceLoader")));
		repository.Register((IResourceSaver)(object)new FileResourceSaver((ILogger)(object)ModuleLogger("FileResourceSaver")));
		repository.Register((IResourceLoader)(object)new NetworkResourceLoader(appConfig, (ILogger)new ModuleLogger(logger, "NetworkResourceLoader")));
	}

	private void InitializeLogging()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected Obj, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		LogFileWriter val = new LogFileWriter((ILogContentFormatter)new DefaultLogFormatter(), appConfig.RuntimeLogFilePath);
		val.Initialize();
		logger.LogLevel = (LogLevel)0;
		logger.Subscribe((ILogObserver)(object)val);
	}

	private void InitializeText()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		text = (IText)new Text((ILogger)(object)ModuleLogger("Text"));
	}

	private IModuleLogger ModuleLogger(string moduleInfo)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		return (IModuleLogger)new ModuleLogger(logger, moduleInfo);
	}
}
