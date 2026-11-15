using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class ApplicationDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (!(key == "prohibitedProcesses"))
		{
			if (key == "permittedProcesses")
			{
				MapApplicationWhitelist(settings, value);
			}
		}
		else
		{
			MapApplicationBlacklist(settings, value);
		}
	}

	private void MapApplicationBlacklist(AppSettings settings, object value)
	{
	}

	private void MapApplicationWhitelist(AppSettings settings, object value)
	{
	}
}
