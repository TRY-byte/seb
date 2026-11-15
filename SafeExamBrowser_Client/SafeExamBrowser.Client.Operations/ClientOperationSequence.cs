using System.Collections.Generic;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.Core.OperationModel;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client.Operations;

internal class ClientOperationSequence : OperationSequence<IOperation>
{
	private readonly ISplashScreen splashScreen;

	public ClientOperationSequence(ILogger logger, IEnumerable<IOperation> operations, ISplashScreen splashScreen)
		: base(logger, operations)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		this.splashScreen = splashScreen;
		base.ProgressChanged += Operations_ProgressChanged;
		base.StatusChanged += Operations_StatusChanged;
	}

	private void Operations_ProgressChanged(ProgressChangedEventArgs args)
	{
		if (args.CurrentValue.HasValue)
		{
			((IProgressIndicator)splashScreen).SetValue(args.CurrentValue.Value);
		}
		if (args.IsIndeterminate == true)
		{
			((IProgressIndicator)splashScreen).SetIndeterminate();
		}
		if (args.MaxValue.HasValue)
		{
			((IProgressIndicator)splashScreen).SetMaxValue(args.MaxValue.Value);
		}
		if (args.Progress == true)
		{
			((IProgressIndicator)splashScreen).Progress();
		}
		if (args.Regress == true)
		{
			((IProgressIndicator)splashScreen).Regress();
		}
	}

	private void Operations_StatusChanged(TextKey status)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((IProgressIndicator)splashScreen).UpdateStatus(status, true);
	}
}
