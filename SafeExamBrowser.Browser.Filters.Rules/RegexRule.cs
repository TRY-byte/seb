using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SafeExamBrowser.Browser.Contracts.Filters;
using SafeExamBrowser.Settings.Browser.Filter;

namespace SafeExamBrowser.Browser.Filters.Rules;

internal class RegexRule : IRule
{
	private string expression;

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
		expression = settings.Expression;
		Result = settings.Result;
	}

	public bool IsMatch(Request request)
	{
		return Regex.IsMatch(request.Url, expression, RegexOptions.IgnoreCase);
	}

	private void ValidateExpression(string expression)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		try
		{
			Regex.Match("", expression);
		}
		catch (Exception innerException)
		{
			throw new ArgumentException("Invalid regular expression!", "expression", innerException);
		}
	}
}
