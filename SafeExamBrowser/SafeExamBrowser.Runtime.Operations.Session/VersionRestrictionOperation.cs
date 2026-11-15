using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Settings.Security;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class VersionRestrictionOperation : SessionOperation
{
	[CompilerGenerated]
	private new StatusChangedEventHandler m_StatusChanged;

	private IList<VersionRestriction> Restrictions => Context.Next.Settings.Security.VersionRestrictions;

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

	public VersionRestrictionOperation(Dependencies dependencies)
		: base(dependencies)
	{
	}

	public override OperationResult Perform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ValidateRestrictions();
	}

	public override OperationResult Repeat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ValidateRestrictions();
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private OperationResult ValidateRestrictions()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		OperationResult result = (OperationResult)3;
		Logger.Info("Validating version restrictions...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)211);
		}
		if (Restrictions.Any())
		{
			string text = "'" + string.Join("', '", Restrictions) + "'";
			string programInformationalVersion = Context.Next.AppConfig.ProgramInformationalVersion;
			if (Restrictions.Any((VersionRestriction r) => IsFulfilled(r)))
			{
				Logger.Info("The installed SEB version '" + programInformationalVersion + "' complies with the version restrictions: " + text + ".");
			}
			else
			{
				result = (OperationResult)1;
				Logger.Error("The installed SEB version '" + programInformationalVersion + "' does not comply with the version restrictions: " + text + ".");
				ShowErrorMessage(programInformationalVersion);
			}
		}
		else
		{
			Logger.Info("There are no version restrictions for the configuration.");
		}
		return result;
	}

	private bool IsFulfilled(VersionRestriction restriction)
	{
		bool flag = true;
		var (num, num2, num3, num4, flag2) = GetVersion();
		if (restriction.IsMinimumRestriction)
		{
			flag &= restriction.Major <= num;
			if (restriction.Major == num)
			{
				flag &= restriction.Minor <= num2;
				if (restriction.Minor == num2)
				{
					flag &= !restriction.Patch.HasValue || restriction.Patch <= num3;
					if (restriction.Patch == num3)
					{
						flag &= !restriction.Build.HasValue || restriction.Build <= num4;
					}
				}
			}
			return flag & (!restriction.RequiresAllianceEdition | flag2);
		}
		flag &= restriction.Major == num;
		flag &= restriction.Minor == num2;
		flag &= !restriction.Patch.HasValue || restriction.Patch == num3;
		flag &= !restriction.Build.HasValue || restriction.Build == num4;
		return flag & (!restriction.RequiresAllianceEdition | flag2);
	}

	private (int major, int minor, int patch, int build, bool isAllianceEdition) GetVersion()
	{
		string[] array = Context.Next.AppConfig.ProgramBuildVersion.Split('.');
		int item = int.Parse(array[0]);
		int item2 = int.Parse(array[1]);
		int item3 = int.Parse(array[2]);
		int item4 = int.Parse(array[3]);
		bool item5 = Context.Next.AppConfig.ProgramInformationalVersion.Contains("Alliance Edition");
		return (major: item, minor: item2, patch: item3, build: item4, isAllianceEdition: item5);
	}

	private string BuildRequiredVersions()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text = Text.Get((TextKey)158);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		foreach (VersionRestriction restriction in Restrictions)
		{
			string text2 = (restriction.Build.HasValue ? $".{restriction.Build}" : "");
			string text3 = (restriction.Patch.HasValue ? $".{restriction.Patch}" : "");
			string text4 = (restriction.RequiresAllianceEdition ? " Alliance Edition" : "");
			string text5 = $"{restriction.Major}.{restriction.Minor}{text3}{text2}{text4}";
			if (restriction.IsMinimumRestriction)
			{
				stringBuilder.AppendLine(text.Replace("%%_VERSION_%%", text5));
			}
			else
			{
				stringBuilder.AppendLine("SEB " + text5);
			}
		}
		stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}

	private void ShowErrorMessage(string version)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		TextKey messageKey = (TextKey)156;
		TextKey titleKey = (TextKey)157;
		Dictionary<string, string> messagePlaceholders = new Dictionary<string, string>
		{
			{ "%%_VERSION_%%", version },
			{
				"%%_REQUIRED_VERSIONS_%%",
				BuildRequiredVersions()
			}
		};
		ShowMessageBox(messageKey, titleKey, (MessageBoxAction)0, (MessageBoxIcon)0, messagePlaceholders);
	}
}
