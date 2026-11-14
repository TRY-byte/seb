using System;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.Notifications;
using SafeExamBrowser.Core.Contracts.Notifications.Events;
using SafeExamBrowser.Core.Contracts.Resources.Icons;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client.Notifications;

internal class AboutNotification : INotification
{
	private readonly AppConfig appConfig;

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

	public AboutNotification(AppConfig appConfig, IText text, IUserInterfaceFactory uiFactory)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		this.appConfig = appConfig;
		this.uiFactory = uiFactory;
		CanActivate = true;
		IconResource = (IconResource)new XamlIconResource
		{
			Uri = new Uri("pack://application:,,,/SafeExamBrowser.UserInterface.Desktop;component/Images/AboutNotification.xaml")
		};
		Tooltip = text.Get((TextKey)166);
	}

	public void Activate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		if (window == null)
		{
			window = uiFactory.CreateAboutWindow(appConfig);
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
