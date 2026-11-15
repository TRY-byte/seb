using System;
using System.Collections.Generic;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Server.Contracts.Data;
using SafeExamBrowser.Settings;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Client.Responsibilities;

internal abstract class ClientResponsibility : IResponsibility<ClientTask>
{
	protected ClientContext Context { get; private set; }

	protected ILogger Logger { get; private set; }

	protected AppSettings Settings => Context.Settings;

	internal ClientResponsibility(ClientContext context, ILogger logger)
	{
		Context = context;
		Logger = logger;
	}

	public abstract void Assume(ClientTask task);

	protected void PauseActivators()
	{
		foreach (IActivator activator in Context.Activators)
		{
			activator.Pause();
		}
	}

	protected bool IsValidQuitPassword(string password)
	{
		string value = Context.HashAlgorithm.GenerateHashFor(password);
		bool flag = Settings.Security.QuitPasswordHash.Equals(value, StringComparison.OrdinalIgnoreCase);
		if (flag)
		{
			Context.QuitPasswordValidated = true;
		}
		return flag;
	}

	protected void PrepareShutdown()
	{
		Context.Responsibilities.Delegate(ClientTask.PrepareShutdown_Wave1);
		Context.Responsibilities.Delegate(ClientTask.PrepareShutdown_Wave2);
	}

	protected void ResumeActivators()
	{
		foreach (IActivator activator in Context.Activators)
		{
			activator.Resume();
		}
	}

	protected LockScreenResult ShowLockScreen(string message, string title, IEnumerable<LockScreenOption> options)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Invalid comparison between Unknown and I4
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Invalid comparison between Unknown and I4
		Logger.Info("Showing lock screen...");
		PauseActivators();
		Context.LockScreen = Context.UserInterfaceFactory.CreateLockScreen(message, title, options, Settings.UserInterface.LockScreen);
		((IWindow)Context.LockScreen).Show();
		if ((int)Settings.SessionMode == 1)
		{
			SendLockScreenNotification(message);
		}
		LockScreenResult result = WaitForLockScreenResolution();
		((IWindow)Context.LockScreen).Close();
		Context.LockScreen = null;
		ResumeActivators();
		if ((int)Settings.SessionMode == 1)
		{
			SendLockScreenConfirmation();
		}
		Logger.Info("Closed lock screen.");
		return result;
	}

	protected bool TryRequestShutdown()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		PrepareShutdown();
		CommunicationResult val = Context.Runtime.RequestShutdown();
		if (!val.Success)
		{
			Logger.Error("Failed to communicate shutdown request to the runtime!");
			Context.MessageBox.Show((TextKey)132, (TextKey)133, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)null);
		}
		return val.Success;
	}

	private void SendLockScreenConfirmation()
	{
		ServerResponse val = Context.Server.ConfirmLockScreen();
		if (!val.Success)
		{
			Logger.Error("Failed to send lock screen confirmation to server! Message: " + val.Message + ".");
		}
	}

	private void SendLockScreenNotification(string message)
	{
		ServerResponse val = Context.Server.LockScreen(message);
		if (!val.Success)
		{
			Logger.Error("Failed to send lock screen notification to server! Message: " + val.Message + ".");
		}
	}

	private LockScreenResult WaitForLockScreenResolution()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !string.IsNullOrEmpty(Settings.Security.QuitPasswordHash);
		LockScreenResult val = null;
		bool flag2 = false;
		while (!flag2)
		{
			val = Context.LockScreen.WaitForResult();
			if (val.Canceled)
			{
				Logger.Info("The lock screen has been canceled automatically.");
				flag2 = true;
			}
			else if (flag)
			{
				string value = Context.HashAlgorithm.GenerateHashFor(val.Password);
				if (Settings.Security.QuitPasswordHash.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					Logger.Info("The user entered the correct unlock password.");
					flag2 = true;
				}
				else
				{
					Logger.Info("The user entered a wrong unlock password.");
					Context.MessageBox.Show((TextKey)120, (TextKey)121, (MessageBoxAction)0, (MessageBoxIcon)3, (IWindow)(object)Context.LockScreen);
				}
			}
			else
			{
				Logger.Warn("No unlock password is defined, allowing user to resume session!");
				flag2 = true;
			}
		}
		return val;
	}
}
