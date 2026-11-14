using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts;

namespace SafeExamBrowser.Client.Operations;

internal class WindowGuardOperation : ClientOperation
{
	private readonly ILogger logger;

	private readonly IWindowGuard guard;

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

	public WindowGuardOperation(ClientContext context, ILogger logger, IWindowGuard guard)
		: base(context)
	{
		this.logger = logger;
		this.guard = guard;
	}

	public override OperationResult Perform()
	{
		logger.Info("Initializing window guard...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)193);
		}
		if (Context.Settings.Proctoring.Enabled || Context.Settings.Security.AllowWindowCapture)
		{
			guard.Deactivate();
			logger.Info("Deactivated window guard because " + (Context.Settings.Proctoring.Enabled ? "proctoring" : "window capturing") + " is enabled.");
		}
		else
		{
			guard.Activate();
			logger.Info("Activated window guard.");
		}
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}
}
