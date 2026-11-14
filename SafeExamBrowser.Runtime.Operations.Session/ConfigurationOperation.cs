using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Communication.Contracts.Data;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Security;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class ConfigurationOperation : ConfigurationBaseOperation
{
	private readonly string[] commandLineArgs;

	private readonly IFileSystem fileSystem;

	private readonly IHashAlgorithm hashAlgorithm;

	[CompilerGenerated]
	private new StatusChangedEventHandler m_StatusChanged;

	public override event StatusChangedEventHandler StatusChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public ConfigurationOperation(string[] commandLineArgs, Dependencies dependencies, IFileSystem fileSystem, IHashAlgorithm hashAlgorithm, IConfigurationRepository repository, IUserInterfaceFactory uiFactory)
		: base(dependencies, repository, uiFactory)
	{
		this.commandLineArgs = commandLineArgs;
		this.fileSystem = fileSystem;
		this.hashAlgorithm = hashAlgorithm;
	}

	public override OperationResult Perform()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Initializing application configuration...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)183);
		}
		OperationResult val2 = (OperationResult)2;
		val2 = ((!TryInitializeSettingsUri(out var uri, out var source)) ? LoadDefaultSettings() : LoadSettingsForStartup(uri, source));
		LogOperationResult(val2);
		return val2;
	}

	public override OperationResult Repeat()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Initializing new application configuration...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)183);
		}
		OperationResult result = (OperationResult)2;
		if (TryValidateSettingsUri(Context.ReconfigurationFilePath, out var uri))
		{
			result = LoadSettingsForReconfiguration(uri);
		}
		else
		{
			Logger.Warn("The resource specified for reconfiguration does not exist or is not valid!");
		}
		LogOperationResult(result);
		return result;
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private OperationResult LoadDefaultSettings()
	{
		Logger.Info("No valid configuration resource specified and no local client configuration found - loading default settings...");
		Context.Next.Settings = repository.LoadDefaultSettings();
		return (OperationResult)3;
	}

	private OperationResult LoadSettingsForStartup(Uri uri, UriSource source)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		string currentPassword = null;
		PasswordParameters passwordParams = null;
		OperationResult result = (OperationResult)1;
		AppSettings settings = null;
		Nullable val = null;
		if (source == UriSource.CommandLine)
		{
			bool flag = File.Exists(AppDataFilePath);
			bool flag2 = File.Exists(ProgramDataFilePath);
			PasswordParameters passwordParams2;
			if (flag2)
			{
				val = TryLoadSettings(new Uri(ProgramDataFilePath, UriKind.Absolute), UriSource.ProgramData, out passwordParams2, out settings);
			}
			else if (flag)
			{
				val = TryLoadSettings(new Uri(AppDataFilePath, UriKind.Absolute), UriSource.AppData, out passwordParams2, out settings);
			}
			if ((!flag2 && !flag) || (int)val.GetValueOrDefault() == 4)
			{
				currentPassword = ((settings != null) ? settings.Security.AdminPasswordHash : null);
				val = TryLoadSettings(uri, source, out passwordParams, out settings, currentPassword);
			}
		}
		else
		{
			val = TryLoadSettings(uri, source, out passwordParams, out settings);
		}
		if (val.HasValue)
		{
			result = DetermineLoadResult(uri, source, settings, val.Value, passwordParams, currentPassword);
		}
		return result;
	}

	private OperationResult LoadSettingsForReconfiguration(Uri uri)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		string adminPasswordHash = Context.Current.Settings.Security.AdminPasswordHash;
		OperationResult val = (OperationResult)1;
		UriSource source = UriSource.Reconfiguration;
		Nullable val2 = TryLoadSettings(uri, source, out var passwordParams, out var settings, adminPasswordHash);
		if (val2.HasValue)
		{
			val = DetermineLoadResult(uri, source, settings, val2.Value, passwordParams, adminPasswordHash);
		}
		if ((int)val == 3 && Context.Current.IsBrowserResource)
		{
			HandleReconfigurationByBrowserResource();
		}
		fileSystem.Delete(uri.LocalPath);
		Logger.Info($"Deleted temporary configuration file '{uri}'.");
		return val;
	}

	private OperationResult DetermineLoadResult(Uri uri, UriSource source, AppSettings settings, LoadStatus status, PasswordParameters passwordParams, string currentPassword = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		OperationResult result = (OperationResult)2;
		if ((int)status == 1 || (int)status == 4)
		{
			bool flag = source == UriSource.CommandLine || source == UriSource.Reconfiguration;
			Context.Next.Settings = settings;
			if ((int)status == 1)
			{
				result = HandleBrowserResource(uri);
			}
			else
			{
				result = ((!flag || (int)settings.ConfigurationMode != 0) ? ((OperationResult)3) : HandleClientConfiguration(uri, passwordParams, currentPassword));
			}
			HandleStartUrlQuery(uri, source);
		}
		else
		{
			ShowFailureMessage(status, uri);
		}
		return result;
	}

	private OperationResult HandleBrowserResource(Uri uri)
	{
		Context.Next.IsBrowserResource = true;
		Context.Next.Settings.Applications.Blacklist.Clear();
		Context.Next.Settings.Applications.Whitelist.Clear();
		Context.Next.Settings.Browser.DeleteCacheOnShutdown = false;
		Context.Next.Settings.Browser.DeleteCookiesOnShutdown = false;
		Context.Next.Settings.Browser.StartUrl = uri.AbsoluteUri;
		Context.Next.Settings.Display.AllowedDisplays = 10;
		Context.Next.Settings.Display.IgnoreError = true;
		Context.Next.Settings.Display.InternalDisplayOnly = false;
		Context.Next.Settings.Security.AllowReconfiguration = true;
		Context.Next.Settings.Security.VirtualMachinePolicy = (VirtualMachinePolicy)0;
		Context.Next.Settings.Service.IgnoreService = true;
		Context.Next.Settings.UserInterface.ActionCenter.EnableActionCenter = false;
		Logger.Info($"The configuration resource needs authentication or is a webpage, using '{uri}' as start URL for the browser.");
		return (OperationResult)3;
	}

	private OperationResult HandleClientConfiguration(Uri uri, PasswordParameters passwordParams, string currentPassword = null)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Context.Current == null;
		bool? flag2 = TryConfigureClient(uri, passwordParams, currentPassword);
		OperationResult result = (OperationResult)2;
		if (!flag2.HasValue || (((flag2 == true) & flag) && AbortAfterClientConfiguration()))
		{
			result = (OperationResult)1;
		}
		else if (flag2 == true)
		{
			result = (OperationResult)3;
		}
		return result;
	}

	private void HandleReconfigurationByBrowserResource()
	{
		Context.Next.Settings.Browser.DeleteCookiesOnStartup = false;
		Logger.Info("Some browser settings were overridden in order to retain a potential LMS / web application session.");
	}

	private void HandleStartUrlQuery(Uri uri, UriSource source)
	{
		if (source == UriSource.Reconfiguration && Uri.TryCreate(Context.ReconfigurationUrl, UriKind.Absolute, out var result))
		{
			uri = result;
		}
		if (uri != null && uri.Query.LastIndexOf('?') > 0)
		{
			Context.Next.Settings.Browser.StartUrlQuery = uri.Query.Substring(uri.Query.LastIndexOf('?'));
		}
	}

	private bool? TryConfigureClient(Uri uri, PasswordParameters passwordParams, string currentPassword = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = IsRequiredToAuthenticateForClientConfiguration(passwordParams, currentPassword);
		bool? result = true;
		Logger.Info("Starting client configuration...");
		if (flag)
		{
			result = AuthenticateForClientConfiguration(currentPassword);
		}
		else
		{
			Logger.Info("Authentication is not required.");
		}
		if (result == true)
		{
			SaveStatus val = repository.ConfigureClientWith(uri, passwordParams);
			result = (int)val == 2;
			if (result == true)
			{
				Logger.Info("Client configuration was successful.");
			}
			else
			{
				Logger.Error($"Client configuration failed with status '{val}'!");
				ShowMessageBox((TextKey)100, (TextKey)101, (MessageBoxAction)0, (MessageBoxIcon)0);
			}
		}
		return result;
	}

	private bool IsRequiredToAuthenticateForClientConfiguration(PasswordParameters passwordParams, string currentPassword = null)
	{
		bool flag = currentPassword != null;
		if (flag)
		{
			string adminPasswordHash = Context.Next.Settings.Security.AdminPasswordHash;
			bool flag2 = passwordParams.Password != null;
			if (currentPassword.Equals(adminPasswordHash, StringComparison.OrdinalIgnoreCase))
			{
				flag = false;
			}
			else if (flag2)
			{
				string value = (passwordParams.IsHash ? passwordParams.Password : hashAlgorithm.GenerateHashFor(passwordParams.Password));
				flag = !currentPassword.Equals(value, StringComparison.OrdinalIgnoreCase);
			}
		}
		return flag;
	}

	private bool? AuthenticateForClientConfiguration(string currentPassword)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		bool? flag = null;
		for (int i = 0; i < 5; i++)
		{
			if (flag == true)
			{
				break;
			}
			if (TryGetPassword((PasswordRequestPurpose)0, out var password))
			{
				flag = currentPassword.Equals(hashAlgorithm.GenerateHashFor(password), StringComparison.OrdinalIgnoreCase);
				continue;
			}
			flag = null;
			break;
		}
		if (flag == true)
		{
			Logger.Info("Authentication was successful.");
		}
		if (flag == false)
		{
			Logger.Info("Authentication has failed!");
			ShowMessageBox((TextKey)116, (TextKey)117, (MessageBoxAction)0, (MessageBoxIcon)0);
		}
		if (!flag.HasValue)
		{
			Logger.Info("Authentication was aborted.");
		}
		return flag;
	}

	private bool AbortAfterClientConfiguration()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		TextKey messageKey = (TextKey)102;
		TextKey titleKey = (TextKey)103;
		bool flag = (int)ShowMessageBox(messageKey, titleKey, (MessageBoxAction)2, (MessageBoxIcon)2) == 4;
		Logger.Info("The user chose to " + (flag ? "abort" : "continue") + " startup after successful client configuration.");
		return flag;
	}

	private void ShowFailureMessage(LoadStatus status, Uri uri)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected I4, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		MessageBoxIcon icon = (MessageBoxIcon)0;
		TextKey messageKey = (TextKey)0;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		TextKey titleKey = (TextKey)0;
		switch ((int)status)
		{
		case 3:
			messageKey = (TextKey)116;
			titleKey = (TextKey)117;
			break;
		case 0:
			messageKey = (TextKey)112;
			dictionary["%%URI%%"] = uri.ToString();
			titleKey = (TextKey)113;
			break;
		case 2:
			messageKey = (TextKey)123;
			dictionary["%%URI%%"] = uri.ToString();
			titleKey = (TextKey)124;
			break;
		case 5:
			messageKey = (TextKey)152;
			dictionary["%%URI%%"] = uri.ToString();
			titleKey = (TextKey)153;
			break;
		}
		ShowMessageBox(messageKey, titleKey, (MessageBoxAction)0, icon, dictionary);
	}

	private bool TryInitializeSettingsUri(out Uri uri, out UriSource source)
	{
		bool flag = false;
		uri = null;
		source = UriSource.Undefined;
		string[] array = commandLineArgs;
		if (array != null && array.Length > 1)
		{
			flag = Uri.TryCreate(commandLineArgs[1], UriKind.Absolute, out uri);
			source = UriSource.CommandLine;
			Logger.Info(string.Format("Found command-line argument for configuration resource: '{0}', the URI is {1}.", uri, flag ? "valid" : "invalid"));
		}
		if (!flag && File.Exists(ProgramDataFilePath))
		{
			flag = Uri.TryCreate(ProgramDataFilePath, UriKind.Absolute, out uri);
			source = UriSource.ProgramData;
			Logger.Info($"Found configuration file in program data directory: '{uri}'.");
		}
		if (!flag && File.Exists(AppDataFilePath))
		{
			flag = Uri.TryCreate(AppDataFilePath, UriKind.Absolute, out uri);
			source = UriSource.AppData;
			Logger.Info($"Found configuration file in app data directory: '{uri}'.");
		}
		return flag;
	}

	private bool TryValidateSettingsUri(string path, out Uri uri)
	{
		return Uri.TryCreate(path, UriKind.Absolute, out uri) & (uri != null && uri.IsFile) & File.Exists(path);
	}

	private void LogOperationResult(OperationResult result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected I4, but got Unknown
		switch (result - 1)
		{
		case 0:
			Logger.Info("The configuration was aborted by the user.");
			break;
		case 1:
			Logger.Warn("The configuration has failed!");
			break;
		case 2:
			Logger.Info("The configuration was successful.");
			break;
		}
	}
}
