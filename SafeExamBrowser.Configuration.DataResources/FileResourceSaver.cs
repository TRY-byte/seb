using System;
using System.IO;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.DataResources;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataResources;

public class FileResourceSaver : IResourceSaver
{
	private ILogger logger;

	public FileResourceSaver(ILogger logger)
	{
		this.logger = logger;
	}

	public bool CanSave(Uri destination)
	{
		int num;
		if (destination.IsFile)
		{
			num = (Path.IsPathRooted(destination.LocalPath) ? 1 : 0);
			if (num != 0)
			{
				logger.Debug($"Can save data as '{destination}' since it defines an absolute file path.");
				return (byte)num != 0;
			}
		}
		else
		{
			num = 0;
		}
		logger.Debug($"Can't save data as '{destination}' since it doesn't define an absolute file path.");
		return (byte)num != 0;
	}

	public SaveStatus TrySave(Uri destination, Stream data)
	{
		string directoryName = Path.GetDirectoryName(destination.LocalPath);
		logger.Debug($"Attempting to save '{data}' with {(double)data.Length / 1000.0} KB data as '{destination}'...");
		if (!Directory.Exists(directoryName))
		{
			logger.Debug("Creating directory '" + directoryName + "'...");
			Directory.CreateDirectory(directoryName);
		}
		using (FileStream destination2 = new FileStream(destination.LocalPath, FileMode.Create))
		{
			data.Seek(0L, SeekOrigin.Begin);
			data.CopyTo(destination2);
		}
		logger.Debug($"Successfully saved data as '{destination}'.");
		return (SaveStatus)2;
	}
}
