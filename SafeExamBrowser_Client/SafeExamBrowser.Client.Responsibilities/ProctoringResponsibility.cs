using System.Threading.Tasks;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Proctoring.Contracts;
using SafeExamBrowser.Proctoring.Contracts.Events;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Proctoring;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client.Responsibilities;

internal class ProctoringResponsibility : ClientResponsibility
{
	private readonly IMessageBox messageBox;

	private readonly IUserInterfaceFactory uiFactory;

	private bool cancel;

	private IProctoringController Proctoring => Context.Proctoring;

	public ProctoringResponsibility(ClientContext context, ILogger logger, IMessageBox messageBox, IUserInterfaceFactory uiFactory)
		: base(context, logger)
	{
		this.messageBox = messageBox;
		this.uiFactory = uiFactory;
	}

	public override void Assume(ClientTask task)
	{
		if (task == ClientTask.PrepareShutdown_Wave1)
		{
			FinalizeProctoring();
		}
	}

	private void FinalizeProctoring()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected Obj, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected Obj, but got Unknown
		if (Proctoring != null && Proctoring.HasRemainingWork())
		{
			IProctoringFinalizationDialog dialog = uiFactory.CreateProctoringFinalizationDialog(!Context.QuitPasswordValidated);
			RemainingWorkUpdatedEventHandler handler = (RemainingWorkUpdatedEventArgs args) =>
			{
				Proctoring_RemainingWorkUpdated(dialog, args);
			};
			dialog.CancellationRequested += () =>
			{
				Dialog_CancellationRequested(dialog);
			};
			Task.Run(() =>
			{
				Proctoring.RemainingWorkUpdated += handler;
				Proctoring.ExecuteRemainingWork();
				Proctoring.RemainingWorkUpdated -= handler;
			});
			((IWindow)dialog).Show();
		}
	}

	private void Dialog_CancellationRequested(IProctoringFinalizationDialog dialog)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		bool quitPasswordValidated = Context.QuitPasswordValidated;
		if (quitPasswordValidated || IsValidQuitPassword(dialog.QuitPassword))
		{
			cancel = true;
			Logger.Info("The user " + (quitPasswordValidated ? "already " : "") + "entered the correct quit password, cancelling remaining work...");
		}
		else
		{
			cancel = false;
			Logger.Info("The user entered the wrong quit password, remaining work will continue.");
			messageBox.Show((TextKey)118, (TextKey)119, (MessageBoxAction)0, (MessageBoxIcon)3, (IWindow)(object)dialog);
		}
	}

	private void Proctoring_RemainingWorkUpdated(IProctoringFinalizationDialog dialog, RemainingWorkUpdatedEventArgs args)
	{
		dialog.Update(args);
		args.CancellationRequested = cancel;
	}
}
