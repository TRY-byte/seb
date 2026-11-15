using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Browser.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Shell;

namespace SafeExamBrowser.Client.Operations;

internal class BrowserOperation : ClientOperation
{
	private readonly IActionCenter actionCenter;

	private readonly ILogger logger;

	private readonly ITaskbar taskbar;

	private readonly ITaskview taskview;

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

	public BrowserOperation(IActionCenter actionCenter, ClientContext context, ILogger logger, ITaskbar taskbar, ITaskview taskview, IUserInterfaceFactory uiFactory)
		: base(context)
	{
		this.actionCenter = actionCenter;
		this.logger = logger;
		this.taskbar = taskbar;
		this.taskview = taskview;
		this.uiFactory = uiFactory;
	}

	public override OperationResult Perform()
	{
		logger.Info("Initializing browser...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)181);
		}
		if (Context.Settings.Browser.EnableBrowser)
		{
			((IApplication<IBrowserWindow>)(object)Context.Browser).Initialize();
			if (Context.Settings.UserInterface.ActionCenter.EnableActionCenter)
			{
				actionCenter.AddApplicationControl(uiFactory.CreateApplicationControl((IApplication<IApplicationWindow>)(object)Context.Browser, (Location)0), true);
			}
			if (Context.Settings.UserInterface.Taskbar.EnableTaskbar)
			{
				taskbar.AddApplicationControl(uiFactory.CreateApplicationControl((IApplication<IApplicationWindow>)(object)Context.Browser, (Location)1), true);
			}
			taskview.Add((IApplication<IApplicationWindow>)(object)Context.Browser);
		}
		else
		{
			logger.Info("Browser application is disabled for this session.");
		}
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		logger.Info("Terminating browser...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)206);
		}
		if (Context.Settings.Browser.EnableBrowser)
		{
			((IApplication<IBrowserWindow>)(object)Context.Browser).Terminate();
		}
		else
		{
			logger.Info("Browser application was disabled for this session.");
		}
		return (OperationResult)3;
	}
}
