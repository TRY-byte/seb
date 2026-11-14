using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Client.Operations;

internal class RuntimeConnectionOperation : ClientOperation
{
	private readonly ILogger logger;

	private readonly IRuntimeProxy runtime;

	private Guid token;

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

	public RuntimeConnectionOperation(ClientContext context, ILogger logger, IRuntimeProxy runtime, Guid token)
		: base(context)
	{
		this.logger = logger;
		this.runtime = runtime;
		this.token = token;
	}

	public override OperationResult Perform()
	{
		logger.Info("Initializing runtime connection...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)187);
		}
		bool flag = ((ICommunicationProxy)runtime).Connect((Guid?)token, true);
		if (flag)
		{
			logger.Info("Successfully connected to the runtime.");
		}
		else
		{
			logger.Error("Failed to connect to the runtime. Aborting startup...");
		}
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	public override OperationResult Revert()
	{
		logger.Info("Closing runtime connection...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)174);
		}
		if (((ICommunicationProxy)runtime).IsConnected)
		{
			bool flag = ((ICommunicationProxy)runtime).Disconnect();
			if (flag)
			{
				logger.Info("Successfully disconnected from the runtime.");
			}
			else
			{
				logger.Error("Failed to disconnect from the runtime!");
			}
			if (flag)
			{
				return (OperationResult)3;
			}
			return (OperationResult)2;
		}
		return (OperationResult)3;
	}
}
