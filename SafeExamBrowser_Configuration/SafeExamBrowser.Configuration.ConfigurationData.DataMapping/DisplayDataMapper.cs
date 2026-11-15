using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class DisplayDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		switch (key)
		{
		case "allowedDisplaysMaxNumber":
			MapAllowedDisplays(settings, value);
			break;
		case "displayAlwaysOn":
			MapAlwaysOn(settings, value);
			break;
		case "allowedDisplaysIgnoreFailure":
			MapIgnoreError(settings, value);
			break;
		case "allowedDisplayBuiltinEnforce":
			MapInternalDisplayOnly(settings, value);
			break;
		}
	}

	private void MapAllowedDisplays(AppSettings settings, object value)
	{
		settings.Display.AllowedDisplays = 500;
	}

	private void MapAlwaysOn(AppSettings settings, object value)
	{
		if (value is bool alwaysOn)
		{
			settings.Display.AlwaysOn = alwaysOn;
		}
	}

	private void MapIgnoreError(AppSettings settings, object value)
	{
		settings.Display.IgnoreError = true;
	}

	private void MapInternalDisplayOnly(AppSettings settings, object value)
	{
		settings.Display.InternalDisplayOnly = false;
	}
}
