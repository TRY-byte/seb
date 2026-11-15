using System;
using System.Timers;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Runtime.Responsibilities;

internal class IntegrityResponsibility : RuntimeResponsibility
{
	private readonly IIntegrityModule integrityModule;

	private readonly Action shutdown;

	private readonly Timer timer;

	public IntegrityResponsibility(IIntegrityModule integrityModule, ILogger logger, RuntimeContext runtimeContext, Action shutdown)
		: base(logger, runtimeContext)
	{
		this.integrityModule = integrityModule;
		this.shutdown = shutdown;
		timer = new Timer();
	}

	public override void Assume(RuntimeTask task)
	{
		switch (task)
		{
		case RuntimeTask.StartIntegrityMonitoring:
			StartIntegrityMonitoring();
			break;
		case RuntimeTask.StopIntegrityMonitoring:
			StopIntegrityMonitoring();
			break;
		}
	}

	private void StartIntegrityMonitoring()
	{
		timer.AutoReset = false;
		timer.Interval = 5000.0;
		timer.Elapsed += Timer_Elapsed;
		timer.Start();
		Logger.Info("Started monitoring runtime integrity.");
	}

	private void StopIntegrityMonitoring()
	{
		timer.Stop();
		timer.Elapsed -= Timer_Elapsed;
		Logger.Info("Stopped monitoring runtime integrity.");
	}

	private void Timer_Elapsed(object sender, ElapsedEventArgs e)
	{
		Logger.Info("Attempting to verify runtime integrity...");
		bool isValid = default;
		if (integrityModule.TryVerifyRuntimeIntegrity(ref isValid))
		{
			HandleRuntimeIntegrityStatus(isValid);
		}
		else
		{
			Logger.Warn("Failed to verify runtime integrity!");
		}
		timer.Start();
	}

	private void HandleRuntimeIntegrityStatus(bool isValid)
	{
		if (isValid)
		{
			Logger.Info("Runtime integrity successfully verified.");
		}
		else
		{
			Logger.Warn("Runtime integrity is compromised!");
		}
	}
}
