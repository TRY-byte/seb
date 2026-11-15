using System;
using SafeExamBrowser.Communication.Contracts.Events;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Runtime.Responsibilities;

internal class CommunicationResponsibility : RuntimeResponsibility
{
	private readonly IRuntimeHost runtimeHost;

	private readonly Action shutdown;

	internal CommunicationResponsibility(ILogger logger, RuntimeContext runtimeContext, IRuntimeHost runtimeHost, Action shutdown)
		: base(logger, runtimeContext)
	{
		this.runtimeHost = runtimeHost;
		this.shutdown = shutdown;
	}

	public override void Assume(RuntimeTask task)
	{
		switch (task)
		{
		case RuntimeTask.DeregisterEvents:
			DeregisterEvents();
			break;
		case RuntimeTask.RegisterEvents:
			RegisterEvents();
			break;
		}
	}

	private void RegisterEvents()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		runtimeHost.ClientConfigurationNeeded += RuntimeHost_ClientConfigurationNeeded;
		runtimeHost.ReconfigurationRequested += RuntimeHost_ReconfigurationRequested;
		runtimeHost.ShutdownRequested += RuntimeHost_ShutdownRequested;
	}

	private void DeregisterEvents()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		runtimeHost.ClientConfigurationNeeded -= RuntimeHost_ClientConfigurationNeeded;
		runtimeHost.ReconfigurationRequested -= RuntimeHost_ReconfigurationRequested;
		runtimeHost.ShutdownRequested -= RuntimeHost_ShutdownRequested;
	}

	private void RuntimeHost_ClientConfigurationNeeded(ClientConfigurationEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected Obj, but got Unknown
		args.ClientConfiguration = new ClientConfiguration
		{
			AppConfig = Context.Next.AppConfig,
			SessionId = Context.Next.SessionId,
			Settings = Context.Next.Settings
		};
	}

	private void RuntimeHost_ReconfigurationRequested(ReconfigurationEventArgs args)
	{
		Logger.Info("Accepted request for reconfiguration with '" + args.ConfigurationPath + "'.");
		Context.ReconfigurationFilePath = args.ConfigurationPath;
		Context.ReconfigurationUrl = args.ResourceUrl;
		Context.Responsibilities.Delegate(RuntimeTask.StartSession);
	}

	private void RuntimeHost_ShutdownRequested()
	{
		Logger.Info("Received shutdown request from the client application.");
		shutdown();
	}
}
