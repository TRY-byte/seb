using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class SessionActivationOperation : SessionOperation
{
	public override event StatusChangedEventHandler StatusChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	public SessionActivationOperation(Dependencies dependencies)
		: base(dependencies)
	{
	}

	public override OperationResult Perform()
	{
		SwitchLogSeverity();
		ActivateNewSession();
		return (OperationResult)3;
	}

	public override OperationResult Repeat()
	{
		SwitchLogSeverity();
		ActivateNewSession();
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}

	private void SwitchLogSeverity()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (Logger.LogLevel != Context.Next.Settings.LogLevel)
		{
			string text = ((object)Logger.LogLevel/*cast due to constrained. prefix*/).ToString().ToUpper();
			string text2 = ((object)Context.Next.Settings.LogLevel/*cast due to constrained. prefix*/).ToString().ToUpper();
			Logger.Info("Switching from log severity '" + text + "' to '" + text2 + "' for new session.");
			Logger.LogLevel = Context.Next.Settings.LogLevel;
		}
	}

	private void ActivateNewSession()
	{
		if (Context.Current == null)
		{
			Logger.Info($"Successfully activated first session '{Context.Next.SessionId}'.");
		}
		else
		{
			Logger.Info($"Successfully terminated old session '{Context.Current.SessionId}' and activated new session '{Context.Next.SessionId}'.");
		}
		Context.Current = Context.Next;
		Context.Next = null;
	}
}
