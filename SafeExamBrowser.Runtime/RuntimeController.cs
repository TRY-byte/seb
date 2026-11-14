using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Runtime.Responsibilities;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime;

internal class RuntimeController
{
	private readonly ILogger logger;

	private readonly IOperationSequence bootstrapSequence;

	private readonly IResponsibilityCollection<RuntimeTask> responsibilities;

	private readonly RuntimeContext runtimeContext;

	private readonly IRuntimeWindow runtimeWindow;

	private readonly ISplashScreen splashScreen;

	private bool SessionIsRunning => runtimeContext.Current != null;

	internal RuntimeController(ILogger logger, IOperationSequence bootstrapSequence, IResponsibilityCollection<RuntimeTask> responsibilities, RuntimeContext runtimeContext, IRuntimeWindow runtimeWindow, ISplashScreen splashScreen)
	{
		this.bootstrapSequence = bootstrapSequence;
		this.responsibilities = responsibilities;
		this.logger = logger;
		this.runtimeWindow = runtimeWindow;
		this.runtimeContext = runtimeContext;
		this.splashScreen = splashScreen;
	}

	internal bool TryStart()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		logger.Info("Initiating startup procedure...");
		((IWindow)runtimeWindow).Show();
		((IWindow)runtimeWindow).BringToForeground();
		((IProgressIndicator)runtimeWindow).SetIndeterminate();
		((IWindow)splashScreen).Show();
		((IWindow)splashScreen).BringToForeground();
		bool flag = (int)bootstrapSequence.TryPerform() == 3;
		if (flag)
		{
			responsibilities.Delegate(RuntimeTask.RegisterEvents);
			((IWindow)splashScreen).Hide();
			logger.Info("Application successfully initialized.");
			logger.Log(string.Empty);
			logger.Subscribe((ILogObserver)(object)runtimeWindow);
			responsibilities.Delegate(RuntimeTask.StartSession);
		}
		else
		{
			logger.Info("Application startup aborted!");
			logger.Log(string.Empty);
			responsibilities.Delegate(RuntimeTask.ShowStartupError);
		}
		if (flag)
		{
			return SessionIsRunning;
		}
		return false;
	}

	internal void Terminate()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Invalid comparison between Unknown and I4
		responsibilities.Delegate(RuntimeTask.DeregisterEvents);
		if (SessionIsRunning)
		{
			responsibilities.Delegate(RuntimeTask.StopSession);
		}
		logger.Unsubscribe((ILogObserver)(object)runtimeWindow);
		((IWindow)runtimeWindow).Close();
		((IWindow)splashScreen).Show();
		((IWindow)splashScreen).BringToForeground();
		logger.Log(string.Empty);
		logger.Info("Initiating shutdown procedure...");
		if ((int)bootstrapSequence.TryRevert() == 3)
		{
			logger.Info("Application successfully finalized.");
			logger.Log(string.Empty);
		}
		else
		{
			logger.Info("Shutdown procedure failed!");
			logger.Log(string.Empty);
			responsibilities.Delegate(RuntimeTask.ShowShutdownError);
		}
		((IWindow)splashScreen).Close();
	}
}
