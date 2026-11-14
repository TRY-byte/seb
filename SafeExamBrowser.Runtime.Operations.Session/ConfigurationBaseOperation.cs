using System;
using SafeExamBrowser.Communication.Contracts.Data;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal abstract class ConfigurationBaseOperation : SessionOperation
{
	protected enum UriSource
	{
		Undefined,
		AppData,
		CommandLine,
		ProgramData,
		Reconfiguration,
		Server
	}

	protected readonly IConfigurationRepository repository;

	protected readonly IUserInterfaceFactory uiFactory;

	protected string AppDataFilePath => Context.Next.AppConfig.AppDataFilePath;

	protected string ProgramDataFilePath => Context.Next.AppConfig.ProgramDataFilePath;

	public ConfigurationBaseOperation(Dependencies dependencies, IConfigurationRepository repository, IUserInterfaceFactory uiFactory)
		: base(dependencies)
	{
		this.repository = repository;
		this.uiFactory = uiFactory;
	}

	protected Nullable TryLoadSettings(Uri uri, UriSource source, out PasswordParameters passwordParams, out AppSettings settings, string currentPassword = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Invalid comparison between Unknown and I4
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		passwordParams = new PasswordParameters
		{
			Password = string.Empty,
			IsHash = true
		};
		LoadStatus val = repository.TryLoadSettings(uri, ref settings, passwordParams);
		if ((int)val == 3 && currentPassword != null)
		{
			passwordParams.Password = currentPassword;
			passwordParams.IsHash = true;
			val = repository.TryLoadSettings(uri, ref settings, passwordParams);
		}
		for (int i = 0; i < 5; i++)
		{
			if ((int)val != 3)
			{
				break;
			}
			PasswordRequestPurpose purpose = (PasswordRequestPurpose)((source == UriSource.AppData || source == UriSource.ProgramData) ? 1 : 2);
			if (TryGetPassword(purpose, out var password))
			{
				passwordParams.Password = password;
				passwordParams.IsHash = false;
				val = repository.TryLoadSettings(uri, ref settings, passwordParams);
				continue;
			}
			return null;
		}
		return val;
	}

	protected bool TryGetPassword(PasswordRequestPurpose purpose, out string password)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (ClientBridge.IsRequired())
		{
			return ClientBridge.TryGetPassword(purpose, out password);
		}
		(TextKey message, TextKey title) textKeysFor = GetTextKeysFor(purpose);
		TextKey item = textKeysFor.message;
		TextKey item2 = textKeysFor.title;
		PasswordDialogResult val = uiFactory.CreatePasswordDialog(Text.Get(item), Text.Get(item2)).Show((IWindow)(object)RuntimeWindow);
		password = val.Password;
		return val.Success;
	}

	private (TextKey message, TextKey title) GetTextKeysFor(PasswordRequestPurpose purpose)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected I4, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TextKey item = (TextKey)0;
		TextKey item2 = (TextKey)0;
		switch ((int)purpose)
		{
		case 0:
			item = (TextKey)224;
			item2 = (TextKey)225;
			break;
		case 1:
			item = (TextKey)226;
			item2 = (TextKey)227;
			break;
		case 2:
			item = (TextKey)230;
			item2 = (TextKey)231;
			break;
		}
		return (message: item, title: item2);
	}
}
