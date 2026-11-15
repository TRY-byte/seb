using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SafeExamBrowser.Browser.Contracts.Filters;
using SafeExamBrowser.Settings.Browser.Filter;

namespace SafeExamBrowser.Browser.Filters.Rules;

internal class SimplifiedRule : IRule
{
	private const string URL_DELIMITER_PATTERN = "(?:([^\\:]*)\\://)?(?:([^\\:\\@]*)(?:\\:([^\\@]*))?\\@)?(?:([^/\\:\\?#]*))?(?:\\:([0-9\\*]*))?([^\\?#]*)?(?:\\?([^#]*))?(?:#(.*))?";

	private Regex fragment;

	private Regex host;

	private Regex path;

	private int? port;

	private Regex query;

	private Regex scheme;

	private Regex userInfo;

	public FilterResult Result
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public void Initialize(FilterRuleSettings settings)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		ValidateExpression(settings.Expression);
		ParseExpression(settings.Expression);
		Result = settings.Result;
	}

	public bool IsMatch(Request request)
	{
		Uri uri = new Uri(request.Url, UriKind.Absolute);
		return (byte)(1u & ((scheme == null || scheme.IsMatch(uri.Scheme)) ? 1u : 0u) & ((userInfo == null || userInfo.IsMatch(uri.UserInfo)) ? 1u : 0u) & (host.IsMatch(uri.Host) ? 1u : 0u) & ((!port.HasValue || port == uri.Port) ? 1u : 0u) & ((path == null || path.IsMatch(uri.AbsolutePath)) ? 1u : 0u) & ((query == null || query.IsMatch(uri.Query)) ? 1u : 0u) & ((fragment == null || fragment.IsMatch(uri.Fragment)) ? 1u : 0u)) != 0;
	}

	private void ParseExpression(string expression)
	{
		Match match = Regex.Match(expression, "(?:([^\\:]*)\\://)?(?:([^\\:\\@]*)(?:\\:([^\\@]*))?\\@)?(?:([^/\\:\\?#]*))?(?:\\:([0-9\\*]*))?([^\\?#]*)?(?:\\?([^#]*))?(?:#(.*))?");
		ParseScheme(match.Groups[1].Value);
		ParseUserInfo(match.Groups[2].Value, match.Groups[3].Value);
		ParseHost(match.Groups[4].Value);
		ParsePort(match.Groups[5].Value);
		ParsePath(match.Groups[6].Value);
		ParseQuery(match.Groups[7].Value);
		ParseFragment(match.Groups[8].Value);
	}

	private void ParseScheme(string expression)
	{
		if (!string.IsNullOrEmpty(expression))
		{
			expression = Regex.Escape(expression);
			expression = ReplaceWildcard(expression);
			scheme = Build(expression);
		}
	}

	private void ParseUserInfo(string username, string password)
	{
		if (!string.IsNullOrEmpty(username))
		{
			string text = null;
			username = Regex.Escape(username);
			password = Regex.Escape(password);
			text = (string.IsNullOrEmpty(password) ? (username + "(:.*)?") : (username + ":" + password));
			text = ReplaceWildcard(text);
			userInfo = Build(text);
		}
	}

	private void ParseHost(string expression)
	{
		bool flag = Regex.IsMatch(expression, "^[a-zA-Z0-9]+$");
		bool flag2 = expression.StartsWith(".");
		expression = (flag2 ? expression.Substring(1) : expression);
		expression = Regex.Escape(expression);
		expression = ReplaceWildcard(expression);
		if (!flag && !flag2)
		{
			expression = "(.+?\\.)*" + expression;
		}
		host = Build(expression);
	}

	private void ParsePort(string expression)
	{
		if (int.TryParse(expression, out var result))
		{
			port = result;
		}
	}

	private void ParsePath(string expression)
	{
		if (!string.IsNullOrWhiteSpace(expression) && !expression.Equals("/"))
		{
			expression = Regex.Escape(expression);
			expression = ReplaceWildcard(expression);
			expression = (expression.EndsWith("/") ? (expression + "?") : (expression + "/?"));
			path = Build(expression);
		}
	}

	private void ParseQuery(string expression)
	{
		if (!string.IsNullOrWhiteSpace(expression))
		{
			if (expression == ".")
			{
				expression = "\\??";
			}
			else
			{
				expression = Regex.Escape(expression);
				expression = ReplaceWildcard(expression);
				expression = "\\??" + expression;
			}
			query = Build(expression);
		}
	}

	private void ParseFragment(string expression)
	{
		if (!string.IsNullOrWhiteSpace(expression))
		{
			expression = Regex.Escape(expression);
			expression = ReplaceWildcard(expression);
			expression = "#?" + expression;
			fragment = Build(expression);
		}
	}

	private Regex Build(string expression)
	{
		return new Regex("^" + expression + "$", RegexOptions.IgnoreCase);
	}

	private string ReplaceWildcard(string expression)
	{
		return expression.Replace("\\*", ".*");
	}

	private void ValidateExpression(string expression)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (!Regex.IsMatch(expression, "[a-zA-Z0-9\\*]+"))
		{
			throw new ArgumentException("Expression must consist of at least one alphanumeric character or asterisk!", "expression");
		}
		try
		{
			Regex.Match(expression, "(?:([^\\:]*)\\://)?(?:([^\\:\\@]*)(?:\\:([^\\@]*))?\\@)?(?:([^/\\:\\?#]*))?(?:\\:([0-9\\*]*))?([^\\?#]*)?(?:\\?([^#]*))?(?:#(.*))?");
		}
		catch (Exception innerException)
		{
			throw new ArgumentException("Expression is not a valid simplified filter expression!", "expression", innerException);
		}
	}
}
