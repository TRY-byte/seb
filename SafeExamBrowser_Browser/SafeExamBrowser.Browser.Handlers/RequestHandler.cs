using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using CefSharp;
using CefSharp.Handler;
using SafeExamBrowser.Browser.Contracts.Filters;
using SafeExamBrowser.Browser.Events;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings.Browser;
using SafeExamBrowser.Settings.Browser.Proxy;

namespace SafeExamBrowser.Browser.Handlers;

internal class RequestHandler : RequestHandler
{
	private readonly AppConfig appConfig;

	private readonly IRequestFilter filter;

	private readonly ILogger logger;

	private readonly ResourceHandler resourceHandler;

	private readonly WindowSettings windowSettings;

	private readonly BrowserSettings settings;

	private string quitUrlPattern;

	internal event UrlEventHandler QuitUrlVisited;

	internal event UrlEventHandler RequestBlocked;

	internal RequestHandler(AppConfig appConfig, IRequestFilter filter, ILogger logger, ResourceHandler resourceHandler, BrowserSettings settings, WindowSettings windowSettings)
	{
		this.appConfig = appConfig;
		this.filter = filter;
		this.logger = logger;
		this.resourceHandler = resourceHandler;
		this.settings = settings;
		this.windowSettings = windowSettings;
	}

	protected override bool GetAuthCredentials(IWebBrowser webBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
	{
		if (isProxy)
		{
			foreach (ProxyConfiguration proxy in settings.Proxy.Proxies)
			{
				if (proxy.RequiresAuthentication && host != null && host.Equals(proxy.Host, StringComparison.OrdinalIgnoreCase) && port == proxy.Port)
				{
					callback.Continue(proxy.Username, proxy.Password);
					return true;
				}
			}
		}
		return ((RequestHandler)this).GetAuthCredentials(webBrowser, browser, originUrl, isProxy, host, port, realm, scheme, callback);
	}

	protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
	{
		return (IResourceRequestHandler)(object)resourceHandler;
	}

	protected override bool OnBeforeBrowse(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (IsQuitUrl(request))
		{
			QuitUrlVisited?.Invoke(request.Url);
			return true;
		}
		if (Block(request))
		{
			if ((int)request.ResourceType == 0)
			{
				RequestBlocked?.Invoke(request.Url);
			}
			return true;
		}
		if (IsConfigurationFile(request, out var downloadUrl))
		{
			browser.GetHost().StartDownload(downloadUrl);
			return true;
		}
		return ((RequestHandler)this).OnBeforeBrowse(webBrowser, browser, frame, request, userGesture, isRedirect);
	}

	protected override bool OnOpenUrlFromTab(IWebBrowser webBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (targetDisposition - 4 <= 3)
		{
			return true;
		}
		return ((RequestHandler)this).OnOpenUrlFromTab(webBrowser, browser, frame, targetUrl, targetDisposition, userGesture);
	}

	private bool IsConfigurationFile(IRequest request, out string downloadUrl)
	{
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Uri.TryCreate(request.Url, UriKind.RelativeOrAbsolute, out var result);
		bool flag2 = string.Equals(appConfig.ConfigurationFileExtension, Path.GetExtension(result.AbsolutePath), StringComparison.OrdinalIgnoreCase);
		bool flag3 = request.Url.Contains(appConfig.ConfigurationFileMimeType);
		bool num = flag && (flag2 | flag3);
		downloadUrl = request.Url;
		if (num)
		{
			if (flag3)
			{
				if (result.Scheme == appConfig.SebUriScheme)
				{
					downloadUrl = request.Url.Replace(appConfig.SebUriScheme + Uri.SchemeDelimiter, "data:");
				}
				else if (result.Scheme == appConfig.SebUriSchemeSecure)
				{
					downloadUrl = request.Url.Replace(appConfig.SebUriSchemeSecure + Uri.SchemeDelimiter, "data:");
				}
			}
			else if (result.Scheme == appConfig.SebUriScheme)
			{
				downloadUrl = new UriBuilder(result)
				{
					Scheme = Uri.UriSchemeHttp
				}.Uri.AbsoluteUri;
			}
			else if (result.Scheme == appConfig.SebUriSchemeSecure)
			{
				downloadUrl = new UriBuilder(result)
				{
					Scheme = Uri.UriSchemeHttps
				}.Uri.AbsoluteUri;
			}
			logger.Debug("Detected configuration file " + (UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? $"'{result}'" : "") + ".");
		}
		return num;
	}

	private bool IsQuitUrl(IRequest request)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (!string.IsNullOrWhiteSpace(settings.QuitUrl))
		{
			if (quitUrlPattern == null)
			{
				quitUrlPattern = "^" + Regex.Escape(settings.QuitUrl.TrimEnd('/')) + "/?$";
			}
			flag = Regex.IsMatch(request.Url, quitUrlPattern, RegexOptions.IgnoreCase);
			if (flag)
			{
				logger.Debug("Detected quit URL" + (UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? ("'" + request.Url + "'") : "") + ".");
			}
		}
		return flag;
	}

	private bool Block(IRequest request)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected Obj, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Invalid comparison between Unknown and I4
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		string text = WebUtility.UrlDecode(request.Url);
		bool flag = Uri.TryCreate(text, UriKind.Absolute, out var _);
		if (((settings.Filter.ProcessMainRequests && (int)request.ResourceType == 0) & flag) && (int)filter.Process(new Request
		{
			Url = text
		}) == 1 && !text.StartsWith("chrome-extension://"))
		{
			result = true;
			logger.Info(string.Format("Blocked main request{0} ({1}, {2}).", UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" for '" + text + "'") : "", request.ResourceType, request.TransitionType));
		}
		if (((settings.Filter.ProcessContentRequests && (int)request.ResourceType > 0) & flag) && (int)filter.Process(new Request
		{
			Url = text
		}) == 1)
		{
			result = true;
			logger.Info(string.Format("Blocked content request{0} ({1}, {2}).", UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" for '" + text + "'") : "", request.ResourceType, request.TransitionType));
		}
		if (!flag)
		{
			logger.Warn(string.Format("Filter could not process request{0} ({1}, {2})!", UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" for '" + text + "'") : "", request.ResourceType, request.TransitionType));
		}
		return result;
	}
}
