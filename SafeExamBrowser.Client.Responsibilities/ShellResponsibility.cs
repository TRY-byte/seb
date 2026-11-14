using System.ComponentModel;
using System.Linq;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Client.Responsibilities;

internal class ShellResponsibility : ClientResponsibility
{
	private readonly IActionCenter actionCenter;

	private readonly IHashAlgorithm hashAlgorithm;

	private readonly IMessageBox messageBox;

	private readonly ITaskbar taskbar;

	private readonly IUserInterfaceFactory uiFactory;

	public ShellResponsibility(IActionCenter actionCenter, ClientContext context, IHashAlgorithm hashAlgorithm, ILogger logger, IMessageBox messageBox, ITaskbar taskbar, IUserInterfaceFactory uiFactory)
		: base(context, logger)
	{
		this.actionCenter = actionCenter;
		this.hashAlgorithm = hashAlgorithm;
		this.messageBox = messageBox;
		this.taskbar = taskbar;
		this.uiFactory = uiFactory;
	}

	public override void Assume(ClientTask task)
	{
		switch (task)
		{
		case ClientTask.CloseShell:
			CloseShell();
			break;
		case ClientTask.DeregisterEvents:
			DeregisterEvents();
			break;
		case ClientTask.RegisterEvents:
			RegisterEvents();
			break;
		case ClientTask.ShowShell:
			ShowShell();
			break;
		case ClientTask.PrepareShutdown_Wave1:
		case ClientTask.PrepareShutdown_Wave2:
		case ClientTask.ScheduleIntegrityVerification:
			break;
		}
	}

	private void DeregisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected Obj, but got Unknown
		actionCenter.QuitButtonClicked -= Shell_QuitButtonClicked;
		taskbar.LoseFocusRequested -= Taskbar_LoseFocusRequested;
		taskbar.QuitButtonClicked -= Shell_QuitButtonClicked;
		foreach (ITerminationActivator item in Context.Activators.OfType<ITerminationActivator>())
		{
			item.Activated -= TerminationActivator_Activated;
		}
	}

	private void RegisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected Obj, but got Unknown
		actionCenter.QuitButtonClicked += Shell_QuitButtonClicked;
		taskbar.LoseFocusRequested += Taskbar_LoseFocusRequested;
		taskbar.QuitButtonClicked += Shell_QuitButtonClicked;
		foreach (ITerminationActivator item in Context.Activators.OfType<ITerminationActivator>())
		{
			item.Activated += TerminationActivator_Activated;
		}
	}

	private void CloseShell()
	{
		AppSettings settings = Settings;
		if (settings != null && settings.UserInterface.ActionCenter.EnableActionCenter)
		{
			actionCenter.Close();
		}
		AppSettings settings2 = Settings;
		if (settings2 != null && settings2.UserInterface.Taskbar.EnableTaskbar)
		{
			taskbar.Close();
		}
	}

	private void ShowShell()
	{
		if (Settings.UserInterface.ActionCenter.EnableActionCenter)
		{
			actionCenter.Promote();
		}
		if (Settings.UserInterface.Taskbar.EnableTaskbar)
		{
			taskbar.Show();
		}
	}

	private void Shell_QuitButtonClicked(CancelEventArgs args)
	{
		PauseActivators();
		args.Cancel = !TryInitiateShutdown();
		ResumeActivators();
	}

	private void Taskbar_LoseFocusRequested(bool forward)
	{
		Context.Browser.Focus(forward);
	}

	private void TerminationActivator_Activated()
	{
		PauseActivators();
		TryInitiateShutdown();
		ResumeActivators();
	}

	private bool TryInitiateShutdown()
	{
		bool flag = ((!string.IsNullOrEmpty(Settings.Security.QuitPasswordHash)) ? TryValidateQuitPassword() : TryConfirmShutdown());
		bool result = false;
		if (flag)
		{
			result = TryRequestShutdown();
		}
		return result;
	}

	private bool TryConfirmShutdown()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		bool flag = (int)messageBox.Show((TextKey)130, (TextKey)131, (MessageBoxAction)2, (MessageBoxIcon)2, (IWindow)null) == 4;
		if (flag)
		{
			Logger.Info("The user chose to terminate the application.");
		}
		return flag;
	}

	private bool TryValidateQuitPassword()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		PasswordDialogResult val = uiFactory.CreatePasswordDialog((TextKey)228, (TextKey)229).Show((IWindow)null);
		bool result = false;
		if (val.Success && IsValidQuitPassword(val.Password))
		{
			result = true;
			Logger.Info("The user entered the correct quit password, the application will now terminate.");
		}
		else if (val.Success)
		{
			Logger.Info("The user entered the wrong quit password.");
			messageBox.Show((TextKey)118, (TextKey)119, (MessageBoxAction)0, (MessageBoxIcon)3, (IWindow)null);
		}
		return result;
	}
}
