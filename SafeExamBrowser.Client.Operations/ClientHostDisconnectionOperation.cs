using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Communication.Contracts.Events;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Client.Operations;

internal class ClientHostDisconnectionOperation : ClientOperation
{
	private readonly ILogger logger;

	private readonly int timeout_ms;

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

	public ClientHostDisconnectionOperation(ClientContext context, ILogger logger, int timeout_ms)
		: base(context)
	{
		this.logger = logger;
		this.timeout_ms = timeout_ms;
	}

	public override OperationResult Perform()
	{
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected Obj, but got Unknown
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)219);
		}
		if (Context.ClientHost.IsConnected)
		{
			AutoResetEvent disconnectedEvent = new AutoResetEvent(initialState: false);
			CommunicationEventHandler val2 = () =>
			{
				disconnectedEvent.Set();
			};
			Context.ClientHost.RuntimeDisconnected += val2;
			logger.Info("Waiting for runtime to disconnect from client communication host...");
			bool flag = disconnectedEvent.WaitOne(timeout_ms);
			Context.ClientHost.RuntimeDisconnected -= val2;
			if (!flag)
			{
				logger.Error($"The runtime failed to disconnect within {timeout_ms / 1000} seconds!");
				return (OperationResult)2;
			}
			logger.Info("The runtime has successfully disconnected from the client communication host.");
		}
		else
		{
			logger.Info("The runtime has already disconnected from the client communication host.");
		}
		return (OperationResult)3;
	}
}
