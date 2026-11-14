using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class SystemDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == "systemAlwaysOn")
		{
			MapAlwaysOn(settings, value);
		}
	}

	private void MapAlwaysOn(AppSettings settings, object value)
	{
		if (value is bool alwaysOn)
		{
			settings.System.AlwaysOn = alwaysOn;
		}
	}
}
