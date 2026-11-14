using System.Collections.Generic;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Browser;
using SafeExamBrowser.Settings.Browser.Filter;
using SafeExamBrowser.Settings.Browser.Proxy;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class BrowserDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == null)
		{
			return;
		}
		switch (key.Length)
		{
		case 24:
			switch (key[0])
			{
			case 'd':
				if (key == "downloadAndOpenSebConfig")
				{
					MapAllowConfigurationDownloads(settings, value);
				}
				break;
			case 'a':
				if (key == "allowBrowsingBackForward")
				{
					MapAllowNavigation(settings, value);
				}
				break;
			case 'b':
				if (key == "browserWindowAllowReload")
				{
					MapAllowReload(settings, value);
				}
				break;
			case 'c':
				break;
			}
			break;
		case 21:
			switch (key[5])
			{
			case 'D':
				if (key == "allowDeveloperConsole")
				{
					MapAllowDeveloperConsole(settings, value);
				}
				break;
			case 'P':
				if (key == "allowPDFReaderToolbar")
				{
					MapAllowPdfReaderToolbar(settings, value);
				}
				break;
			}
			break;
		case 14:
			switch (key[7])
			{
			case 'w':
				if (key == "allowDownloads")
				{
					MapAllowDownloads(settings, value);
				}
				break;
			case 'o':
				if (key == "enableZoomPage")
				{
					MapAllowPageZoom(settings, value);
				}
				break;
			case 'e':
				if (key == "URLFilterRules")
				{
					MapFilterRules(settings, value);
				}
				break;
			case 'E':
				if (key == "restartExamURL")
				{
					MapHomeButtonUrl(settings, value);
				}
				break;
			case 'C':
				if (key == "quitURLConfirm")
				{
					MapQuitUrlConfirmation(settings, value);
				}
				break;
			case 'R':
				if (key == "quitURLRestart")
				{
					MapResetOnQuitUrl(settings, value);
				}
				break;
			}
			break;
		case 15:
			switch (key[0])
			{
			case 'a':
				if (key == "allowSpellCheck")
				{
					MapAllowSpellChecking(settings, value);
				}
				break;
			case 'r':
				if (key == "restartExamText")
				{
					MapHomeButtonMessage(settings, value);
				}
				break;
			case 'b':
				if (key == "browserViewMode")
				{
					MapMainWindowMode(settings, value);
				}
				break;
			}
			break;
		case 31:
			switch (key[0])
			{
			case 'n':
				if (key == "newBrowserWindowAllowAddressBar")
				{
					MapAllowAddressBarAdditionalWindow(settings, value);
				}
				break;
			case 'u':
				if (key == "useTemporaryDownUploadDirectory")
				{
					MapUseTemporaryDownUploadDirectory(settings, value);
				}
				break;
			}
			break;
		case 26:
			switch (key[0])
			{
			case 'n':
				if (key == "newBrowserWindowNavigation")
				{
					MapAllowNavigationAdditionalWindow(settings, value);
				}
				break;
			case 'e':
				if (key == "enableBrowserWindowToolbar")
				{
					MapShowToolbar(settings, value);
				}
				break;
			}
			break;
		case 27:
			switch (key[16])
			{
			case 'A':
				if (key == "newBrowserWindowAllowReload")
				{
					MapAllowReloadAdditionalWindow(settings, value);
				}
				break;
			case 'B':
				if (key == "newBrowserWindowByLinkWidth")
				{
					MapWindowWidthAdditionalWindow(settings, value);
				}
				break;
			}
			break;
		case 33:
			switch (key[16])
			{
			case 'S':
				if (key == "newBrowserWindowShowReloadWarning")
				{
					MapShowReloadWarningAdditionalWindow(settings, value);
				}
				break;
			case 'B':
				if (key == "newBrowserWindowByLinkPositioning")
				{
					MapWindowPositionAdditionalWindow(settings, value);
				}
				break;
			}
			break;
		case 23:
			switch (key[0])
			{
			case 'n':
				if (key == "newBrowserWindowShowURL")
				{
					MapUrlPolicy(settings.Browser.AdditionalWindow, value);
				}
				break;
			case 'm':
				if (key == "mainBrowserWindowHeight")
				{
					MapWindowHeightMainWindow(settings, value);
				}
				break;
			}
			break;
		case 28:
			switch (key[0])
			{
			case 'n':
				if (key == "newBrowserWindowByLinkHeight")
				{
					MapWindowHeightAdditionalWindow(settings, value);
				}
				break;
			case 'e':
				if (key == "examSessionClearCookiesOnEnd")
				{
					MapDeleteCookiesOnShutdown(settings, value);
				}
				break;
			case 'r':
				if (key == "restartExamPasswordProtected")
				{
					MapHomeButtonRequiresPassword(settings, value);
				}
				break;
			case 'b':
				if (key == "browserWindowAllowAddressBar")
				{
					MapAllowAddressBar(settings, value);
				}
				break;
			case 'm':
				if (key == "mainBrowserWindowPositioning")
				{
					MapWindowPositionMainWindow(settings, value);
				}
				break;
			case 's':
				if (key == "startURLAppendQueryParameter")
				{
					MapUseStartUrlQuery(settings, value);
				}
				break;
			}
			break;
		case 20:
			switch (key[0])
			{
			case 'r':
				if (key == "removeBrowserProfile")
				{
					MapDeleteCacheOnShutdown(settings, value);
				}
				break;
			case 'd':
				if (key == "downloadDirectoryWin")
				{
					MapDownloadDirectory(settings, value);
				}
				break;
			case 'b':
				if (key == "browserWindowShowURL")
				{
					MapUrlPolicy(settings.Browser.MainWindow, value);
				}
				break;
			}
			break;
		case 16:
			switch (key[0])
			{
			case 'd':
				if (key == "downloadPDFFiles")
				{
					MapDownloadPdfFiles(settings, value);
				}
				break;
			case 'e':
				if (key == "enableSebBrowser")
				{
					MapEnableBrowser(settings, value);
				}
				break;
			case 's':
				if (key == "showReloadButton")
				{
					MapShowReloadButton(settings, value);
				}
				break;
			case 'b':
				if (key == "browserUserAgent")
				{
					MapUserAgentSuffix(settings, value);
				}
				break;
			}
			break;
		case 22:
			switch (key[0])
			{
			case 'r':
				if (key == "restartExamUseStartURL")
				{
					MapHomeButtonUseStartUrl(settings, value);
				}
				break;
			case 'm':
				if (key == "mainBrowserWindowWidth")
				{
					MapWindowWidthMainWindow(settings, value);
				}
				break;
			}
			break;
		case 7:
			switch (key[0])
			{
			case 'p':
				if (key == "proxies")
				{
					MapProxySettings(settings, value);
				}
				break;
			case 'q':
				if (key == "quitURL")
				{
					MapQuitUrl(settings, value);
				}
				break;
			}
			break;
		case 29:
			if (key == "allowCustomDownUploadLocation")
			{
				MapAllowCustomDownloadLocation(settings, value);
			}
			break;
		case 9:
			if (key == "allowFind")
			{
				MapAllowFind(settings, value);
			}
			break;
		case 10:
			if (key == "allowPrint")
			{
				MapAllowPrint(settings, value);
			}
			break;
		case 12:
			if (key == "allowUploads")
			{
				MapAllowUploads(settings, value);
			}
			break;
		case 30:
			if (key == "examSessionClearCookiesOnStart")
			{
				MapDeleteCookiesOnStartup(settings, value);
			}
			break;
		case 11:
			if (key == "examKeySalt")
			{
				MapExamKeySalt(settings, value);
			}
			break;
		case 17:
			if (key == "showReloadWarning")
			{
				MapShowReloadWarning(settings, value);
			}
			break;
		case 19:
			if (key == "proxySettingsPolicy")
			{
				MapProxyPolicy(settings, value);
			}
			break;
		case 18:
			if (key == "sendBrowserExamKey")
			{
				MapSendCustomHeaders(settings, value);
			}
			break;
		case 32:
			if (key == "browserShowFileSystemElementPath")
			{
				MapShowFileSystemElementPath(settings, value);
			}
			break;
		case 8:
			if (key == "startURL")
			{
				MapStartUrl(settings, value);
			}
			break;
		case 13:
		case 25:
			break;
		}
	}

	internal override void MapGlobal(IDictionary<string, object> rawData, AppSettings settings)
	{
		MapPopupPolicy(rawData, settings);
		MapRequestFilter(rawData, settings);
		MapUserAgentMode(rawData, settings);
	}

	private void MapAllowAddressBar(AppSettings settings, object value)
	{
		if (value is bool allowAddressBar)
		{
			settings.Browser.MainWindow.AllowAddressBar = allowAddressBar;
		}
	}

	private void MapAllowAddressBarAdditionalWindow(AppSettings settings, object value)
	{
		if (value is bool allowAddressBar)
		{
			settings.Browser.AdditionalWindow.AllowAddressBar = allowAddressBar;
		}
	}

	private void MapAllowConfigurationDownloads(AppSettings settings, object value)
	{
		settings.Browser.AllowConfigurationDownloads = true;
	}

	private void MapAllowCustomDownloadLocation(AppSettings settings, object value)
	{
		settings.Browser.AllowCustomDownAndUploadLocation = true;
	}

	private void MapAllowDeveloperConsole(AppSettings settings, object value)
	{
		settings.Browser.MainWindow.AllowDeveloperConsole = true;
		settings.Browser.AdditionalWindow.AllowDeveloperConsole = true;
	}

	private void MapAllowDownloads(AppSettings settings, object value)
	{
		settings.Browser.AllowDownloads = true;
	}

	private void MapAllowFind(AppSettings settings, object value)
	{
		settings.Browser.AllowFind = true;
	}

	private void MapAllowNavigation(AppSettings settings, object value)
	{
		settings.Browser.MainWindow.AllowBackwardNavigation = true;
		settings.Browser.MainWindow.AllowForwardNavigation = true;
	}

	private void MapAllowNavigationAdditionalWindow(AppSettings settings, object value)
	{
		settings.Browser.AdditionalWindow.AllowBackwardNavigation = true;
		settings.Browser.AdditionalWindow.AllowForwardNavigation = true;
	}

	private void MapAllowPageZoom(AppSettings settings, object value)
	{
		settings.Browser.AllowPageZoom = true;
	}

	private void MapAllowPdfReaderToolbar(AppSettings settings, object value)
	{
		settings.Browser.AllowPdfReaderToolbar = true;
	}

	private void MapAllowPrint(AppSettings settings, object value)
	{
		settings.Browser.AllowPrint = true;
	}

	private void MapAllowReload(AppSettings settings, object value)
	{
		settings.Browser.MainWindow.AllowReloading = true;
	}

	private void MapAllowReloadAdditionalWindow(AppSettings settings, object value)
	{
		settings.Browser.AdditionalWindow.AllowReloading = true;
	}

	private void MapAllowSpellChecking(AppSettings settings, object value)
	{
		settings.Browser.AllowSpellChecking = true;
	}

	private void MapAllowUploads(AppSettings settings, object value)
	{
		settings.Browser.AllowUploads = true;
	}

	private void MapDeleteCacheOnShutdown(AppSettings settings, object value)
	{
		if (value is bool deleteCacheOnShutdown)
		{
			settings.Browser.DeleteCacheOnShutdown = deleteCacheOnShutdown;
		}
	}

	private void MapDeleteCookiesOnShutdown(AppSettings settings, object value)
	{
		if (value is bool deleteCookiesOnShutdown)
		{
			settings.Browser.DeleteCookiesOnShutdown = deleteCookiesOnShutdown;
		}
	}

	private void MapDeleteCookiesOnStartup(AppSettings settings, object value)
	{
		if (value is bool deleteCookiesOnStartup)
		{
			settings.Browser.DeleteCookiesOnStartup = deleteCookiesOnStartup;
		}
	}

	private void MapDownloadDirectory(AppSettings settings, object value)
	{
		if (value is string downAndUploadDirectory)
		{
			settings.Browser.DownAndUploadDirectory = downAndUploadDirectory;
		}
	}

	private void MapDownloadPdfFiles(AppSettings settings, object value)
	{
		if (value is bool flag)
		{
			settings.Browser.AllowPdfReader = !flag;
		}
	}

	private void MapEnableBrowser(AppSettings settings, object value)
	{
		settings.Browser.EnableBrowser = true;
	}

	private void MapExamKeySalt(AppSettings settings, object value)
	{
		if (value is byte[] browserExamKeySalt)
		{
			settings.Browser.BrowserExamKeySalt = browserExamKeySalt;
		}
	}

	private void MapHomeButtonMessage(AppSettings settings, object value)
	{
		if (value is string homeNavigationMessage)
		{
			settings.Browser.HomeNavigationMessage = homeNavigationMessage;
		}
	}

	private void MapHomeButtonRequiresPassword(AppSettings settings, object value)
	{
		settings.Browser.HomeNavigationRequiresPassword = false;
	}

	private void MapHomeButtonUrl(AppSettings settings, object value)
	{
		settings.Browser.HomeUrl = "https://google.com";
	}

	private void MapHomeButtonUseStartUrl(AppSettings settings, object value)
	{
		settings.Browser.UseStartUrlAsHomeUrl = false;
	}

	private void MapMainWindowMode(AppSettings settings, object value)
	{
		settings.Browser.MainWindow.FullScreenMode = false;
	}

	private void MapPopupPolicy(IDictionary<string, object> rawData, AppSettings settings)
	{
		rawData.TryGetValue("newBrowserWindowByLinkPolicy", out var value);
		bool flag = rawData.TryGetValue("newBrowserWindowByLinkBlockForeign", out var value2) && value2 as bool? == true;
		if (value is int)
		{
			switch ((int)value)
			{
			case 2:
				settings.Browser.PopupPolicy = (PopupPolicy)(flag ? 1 : 0);
				break;
			case 0:
				settings.Browser.PopupPolicy = (PopupPolicy)4;
				break;
			case 1:
				settings.Browser.PopupPolicy = (PopupPolicy)(flag ? 2 : 3);
				break;
			}
		}
	}

	private void MapQuitUrl(AppSettings settings, object value)
	{
		if (value is string quitUrl)
		{
			settings.Browser.QuitUrl = quitUrl;
		}
	}

	private void MapQuitUrlConfirmation(AppSettings settings, object value)
	{
		if (value is bool confirmQuitUrl)
		{
			settings.Browser.ConfirmQuitUrl = confirmQuitUrl;
		}
	}

	private void MapResetOnQuitUrl(AppSettings settings, object value)
	{
		if (value is bool resetOnQuitUrl)
		{
			settings.Browser.ResetOnQuitUrl = resetOnQuitUrl;
		}
	}

	private void MapShowReloadButton(AppSettings settings, object value)
	{
		settings.Browser.AdditionalWindow.ShowReloadButton = true;
		settings.Browser.MainWindow.ShowReloadButton = true;
	}

	private void MapRequestFilter(IDictionary<string, object> rawData, AppSettings settings)
	{
		bool flag = rawData.TryGetValue("URLFilterEnable", out var value) && value as bool? == true;
		bool flag2 = rawData.TryGetValue("URLFilterEnableContentFilter", out value) && value as bool? == true;
		settings.Browser.Filter.ProcessMainRequests = flag;
		settings.Browser.Filter.ProcessContentRequests = flag & flag2;
	}

	private void MapShowReloadWarning(AppSettings settings, object value)
	{
		if (value is bool showReloadWarning)
		{
			settings.Browser.MainWindow.ShowReloadWarning = showReloadWarning;
		}
	}

	private void MapSendCustomHeaders(AppSettings settings, object value)
	{
		if (value is bool flag)
		{
			settings.Browser.SendConfigurationKey = flag;
			settings.Browser.SendBrowserExamKey = flag;
		}
	}

	private void MapShowFileSystemElementPath(AppSettings settings, object value)
	{
		settings.Browser.ShowFileSystemElementPath = true;
	}

	private void MapShowReloadWarningAdditionalWindow(AppSettings settings, object value)
	{
		if (value is bool showReloadWarning)
		{
			settings.Browser.AdditionalWindow.ShowReloadWarning = showReloadWarning;
		}
	}

	private void MapShowToolbar(AppSettings settings, object value)
	{
		settings.Browser.AdditionalWindow.ShowToolbar = true;
		settings.Browser.MainWindow.ShowToolbar = true;
	}

	private void MapStartUrl(AppSettings settings, object value)
	{
		if (value is string startUrl)
		{
			settings.Browser.StartUrl = startUrl;
		}
	}

	private void MapUrlPolicy(WindowSettings settings, object value)
	{
		if (value is int)
		{
			switch ((int)value)
			{
			case 3:
				settings.UrlPolicy = (UrlPolicy)0;
				break;
			case 2:
				settings.UrlPolicy = (UrlPolicy)1;
				break;
			case 1:
				settings.UrlPolicy = (UrlPolicy)2;
				break;
			case 0:
				settings.UrlPolicy = (UrlPolicy)3;
				break;
			}
		}
	}

	private void MapUserAgentMode(IDictionary<string, object> rawData, AppSettings settings)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		bool flag = rawData.TryGetValue("browserUserAgentWinDesktopMode", out var value) && value as int? != 0;
		bool flag2 = rawData.TryGetValue("browserUserAgentWinTouchMode", out value) && value as int? != 0;
		if (((int)settings.UserInterface.Mode == 0) & flag)
		{
			settings.Browser.UseCustomUserAgent = true;
			settings.Browser.CustomUserAgent = rawData["browserUserAgentWinDesktopModeCustom"] as string;
		}
		else if (((int)settings.UserInterface.Mode == 1) & flag2)
		{
			settings.Browser.UseCustomUserAgent = true;
			settings.Browser.CustomUserAgent = rawData["browserUserAgentWinTouchModeCustom"] as string;
		}
	}

	private void MapUserAgentSuffix(AppSettings settings, object value)
	{
		if (value is string userAgentSuffix)
		{
			settings.Browser.UserAgentSuffix = userAgentSuffix;
		}
	}

	private void MapUseStartUrlQuery(AppSettings settings, object value)
	{
		if (value is bool useQueryParameter)
		{
			settings.Browser.UseQueryParameter = useQueryParameter;
		}
	}

	private void MapUseTemporaryDownUploadDirectory(AppSettings settings, object value)
	{
		settings.Browser.UseTemporaryDownAndUploadDirectory = false;
	}

	private void MapFilterRules(AppSettings settings, object value)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected Obj, but got Unknown
		if (!(value is IList<object> list))
		{
			return;
		}
		bool flag = default;
		foreach (object item in list)
		{
			if (!(item is IDictionary<string, object> dictionary))
			{
				continue;
			}
			int num;
			if (dictionary.TryGetValue("active", out var value2))
			{
				if (value2 is bool)
				{
					flag = (bool)value2;
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
			else
			{
				num = 0;
			}
			if (((uint)num & (flag ? 1u : 0u)) != 0)
			{
				FilterRuleSettings val = new FilterRuleSettings();
				if (dictionary.TryGetValue("expression", out value2) && value2 is string expression)
				{
					val.Expression = expression;
				}
				if (dictionary.TryGetValue("action", out value2) && value2 is int)
				{
					_ = (int)value2;
					val.Result = (FilterResult)0;
				}
				if (dictionary.TryGetValue("regex", out value2) && value2 is bool flag2)
				{
					val.Type = (FilterRuleType)(!flag2);
				}
				settings.Browser.Filter.Rules.Add(val);
			}
		}
	}

	private void MapProxySettings(AppSettings settings, object value)
	{
		if (!(value is IDictionary<string, object> dictionary))
		{
			return;
		}
		if (dictionary.TryGetValue("AutoConfigurationEnabled", out var value2) && value2 is bool autoConfigure)
		{
			settings.Browser.Proxy.AutoConfigure = autoConfigure;
		}
		if (dictionary.TryGetValue("AutoConfigurationURL", out value2) && value2 is string autoConfigureUrl)
		{
			settings.Browser.Proxy.AutoConfigureUrl = autoConfigureUrl;
		}
		if (dictionary.TryGetValue("AutoDiscoveryEnabled", out value2) && value2 is bool autoDetect)
		{
			settings.Browser.Proxy.AutoDetect = autoDetect;
		}
		if (dictionary.TryGetValue("ExceptionsList", out value2) && value2 is IList<object> bypassList)
		{
			MapProxyBypassList(settings, bypassList);
		}
		bool flag = default;
		int num;
		if (dictionary.TryGetValue("FTPEnable", out value2))
		{
			if (value2 is bool)
			{
				flag = (bool)value2;
				num = 1;
			}
			else
			{
				num = 0;
			}
		}
		else
		{
			num = 0;
		}
		if (((uint)num & (flag ? 1u : 0u)) != 0)
		{
			MapFtpProxy(settings, dictionary);
		}
		bool flag2 = default;
		int num2;
		if (dictionary.TryGetValue("HTTPEnable", out value2))
		{
			if (value2 is bool)
			{
				flag2 = (bool)value2;
				num2 = 1;
			}
			else
			{
				num2 = 0;
			}
		}
		else
		{
			num2 = 0;
		}
		if (((uint)num2 & (flag2 ? 1u : 0u)) != 0)
		{
			MapHttpProxy(settings, dictionary);
		}
		bool flag3 = default;
		int num3;
		if (dictionary.TryGetValue("HTTPSEnable", out value2))
		{
			if (value2 is bool)
			{
				flag3 = (bool)value2;
				num3 = 1;
			}
			else
			{
				num3 = 0;
			}
		}
		else
		{
			num3 = 0;
		}
		if (((uint)num3 & (flag3 ? 1u : 0u)) != 0)
		{
			MapHttpsProxy(settings, dictionary);
		}
		bool flag4 = default;
		int num4;
		if (dictionary.TryGetValue("SOCKSEnable", out value2))
		{
			if (value2 is bool)
			{
				flag4 = (bool)value2;
				num4 = 1;
			}
			else
			{
				num4 = 0;
			}
		}
		else
		{
			num4 = 0;
		}
		if (((uint)num4 & (flag4 ? 1u : 0u)) != 0)
		{
			MapSocksProxy(settings, dictionary);
		}
	}

	private void MapProxyBypassList(AppSettings settings, IList<object> bypassList)
	{
		foreach (object bypass in bypassList)
		{
			if (bypass is string item)
			{
				settings.Browser.Proxy.BypassList.Add(item);
			}
		}
	}

	private void MapFtpProxy(AppSettings settings, IDictionary<string, object> data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		ProxyConfiguration val = new ProxyConfiguration
		{
			Protocol = (ProxyProtocol)0
		};
		if (data.TryGetValue("FTPProxy", out var value) && value is string host)
		{
			val.Host = host;
		}
		if (data.TryGetValue("FTPPassword", out value) && value is string password)
		{
			val.Password = password;
		}
		if (data.TryGetValue("FTPPort", out value) && value is int port)
		{
			val.Port = port;
		}
		if (data.TryGetValue("FTPRequiresPassword", out value) && value is bool requiresAuthentication)
		{
			val.RequiresAuthentication = requiresAuthentication;
		}
		if (data.TryGetValue("FTPUsername", out value) && value is string username)
		{
			val.Username = username;
		}
		settings.Browser.Proxy.Proxies.Add(val);
	}

	private void MapHttpProxy(AppSettings settings, IDictionary<string, object> data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		ProxyConfiguration val = new ProxyConfiguration
		{
			Protocol = (ProxyProtocol)1
		};
		if (data.TryGetValue("HTTPProxy", out var value) && value is string host)
		{
			val.Host = host;
		}
		if (data.TryGetValue("HTTPPassword", out value) && value is string password)
		{
			val.Password = password;
		}
		if (data.TryGetValue("HTTPPort", out value) && value is int port)
		{
			val.Port = port;
		}
		if (data.TryGetValue("HTTPRequiresPassword", out value) && value is bool requiresAuthentication)
		{
			val.RequiresAuthentication = requiresAuthentication;
		}
		if (data.TryGetValue("HTTPUsername", out value) && value is string username)
		{
			val.Username = username;
		}
		settings.Browser.Proxy.Proxies.Add(val);
	}

	private void MapHttpsProxy(AppSettings settings, IDictionary<string, object> data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		ProxyConfiguration val = new ProxyConfiguration
		{
			Protocol = (ProxyProtocol)2
		};
		if (data.TryGetValue("HTTPSProxy", out var value) && value is string host)
		{
			val.Host = host;
		}
		if (data.TryGetValue("HTTPSPassword", out value) && value is string password)
		{
			val.Password = password;
		}
		if (data.TryGetValue("HTTPSPort", out value) && value is int port)
		{
			val.Port = port;
		}
		if (data.TryGetValue("HTTPSRequiresPassword", out value) && value is bool requiresAuthentication)
		{
			val.RequiresAuthentication = requiresAuthentication;
		}
		if (data.TryGetValue("HTTPSUsername", out value) && value is string username)
		{
			val.Username = username;
		}
		settings.Browser.Proxy.Proxies.Add(val);
	}

	private void MapSocksProxy(AppSettings settings, IDictionary<string, object> data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		ProxyConfiguration val = new ProxyConfiguration
		{
			Protocol = (ProxyProtocol)3
		};
		if (data.TryGetValue("SOCKSProxy", out var value) && value is string host)
		{
			val.Host = host;
		}
		if (data.TryGetValue("SOCKSPassword", out value) && value is string password)
		{
			val.Password = password;
		}
		if (data.TryGetValue("SOCKSPort", out value) && value is int port)
		{
			val.Port = port;
		}
		if (data.TryGetValue("SOCKSRequiresPassword", out value) && value is bool requiresAuthentication)
		{
			val.RequiresAuthentication = requiresAuthentication;
		}
		if (data.TryGetValue("SOCKSUsername", out value) && value is string username)
		{
			val.Username = username;
		}
		settings.Browser.Proxy.Proxies.Add(val);
	}

	private void MapProxyPolicy(AppSettings settings, object value)
	{
		if (value is int num)
		{
			switch (num)
			{
			case 1:
				settings.Browser.Proxy.Policy = (ProxyPolicy)0;
				break;
			case 0:
				settings.Browser.Proxy.Policy = (ProxyPolicy)1;
				break;
			}
		}
	}

	private void MapWindowHeightAdditionalWindow(AppSettings settings, object value)
	{
		if (value is string text)
		{
			int result2;
			if (text.EndsWith("%") && int.TryParse(text.Replace("%", string.Empty), out var result))
			{
				settings.Browser.AdditionalWindow.AbsoluteHeight = null;
				settings.Browser.AdditionalWindow.RelativeHeight = result;
			}
			else if (int.TryParse(text, out result2))
			{
				settings.Browser.AdditionalWindow.AbsoluteHeight = result2;
				settings.Browser.AdditionalWindow.RelativeHeight = null;
			}
		}
	}

	private void MapWindowHeightMainWindow(AppSettings settings, object value)
	{
		if (value is string text)
		{
			int result2;
			if (text.EndsWith("%") && int.TryParse(text.Replace("%", string.Empty), out var result))
			{
				settings.Browser.MainWindow.AbsoluteHeight = null;
				settings.Browser.MainWindow.RelativeHeight = result;
			}
			else if (int.TryParse(text, out result2))
			{
				settings.Browser.MainWindow.AbsoluteHeight = result2;
				settings.Browser.MainWindow.RelativeHeight = null;
			}
		}
	}

	private void MapWindowPositionAdditionalWindow(AppSettings settings, object value)
	{
		if (value is int)
		{
			switch ((int)value)
			{
			case 0:
				settings.Browser.AdditionalWindow.Position = (WindowPosition)0;
				break;
			case 1:
				settings.Browser.AdditionalWindow.Position = (WindowPosition)1;
				break;
			case 2:
				settings.Browser.AdditionalWindow.Position = (WindowPosition)2;
				break;
			}
		}
	}

	private void MapWindowPositionMainWindow(AppSettings settings, object value)
	{
		if (value is int)
		{
			switch ((int)value)
			{
			case 0:
				settings.Browser.MainWindow.Position = (WindowPosition)0;
				break;
			case 1:
				settings.Browser.MainWindow.Position = (WindowPosition)1;
				break;
			case 2:
				settings.Browser.MainWindow.Position = (WindowPosition)2;
				break;
			}
		}
	}

	private void MapWindowWidthAdditionalWindow(AppSettings settings, object value)
	{
		if (value is string text)
		{
			int result2;
			if (text.EndsWith("%") && int.TryParse(text.Replace("%", string.Empty), out var result))
			{
				settings.Browser.AdditionalWindow.AbsoluteWidth = null;
				settings.Browser.AdditionalWindow.RelativeWidth = result;
			}
			else if (int.TryParse(text, out result2))
			{
				settings.Browser.AdditionalWindow.AbsoluteWidth = result2;
				settings.Browser.AdditionalWindow.RelativeWidth = null;
			}
		}
	}

	private void MapWindowWidthMainWindow(AppSettings settings, object value)
	{
		if (value is string text)
		{
			int result2;
			if (text.EndsWith("%") && int.TryParse(text.Replace("%", string.Empty), out var result))
			{
				settings.Browser.MainWindow.AbsoluteWidth = null;
				settings.Browser.MainWindow.RelativeWidth = result;
			}
			else if (int.TryParse(text, out result2))
			{
				settings.Browser.MainWindow.AbsoluteWidth = result2;
				settings.Browser.MainWindow.RelativeWidth = null;
			}
		}
	}
}
