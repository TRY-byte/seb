using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SafeExamBrowser.Browser.Contracts.Filters;
using SafeExamBrowser.Settings.Browser.Filter;

namespace SafeExamBrowser.Browser.Filters;

internal class RequestFilter : IRequestFilter
{
	private IList<IRule> allowRules;

	private IList<IRule> blockRules;

	public FilterResult Default
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	internal RequestFilter()
	{
		allowRules = new List<IRule>();
		blockRules = new List<IRule>();
		Default = (FilterResult)1;
	}

	public void Load(IRule rule)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		FilterResult result = rule.Result;
		if ((int)result != 0)
		{
			if ((int)result != 1)
			{
				throw new NotImplementedException($"Filter processing for result '{rule.Result}' is not yet implemented!");
			}
			blockRules.Add(rule);
		}
		else
		{
			allowRules.Add(rule);
		}
	}

	public FilterResult Process(Request request)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		foreach (IRule blockRule in blockRules)
		{
			if (blockRule.IsMatch(request))
			{
				return (FilterResult)1;
			}
		}
		foreach (IRule allowRule in allowRules)
		{
			if (allowRule.IsMatch(request))
			{
				return (FilterResult)0;
			}
		}
		return Default;
	}
}
