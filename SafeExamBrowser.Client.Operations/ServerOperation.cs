using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Server.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Shell;

namespace SafeExamBrowser.Client.Operations;

internal class ServerOperation : ClientOperation
{
	private readonly IActionCenter actionCenter;

	private readonly IInvigilator invigilator;

	private readonly ILogger logger;

	private readonly IServerProxy server;

	private readonly ITaskbar taskbar;

	private readonly IUserInterfaceFactory uiFactory;

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

	public ServerOperation(IActionCenter actionCenter, ClientContext context, IInvigilator invigilator, ILogger logger, IServerProxy server, ITaskbar taskbar, IUserInterfaceFactory uiFactory)
		: base(context)
	{
		this.actionCenter = actionCenter;
		this.invigilator = invigilator;
		this.logger = logger;
		this.server = server;
		this.taskbar = taskbar;
		this.uiFactory = uiFactory;
	}

	public override OperationResult Perform()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		if ((int)Context.Settings.SessionMode == 1)
		{
			logger.Info("Initializing server...");
			StatusChangedEventHandler val = m_StatusChanged;
			if (val != null)
			{
				val.Invoke((TextKey)188);
			}
			server.Initialize(Context.AppConfig.ServerApi, Context.AppConfig.ServerConnectionToken, Context.AppConfig.ServerExamId, Context.AppConfig.ServerOauth2Token, Context.Settings.Server);
			server.StartConnectivity();
			if (Context.Settings.Server.Invigilation.ShowRaiseHandNotification)
			{
				invigilator.Initialize(Context.Settings.Server.Invigilation);
				actionCenter.AddNotificationControl(uiFactory.CreateRaiseHandControl(invigilator, (Location)0, Context.Settings.Server.Invigilation));
				taskbar.AddNotificationControl(uiFactory.CreateRaiseHandControl(invigilator, (Location)1, Context.Settings.Server.Invigilation));
			}
		}
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		AppSettings settings = Context.Settings;
		if (settings != null && (int)settings.SessionMode == 1)
		{
			logger.Info("Finalizing server...");
			StatusChangedEventHandler val = m_StatusChanged;
			if (val != null)
			{
				val.Invoke((TextKey)177);
			}
			server.StopConnectivity();
		}
		return (OperationResult)3;
	}
}
