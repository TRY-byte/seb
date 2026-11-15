namespace SafeExamBrowser.Runtime.Responsibilities;

internal enum RuntimeTask
{
	DeregisterEvents,
	DeregisterSessionEvents,
	RegisterEvents,
	RegisterSessionEvents,
	ShowShutdownError,
	ShowStartupError,
	StartIntegrityMonitoring,
	StartSession,
	StopIntegrityMonitoring,
	StopSession
}
