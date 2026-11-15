using System;
using SafeExamBrowser.Monitoring.Contracts.Applications;

namespace SafeExamBrowser.Monitoring.Applications;

internal class Window : IWindow
{
	internal bool IsMinimized { get; set; }

	internal bool IsOverlay { get; set; }

	internal bool IsVisible { get; set; }

	public IntPtr Handle { get; set; }

	public string Title { get; set; }

	public override string ToString()
	{
		return string.Format("'{0}' ({1}{2})", Title, Handle, IsOverlay ? ", Overlay" : "");
	}
}
