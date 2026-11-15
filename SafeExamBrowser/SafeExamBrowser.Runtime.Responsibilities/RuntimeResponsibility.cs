using System;
using System.IO;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Runtime.Responsibilities;

internal abstract class RuntimeResponsibility : IResponsibility<RuntimeTask>
{
	protected RuntimeContext Context { get; private set; }

	protected ILogger Logger { get; private set; }

	protected SessionConfiguration Session => Context.Current;

	protected bool SessionIsRunning => Session != null;

	internal RuntimeResponsibility(ILogger logger, RuntimeContext runtimeContext)
	{
		Logger = logger;
		Context = runtimeContext;
	}

	public abstract void Assume(RuntimeTask task);

	protected string AppendLogFilePaths(AppConfig appConfig, string message)
	{
		if (File.Exists(appConfig.BrowserLogFilePath))
		{
			message = message + Environment.NewLine + Environment.NewLine + appConfig.BrowserLogFilePath;
		}
		if (File.Exists(appConfig.ClientLogFilePath))
		{
			message = message + Environment.NewLine + Environment.NewLine + appConfig.ClientLogFilePath;
		}
		if (File.Exists(appConfig.RuntimeLogFilePath))
		{
			message = message + Environment.NewLine + Environment.NewLine + appConfig.RuntimeLogFilePath;
		}
		if (File.Exists(appConfig.ServiceLogFilePath))
		{
			message = message + Environment.NewLine + Environment.NewLine + appConfig.ServiceLogFilePath;
		}
		return message;
	}
}
