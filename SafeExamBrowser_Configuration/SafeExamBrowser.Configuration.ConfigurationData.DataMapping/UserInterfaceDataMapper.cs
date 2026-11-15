using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.UserInterface;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class UserInterfaceDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == null)
		{
			return;
		}
		switch (key.Length)
		{
		case 25:
			switch (key[0])
			{
			case 'l':
				if (key == "lockScreenBackgroundColor")
				{
					MapLockScreenBackgroundColor(settings, value);
				}
				break;
			case 'b':
				if (key == "batteryChargeThresholdLow")
				{
					MapChargeThresholdLow(settings, value);
				}
				break;
			}
			break;
		case 12:
			if (key == "showSideMenu")
			{
				MapEnableActionCenter(settings, value);
			}
			break;
		case 19:
			if (key == "audioControlEnabled")
			{
				MapShowAudio(settings, value);
			}
			break;
		case 8:
			if (key == "showTime")
			{
				MapShowClock(settings, value);
			}
			break;
		case 17:
			if (key == "showInputLanguage")
			{
				MapShowKeyboardLayout(settings, value);
			}
			break;
		case 9:
			if (key == "allowWlan")
			{
				MapShowNetwork(settings, value);
			}
			break;
		case 30:
			if (key == "batteryChargeThresholdCritical")
			{
				MapChargeThresholdCritical(settings, value);
			}
			break;
		case 11:
			if (key == "showTaskBar")
			{
				MapEnableTaskbar(settings, value);
			}
			break;
		case 24:
			if (key == "showApplicationLogButton")
			{
				MapShowApplicationLog(settings, value);
			}
			break;
		case 14:
			if (key == "touchOptimized")
			{
				MapUserInterfaceMode(settings, value);
			}
			break;
		}
	}

	private void MapEnableActionCenter(AppSettings settings, object value)
	{
		if (value is bool enableActionCenter)
		{
			settings.UserInterface.ActionCenter.EnableActionCenter = enableActionCenter;
		}
	}

	private void MapLockScreenBackgroundColor(AppSettings settings, object value)
	{
		if (value is string backgroundColor)
		{
			settings.UserInterface.LockScreen.BackgroundColor = backgroundColor;
		}
	}

	private void MapShowAudio(AppSettings settings, object value)
	{
		if (value is bool showAudio)
		{
			settings.UserInterface.ActionCenter.ShowAudio = showAudio;
			settings.UserInterface.Taskbar.ShowAudio = showAudio;
		}
	}

	private void MapShowClock(AppSettings settings, object value)
	{
		if (value is bool showClock)
		{
			settings.UserInterface.ActionCenter.ShowClock = showClock;
			settings.UserInterface.Taskbar.ShowClock = showClock;
		}
	}

	private void MapShowKeyboardLayout(AppSettings settings, object value)
	{
		if (value is bool showKeyboardLayout)
		{
			settings.UserInterface.ActionCenter.ShowKeyboardLayout = showKeyboardLayout;
			settings.UserInterface.Taskbar.ShowKeyboardLayout = showKeyboardLayout;
		}
	}

	private void MapShowNetwork(AppSettings settings, object value)
	{
		if (value is bool showNetwork)
		{
			settings.UserInterface.ActionCenter.ShowNetwork = showNetwork;
			settings.UserInterface.Taskbar.ShowNetwork = showNetwork;
		}
	}

	private void MapChargeThresholdCritical(AppSettings settings, object value)
	{
		if (value is double chargeThresholdCritical)
		{
			settings.PowerSupply.ChargeThresholdCritical = chargeThresholdCritical;
		}
	}

	private void MapChargeThresholdLow(AppSettings settings, object value)
	{
		if (value is double chargeThresholdLow)
		{
			settings.PowerSupply.ChargeThresholdLow = chargeThresholdLow;
		}
	}

	private void MapEnableTaskbar(AppSettings settings, object value)
	{
		if (value is bool enableTaskbar)
		{
			settings.UserInterface.Taskbar.EnableTaskbar = enableTaskbar;
		}
	}

	private void MapShowApplicationLog(AppSettings settings, object value)
	{
		if (value is bool showApplicationLog)
		{
			settings.UserInterface.Taskbar.ShowApplicationLog = showApplicationLog;
		}
	}

	private void MapUserInterfaceMode(AppSettings settings, object value)
	{
		if (value is bool flag)
		{
			settings.UserInterface.Mode = (UserInterfaceMode)(flag ? 1 : 0);
		}
	}
}
