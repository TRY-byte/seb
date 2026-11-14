using System;
using SafeExamBrowser.Browser.Contracts.Filters;
using SafeExamBrowser.Browser.Filters.Rules;
using SafeExamBrowser.Settings.Browser.Filter;

namespace SafeExamBrowser.Browser.Filters;

internal class RuleFactory : IRuleFactory
{
	public IRule CreateRule(FilterRuleType type)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 0)
		{
			if ((int)type == 1)
			{
				return (IRule)(object)new SimplifiedRule();
			}
			throw new NotImplementedException($"Filter rule of type '{type}' is not yet implemented!");
		}
		return (IRule)(object)new RegexRule();
	}
}
