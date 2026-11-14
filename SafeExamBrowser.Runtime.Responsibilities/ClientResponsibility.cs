using System;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime.Responsibilities;

internal class ClientResponsibility : RuntimeResponsibility
{
	private readonly IMessageBox messageBox;

	private readonly IRuntimeWindow runtimeWindow;

	private readonly Action shutdown;

	internal ClientResponsibility(ILogger logger, IMessageBox messageBox, RuntimeContext runtimeContext, IRuntimeWindow runtimeWindow, Action shutdown)
		: base(logger, runtimeContext)
	{
		this.messageBox = messageBox;
		this.runtimeWindow = runtimeWindow;
		this.shutdown = shutdown;
	}

	public override void Assume(RuntimeTask task)
	{
		switch (task)
		{
		case RuntimeTask.DeregisterSessionEvents:
			DeregisterEvents();
			break;
		case RuntimeTask.RegisterSessionEvents:
			RegisterEvents();
			break;
		}
	}

	private void DeregisterEvents()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected Obj, but got Unknown
		if (Context.ClientProcess != null)
		{
			Context.ClientProcess.Terminated -= ClientProcess_Terminated;
		}
		if (Context.ClientProxy != null)
		{
			((ICommunicationProxy)Context.ClientProxy).ConnectionLost -= ClientProxy_ConnectionLost;
		}
	}

	private void RegisterEvents()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected Obj, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		Context.ClientProcess.Terminated += ClientProcess_Terminated;
		((ICommunicationProxy)Context.ClientProxy).ConnectionLost += ClientProxy_ConnectionLost;
	}

	private void ClientProcess_Terminated(int exitCode)
	{
		Logger.Error($"Client application has unexpectedly terminated with exit code {exitCode}!");
		if (SessionIsRunning)
		{
			Context.Responsibilities.Delegate(RuntimeTask.StopSession);
		}
		shutdown();
	}

	private void ClientProxy_ConnectionLost()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Logger.Error("Lost connection to the client application!");
		if (SessionIsRunning)
		{
			Context.Responsibilities.Delegate(RuntimeTask.StopSession);
		}
		messageBox.Show((TextKey)85, (TextKey)86, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)runtimeWindow);
		shutdown();
	}
}
