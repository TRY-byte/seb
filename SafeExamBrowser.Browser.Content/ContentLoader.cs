using System.IO;
using System.Reflection;
using SafeExamBrowser.I18n.Contracts;

namespace SafeExamBrowser.Browser.Content;

internal class ContentLoader
{
	private readonly IText text;

	private string api;

	private string clipboard;

	private string pageZoom;

	internal ContentLoader(IText text)
	{
		this.text = text;
	}

	internal string LoadApi(string browserExamKey, string configurationKey, string version)
	{
		if (api == null)
		{
			Assembly assembly = Assembly.GetAssembly(typeof(ContentLoader));
			string name = typeof(ContentLoader).Namespace + ".Api.js";
			using Stream stream = assembly.GetManifestResourceStream(name);
			using StreamReader streamReader = new StreamReader(stream);
			api = streamReader.ReadToEnd();
		}
		return api.Replace("%%_BEK_%%", browserExamKey).Replace("%%_CK_%%", configurationKey).Replace("%%_VERSION_%%", version);
	}

	internal string LoadBlockedContent()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(ContentLoader));
		string name = typeof(ContentLoader).Namespace + ".BlockedContent.html";
		using Stream stream = assembly.GetManifestResourceStream(name);
		using StreamReader streamReader = new StreamReader(stream);
		return streamReader.ReadToEnd().Replace("%%MESSAGE%%", text.Get((TextKey)2));
	}

	internal string LoadBlockedPage()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(ContentLoader));
		string name = typeof(ContentLoader).Namespace + ".BlockedPage.html";
		using Stream stream = assembly.GetManifestResourceStream(name);
		using StreamReader streamReader = new StreamReader(stream);
		return streamReader.ReadToEnd().Replace("%%BACK_BUTTON%%", text.Get((TextKey)3)).Replace("%%MESSAGE%%", text.Get((TextKey)4))
			.Replace("%%TITLE%%", text.Get((TextKey)5));
	}

	internal string LoadClipboard()
	{
		if (clipboard == null)
		{
			Assembly assembly = Assembly.GetAssembly(typeof(ContentLoader));
			string name = typeof(ContentLoader).Namespace + ".Clipboard.js";
			using Stream stream = assembly.GetManifestResourceStream(name);
			using StreamReader streamReader = new StreamReader(stream);
			clipboard = streamReader.ReadToEnd();
		}
		return clipboard;
	}

	internal string LoadPageZoom()
	{
		if (pageZoom == null)
		{
			Assembly assembly = Assembly.GetAssembly(typeof(ContentLoader));
			string name = typeof(ContentLoader).Namespace + ".PageZoom.js";
			using Stream stream = assembly.GetManifestResourceStream(name);
			using StreamReader streamReader = new StreamReader(stream);
			pageZoom = streamReader.ReadToEnd();
		}
		return pageZoom;
	}
}
