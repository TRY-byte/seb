using System;
using System.Collections.Generic;
using System.Linq;
using CefSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Browser.Integrations;

internal class EdxIntegration : Integration
{
	private readonly ILogger logger;

	public EdxIntegration(ILogger logger)
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
		string userInfo = response.Headers.GetValues("Set-Cookie")?.FirstOrDefault((string c) => c.Contains("edx-user-info"));
		userIdentifier = null;
		if (TryParseCookie(userInfo, out var userIdentifier2) && HasChanged(userIdentifier2))
		{
			userIdentifier = userIdentifier2;
			logger.Info("User identifier '" + userIdentifier2 + "' detected by session cookie on response.");
		}
		return userIdentifier != null;
	}

	private bool TryParseCookie(string userInfo, out string userIdentifier)
	{
		userIdentifier = null;
		try
		{
			if (userInfo != null)
			{
				int num = userInfo.IndexOf("=") + 1;
				int num2 = userInfo.IndexOf("; expires");
				object obj = JsonConvert.DeserializeObject(userInfo.Substring(num, num2 - num).Replace("\\\"", "\"").Replace("\\054", ",")
					.Trim('"'));
				JObject val = (JObject)((obj is JObject) ? obj : null);
				userIdentifier = Extensions.Value<string>((IEnumerable<JToken>)val["username"]);
			}
		}
		catch (Exception ex)
		{
			logger.Error("Failed to parse user identifier!", ex);
		}
		return userIdentifier != null;
	}
}
