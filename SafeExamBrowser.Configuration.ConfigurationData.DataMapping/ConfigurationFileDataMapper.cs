using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class ConfigurationFileDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (!(key == "sebConfigPurpose"))
		{
			if (key == "sebMode")
			{
				MapSessionMode(settings, value);
			}
		}
		else
		{
			MapConfigurationMode(settings, value);
		}
	}

	private void MapConfigurationMode(AppSettings settings, object value)
	{
		if (value is int num)
		{
			settings.ConfigurationMode = (ConfigurationMode)(num != 1);
		}
	}

	private void MapSessionMode(AppSettings settings, object value)
	{
		if (value is int num)
		{
			settings.SessionMode = (SessionMode)(num == 1);
		}
	}
}
