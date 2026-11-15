using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Handler;
using SafeExamBrowser.Browser.Content;
using SafeExamBrowser.Browser.Contracts.Events;
using SafeExamBrowser.Browser.Contracts.Filters;
using SafeExamBrowser.Browser.Integrations;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Browser;

namespace SafeExamBrowser.Browser.Handlers;

internal class ResourceHandler : ResourceRequestHandler
{
	private readonly AppConfig appConfig;

	private readonly ContentLoader contentLoader;

	private readonly IRequestFilter filter;

	private readonly IEnumerable<Integration> integrations;

	private readonly IKeyGenerator keyGenerator;

	private readonly ILogger logger;

	private readonly SessionMode sessionMode;

	private readonly BrowserSettings settings;

	private readonly WindowSettings windowSettings;

	private IResourceHandler contentHandler;

	private IResourceHandler pageHandler;

	[CompilerGenerated]
	private UserIdentifierDetectedEventHandler m_UserIdentifierDetected;

	internal event UserIdentifierDetectedEventHandler UserIdentifierDetected
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			UserIdentifierDetectedEventHandler val = this.m_UserIdentifierDetected;
			UserIdentifierDetectedEventHandler val2;
			do
			{
				val2 = val;
				UserIdentifierDetectedEventHandler value2 = (UserIdentifierDetectedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_UserIdentifierDetected, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			UserIdentifierDetectedEventHandler val = this.m_UserIdentifierDetected;
			UserIdentifierDetectedEventHandler val2;
			do
			{
				val2 = val;
				UserIdentifierDetectedEventHandler value2 = (UserIdentifierDetectedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_UserIdentifierDetected, value2, val2);
			}
			while (val != val2);
		}
	}

	internal ResourceHandler(AppConfig appConfig, IRequestFilter filter, IEnumerable<Integration> integrations, IKeyGenerator keyGenerator, ILogger logger, SessionMode sessionMode, BrowserSettings settings, WindowSettings windowSettings, IText text)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		this.appConfig = appConfig;
		this.filter = filter;
		this.integrations = integrations;
		contentLoader = new ContentLoader(text);
		this.keyGenerator = keyGenerator;
		this.logger = logger;
		this.sessionMode = sessionMode;
		this.settings = settings;
		this.windowSettings = windowSettings;
	}

	protected override IResourceHandler GetResourceHandler(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (Block(request))
		{
			return ResourceHandlerFor(request.ResourceType);
		}
		return ((ResourceRequestHandler)this).GetResourceHandler(webBrowser, browser, frame, request);
	}

	protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (IsMailtoUrl(request.Url))
		{
			return (CefReturnValue)0;
		}
		AppendCustomHeaders(webBrowser, request);
		ReplaceSebScheme(request);
		return ((ResourceRequestHandler)this).OnBeforeResourceLoad(webBrowser, browser, frame, request, callback);
	}

	protected override bool OnProtocolExecution(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request)
	{
		return true;
	}

	protected override void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)sessionMode == 1)
		{
			SearchUserIdentifier(request, response);
		}
		((ResourceRequestHandler)this).OnResourceRedirect(chromiumWebBrowser, browser, frame, request, response, ref newUrl);
	}

	protected override bool OnResourceResponse(IWebBrowser webBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		if (RedirectToDisablePdfReaderToolbar(request, response, out var url))
		{
			if (frame != null)
			{
				frame.LoadUrl(url);
			}
			return true;
		}
		if ((int)sessionMode == 1)
		{
			SearchUserIdentifier(request, response);
		}
		return ((ResourceRequestHandler)this).OnResourceResponse(webBrowser, browser, frame, request, response);
	}

	private void AppendCustomHeaders(IWebBrowser webBrowser, IRequest request)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Uri.TryCreate(((IChromiumWebBrowserBase)webBrowser).Address, UriKind.Absolute, out var result);
		Uri.TryCreate(request.Url, UriKind.Absolute, out var result2);
		if ((int)request.ResourceType == 0 || ((object)result != null && result.Host?.Equals(result2?.Host) == true))
		{
			NameValueCollection nameValueCollection = new NameValueCollection(request.Headers);
			if (settings.SendConfigurationKey)
			{
				nameValueCollection["X-SafeExamBrowser-ConfigKeyHash"] = keyGenerator.CalculateConfigurationKeyHash(settings.ConfigurationKey, request.Url);
			}
			if (settings.SendBrowserExamKey)
			{
				nameValueCollection["X-SafeExamBrowser-RequestHash"] = keyGenerator.CalculateBrowserExamKeyHash(settings.ConfigurationKey, settings.BrowserExamKeySalt, request.Url);
			}
			request.Headers = nameValueCollection;
		}
	}

	private bool Block(IRequest request)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		string text = WebUtility.UrlDecode(request.Url);
		bool flag = Uri.TryCreate(text, UriKind.Absolute, out var _);
		if (settings.Filter.ProcessContentRequests & flag)
		{
			if ((int)filter.Process(new Request
			{
				Url = text
			}) == 1)
			{
				result = true;
				logger.Info(string.Format("Blocked content request{0} ({1}, {2}).", UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" for '" + text + "'") : "", request.ResourceType, request.TransitionType));
			}
		}
		else if (!flag)
		{
			logger.Warn(string.Format("Filter could not process request{0} ({1}, {2})!", UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" for '" + text + "'") : "", request.ResourceType, request.TransitionType));
		}
		return result;
	}

	private bool IsMailtoUrl(string url)
	{
		return url.StartsWith(Uri.UriSchemeMailto);
	}

	private bool RedirectToDisablePdfReaderToolbar(IRequest request, IResponse response, out string url)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		bool flag = response.Headers["Content-Type"] == "application/pdf";
		bool flag2 = (int)request.ResourceType == 0;
		bool flag3 = request.Url.Contains("#toolbar=0");
		bool num = ((settings.AllowPdfReader && !settings.AllowPdfReaderToolbar) & flag & flag2) && !flag3;
		url = request.Url + "#toolbar=0";
		if (num)
		{
			logger.Info("Redirecting" + (UrlPolicyExtensions.CanLog(windowSettings.UrlPolicy) ? (" to '" + url + "'") : "") + " to disable PDF reader toolbar.");
		}
		return num;
	}

	private void ReplaceSebScheme(IRequest request)
	{
		if (Uri.IsWellFormedUriString(request.Url, UriKind.RelativeOrAbsolute))
		{
			Uri uri = new Uri(request.Url);
			if (uri.Scheme == appConfig.SebUriScheme)
			{
				request.Url = new UriBuilder(uri)
				{
					Scheme = Uri.UriSchemeHttp
				}.Uri.AbsoluteUri;
			}
			else if (uri.Scheme == appConfig.SebUriSchemeSecure)
			{
				request.Url = new UriBuilder(uri)
				{
					Scheme = Uri.UriSchemeHttps
				}.Uri.AbsoluteUri;
			}
		}
	}

	private IResourceHandler ResourceHandlerFor(ResourceType resourceType)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		if (contentHandler == null)
		{
			contentHandler = ResourceHandler.FromString(contentLoader.LoadBlockedContent(), (Encoding)null, true, "text/html");
		}
		if (pageHandler == null)
		{
			pageHandler = ResourceHandler.FromString(contentLoader.LoadBlockedPage(), (Encoding)null, true, "text/html");
		}
		if ((int)resourceType <= 1)
		{
			return pageHandler;
		}
		return contentHandler;
	}

	private void SearchUserIdentifier(IRequest request, IResponse response)
	{
		foreach (Integration integration in integrations)
		{
			if (!integration.TrySearchUserIdentifier(request, response, out var userIdentifier))
			{
				continue;
			}
			Task.Run(() =>
			{
				UserIdentifierDetectedEventHandler val = this.m_UserIdentifierDetected;
				if (val != null)
				{
					val.Invoke(userIdentifier);
				}
			});
			break;
		}
	}
}
