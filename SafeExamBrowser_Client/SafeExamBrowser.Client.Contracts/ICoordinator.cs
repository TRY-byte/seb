namespace SafeExamBrowser.Client.Contracts;

internal interface ICoordinator
{
	bool IsReconfigurationLocked();

	bool IsSessionLocked();

	void ReleaseReconfigurationLock();

	void ReleaseSessionLock();

	bool RequestReconfigurationLock();

	bool RequestSessionLock();
}
