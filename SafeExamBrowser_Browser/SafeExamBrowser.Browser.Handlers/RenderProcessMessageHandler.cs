using CefSharp;
using SafeExamBrowser.Browser.Content;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Settings.Browser;

namespace SafeExamBrowser.Browser.Handlers;

internal class RenderProcessMessageHandler : IRenderProcessMessageHandler
{
	private readonly AppConfig appConfig;

	private readonly Clipboard clipboard;

	private readonly ContentLoader contentLoader;

	private readonly IKeyGenerator keyGenerator;

	private readonly BrowserSettings settings;

	private readonly IText text;

	internal RenderProcessMessageHandler(AppConfig appConfig, Clipboard clipboard, IKeyGenerator keyGenerator, BrowserSettings settings, IText text)
	{
		this.appConfig = appConfig;
		this.clipboard = clipboard;
		contentLoader = new ContentLoader(text);
		this.keyGenerator = keyGenerator;
		this.settings = settings;
		this.text = text;
	}

	public void OnContextCreated(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
	{
		string browserExamKey = keyGenerator.CalculateBrowserExamKeyHash(settings.ConfigurationKey, settings.BrowserExamKeySalt, frame.Url);
		string configurationKey = keyGenerator.CalculateConfigurationKeyHash(settings.ConfigurationKey, frame.Url);
		string text = contentLoader.LoadApi(browserExamKey, configurationKey, appConfig.ProgramBuildVersion);
		string text2 = contentLoader.LoadClipboard();
		string text3 = contentLoader.LoadPageZoom();
		frame.ExecuteJavaScriptAsync(text, "about:blank", 1);
		if (!settings.AllowPageZoom)
		{
			frame.ExecuteJavaScriptAsync(text3, "about:blank", 1);
		}
		if (!settings.AllowPrint)
		{
			frame.ExecuteJavaScriptAsync("window.print = function() { alert('" + this.text.Get((TextKey)9) + "') }", "about:blank", 1);
		}
		if (settings.UseIsolatedClipboard)
		{
			frame.ExecuteJavaScriptAsync(text2, "about:blank", 1);
			if (clipboard.Content != null)
			{
				frame.ExecuteJavaScriptAsync("SafeExamBrowser.clipboard.update('', '" + clipboard.Content + "');", "about:blank", 1);
			}
		}
	}

	public void OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
	{
	}

	public void OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node)
	{
	}

	public void OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, JavascriptException exception)
	{
	}
}
