namespace SafeExamBrowser.Browser.Events;

internal class JavaScriptDialogRequestedEventArgs
{
	internal bool Success { get; set; }

	internal JavaScriptDialogType Type { get; set; }
}
