namespace SafeExamBrowser.Monitoring.Display;

internal class Display
{
	public string Identifier { get; set; }

	public bool IsActive { get; set; }

	public bool IsInternal
	{
		get
		{
			if (Technology != VideoOutputTechnology.DisplayPortEmbedded)
			{
				return Technology == VideoOutputTechnology.Internal;
			}
			return true;
		}
	}

	public VideoOutputTechnology Technology { get; set; } = VideoOutputTechnology.Uninitialized;
}
