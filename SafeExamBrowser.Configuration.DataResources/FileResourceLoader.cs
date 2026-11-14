using System;
using System.IO;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.DataResources;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataResources;

public class FileResourceLoader : IResourceLoader
{
	private ILogger logger;

	public FileResourceLoader(ILogger logger)
	{
		this.logger = logger;
	}

	public bool CanLoad(Uri resource)
	{
		int num;
		if (resource.IsFile)
		{
			num = (File.Exists(resource.LocalPath) ? 1 : 0);
			if (num != 0)
			{
				logger.Debug($"Can load '{resource}' as it is an existing file.");
				return (byte)num != 0;
			}
		}
		else
		{
			num = 0;
		}
		logger.Debug($"Can't load '{resource}' as it isn't an existing file.");
		return (byte)num != 0;
	}

	public LoadStatus TryLoad(Uri resource, out Stream data)
	{
		logger.Debug($"Loading data from '{resource}'...");
		data = new FileStream(resource.LocalPath, FileMode.Open, FileAccess.Read);
		logger.Debug($"Created '{data}' for {(double)data.Length / 1000.0} KB data in '{resource}'.");
		return (LoadStatus)4;
	}
}
