using SafeExamBrowser.Client.Responsibilities;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client;

internal class ClientController
{
	private readonly ClientContext context;

	private readonly ILogger logger;

	private readonly IOperationSequence operations;

	private readonly IResponsibilityCollection<ClientTask> responsibilities;

	private readonly IRuntimeProxy runtime;

	private readonly ISplashScreen splashScreen;

	internal ClientController(ClientContext context, ILogger logger, IOperationSequence operations, IResponsibilityCollection<ClientTask> responsibilities, IRuntimeProxy runtime, ISplashScreen splashScreen)
	{
		this.context = context;
		this.logger = logger;
		this.operations = operations;
		this.responsibilities = responsibilities;
		this.runtime = runtime;
		this.splashScreen = splashScreen;
	}

	internal bool TryStart()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Invalid comparison between Unknown and I4
		logger.Info("Initiating startup procedure...");
		((IWindow)splashScreen).Show();
		((IWindow)splashScreen).BringToForeground();
		bool flag = false;
		OperationResult val = operations.TryPerform();
		if ((int)val == 3)
		{
			DelegateStartupResponsibilities();
			flag = TryInformRuntime();
		}
		else
		{
			logger.Info("Application startup " + (((int)val == 1) ? "aborted" : "failed") + "!");
			logger.Log(string.Empty);
		}
		if (flag)
		{
			logger.Info("Application successfully initialized.");
			logger.Log(string.Empty);
			DelegateClientReadyResponsibilities();
		}
		((IWindow)splashScreen).Hide();
		return flag;
	}

	internal void Terminate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		logger.Log(string.Empty);
		logger.Info("Initiating shutdown procedure...");
		((IWindow)splashScreen).Show();
		((IWindow)splashScreen).BringToForeground();
		DelegateShutdownResponsibilities();
		if ((int)operations.TryRevert() == 3)
		{
			logger.Info("Application successfully finalized.");
			logger.Log(string.Empty);
		}
		else
		{
			logger.Info("Application shutdown failed!");
			logger.Log(string.Empty);
		}
		((IWindow)splashScreen).Close();
	}

	internal void UpdateAppConfig()
	{
		splashScreen.AppConfig = context.AppConfig;
	}

	private void DelegateStartupResponsibilities()
	{
		responsibilities.Delegate(ClientTask.RegisterEvents);
		responsibilities.Delegate(ClientTask.ShowShell);
		responsibilities.Delegate(ClientTask.AutoStartApplications);
	}

	private void DelegateClientReadyResponsibilities()
	{
	}

	private void DelegateShutdownResponsibilities()
	{
		responsibilities.Delegate(ClientTask.CloseShell);
		responsibilities.Delegate(ClientTask.DeregisterEvents);
	}

	private bool TryInformRuntime()
	{
		CommunicationResult val = runtime.InformClientReady();
		if (!val.Success)
		{
			logger.Error("Failed to inform runtime that client is ready!");
		}
		return val.Success;
	}
}
