using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Display;
using SafeExamBrowser.UserInterface.Contracts.Shell;

namespace SafeExamBrowser.Client.Operations;

internal class DisplayMonitorOperation : ClientOperation
{
	private readonly IDisplayMonitor displayMonitor;

	private readonly ILogger logger;

	private readonly ITaskbar taskbar;

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

	public DisplayMonitorOperation(ClientContext context, IDisplayMonitor displayMonitor, ILogger logger, ITaskbar taskbar)
		: base(context)
	{
		this.displayMonitor = displayMonitor;
		this.logger = logger;
		this.taskbar = taskbar;
	}

	public override OperationResult Perform()
	{
		logger.Info("Initializing working area...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)194);
		}
		displayMonitor.InitializePrimaryDisplay(Context.Settings.UserInterface.Taskbar.EnableTaskbar ? taskbar.GetAbsoluteHeight() : 0);
		displayMonitor.StartMonitoringDisplayChanges();
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		logger.Info("Restoring working area...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)196);
		}
		displayMonitor.StopMonitoringDisplayChanges();
		displayMonitor.ResetPrimaryDisplay();
		return (OperationResult)3;
	}
}
