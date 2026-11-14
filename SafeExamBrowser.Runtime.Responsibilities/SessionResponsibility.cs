using System;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime.Responsibilities;

internal class SessionResponsibility : RuntimeResponsibility
{
	private readonly AppConfig appConfig;

	private readonly IMessageBox messageBox;

	private readonly IRuntimeWindow runtimeWindow;

	private readonly IRepeatableOperationSequence sessionSequence;

	private readonly Action shutdown;

	private readonly IText text;

	internal SessionResponsibility(AppConfig appConfig, ILogger logger, IMessageBox messageBox, RuntimeContext runtimeContext, IRuntimeWindow runtimeWindow, IRepeatableOperationSequence sessionSequence, Action shutdown, IText text)
		: base(logger, runtimeContext)
	{
		this.appConfig = appConfig;
		this.messageBox = messageBox;
		this.runtimeWindow = runtimeWindow;
		this.sessionSequence = sessionSequence;
		this.shutdown = shutdown;
		this.text = text;
	}

	public override void Assume(RuntimeTask task)
	{
		switch (task)
		{
		case RuntimeTask.StartSession:
			StartSession();
			break;
		case RuntimeTask.StopSession:
			StopSession();
			break;
		}
	}

	private void StartSession()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Invalid comparison between Unknown and I4
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		((IWindow)runtimeWindow).Show();
		((IWindow)runtimeWindow).BringToForeground();
		runtimeWindow.ShowProgressBar = true;
		Logger.Info(AppendDivider("Session Start Procedure"));
		if (SessionIsRunning)
		{
			Context.Responsibilities.Delegate(RuntimeTask.DeregisterSessionEvents);
		}
		OperationResult val = (SessionIsRunning ? sessionSequence.TryRepeat() : ((IOperationSequence)sessionSequence).TryPerform());
		if ((int)val == 3)
		{
			Logger.Info(AppendDivider("Session Running"));
			HandleSessionStartSuccess();
		}
		else if ((int)val == 2)
		{
			Logger.Info(AppendDivider("Session Start Failed"));
			HandleSessionStartFailure();
		}
		else if ((int)val == 1)
		{
			Logger.Info(AppendDivider("Session Start Aborted"));
			HandleSessionStartAbortion();
		}
	}

	private void StopSession()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		((IWindow)runtimeWindow).Show();
		((IWindow)runtimeWindow).BringToForeground();
		runtimeWindow.ShowProgressBar = true;
		Logger.Info(AppendDivider("Session Stop Procedure"));
		Context.Responsibilities.Delegate(RuntimeTask.DeregisterSessionEvents);
		if ((int)((IOperationSequence)sessionSequence).TryRevert() == 3)
		{
			Logger.Info(AppendDivider("Session Terminated"));
		}
		else
		{
			Logger.Info(AppendDivider("Session Stop Failed"));
		}
	}

	private void HandleSessionStartSuccess()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		Context.Responsibilities.Delegate(RuntimeTask.RegisterSessionEvents);
		runtimeWindow.ShowProgressBar = false;
		runtimeWindow.ShowLog = Session.Settings.Security.AllowApplicationLogAccess;
		runtimeWindow.TopMost = (int)Session.Settings.Security.KioskMode > 0;
		((IProgressIndicator)runtimeWindow).UpdateStatus((TextKey)242, false);
		((IWindow)runtimeWindow).Hide();
	}

	private void HandleSessionStartFailure()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		string text = AppendLogFilePaths(appConfig, this.text.Get((TextKey)146));
		string text2 = this.text.Get((TextKey)147);
		if (SessionIsRunning)
		{
			StopSession();
			messageBox.Show(text, text2, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)runtimeWindow);
			Logger.Info("Terminating application...");
			shutdown();
		}
		else
		{
			messageBox.Show(text, text2, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)runtimeWindow);
		}
	}

	private void HandleSessionStartAbortion()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		if (SessionIsRunning)
		{
			Context.Responsibilities.Delegate(RuntimeTask.RegisterSessionEvents);
			runtimeWindow.ShowProgressBar = false;
			((IProgressIndicator)runtimeWindow).UpdateStatus((TextKey)242, false);
			runtimeWindow.TopMost = (int)Session.Settings.Security.KioskMode > 0;
			((IWindow)runtimeWindow).Hide();
			Context.ClientProxy.InformReconfigurationAborted();
		}
	}

	private string AppendDivider(string message)
	{
		string text = new string('-', 48 - message.Length / 2 - message.Length % 2);
		string text2 = new string('-', 48 - message.Length / 2);
		return "### " + text + " " + message + " " + text2 + " ###";
	}
}
