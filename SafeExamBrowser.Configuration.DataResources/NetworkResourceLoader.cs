using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.DataResources;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataResources;

public class NetworkResourceLoader : IResourceLoader
{
	private readonly AppConfig appConfig;

	private readonly ILogger logger;

	private string[] SupportedContentTypes => new string[2] { "application/octet-stream", "text/xml" };

	private string[] SupportedSchemes => new string[4]
	{
		appConfig.SebUriScheme,
		appConfig.SebUriSchemeSecure,
		Uri.UriSchemeHttp,
		Uri.UriSchemeHttps
	};

	public NetworkResourceLoader(AppConfig appConfig, ILogger logger)
	{
		this.appConfig = appConfig;
		this.logger = logger;
	}

	public bool CanLoad(Uri resource)
	{
		int num;
		if (SupportedSchemes.Contains(resource.Scheme))
		{
			num = (IsAvailable(resource) ? 1 : 0);
			if (num != 0)
			{
				logger.Debug($"Can load '{resource}' as it references an existing network resource.");
				return (byte)num != 0;
			}
		}
		else
		{
			num = 0;
		}
		logger.Debug($"Can't load '{resource}' since its URI scheme is not supported or the resource is unavailable.");
		return (byte)num != 0;
	}

	public LoadStatus TryLoad(Uri resource, out Stream data)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Uri uri = BuildUriFor(resource);
		logger.Debug($"Sending GET request for '{uri}'...");
		HttpRequestMessage request = Build(HttpMethod.Get, uri);
		HttpResponseMessage httpResponseMessage = Execute(request);
		logger.Debug("Received response '" + ToString(httpResponseMessage) + "'.");
		if (IsUnauthorized(httpResponseMessage) || HasHtmlContent(httpResponseMessage))
		{
			return HandleBrowserResource(httpResponseMessage, out data);
		}
		logger.Debug("Trying to extract response data...");
		data = Extract(httpResponseMessage.Content);
		logger.Debug($"Created '{data}' for {(double)data.Length / 1000.0} KB data of response body.");
		return (LoadStatus)4;
	}

	private HttpRequestMessage Build(HttpMethod method, Uri uri)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uri);
		if (!httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", "SEB/" + appConfig.ProgramInformationalVersion))
		{
			logger.Warn("Failed to add user agent header to request!");
		}
		return httpRequestMessage;
	}

	private Uri BuildUriFor(Uri resource)
	{
		string schemeFor = GetSchemeFor(resource);
		return new UriBuilder(resource)
		{
			Scheme = schemeFor
		}.Uri;
	}

	private HttpResponseMessage Execute(HttpRequestMessage request)
	{
		return Task.Run(async () =>
		{
			using HttpClient client = new HttpClient();
			return await client.SendAsync(request);
		}).GetAwaiter().GetResult();
	}

	private Stream Extract(HttpContent content)
	{
		return Task.Run(async () => await content.ReadAsStreamAsync()).GetAwaiter().GetResult();
	}

	private string GetSchemeFor(Uri resource)
	{
		if (resource.Scheme == appConfig.SebUriScheme)
		{
			return Uri.UriSchemeHttp;
		}
		if (resource.Scheme == appConfig.SebUriSchemeSecure)
		{
			return Uri.UriSchemeHttps;
		}
		return resource.Scheme;
	}

	private LoadStatus HandleBrowserResource(HttpResponseMessage response, out Stream data)
	{
		data = null;
		logger.Debug("The " + (IsUnauthorized(response) ? "resource needs authentication" : "response data is HTML") + ".");
		return (LoadStatus)1;
	}

	private bool HasHtmlContent(HttpResponseMessage response)
	{
		return response.Content.Headers.ContentType?.MediaType == "text/html";
	}

	private bool IsAvailable(Uri resource)
	{
		bool flag = false;
		Uri uri = BuildUriFor(resource);
		try
		{
			logger.Debug($"Sending HEAD request for '{uri}'...");
			HttpRequestMessage request = Build(HttpMethod.Head, uri);
			HttpResponseMessage httpResponseMessage = Execute(request);
			flag = httpResponseMessage.IsSuccessStatusCode || IsUnauthorized(httpResponseMessage);
			logger.Debug("Received response '" + ToString(httpResponseMessage) + "'.");
		}
		catch (Exception ex)
		{
			logger.Error($"Failed to check availability of '{resource}' via HEAD request!", ex);
		}
		if (!flag)
		{
			try
			{
				logger.Debug($"HEAD request was not successful, trying GET request for '{uri}'...");
				HttpRequestMessage request2 = Build(HttpMethod.Get, uri);
				HttpResponseMessage httpResponseMessage2 = Execute(request2);
				flag = httpResponseMessage2.IsSuccessStatusCode || IsUnauthorized(httpResponseMessage2);
				logger.Debug("Received response '" + ToString(httpResponseMessage2) + "'.");
			}
			catch (Exception ex2)
			{
				logger.Error($"Failed to check availability of '{resource}' via GET request!", ex2);
			}
		}
		return flag;
	}

	private bool IsUnauthorized(HttpResponseMessage response)
	{
		return response.StatusCode == HttpStatusCode.Unauthorized;
	}

	private string ToString(HttpResponseMessage response)
	{
		return $"{(int)response.StatusCode} - {response.ReasonPhrase}";
	}
}
