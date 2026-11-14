using System.Collections.Generic;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Security;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class SecurityDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == null)
		{
			return;
		}
		switch (key.Length)
		{
		case 19:
			switch (key[0])
			{
			case 'h':
				if (key == "hashedAdminPassword")
				{
					MapAdminPasswordHash(settings, value);
				}
				break;
			case 'a':
				if (key == "allowVirtualMachine")
				{
					MapVirtualMachinePolicy(settings, value);
				}
				break;
			}
			break;
		case 15:
			switch (key[0])
			{
			case 'a':
				if (key == "allowStickyKeys")
				{
					MapAllowStickyKeys(settings, value);
				}
				break;
			case 'c':
				if (key == "clipboardPolicy")
				{
					MapClipboardPolicy(settings, value);
				}
				break;
			}
			break;
		case 18:
			switch (key[0])
			{
			case 'a':
				if (key == "allowScreenSharing")
				{
					MapAllowWindowCapture(settings, value);
				}
				break;
			case 'h':
				if (key == "hashedQuitPassword")
				{
					MapQuitPasswordHash(settings, value);
				}
				break;
			case 's':
				if (key == "sebAllowedVersions")
				{
					MapVersionRestrictions(settings, value);
				}
				break;
			}
			break;
		case 27:
			if (key == "examSessionReconfigureAllow")
			{
				MapAllowReconfiguration(settings, value);
			}
			break;
		case 9:
			if (key == "allowQuit")
			{
				MapAllowTermination(settings, value);
			}
			break;
		case 30:
			if (key == "disableSessionChangeLockScreen")
			{
				MapDisableSessionChangeLockScreen(settings, value);
			}
			break;
		case 31:
			if (key == "examSessionReconfigureConfigURL")
			{
				MapReconfigurationUrl(settings, value);
			}
			break;
		case 24:
			if (key == "enableCursorVerification")
			{
				MapVerifyCursorConfiguration(settings, value);
			}
			break;
		case 25:
			if (key == "enableSessionVerification")
			{
				MapVerifySessionIntegrity(settings, value);
			}
			break;
		}
	}

	internal override void MapGlobal(IDictionary<string, object> rawData, AppSettings settings)
	{
		MapApplicationLogAccess(rawData, settings);
		MapKioskMode(rawData, settings);
	}

	private void MapAdminPasswordHash(AppSettings settings, object value)
	{
		settings.Security.AdminPasswordHash = "";
	}

	private void MapAllowReconfiguration(AppSettings settings, object value)
	{
		settings.Security.AllowReconfiguration = true;
	}

	private void MapAllowStickyKeys(AppSettings settings, object value)
	{
		settings.Security.AllowStickyKeys = true;
	}

	private void MapAllowTermination(AppSettings settings, object value)
	{
		settings.Security.AllowTermination = true;
	}

	private void MapAllowWindowCapture(AppSettings settings, object value)
	{
		settings.Security.AllowWindowCapture = true;
	}

	private void MapApplicationLogAccess(IDictionary<string, object> rawData, AppSettings settings)
	{
		if (rawData.TryGetValue("allowApplicationLog", out var value) && value is bool allowApplicationLogAccess)
		{
			settings.Security.AllowApplicationLogAccess = allowApplicationLogAccess;
		}
		if (settings.Security.AllowApplicationLogAccess)
		{
			settings.UserInterface.ActionCenter.ShowApplicationLog = true;
			return;
		}
		settings.UserInterface.ActionCenter.ShowApplicationLog = false;
		settings.UserInterface.Taskbar.ShowApplicationLog = false;
	}

	private void MapKioskMode(IDictionary<string, object> rawData, AppSettings settings)
	{
		bool flag = rawData.TryGetValue("createNewDesktop", out var value);
		bool flag2 = rawData.TryGetValue("killExplorerShell", out var value2);
		if ((flag & flag2) && value as bool? == false && value2 as bool? == false)
		{
			settings.Security.KioskMode = (KioskMode)0;
		}
		else
		{
			settings.Security.KioskMode = (KioskMode)2;
		}
	}

	private void MapQuitPasswordHash(AppSettings settings, object value)
	{
		settings.Security.QuitPasswordHash = "";
	}

	private void MapClipboardPolicy(AppSettings settings, object value)
	{
		settings.Security.ClipboardPolicy = (ClipboardPolicy)0;
	}

	private void MapDisableSessionChangeLockScreen(AppSettings settings, object value)
	{
		settings.Security.DisableSessionChangeLockScreen = false;
	}

	private void MapVirtualMachinePolicy(AppSettings settings, object value)
	{
		settings.Security.VirtualMachinePolicy = (VirtualMachinePolicy)0;
	}

	private void MapReconfigurationUrl(AppSettings settings, object value)
	{
		if (value is string reconfigurationUrl)
		{
			settings.Security.ReconfigurationUrl = reconfigurationUrl;
		}
	}

	private void MapVerifyCursorConfiguration(AppSettings settings, object value)
	{
		settings.Security.VerifyCursorConfiguration = false;
	}

	private void MapVerifySessionIntegrity(AppSettings settings, object value)
	{
		settings.Security.VerifySessionIntegrity = false;
	}

	private void MapVersionRestrictions(AppSettings settings, object value)
	{
	}
}
