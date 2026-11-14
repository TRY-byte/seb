using CefSharp;

namespace SafeExamBrowser.Browser.Integrations;

internal abstract class Integration
{
	private static string activeUserIdentifier;

	internal abstract bool TrySearchUserIdentifier(Cookie cookie, out string userIdentifier);

	internal abstract bool TrySearchUserIdentifier(IRequest request, IResponse response, out string userIdentifier);

	protected bool HasChanged(string userIdentifier)
	{
		string text = activeUserIdentifier;
		if (userIdentifier != null && activeUserIdentifier != userIdentifier)
		{
			activeUserIdentifier = userIdentifier;
		}
		return activeUserIdentifier != text;
	}
}
