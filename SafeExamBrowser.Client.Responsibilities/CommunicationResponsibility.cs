using System;
using System.Collections.Generic;
using System.Linq;
using SafeExamBrowser.Client.Contracts;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Data;
using SafeExamBrowser.Communication.Contracts.Events;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Server.Contracts.Data;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Client.Responsibilities;

internal class CommunicationResponsibility : ClientResponsibility
{
	private readonly ICoordinator coordinator;

	private readonly IMessageBox messageBox;

	private readonly IRuntimeProxy runtime;

	private readonly Action shutdown;

	private readonly ISplashScreen splashScreen;

	private readonly IText text;

	private readonly IUserInterfaceFactory uiFactory;

	private IClientHost ClientHost => Context.ClientHost;

	public CommunicationResponsibility(ClientContext context, ICoordinator coordinator, ILogger logger, IMessageBox messageBox, IRuntimeProxy runtime, Action shutdown, ISplashScreen splashScreen, IText text, IUserInterfaceFactory uiFactory)
		: base(context, logger)
	{
		this.coordinator = coordinator;
		this.messageBox = messageBox;
		this.runtime = runtime;
		this.shutdown = shutdown;
		this.splashScreen = splashScreen;
		this.text = text;
		this.uiFactory = uiFactory;
	}

	public override void Assume(ClientTask task)
	{
		switch (task)
		{
		case ClientTask.DeregisterEvents:
			DeregisterEvents();
			break;
		case ClientTask.RegisterEvents:
			RegisterEvents();
			break;
		}
	}

	private void DeregisterEvents()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected Obj, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected Obj, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected Obj, but got Unknown
		if (ClientHost != null)
		{
			ClientHost.ExamSelectionRequested -= ClientHost_ExamSelectionRequested;
			ClientHost.MessageBoxRequested -= ClientHost_MessageBoxRequested;
			ClientHost.PasswordRequested -= ClientHost_PasswordRequested;
			ClientHost.ReconfigurationAborted -= ClientHost_ReconfigurationAborted;
			ClientHost.ReconfigurationDenied -= ClientHost_ReconfigurationDenied;
			ClientHost.ServerFailureActionRequested -= ClientHost_ServerFailureActionRequested;
			ClientHost.Shutdown -= ClientHost_Shutdown;
		}
		((ICommunicationProxy)runtime).ConnectionLost -= Runtime_ConnectionLost;
	}

	private void RegisterEvents()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected Obj, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected Obj, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected Obj, but got Unknown
		ClientHost.ExamSelectionRequested += ClientHost_ExamSelectionRequested;
		ClientHost.MessageBoxRequested += ClientHost_MessageBoxRequested;
		ClientHost.PasswordRequested += ClientHost_PasswordRequested;
		ClientHost.ReconfigurationAborted += ClientHost_ReconfigurationAborted;
		ClientHost.ReconfigurationDenied += ClientHost_ReconfigurationDenied;
		ClientHost.ServerFailureActionRequested += ClientHost_ServerFailureActionRequested;
		ClientHost.Shutdown += ClientHost_Shutdown;
		((ICommunicationProxy)runtime).ConnectionLost += Runtime_ConnectionLost;
	}

	private void ClientHost_ExamSelectionRequested(ExamSelectionRequestEventArgs args)
	{
		Logger.Info($"Received exam selection request with id '{args.RequestId}'.");
		IEnumerable<Exam> enumerable = args.Exams.Select((Func<(string, string, string, string), Exam>)(((string id, string lms, string name, string url) e) =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected Obj, but got Unknown
			Exam val3 = new Exam();
			(val3.Id, val3.LmsName, val3.Name, val3.Url) = e;
			return val3;
		}));
		ExamSelectionDialogResult val = uiFactory.CreateExamSelectionDialog(enumerable).Show((IWindow)null);
		IRuntimeProxy val2 = runtime;
		Guid requestId = args.RequestId;
		bool success = val.Success;
		Exam selectedExam = val.SelectedExam;
		val2.SubmitExamSelectionResult(requestId, success, (selectedExam != null) ? selectedExam.Id : null);
		Logger.Info($"Exam selection request with id '{args.RequestId}' is complete.");
	}

	private void ClientHost_MessageBoxRequested(MessageBoxRequestEventArgs args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected I4, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info($"Received message box request with id '{args.RequestId}'.");
		MessageBoxAction val = (MessageBoxAction)args.Action;
		MessageBoxIcon val2 = (MessageBoxIcon)args.Icon;
		MessageBoxResult val3 = messageBox.Show(args.Message, args.Title, val, val2, (IWindow)(object)splashScreen);
		runtime.SubmitMessageBoxResult(args.RequestId, (int)val3);
		Logger.Info($"Message box request with id '{args.RequestId}' yielded result '{val3}'.");
	}

	private void ClientHost_PasswordRequested(PasswordRequestEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected I4, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		TextKey val = (TextKey)0;
		TextKey val2 = (TextKey)0;
		Logger.Info($"Received password request with id '{args.RequestId}' for the {((object)args.Purpose/*cast due to constrained. prefix*/).ToString().ToLower()} password.");
		PasswordRequestPurpose purpose = args.Purpose;
		switch ((int)purpose)
		{
		case 0:
			val = (TextKey)224;
			val2 = (TextKey)225;
			break;
		case 1:
			val = (TextKey)226;
			val2 = (TextKey)227;
			break;
		case 2:
			val = (TextKey)230;
			val2 = (TextKey)231;
			break;
		}
		PasswordDialogResult val3 = uiFactory.CreatePasswordDialog(text.Get(val), text.Get(val2)).Show((IWindow)null);
		runtime.SubmitPassword(args.RequestId, val3.Success, val3.Password);
		Logger.Info(string.Format("Password request with id '{0}' was {1}.", args.RequestId, val3.Success ? "successful" : "aborted by the user"));
	}

	private void ClientHost_ReconfigurationAborted()
	{
		Logger.Info("The reconfiguration was aborted by the runtime.");
		((IWindow)splashScreen).Hide();
		coordinator.ReleaseReconfigurationLock();
	}

	private void ClientHost_ReconfigurationDenied(ReconfigurationEventArgs args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("The reconfiguration request for '" + args.ConfigurationPath + "' was denied by the runtime!");
		messageBox.Show((TextKey)134, (TextKey)135, (MessageBoxAction)0, (MessageBoxIcon)1, (IWindow)(object)splashScreen);
		((IWindow)splashScreen).Hide();
		coordinator.ReleaseReconfigurationLock();
	}

	private void ClientHost_ServerFailureActionRequested(ServerFailureActionRequestEventArgs args)
	{
		Logger.Info($"Received server failure action request with id '{args.RequestId}'.");
		ServerFailureDialogResult val = uiFactory.CreateServerFailureDialog(args.Message, args.ShowFallback).Show((IWindow)null);
		runtime.SubmitServerFailureActionResult(args.RequestId, val.Abort, val.Fallback, val.Retry);
		_003F val2 = Logger;
		object arg = args.RequestId;
		string arg2;
		if (val.Abort)
		{
			arg2 = "abort";
		}
		else
		{
			arg2 = (val.Fallback ? "fallback" : "retry");
		}
		((ILogger)val2).Info($"Server failure action request with id '{arg}' is complete, the user chose to {arg2}.");
	}

	private void ClientHost_Shutdown()
	{
		shutdown();
	}

	private void Runtime_ConnectionLost()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Logger.Error("Lost connection to the runtime!");
		messageBox.Show((TextKey)85, (TextKey)86, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)null);
		shutdown();
	}
}
