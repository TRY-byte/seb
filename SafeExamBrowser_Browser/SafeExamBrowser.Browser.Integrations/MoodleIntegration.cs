using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using CefSharp;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Browser.Integrations;

internal class MoodleIntegration : Integration
{
	private enum RequestType
	{
		Plugin,
		Theme
	}

	private const string PLUGIN_PATH = "/mod/quiz/accessrule/sebserver/classes/external/user.php";

	private const string SESSION_COOKIE_NAME = "MoodleSession";

	private const string THEME_PATH = "/theme/boost_ethz/sebuser.php";

	private readonly ILogger logger;

	public MoodleIntegration(ILogger logger)
	{
		this.logger = logger;
	}

	internal override bool TrySearchUserIdentifier(Cookie cookie, out string userIdentifier)
	{
		return TrySearchByCookie(cookie, out userIdentifier);
	}

	internal override bool TrySearchUserIdentifier(IRequest request, IResponse response, out string userIdentifier)
	{
		bool flag = TrySearchByLocation(response, out userIdentifier);
		if (!flag)
		{
			flag = TrySearchByRequests(request, response, out userIdentifier);
		}
		return flag;
	}

	private bool TrySearchByCookie(Cookie cookie, out string userIdentifier)
	{
		string userId = null;
		RequestType requestType = RequestType.Plugin;
		bool flag = cookie.Name.Contains("MoodleSession");
		string originUrl = (cookie.Secure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp) + Uri.SchemeDelimiter + cookie.Domain + cookie.Path;
		bool num = flag && TryExecuteRequests(originUrl, (name: cookie.Name, value: cookie.Value), out requestType, out userId);
		userIdentifier = null;
		if (num && HasChanged(userId))
		{
			userIdentifier = userId;
			logger.Info($"User identifier '{userId}' detected by request on cookie traversal ({requestType}).");
		}
		return userIdentifier != null;
	}

	private bool TrySearchByLocation(IResponse response, out string userIdentifier)
	{
		string location = response.Headers.GetValues("Location")?.FirstOrDefault((string l) => l.Contains("/login/index.php?testsession"));
		userIdentifier = null;
		if (TryParseLocation(location, out var userId) && HasChanged(userId))
		{
			userIdentifier = userId;
			logger.Info("User identifier '" + userId + "' detected by location header of response.");
		}
		return userIdentifier != null;
	}

	private bool TrySearchByRequests(IRequest request, IResponse response, out string userIdentifier)
	{
		string userId = null;
		RequestType requestType = RequestType.Plugin;
		string session = response.Headers.GetValues("Set-Cookie")?.FirstOrDefault((string c) => c.Contains("MoodleSession"));
		bool num = TryParseCookie(session, out (string, string) cookie) && TryExecuteRequests(request.Url, cookie, out requestType, out userId);
		userIdentifier = null;
		if (num && HasChanged(userId))
		{
			userIdentifier = userId;
			logger.Info($"User identifier '{userId}' detected by request on response ({requestType}).");
		}
		return userIdentifier != null;
	}

	private bool TryExecuteRequests(string originUrl, (string name, string value) session, out RequestType requestType, out string userId)
	{
		RequestType[] array = new RequestType[2]
		{
			RequestType.Plugin,
			RequestType.Theme
		};
		requestType = RequestType.Plugin;
		userId = null;
		RequestType[] array2 = array;
		foreach (RequestType requestType2 in array2)
		{
			try
			{
				string url = BuildUrl(originUrl, requestType2);
				using HttpResponseMessage response = ExecuteRequest(url, session);
				if (TryParseResponse(response, requestType2, out var userId2))
				{
					requestType = requestType2;
					userId = userId2;
					break;
				}
			}
			catch (Exception ex)
			{
				logger.Error($"Failed to execute user identifier request ({requestType2})!", ex);
			}
		}
		return userId != null;
	}

	private string BuildUrl(string originUrl, RequestType type)
	{
		Uri uri = new Uri(originUrl);
		return uri.Scheme + Uri.SchemeDelimiter + uri.Host + ((type == RequestType.Plugin) ? "/mod/quiz/accessrule/sebserver/classes/external/user.php" : "/theme/boost_ethz/sebuser.php");
	}

	private HttpResponseMessage ExecuteRequest(string url, (string name, string value) session)
	{
		using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
		using HttpClientHandler handler = new HttpClientHandler
		{
			UseCookies = false
		};
		using HttpClient httpClient = new HttpClient(handler);
		httpRequestMessage.Headers.Add("Cookie", session.name + "=" + session.value);
		return httpClient.SendAsync(httpRequestMessage).GetAwaiter().GetResult();
	}

	private bool TryParseCookie(string session, out (string name, string value) cookie)
	{
		cookie = default;
		try
		{
			if (session != null)
			{
				int num = session.IndexOf("=") + 1;
				int num2 = session.IndexOf(";");
				cookie.name = session.Substring(0, num - 1);
				cookie.value = session.Substring(num, num2 - num);
			}
		}
		catch (Exception ex)
		{
			logger.Error("Failed to parse session cookie!", ex);
		}
		if (cookie.name != null)
		{
			return cookie.value != null;
		}
		return false;
	}

	private bool TryParseLocation(string location, out string userId)
	{
		userId = null;
		try
		{
			if (location != null)
			{
				userId = location.Substring(location.IndexOf("=") + 1);
			}
		}
		catch (Exception ex)
		{
			logger.Error("Failed to parse location!", ex);
		}
		return userId != null;
	}

	private bool TryParseResponse(HttpResponseMessage response, RequestType type, out string userId)
	{
		userId = null;
		if (response.IsSuccessStatusCode)
		{
			string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			if (int.TryParse(result, out var result2) && result2 > 0)
			{
				userId = result;
			}
		}
		else if (response.StatusCode != HttpStatusCode.NotFound)
		{
			logger.Error($"Failed to retrieve user identifier by request ({type})! Response: {(int)response.StatusCode} {response.ReasonPhrase}");
		}
		return userId != null;
	}
}
