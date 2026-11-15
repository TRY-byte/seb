using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Logging;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class GeneralDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == "logLevel")
		{
			MapLogLevel(settings, value);
		}
	}

	private void MapLogLevel(AppSettings settings, object value)
	{
		settings.LogLevel = (LogLevel)0;
	}
}
