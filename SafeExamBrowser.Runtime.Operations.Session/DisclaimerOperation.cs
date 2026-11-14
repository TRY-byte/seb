using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class DisclaimerOperation : SessionOperation
{
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

	public DisclaimerOperation(Dependencies dependencies)
		: base(dependencies)
	{
	}

	public override OperationResult Perform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		OperationResult result = (OperationResult)3;
		if (Context.Next.Settings.Proctoring.ScreenProctoring.Enabled)
		{
			result = ShowScreenProctoringDisclaimer();
		}
		return result;
	}

	public override OperationResult Repeat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		OperationResult result = (OperationResult)3;
		if (Context.Next.Settings.Proctoring.ScreenProctoring.Enabled)
		{
			result = ShowScreenProctoringDisclaimer();
		}
		return result;
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private OperationResult ShowScreenProctoringDisclaimer()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)215);
		}
		TextKey messageKey = (TextKey)140;
		TextKey titleKey = (TextKey)141;
		MessageBoxResult val2 = ShowMessageBox(messageKey, titleKey, (MessageBoxAction)1, (MessageBoxIcon)1);
		OperationResult result = (OperationResult)(((int)val2 != 3) ? 1 : 3);
		if ((int)val2 == 3)
		{
			Logger.Info("The user confirmed the screen proctoring disclaimer.");
		}
		else
		{
			Logger.Warn("The user did not confirm the screen proctoring disclaimer! Aborting session initialization...");
		}
		return result;
	}
}
