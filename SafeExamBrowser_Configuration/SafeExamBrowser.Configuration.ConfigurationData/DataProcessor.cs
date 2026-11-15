using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Applications;

namespace SafeExamBrowser.Configuration.ConfigurationData;

internal class DataProcessor
{
	internal void Process(IDictionary<string, object> rawData, AppSettings settings)
	{
		ProcessDefault(settings);
		CalculateConfigurationKey(rawData, settings);
	}

	internal void ProcessDefault(AppSettings settings)
	{
		AllowBrowserToolbarForReloading(settings);
		InitializeBrowserHomeFunctionality(settings);
		InitializeClipboardSettings(settings);
		InitializeProctoringSettings(settings);
		RemoveLegacyBrowsers(settings);
	}

	private void AllowBrowserToolbarForReloading(AppSettings settings)
	{
		if (settings.Browser.AdditionalWindow.AllowReloading && settings.Browser.AdditionalWindow.ShowReloadButton)
		{
			settings.Browser.AdditionalWindow.ShowToolbar = true;
		}
		if (settings.Browser.MainWindow.AllowReloading && settings.Browser.MainWindow.ShowReloadButton)
		{
			settings.Browser.MainWindow.ShowToolbar = true;
		}
	}

	private void CalculateConfigurationKey(IDictionary<string, object> rawData, AppSettings settings)
	{
		using SHA256Managed sHA256Managed = new SHA256Managed();
		using MemoryStream memoryStream = new MemoryStream();
		using StreamWriter streamWriter = new StreamWriter(memoryStream);
		Json.Serialize(rawData, streamWriter);
		streamWriter.Flush();
		memoryStream.Seek(0L, SeekOrigin.Begin);
		string configurationKey = BitConverter.ToString(sHA256Managed.ComputeHash(memoryStream)).ToLower().Replace("-", string.Empty);
		settings.Browser.ConfigurationKey = configurationKey;
	}

	private void InitializeBrowserHomeFunctionality(AppSettings settings)
	{
		settings.Browser.MainWindow.ShowHomeButton = settings.Browser.UseStartUrlAsHomeUrl || !string.IsNullOrWhiteSpace(settings.Browser.HomeUrl);
		settings.Browser.HomePasswordHash = settings.Security.QuitPasswordHash;
	}

	private void InitializeClipboardSettings(AppSettings settings)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Invalid comparison between Unknown and I4
		settings.Browser.UseIsolatedClipboard = (int)settings.Security.ClipboardPolicy == 2;
		settings.Keyboard.AllowCtrlC = (int)settings.Security.ClipboardPolicy != 1;
		settings.Keyboard.AllowCtrlV = (int)settings.Security.ClipboardPolicy != 1;
		settings.Keyboard.AllowCtrlX = (int)settings.Security.ClipboardPolicy != 1;
	}

	private void InitializeProctoringSettings(AppSettings settings)
	{
		settings.Proctoring.Enabled = settings.Proctoring.ScreenProctoring.Enabled;
	}

	private void RemoveLegacyBrowsers(AppSettings settings)
	{
		List<WhitelistApplication> list = new List<WhitelistApplication>();
		foreach (WhitelistApplication item in settings.Applications.Whitelist)
		{
			bool num = item.ExecutablePath?.Contains("xulrunner") ?? false;
			bool flag = item.ExecutableName?.Equals("firefox.exe", StringComparison.OrdinalIgnoreCase) ?? false;
			bool flag2 = item.ExecutableName?.Equals("xulrunner.exe", StringComparison.OrdinalIgnoreCase) ?? false;
			if (num && (flag | flag2))
			{
				list.Add(item);
			}
		}
		foreach (WhitelistApplication item2 in list)
		{
			settings.Applications.Whitelist.Remove(item2);
		}
	}
}
