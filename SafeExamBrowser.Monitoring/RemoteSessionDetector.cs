using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts;

namespace SafeExamBrowser.Monitoring;

public class RemoteSessionDetector : IRemoteSessionDetector
{
	private readonly IIntegrityModule integrityModule;

	private readonly ILogger logger;

	public RemoteSessionDetector(IIntegrityModule integrityModule, ILogger logger)
	{
		this.logger = logger;
		this.integrityModule = integrityModule;
	}

	public bool IsRemoteSession()
	{
		logger.Debug("System appears not to be running in a remote session.");
		return false;
	}
}
