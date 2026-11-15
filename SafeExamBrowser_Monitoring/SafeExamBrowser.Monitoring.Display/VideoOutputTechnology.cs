namespace SafeExamBrowser.Monitoring.Display;

internal enum VideoOutputTechnology : long
{
	Uninitialized = -2L,
	Other = -1L,
	HD15 = 0L,
	SVideo = 1L,
	CompositeVideo = 2L,
	ComponentVideo = 3L,
	DVI = 4L,
	HDMI = 5L,
	LVDS = 6L,
	DJPN = 8L,
	SDI = 9L,
	DisplayPortExternal = 10L,
	DisplayPortEmbedded = 11L,
	UDIExternal = 12L,
	UDIEmbedded = 13L,
	SDTVDongle = 14L,
	MiraCast = 15L,
	IndirectWired = 16L,
	Internal = 2147483648L,
	SVideo4Pin = SVideo,
	SVideo7Pin = SVideo,
	RF = ComponentVideo,
	RCA3Component = ComponentVideo,
	BNC = ComponentVideo
}
