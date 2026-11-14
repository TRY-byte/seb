using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using SafeExamBrowser.Browser.Contracts.Events;
using SafeExamBrowser.Browser.Integrations;

namespace SafeExamBrowser.Browser.Handlers;

internal class CookieVisitor : ICookieVisitor, IDisposable
{
	private readonly IEnumerable<Integration> integrations;

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

	internal CookieVisitor(IEnumerable<Integration> integrations)
	{
		this.integrations = integrations;
	}

	public void Dispose()
	{
	}

	public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
	{
		foreach (Integration integration in integrations)
		{
			if (!integration.TrySearchUserIdentifier(cookie, out var userIdentifier))
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
		return true;
	}
}
