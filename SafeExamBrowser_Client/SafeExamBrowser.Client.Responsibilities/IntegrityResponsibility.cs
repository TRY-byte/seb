using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using SafeExamBrowser.Client.Contracts;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Client.Responsibilities;

internal class IntegrityResponsibility : ClientResponsibility
{
	private readonly ICoordinator coordinator;

	private readonly IText text;

	private readonly Timer timer;

	private IIntegrityModule IntegrityModule => Context.IntegrityModule;

	public IntegrityResponsibility(ClientContext context, ICoordinator coordinator, ILogger logger, IText text)
		: base(context, logger)
	{
		this.coordinator = coordinator;
		this.text = text;
		timer = new Timer();
	}

	public override void Assume(ClientTask task)
	{
		switch (task)
		{
		case ClientTask.PrepareShutdown_Wave2:
			StopIntegrityMonitoring();
			break;
		case ClientTask.ScheduleIntegrityVerification:
			ScheduleIntegrityVerification();
			break;
		case ClientTask.StartMonitoring:
			StartIntegrityMonitoring();
			break;
		case ClientTask.UpdateSessionIntegrity:
			UpdateSessionIntegrity();
			break;
		case ClientTask.VerifySessionIntegrity:
			VerifySessionIntegrity();
			break;
		case ClientTask.RegisterEvents:
		case ClientTask.ShowShell:
			break;
		}
	}

	private void ScheduleIntegrityVerification()
	{
		Task.Delay(TimeSpan.FromMinutes(10.0) + TimeSpan.FromMinutes(new Random().NextDouble() * 5.0)).ContinueWith((Task _) =>
		{
			VerifyApplicationIntegrity();
		});
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
		if (IntegrityModule.TryVerifyRuntimeIntegrity(ref isValid))
		{
			HandleRuntimeIntegrityStatus(isValid);
		}
		else
		{
			Logger.Warn("Failed to verify runtime integrity!");
		}
		timer.Start();
	}

	private void UpdateSessionIntegrity()
	{
		AppSettings settings = Settings;
		if (!string.IsNullOrEmpty((settings != null) ? settings.Security.QuitPasswordHash : null))
		{
			IIntegrityModule integrityModule = IntegrityModule;
			if (integrityModule != null)
			{
				integrityModule.ClearSession(Settings.Browser.ConfigurationKey, Settings.Browser.StartUrl);
			}
		}
	}

	private void VerifyApplicationIntegrity()
	{
		Logger.Info("Attempting to verify application integrity...");
		bool isValid = default;
		if (IntegrityModule.TryVerifyCodeSignature(ref isValid))
		{
			HandleApplicationIntegrityStatus(isValid);
		}
		else
		{
			Logger.Warn("Failed to verify application integrity!");
		}
	}

	private void VerifySessionIntegrity()
	{
		if (!string.IsNullOrEmpty(Settings.Security.QuitPasswordHash) && Settings.Security.VerifySessionIntegrity)
		{
			Logger.Info("Attempting to verify session integrity...");
			bool isValid = default;
			if (IntegrityModule.TryVerifySessionIntegrity(Settings.Browser.ConfigurationKey, Settings.Browser.StartUrl, ref isValid))
			{
				HandleSessionIntegrityStatus(isValid);
			}
			else
			{
				Logger.Warn("Failed to verify session integrity!");
			}
		}
	}

	private void HandleApplicationIntegrityStatus(bool isValid)
	{
		if (isValid)
		{
			Logger.Info("Application integrity successfully verified.");
		}
		else if (coordinator.RequestSessionLock())
		{
			Logger.Warn("Application integrity is compromised!");
			ShowLockScreen(text.Get((TextKey)55), text.Get((TextKey)73), Enumerable.Empty<LockScreenOption>());
			coordinator.ReleaseSessionLock();
		}
		else
		{
			Logger.Warn("Application integrity is compromised but lock screen is already active!");
			Task.Delay(TimeSpan.FromMinutes(1.0)).ContinueWith((Task _) =>
			{
				VerifyApplicationIntegrity();
			});
		}
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

	private void HandleSessionIntegrityStatus(bool isValid)
	{
		if (isValid)
		{
			Logger.Info("Session integrity successfully verified.");
			IntegrityModule.CacheSession(Settings.Browser.ConfigurationKey, Settings.Browser.StartUrl);
		}
		else if (coordinator.RequestSessionLock())
		{
			Logger.Warn("Session integrity is compromised!");
			Task.Delay(1000).ContinueWith((Task _) =>
			{
				ShowLockScreen(text.Get((TextKey)69), text.Get((TextKey)73), Enumerable.Empty<LockScreenOption>());
				coordinator.ReleaseSessionLock();
			});
		}
		else
		{
			Logger.Warn("Session integrity is compromised but lock screen is already active!");
			Task.Delay(TimeSpan.FromMinutes(1.0)).ContinueWith((Task _) =>
			{
				VerifySessionIntegrity();
			});
		}
	}
}
