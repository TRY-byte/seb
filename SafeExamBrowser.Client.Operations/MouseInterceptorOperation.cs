using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Mouse;

namespace SafeExamBrowser.Client.Operations;

internal class MouseInterceptorOperation : ClientOperation
{
	private readonly ILogger logger;

	private readonly IMouseInterceptor mouseInterceptor;

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

	public MouseInterceptorOperation(ClientContext context, ILogger logger, IMouseInterceptor mouseInterceptor)
		: base(context)
	{
		this.logger = logger;
		this.mouseInterceptor = mouseInterceptor;
	}

	public override OperationResult Perform()
	{
		logger.Info("Starting mouse interception...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)201);
		}
		mouseInterceptor.Start();
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		logger.Info("Stopping mouse interception...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)205);
		}
		mouseInterceptor.Stop();
		return (OperationResult)3;
	}
}
