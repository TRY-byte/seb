using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.SystemComponents.Contracts;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class SessionInitializationOperation : SessionOperation
{
	private readonly IConfigurationRepository repository;

	private readonly IFileSystem fileSystem;

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

	public SessionInitializationOperation(Dependencies dependencies, IFileSystem fileSystem, IConfigurationRepository repository)
		: base(dependencies)
	{
		this.fileSystem = fileSystem;
		this.repository = repository;
	}

	public override OperationResult Perform()
	{
		InitializeSessionConfiguration();
		return (OperationResult)3;
	}

	public override OperationResult Repeat()
	{
		InitializeSessionConfiguration();
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		FinalizeSessionConfiguration();
		return (OperationResult)3;
	}

	private void InitializeSessionConfiguration()
	{
		Logger.Info("Initializing new session configuration...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)190);
		}
		Context.Next = repository.InitializeSessionConfiguration();
		Logger.Info($" -> Client-ID: {Context.Next.AppConfig.ClientId}");
		Logger.Info($" -> Runtime-ID: {Context.Next.AppConfig.RuntimeId}");
		Logger.Info($" -> Session-ID: {Context.Next.SessionId}");
		fileSystem.CreateDirectory(Context.Next.AppConfig.TemporaryDirectory);
	}

	private void FinalizeSessionConfiguration()
	{
		Context.Current = null;
		Context.Next = null;
	}
}
