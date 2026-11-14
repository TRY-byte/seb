using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.SystemComponents.Contracts.Network;
using SafeExamBrowser.SystemComponents.Contracts.Network.Events;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Client.Responsibilities;

internal class NetworkResponsibility : ClientResponsibility
{
	private readonly INetworkAdapter networkAdapter;

	private readonly IText text;

	private readonly IUserInterfaceFactory uiFactory;

	public NetworkResponsibility(ClientContext context, ILogger logger, INetworkAdapter networkAdapter, IText text, IUserInterfaceFactory uiFactory)
		: base(context, logger)
	{
		this.networkAdapter = networkAdapter;
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		networkAdapter.CredentialsRequired -= NetworkAdapter_CredentialsRequired;
	}

	private void RegisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		networkAdapter.CredentialsRequired += NetworkAdapter_CredentialsRequired;
	}

	private void NetworkAdapter_CredentialsRequired(CredentialsRequiredEventArgs args)
	{
		string text = this.text.Get((TextKey)36).Replace("%%_NAME_%%", args.NetworkName);
		string text2 = this.text.Get((TextKey)37);
		CredentialsDialogResult val = uiFactory.CreateCredentialsDialog((CredentialsDialogPurpose)1, text, text2).Show((IWindow)null);
		args.Password = val.Password;
		args.Success = val.Success;
		args.Username = val.Username;
	}
}
