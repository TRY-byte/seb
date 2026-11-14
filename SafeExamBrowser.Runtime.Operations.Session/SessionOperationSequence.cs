using System.Collections.Generic;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.Core.OperationModel;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class SessionOperationSequence : RepeatableOperationSequence<SessionOperation>
{
	private readonly IRuntimeWindow runtimeWindow;

	internal SessionOperationSequence(ILogger logger, IEnumerable<SessionOperation> operations, IRuntimeWindow runtimeWindow)
		: base(logger, operations)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		this.runtimeWindow = runtimeWindow;
		((OperationSequence<SessionOperation>)(object)this).ProgressChanged += SessionSequence_ProgressChanged;
		((OperationSequence<SessionOperation>)(object)this).StatusChanged += SessionSequence_StatusChanged;
	}

	private void SessionSequence_ProgressChanged(ProgressChangedEventArgs args)
	{
		if (args.CurrentValue.HasValue)
		{
			IRuntimeWindow val = runtimeWindow;
			if (val != null)
			{
				((IProgressIndicator)val).SetValue(args.CurrentValue.Value);
			}
		}
		if (args.IsIndeterminate == true)
		{
			IRuntimeWindow val2 = runtimeWindow;
			if (val2 != null)
			{
				((IProgressIndicator)val2).SetIndeterminate();
			}
		}
		if (args.MaxValue.HasValue)
		{
			IRuntimeWindow val3 = runtimeWindow;
			if (val3 != null)
			{
				((IProgressIndicator)val3).SetMaxValue(args.MaxValue.Value);
			}
		}
		if (args.Progress == true)
		{
			IRuntimeWindow val4 = runtimeWindow;
			if (val4 != null)
			{
				((IProgressIndicator)val4).Progress();
			}
		}
		if (args.Regress == true)
		{
			IRuntimeWindow val5 = runtimeWindow;
			if (val5 != null)
			{
				((IProgressIndicator)val5).Regress();
			}
		}
	}

	private void SessionSequence_StatusChanged(TextKey status)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		IRuntimeWindow val = runtimeWindow;
		if (val != null)
		{
			((IProgressIndicator)val).UpdateStatus(status, true);
		}
	}
}
