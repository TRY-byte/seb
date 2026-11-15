using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Runtime.Operations.Bootstrap;

internal class ApplicationIntegrityOperation : IOperation
{
	private readonly IIntegrityModule module;

	private readonly ILogger logger;

	[CompilerGenerated]
	private StatusChangedEventHandler m_StatusChanged;

	public event StatusChangedEventHandler StatusChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = this.m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			StatusChangedEventHandler val = this.m_StatusChanged;
			StatusChangedEventHandler val2;
			do
			{
				val2 = val;
				StatusChangedEventHandler value2 = (StatusChangedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_StatusChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public ApplicationIntegrityOperation(IIntegrityModule module, ILogger logger)
	{
		this.module = module;
		this.logger = logger;
	}

	public OperationResult Perform()
	{
		logger.Info("Attempting to verify application and runtime integrity...");
		StatusChangedEventHandler val = this.m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)213);
		}
		VerifyCodeSignature();
		VerifyRuntimeIntegrity();
		return (OperationResult)3;
	}

	public OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private void VerifyCodeSignature()
	{
		bool flag = default;
		if (module.TryVerifyCodeSignature(ref flag))
		{
			if (flag)
			{
				logger.Info("Application integrity successfully verified.");
			}
			else
			{
				logger.Warn("Application integrity is compromised!");
			}
		}
		else
		{
			logger.Warn("Failed to verify application integrity!");
		}
	}

	private bool VerifyRuntimeIntegrity()
	{
		bool flag = default;
		if (module.TryVerifyRuntimeIntegrity(ref flag))
		{
			if (flag)
			{
				logger.Info("Runtime integrity successfully verified.");
			}
			else
			{
				logger.Warn("Runtime integrity is compromised!");
			}
		}
		else
		{
			logger.Warn("Failed to verify runtime integrity!");
		}
		return flag;
	}
}
