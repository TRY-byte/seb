using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Core.Contracts.Notifications;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings.Applications;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.SystemComponents.Contracts.Audio;
using SafeExamBrowser.SystemComponents.Contracts.Keyboard;
using SafeExamBrowser.SystemComponents.Contracts.Network;
using SafeExamBrowser.SystemComponents.Contracts.PowerSupply;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Client.Operations;

internal class ShellOperation : ClientOperation
{
	private readonly IActionCenter actionCenter;

	private readonly IAudio audio;

	private readonly INotification aboutNotification;

	private readonly IKeyboard keyboard;

	private readonly ILogger logger;

	private readonly INotification logNotification;

	private readonly INativeMethods nativeMethods;

	private readonly INetworkAdapter networkAdapter;

	private readonly IPowerSupply powerSupply;

	private readonly ISystemInfo systemInfo;

	private readonly ITaskbar taskbar;

	private readonly ITaskview taskview;

	private readonly IText text;

	private readonly IUserInterfaceFactory uiFactory;

	[CompilerGenerated]
	private new StatusChangedEventHandler m_StatusChanged;

	public override event StatusChangedEventHandler StatusChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public ShellOperation(IActionCenter actionCenter, IAudio audio, INotification aboutNotification, ClientContext context, IKeyboard keyboard, ILogger logger, INotification logNotification, INativeMethods nativeMethods, INetworkAdapter networkAdapter, IPowerSupply powerSupply, ISystemInfo systemInfo, ITaskbar taskbar, ITaskview taskview, IText text, IUserInterfaceFactory uiFactory)
		: base(context)
	{
		this.aboutNotification = aboutNotification;
		this.actionCenter = actionCenter;
		this.audio = audio;
		this.keyboard = keyboard;
		this.logger = logger;
		this.logNotification = logNotification;
		this.nativeMethods = nativeMethods;
		this.networkAdapter = networkAdapter;
		this.powerSupply = powerSupply;
		this.systemInfo = systemInfo;
		this.text = text;
		this.taskbar = taskbar;
		this.taskview = taskview;
		this.uiFactory = uiFactory;
	}

	public override OperationResult Perform()
	{
		logger.Info("Initializing shell...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)191);
		}
		InitializeSystemComponents();
		InitializeActionCenter();
		InitializeTaskbar();
		InitializeActivators();
		InitializeAlwaysOnState();
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		logger.Info("Terminating shell...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)208);
		}
		TerminateActivators();
		TerminateNotifications();
		TerminateSystemComponents();
		return (OperationResult)3;
	}

	private void InitializeActivators()
	{
		foreach (IActivator activator in Context.Activators)
		{
			if (Context.Settings.UserInterface.ActionCenter.EnableActionCenter)
			{
				IActionCenterActivator val = (IActionCenterActivator)(object)((activator is IActionCenterActivator) ? activator : null);
				if (val != null)
				{
					actionCenter.Register(val);
					((IActivator)val).Start();
				}
			}
			if (Context.Settings.Security.AllowTermination)
			{
				ITerminationActivator val2 = (ITerminationActivator)(object)((activator is ITerminationActivator) ? activator : null);
				if (val2 != null)
				{
					((IActivator)val2).Start();
				}
			}
			if (Context.Settings.UserInterface.Taskbar.EnableTaskbar)
			{
				ITaskbarActivator val3 = (ITaskbarActivator)(object)((activator is ITaskbarActivator) ? activator : null);
				if (val3 != null)
				{
					taskbar.Register(val3);
					((IActivator)val3).Start();
				}
			}
		}
	}

	private void InitializeActionCenter()
	{
		if (Context.Settings.UserInterface.ActionCenter.EnableActionCenter)
		{
			logger.Info("Initializing action center...");
			actionCenter.InitializeText(text);
			InitializeApplicationsFor((Location)0);
			InitializeAboutNotificationForActionCenter();
			InitializeAudioForActionCenter();
			InitializeClockForActionCenter();
			InitializeLogNotificationForActionCenter();
			InitializeKeyboardLayoutForActionCenter();
			InitializeNetworkForActionCenter();
			InitializePowerSupplyForActionCenter();
			InitializeQuitButtonForActionCenter();
		}
		else
		{
			logger.Info("Action center is disabled, skipping initialization.");
		}
	}

	private void InitializeAlwaysOnState()
	{
		bool alwaysOn = Context.Settings.Display.AlwaysOn;
		bool alwaysOn2 = Context.Settings.System.AlwaysOn;
		nativeMethods.SetAlwaysOnState(alwaysOn, alwaysOn2);
		logger.Info("Display(s) will " + (alwaysOn ? "be always on" : "use the operating system configuration and may turn off") + ".");
		logger.Info("System will " + (alwaysOn2 ? "be always on" : "use the operating system configuration and may enter sleep mode or standby") + ".");
	}

	private void InitializeTaskbar()
	{
		if (Context.Settings.UserInterface.Taskbar.EnableTaskbar)
		{
			logger.Info("Initializing taskbar...");
			taskbar.InitializeText(text);
			InitializeApplicationsFor((Location)1);
			InitializeAboutNotificationForTaskbar();
			InitializeLogNotificationForTaskbar();
			InitializePowerSupplyForTaskbar();
			InitializeNetworkForTaskbar();
			InitializeAudioForTaskbar();
			InitializeKeyboardLayoutForTaskbar();
			InitializeClockForTaskbar();
			InitializeQuitButtonForTaskbar();
		}
		else
		{
			logger.Info("Taskbar is disabled, skipping initialization.");
		}
	}

	private void InitializeTaskview()
	{
		logger.Info("Initializing task view...");
		foreach (IApplication<IApplicationWindow> application in Context.Applications)
		{
			taskview.Add(application);
		}
	}

	private void InitializeApplicationsFor(Location location)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Invalid comparison between Unknown and I4
		foreach (IApplication<IApplicationWindow> application in Context.Applications)
		{
			if (!Context.Settings.Applications.Whitelist.First((WhitelistApplication a) => a.Id == application.Id).ShowInShell)
			{
				continue;
			}
			IApplicationControl val = uiFactory.CreateApplicationControl(application, location);
			if ((int)location != 0)
			{
				if ((int)location == 1)
				{
					taskbar.AddApplicationControl(val, false);
				}
			}
			else
			{
				actionCenter.AddApplicationControl(val, false);
			}
		}
	}

	private void InitializeSystemComponents()
	{
		((ISystemComponent)audio).Initialize();
		((ISystemComponent)keyboard).Initialize();
		((ISystemComponent)networkAdapter).Initialize();
		((ISystemComponent)powerSupply).Initialize();
	}

	private void InitializeAboutNotificationForActionCenter()
	{
		if (Context.Settings.UserInterface.ActionCenter.ShowApplicationInfo)
		{
			actionCenter.AddNotificationControl(uiFactory.CreateNotificationControl(aboutNotification, (Location)0));
		}
	}

	private void InitializeAboutNotificationForTaskbar()
	{
		if (Context.Settings.UserInterface.Taskbar.ShowApplicationInfo)
		{
			taskbar.AddNotificationControl(uiFactory.CreateNotificationControl(aboutNotification, (Location)1));
		}
	}

	private void InitializeAudioForActionCenter()
	{
		if (Context.Settings.UserInterface.ActionCenter.ShowAudio)
		{
			actionCenter.AddSystemControl(uiFactory.CreateAudioControl(audio, (Location)0));
		}
	}

	private void InitializeAudioForTaskbar()
	{
		if (Context.Settings.UserInterface.Taskbar.ShowAudio)
		{
			taskbar.AddSystemControl(uiFactory.CreateAudioControl(audio, (Location)1));
		}
	}

	private void InitializeClockForActionCenter()
	{
		actionCenter.ShowClock = Context.Settings.UserInterface.ActionCenter.ShowClock;
	}

	private void InitializeClockForTaskbar()
	{
		taskbar.ShowClock = Context.Settings.UserInterface.Taskbar.ShowClock;
	}

	private void InitializeLogNotificationForActionCenter()
	{
		if (Context.Settings.UserInterface.ActionCenter.ShowApplicationLog)
		{
			actionCenter.AddNotificationControl(uiFactory.CreateNotificationControl(logNotification, (Location)0));
		}
	}

	private void InitializeLogNotificationForTaskbar()
	{
		if (Context.Settings.UserInterface.Taskbar.ShowApplicationLog)
		{
			taskbar.AddNotificationControl(uiFactory.CreateNotificationControl(logNotification, (Location)1));
		}
	}

	private void InitializeKeyboardLayoutForActionCenter()
	{
		if (Context.Settings.UserInterface.ActionCenter.ShowKeyboardLayout)
		{
			actionCenter.AddSystemControl(uiFactory.CreateKeyboardLayoutControl(keyboard, (Location)0));
		}
	}

	private void InitializeKeyboardLayoutForTaskbar()
	{
		if (Context.Settings.UserInterface.Taskbar.ShowKeyboardLayout)
		{
			taskbar.AddSystemControl(uiFactory.CreateKeyboardLayoutControl(keyboard, (Location)1));
		}
	}

	private void InitializePowerSupplyForActionCenter()
	{
		if (systemInfo.HasBattery)
		{
			actionCenter.AddSystemControl(uiFactory.CreatePowerSupplyControl(powerSupply, (Location)0));
		}
	}

	private void InitializePowerSupplyForTaskbar()
	{
		if (systemInfo.HasBattery)
		{
			taskbar.AddSystemControl(uiFactory.CreatePowerSupplyControl(powerSupply, (Location)1));
		}
	}

	private void InitializeQuitButtonForActionCenter()
	{
		actionCenter.ShowQuitButton = Context.Settings.Security.AllowTermination;
	}

	private void InitializeQuitButtonForTaskbar()
	{
		taskbar.ShowQuitButton = Context.Settings.Security.AllowTermination;
	}

	private void InitializeNetworkForActionCenter()
	{
		if (Context.Settings.UserInterface.ActionCenter.ShowNetwork)
		{
			actionCenter.AddSystemControl(uiFactory.CreateNetworkControl(networkAdapter, (Location)0));
		}
	}

	private void InitializeNetworkForTaskbar()
	{
		if (Context.Settings.UserInterface.Taskbar.ShowNetwork)
		{
			taskbar.AddSystemControl(uiFactory.CreateNetworkControl(networkAdapter, (Location)1));
		}
	}

	private void TerminateActivators()
	{
		foreach (IActivator activator in Context.Activators)
		{
			activator.Stop();
		}
	}

	private void TerminateNotifications()
	{
		aboutNotification.Terminate();
		logNotification.Terminate();
	}

	private void TerminateSystemComponents()
	{
		((ISystemComponent)audio).Terminate();
		((ISystemComponent)keyboard).Terminate();
		((ISystemComponent)networkAdapter).Terminate();
		((ISystemComponent)powerSupply).Terminate();
	}
}
