using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Monitoring.Contracts.System;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class SessionIntegrityOperation : SessionOperation
{
	private readonly ISystemSentinel sentinel;

	[CompilerGenerated]
	private new StatusChangedEventHandler m_StatusChanged;

	public override event StatusChangedEventHandler StatusChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public SessionIntegrityOperation(Dependencies dependencies, ISystemSentinel sentinel)
		: base(dependencies)
	{
		this.sentinel = sentinel;
	}

	public override OperationResult Perform()
	{
		bool flag = true;
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)214);
		}
		flag &= InitializeStickyKeys();
		flag &= VerifyCursors();
		flag &= VerifyEaseOfAccess();
		LogResult(flag);
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	public override OperationResult Repeat()
	{
		bool flag = true;
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)214);
		}
		flag &= InitializeStickyKeys();
		flag &= VerifyCursors();
		flag &= VerifyEaseOfAccess();
		LogResult(flag);
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	public override OperationResult Revert()
	{
		FinalizeStickyKeys();
		return (OperationResult)3;
	}

	private void FinalizeStickyKeys()
	{
		sentinel.RevertStickyKeys();
	}

	private bool InitializeStickyKeys()
	{
		bool result = true;
		sentinel.RevertStickyKeys();
		if (!Context.Next.Settings.Security.AllowStickyKeys)
		{
			result = sentinel.DisableStickyKeys();
		}
		return result;
	}

	private void LogResult(bool success)
	{
		if (success)
		{
			Logger.Info("Successfully ensured session integrity.");
		}
		else
		{
			Logger.Error("Failed to ensure session integrity! Aborting session initialization...");
		}
	}

	private bool VerifyCursors()
	{
		bool result = true;
		if (Context.Next.Settings.Security.VerifyCursorConfiguration)
		{
			result = sentinel.VerifyCursors();
		}
		else
		{
			Logger.Debug("Verification of cursor configuration is disabled.");
		}
		return result;
	}

	private bool VerifyEaseOfAccess()
	{
		bool flag = sentinel.VerifyEaseOfAccess();
		if (!flag)
		{
			SessionConfiguration current = Context.Current;
			if (current != null && !current.Settings.Service.IgnoreService)
			{
				Logger.Info("Ease of access configuration is compromised but service was active in the current session.");
				flag = true;
			}
			else if (!Context.Next.Settings.Service.IgnoreService)
			{
				Logger.Info("Ease of access configuration is compromised but service will be active in the next session.");
				flag = true;
			}
		}
		return flag;
	}
}
