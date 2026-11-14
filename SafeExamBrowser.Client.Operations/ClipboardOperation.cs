using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts;

namespace SafeExamBrowser.Client.Operations;

internal class ClipboardOperation : ClientOperation
{
	private readonly IClipboard clipboard;

	private readonly ILogger logger;

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

	public ClipboardOperation(ClientContext context, IClipboard clipboard, ILogger logger)
		: base(context)
	{
		this.clipboard = clipboard;
		this.logger = logger;
	}

	public override OperationResult Perform()
	{
		InitializeClipboard();
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		FinalizeClipboard();
		return (OperationResult)3;
	}

	private void InitializeClipboard()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		logger.Info("Initializing clipboard...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)182);
		}
		clipboard.Initialize(Context.Settings.Security.ClipboardPolicy);
	}

	private void FinalizeClipboard()
	{
		logger.Info("Finalizing clipboard...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)176);
		}
		clipboard.Terminate();
	}
}
