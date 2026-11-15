using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Client.Responsibilities;

internal class ApplicationsResponsibility : ClientResponsibility
{
	public ApplicationsResponsibility(ClientContext context, ILogger logger)
		: base(context, logger)
	{
	}

	public override void Assume(ClientTask task)
	{
		if (task == ClientTask.AutoStartApplications)
		{
			AutoStart();
		}
	}

	private void AutoStart()
	{
		foreach (IApplication<IApplicationWindow> application in Context.Applications)
		{
			if (application.AutoStart)
			{
				Logger.Info("Auto-starting '" + application.Name + "'...");
				application.Start();
			}
		}
	}
}
