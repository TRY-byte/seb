using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime.Responsibilities;

internal class ErrorMessageResponsibility : RuntimeResponsibility
{
	private readonly AppConfig appConfig;

	private readonly IMessageBox messageBox;

	private readonly ISplashScreen splashScreen;

	private readonly IText text;

	internal ErrorMessageResponsibility(AppConfig appConfig, ILogger logger, IMessageBox messageBox, RuntimeContext runtimeContext, ISplashScreen splashScreen, IText text)
		: base(logger, runtimeContext)
	{
		this.appConfig = appConfig;
		this.messageBox = messageBox;
		this.splashScreen = splashScreen;
		this.text = text;
	}

	public override void Assume(RuntimeTask task)
	{
		switch (task)
		{
		case RuntimeTask.ShowShutdownError:
			ShowShutdownErrorMessage();
			break;
		case RuntimeTask.ShowStartupError:
			ShowStartupErrorMessage();
			break;
		}
	}

	private void ShowShutdownErrorMessage()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		string text = AppendLogFilePaths(appConfig, this.text.Get((TextKey)148));
		string text2 = this.text.Get((TextKey)149);
		messageBox.Show(text, text2, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)splashScreen);
	}

	private void ShowStartupErrorMessage()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		string text = AppendLogFilePaths(appConfig, this.text.Get((TextKey)150));
		string text2 = this.text.Get((TextKey)151);
		messageBox.Show(text, text2, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)splashScreen);
	}
}
