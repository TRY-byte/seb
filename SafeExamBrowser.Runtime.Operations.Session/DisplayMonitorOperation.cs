using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Display;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class DisplayMonitorOperation : SessionOperation
{
	private readonly IDisplayMonitor displayMonitor;

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

	public DisplayMonitorOperation(Dependencies dependencies, IDisplayMonitor displayMonitor)
		: base(dependencies)
	{
		this.displayMonitor = displayMonitor;
	}

	public override OperationResult Perform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return CheckDisplayConfiguration();
	}

	public override OperationResult Repeat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return CheckDisplayConfiguration();
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private OperationResult CheckDisplayConfiguration()
	{
		Logger.Info("Validating display configuration...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)209);
		}
		ValidationResult val2 = displayMonitor.ValidateConfiguration(Context.Next.Settings.Display);
		int num = (val2.IsAllowed ? 3 : 2);
		if (val2.IsAllowed)
		{
			Logger.Info("Display configuration is allowed.");
			return (OperationResult)num;
		}
		Logger.Error("Display configuration is not allowed!");
		ShowError(val2);
		return (OperationResult)num;
	}

	private void ShowError(ValidationResult validation)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		string text = Text.Get((TextKey)108);
		string text2 = Text.Get((TextKey)109);
		TextKey messageKey = (TextKey)106;
		TextKey titleKey = (TextKey)107;
		Dictionary<string, string> messagePlaceholders = new Dictionary<string, string>
		{
			{
				"%%_ALLOWED_COUNT_%%",
				Convert.ToString(Context.Next.Settings.Display.AllowedDisplays)
			},
			{
				"%%_EXTERNAL_COUNT_%%",
				Convert.ToString(validation.ExternalDisplays)
			},
			{
				"%%_INTERNAL_COUNT_%%",
				Convert.ToString(validation.InternalDisplays)
			},
			{
				"%%_TYPE_%%",
				Context.Next.Settings.Display.InternalDisplayOnly ? text : text2
			}
		};
		ShowMessageBox(messageKey, titleKey, (MessageBoxAction)0, (MessageBoxIcon)0, messagePlaceholders);
	}
}
