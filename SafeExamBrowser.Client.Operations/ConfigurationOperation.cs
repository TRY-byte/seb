using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Client.Operations;

internal class ConfigurationOperation : ClientOperation
{
	private readonly ILogger logger;

	private readonly IRuntimeProxy runtime;

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

	public ConfigurationOperation(ClientContext context, ILogger logger, IRuntimeProxy runtime)
		: base(context)
	{
		this.logger = logger;
		this.runtime = runtime;
	}

	public override OperationResult Perform()
	{
		logger.Info("Initializing application configuration...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)183);
		}
		ClientConfiguration configuration = runtime.GetConfiguration().Value.Configuration;
		Context.AppConfig = configuration.AppConfig;
		Context.SessionId = configuration.SessionId;
		Context.Settings = configuration.Settings;
		logger.Info("Successfully retrieved the application configuration from the runtime.");
		logger.Info($" -> Client-ID: {Context.AppConfig.ClientId}");
		logger.Info($" -> Runtime-ID: {Context.AppConfig.RuntimeId}");
		logger.Info($" -> Session-ID: {Context.SessionId}");
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}
}
