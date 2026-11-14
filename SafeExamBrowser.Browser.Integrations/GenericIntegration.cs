using System.Linq;
using CefSharp;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Browser.Integrations;

internal class GenericIntegration : Integration
{
	private readonly ILogger logger;

	public GenericIntegration(ILogger logger)
	{
		this.logger = logger;
	}

	internal override bool TrySearchUserIdentifier(Cookie cookie, out string userIdentifier)
	{
		userIdentifier = null;
		return false;
	}

	internal override bool TrySearchUserIdentifier(IRequest request, IResponse response, out string userIdentifier)
	{
		string text = response.Headers.GetValues("X-LMS-USER-ID")?.FirstOrDefault();
		userIdentifier = null;
		if (HasChanged(text))
		{
			userIdentifier = text;
			logger.Info("User identifier '" + text + "' detected by header of response.");
		}
		return userIdentifier != null;
	}
}
