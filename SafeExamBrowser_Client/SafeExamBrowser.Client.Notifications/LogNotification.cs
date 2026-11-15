using System;
using SafeExamBrowser.Core.Contracts.Notifications;
using SafeExamBrowser.Core.Contracts.Notifications.Events;
using SafeExamBrowser.Core.Contracts.Resources.Icons;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client.Notifications;

internal class LogNotification : INotification
{
	private readonly ILogger logger;

	private readonly IUserInterfaceFactory uiFactory;

	private IWindow window;

	public bool CanActivate { get; }

	public string Tooltip { get; }

	public IconResource IconResource { get; }

	public event NotificationChangedEventHandler NotificationChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	public LogNotification(ILogger logger, IText text, IUserInterfaceFactory uiFactory)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		this.logger = logger;
		this.uiFactory = uiFactory;
		CanActivate = true;
		IconResource = (IconResource)new BitmapIconResource
		{
			Uri = new Uri("pack://application:,,,/SafeExamBrowser.UserInterface.Desktop;component/Images/LogNotification.ico")
		};
		Tooltip = text.Get((TextKey)167);
	}

	public void Activate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		if (window == null)
		{
			window = uiFactory.CreateLogWindow(logger);
			window.Closed += () =>
			{
				window = null;
			};
			window.Show();
		}
		else
		{
			window.BringToForeground();
		}
	}

	public void Terminate()
	{
		IWindow val = window;
		if (val != null)
		{
			val.Close();
		}
	}
}
