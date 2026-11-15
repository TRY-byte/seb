namespace SafeExamBrowser.Client.Responsibilities;

internal enum ClientTask
{
	AutoStartApplications,
	CloseShell,
	DeregisterEvents,
	PrepareShutdown_Wave1,
	PrepareShutdown_Wave2,
	RegisterEvents,
	ScheduleIntegrityVerification,
	ShowShell,
	StartMonitoring,
	UpdateSessionIntegrity,
	VerifySessionIntegrity
}
