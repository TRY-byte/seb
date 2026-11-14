using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Monitoring.Display;

internal class Bounds : IBounds
{
	public int Left { get; set; }

	public int Top { get; set; }

	public int Right { get; set; }

	public int Bottom { get; set; }
}
