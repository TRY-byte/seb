using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class AudioDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		switch (key)
		{
		case "audioVolumeLevel":
			MapInitialVolumeLevel(settings, value);
			break;
		case "audioMute":
			MapMuteAudio(settings, value);
			break;
		case "audioSetVolumeLevel":
			MapSetInitialVolumeLevel(settings, value);
			break;
		}
	}

	private void MapInitialVolumeLevel(AppSettings settings, object value)
	{
		settings.Audio.InitialVolume = 67;
	}

	private void MapMuteAudio(AppSettings settings, object value)
	{
		settings.Audio.MuteAudio = false;
	}

	private void MapSetInitialVolumeLevel(AppSettings settings, object value)
	{
		settings.Audio.InitializeVolume = true;
	}
}
