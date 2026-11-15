using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.SystemComponents.Contracts.Network;

namespace SafeExamBrowser.Client.Operations;

internal class PermissionOperation : ClientOperation
{
	private readonly ILogger logger;

	private readonly INetworkAdapter networkAdapter;

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

	public PermissionOperation(ClientContext context, ILogger logger, INetworkAdapter networkAdapter)
		: base(context)
	{
		this.logger = logger;
		this.networkAdapter = networkAdapter;
	}

	public override OperationResult Perform()
	{
		logger.Info("Initializing permissions...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)185);
		}
		RequestNetworkAdapterAccess();
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private void RequestNetworkAdapterAccess()
	{
		if (networkAdapter.RequestAccess())
		{
			logger.Info("Permission to access the wireless networking functionality has been granted.");
		}
		else
		{
			logger.Warn("Permission to access the wireless networking functionality has not been granted! If required, please grant the location permission manually under 'Privacy & Security' in the system settings.");
		}
	}
}
