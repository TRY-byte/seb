using CefSharp;

namespace SafeExamBrowser.Browser.Wrapper.Events;

internal class ResourceRequestEventArgs
{
	public IResourceRequestHandler Handler { get; set; }
}
