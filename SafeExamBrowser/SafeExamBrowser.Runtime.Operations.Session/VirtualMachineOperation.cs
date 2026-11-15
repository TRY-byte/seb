using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Monitoring.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class VirtualMachineOperation : SessionOperation
{
	private readonly IVirtualMachineDetector detector;

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

	public VirtualMachineOperation(Dependencies dependencies, IVirtualMachineDetector detector)
		: base(dependencies)
	{
		this.detector = detector;
	}

	public override OperationResult Perform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ValidatePolicy();
	}

	public override OperationResult Repeat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ValidatePolicy();
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private OperationResult ValidatePolicy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		OperationResult result = (OperationResult)3;
		Logger.Info("Validating virtual machine policy...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)212);
		}
		if ((int)Context.Next.Settings.Security.VirtualMachinePolicy == 1 && detector.IsVirtualMachine())
		{
			result = (OperationResult)1;
			Logger.Error("Detected virtual machine while SEB is not allowed to be run in a virtual machine! Aborting...");
			ShowMessageBox((TextKey)161, (TextKey)162, (MessageBoxAction)0, (MessageBoxIcon)0);
		}
		return result;
	}
}
