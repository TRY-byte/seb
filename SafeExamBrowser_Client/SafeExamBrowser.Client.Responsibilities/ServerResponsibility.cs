using System.Linq;
using SafeExamBrowser.Client.Contracts;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Server.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Client.Responsibilities;

internal class ServerResponsibility : ClientResponsibility
{
	private readonly ICoordinator coordinator;

	private readonly IText text;

	private IServerProxy Server => Context.Server;

	public ServerResponsibility(ClientContext context, ICoordinator coordinator, ILogger logger, IText text)
		: base(context, logger)
	{
		this.coordinator = coordinator;
		this.text = text;
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
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected Obj, but got Unknown
		if (Server != null)
		{
			Server.LockScreenConfirmed -= Server_LockScreenConfirmed;
			Server.LockScreenRequested -= Server_LockScreenRequested;
			Server.TerminationRequested -= Server_TerminationRequested;
		}
	}

	private void RegisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		Server.LockScreenConfirmed += Server_LockScreenConfirmed;
		Server.LockScreenRequested += Server_LockScreenRequested;
		Server.TerminationRequested += Server_TerminationRequested;
	}

	private void Server_LockScreenConfirmed()
	{
		Logger.Info("Closing lock screen as requested by the server...");
		ILockScreen lockScreen = Context.LockScreen;
		if (lockScreen != null)
		{
			lockScreen.Cancel();
		}
	}

	private void Server_LockScreenRequested(string message)
	{
		Logger.Info("Attempting to show lock screen as requested by the server...");
		if (coordinator.RequestSessionLock())
		{
			ShowLockScreen(message, text.Get((TextKey)73), Enumerable.Empty<LockScreenOption>());
			coordinator.ReleaseSessionLock();
		}
		else
		{
			Logger.Info("Lock screen is already active.");
		}
	}

	private void Server_TerminationRequested()
	{
		Logger.Info("Attempting to shutdown as requested by the server...");
		TryRequestShutdown();
	}
}
