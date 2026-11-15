using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Service;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class ServiceDataMapper : BaseDataMapper
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
			case 'e':
				if (key == "enableChromeNotifications")
				{
					MapEnableChromeNotifications(settings, value);
				}
				break;
			case 'i':
				if (key == "insideSebEnableSwitchUser")
				{
					MapEnableUserSwitch(settings, value);
				}
				break;
			}
			break;
		case 31:
			switch (key[15])
			{
			case 'S':
				if (key == "insideSebEnableStartTaskManager")
				{
					MapEnableTaskManager(settings, value);
				}
				break;
			case 'L':
				if (key == "insideSebEnableLockThisComputer")
				{
					MapEnableUserLock(settings, value);
				}
				break;
			}
			break;
		case 16:
			switch (key[10])
			{
			case 'I':
				if (key == "sebServiceIgnore")
				{
					MapIgnoreService(settings, value);
				}
				break;
			case 'P':
				if (key == "sebServicePolicy")
				{
					MapPolicy(settings, value);
				}
				break;
			}
			break;
		case 27:
			if (key == "insideSebEnableEaseOfAccess")
			{
				MapEnableEaseOfAccessOptions(settings, value);
			}
			break;
		case 17:
			if (key == "enableFindPrinter")
			{
				MapEnableFindPrinter(settings, value);
			}
			break;
		case 40:
			if (key == "insideSebEnableNetworkConnectionSelector")
			{
				MapEnableNetworkOptions(settings, value);
			}
			break;
		case 30:
			if (key == "insideSebEnableChangeAPassword")
			{
				MapEnablePasswordChange(settings, value);
			}
			break;
		case 23:
			if (key == "insideSebEnableShutDown")
			{
				MapEnablePowerOptions(settings, value);
			}
			break;
		case 18:
			if (key == "allowScreenSharing")
			{
				MapEnableRemoteConnections(settings, value);
			}
			break;
		case 21:
			if (key == "insideSebEnableLogOff")
			{
				MapEnableSignout(settings, value);
			}
			break;
		case 32:
			if (key == "insideSebEnableVmWareClientShade")
			{
				MapEnableVmwareOverlay(settings, value);
			}
			break;
		case 19:
			if (key == "enableWindowsUpdate")
			{
				MapEnableWindowsUpdate(settings, value);
			}
			break;
		case 22:
			if (key == "setVmwareConfiguration")
			{
				MapSetVmwareConfiguration(settings, value);
			}
			break;
		case 20:
		case 24:
		case 26:
		case 28:
		case 29:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
			break;
		}
	}

	private void MapEnableChromeNotifications(AppSettings settings, object value)
	{
		settings.Service.DisableChromeNotifications = false;
	}

	private void MapEnableEaseOfAccessOptions(AppSettings settings, object value)
	{
		settings.Service.DisableEaseOfAccessOptions = false;
	}

	private void MapEnableFindPrinter(AppSettings settings, object value)
	{
		settings.Service.DisableFindPrinter = false;
	}

	private void MapEnableNetworkOptions(AppSettings settings, object value)
	{
		settings.Service.DisableNetworkOptions = false;
	}

	private void MapEnablePasswordChange(AppSettings settings, object value)
	{
		settings.Service.DisablePasswordChange = false;
	}

	private void MapEnablePowerOptions(AppSettings settings, object value)
	{
		settings.Service.DisablePowerOptions = false;
	}

	private void MapEnableRemoteConnections(AppSettings settings, object value)
	{
		settings.Service.DisableRemoteConnections = false;
	}

	private void MapEnableSignout(AppSettings settings, object value)
	{
		settings.Service.DisableSignout = false;
	}

	private void MapEnableTaskManager(AppSettings settings, object value)
	{
		settings.Service.DisableTaskManager = false;
	}

	private void MapEnableUserLock(AppSettings settings, object value)
	{
		settings.Service.DisableUserLock = false;
	}

	private void MapEnableUserSwitch(AppSettings settings, object value)
	{
		settings.Service.DisableUserSwitch = false;
	}

	private void MapEnableVmwareOverlay(AppSettings settings, object value)
	{
		settings.Service.DisableVmwareOverlay = false;
	}

	private void MapEnableWindowsUpdate(AppSettings settings, object value)
	{
		settings.Service.DisableWindowsUpdate = false;
	}

	private void MapIgnoreService(AppSettings settings, object value)
	{
		settings.Service.IgnoreService = true;
	}

	private void MapPolicy(AppSettings settings, object value)
	{
		if (value is int num)
		{
			settings.Service.Policy = (ServicePolicy)(num switch
			{
				1 => 2, 
				2 => 0, 
				_ => 1, 
			});
		}
	}

	private void MapSetVmwareConfiguration(AppSettings settings, object value)
	{
		settings.Service.SetVmwareConfiguration = false;
	}
}
