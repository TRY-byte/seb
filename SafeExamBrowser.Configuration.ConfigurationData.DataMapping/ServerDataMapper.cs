using System.Collections.Generic;
using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal class ServerDataMapper : BaseDataMapper
{
	internal override void Map(string key, object value, AppSettings settings)
	{
		if (key == null)
		{
			return;
		}
		switch (key.Length)
		{
		case 22:
			if (key == "sebServerConfiguration")
			{
				MapConfiguration(settings, value);
			}
			break;
		case 29:
			if (key == "sebServerFallbackPasswordHash")
			{
				MapFallbackPasswordHash(settings, value);
			}
			break;
		case 34:
			if (key == "raiseHandButtonAlwaysPromptMessage")
			{
				MapForceRaiseHandMessage(settings, value);
			}
			break;
		case 17:
			if (key == "sebServerFallback")
			{
				MapPerformFallback(settings, value);
			}
			break;
		case 25:
			if (key == "sebServerFallbackAttempts")
			{
				MapRequestAttempts(settings, value);
			}
			break;
		case 32:
			if (key == "sebServerFallbackAttemptInterval")
			{
				MapRequestAttemptInterval(settings, value);
			}
			break;
		case 24:
			if (key == "sebServerFallbackTimeout")
			{
				MapRequestTimeout(settings, value);
			}
			break;
		case 12:
			if (key == "sebServerURL")
			{
				MapServerUrl(settings, value);
			}
			break;
		case 19:
			if (key == "raiseHandButtonShow")
			{
				MapShowRaiseHand(settings, value);
			}
			break;
		}
	}

	private void MapConfiguration(AppSettings settings, object value)
	{
		if (value is IDictionary<string, object> dictionary)
		{
			if (dictionary.TryGetValue("apiDiscovery", out var value2) && value2 is string apiEndpoint)
			{
				settings.Server.ApiEndpoint = apiEndpoint;
			}
			if (dictionary.TryGetValue("clientName", out value2) && value2 is string clientName)
			{
				settings.Server.ClientName = clientName;
			}
			if (dictionary.TryGetValue("clientSecret", out value2) && value2 is string clientSecret)
			{
				settings.Server.ClientSecret = clientSecret;
			}
			if (dictionary.TryGetValue("exam", out value2) && value2 is string examId)
			{
				settings.Server.ExamId = examId;
			}
			if (dictionary.TryGetValue("institution", out value2) && value2 is string institution)
			{
				settings.Server.Institution = institution;
			}
			if (dictionary.TryGetValue("pingInterval", out value2) && value2 is int pingInterval)
			{
				settings.Server.PingInterval = pingInterval;
			}
		}
	}

	private void MapFallbackPasswordHash(AppSettings settings, object value)
	{
		settings.Server.FallbackPasswordHash = "";
	}

	private void MapPerformFallback(AppSettings settings, object value)
	{
		settings.Server.PerformFallback = true;
	}

	private void MapForceRaiseHandMessage(AppSettings settings, object value)
	{
		if (value is bool forceRaiseHandMessage)
		{
			settings.Server.Invigilation.ForceRaiseHandMessage = forceRaiseHandMessage;
		}
	}

	private void MapRequestAttempts(AppSettings settings, object value)
	{
		if (value is int requestAttempts)
		{
			settings.Server.RequestAttempts = requestAttempts;
		}
	}

	private void MapRequestAttemptInterval(AppSettings settings, object value)
	{
		if (value is int requestAttemptInterval)
		{
			settings.Server.RequestAttemptInterval = requestAttemptInterval;
		}
	}

	private void MapRequestTimeout(AppSettings settings, object value)
	{
		if (value is int requestTimeout)
		{
			settings.Server.RequestTimeout = requestTimeout;
		}
	}

	private void MapServerUrl(AppSettings settings, object value)
	{
		if (value is string serverUrl)
		{
			settings.Server.ServerUrl = serverUrl;
		}
	}

	private void MapShowRaiseHand(AppSettings settings, object value)
	{
		if (value is bool showRaiseHandNotification)
		{
			settings.Server.Invigilation.ShowRaiseHandNotification = showRaiseHandNotification;
		}
	}
}
