using System;
using SafeExamBrowser.Core.Contracts.Resources.Icons;

namespace SafeExamBrowser.Browser;

public class BrowserIconResource : BitmapIconResource
{
	public BrowserIconResource(string uri = null)
	{
		((BitmapIconResource)this).Uri = new Uri(uri ?? "pack://application:,,,/SafeExamBrowser.UserInterface.Desktop;component/Images/SafeExamBrowser.ico");
	}
}
