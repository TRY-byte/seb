using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SafeExamBrowser.Communication.Contracts.Data;
using SafeExamBrowser.Communication.Contracts.Events;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Server.Contracts.Data;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;

namespace SafeExamBrowser.Runtime.Communication;

internal class ClientBridge
{
	private readonly IRuntimeHost runtimeHost;

	private readonly RuntimeContext runtimeContext;

	internal ClientBridge(IRuntimeHost runtimeHost, RuntimeContext runtimeContext)
	{
		this.runtimeHost = runtimeHost;
		this.runtimeContext = runtimeContext;
	}

	internal bool IsRequired()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		SessionConfiguration current = runtimeContext.Current;
		bool flag = current == null;
		bool flag2 = current != null && (int)current.Settings.Security.KioskMode != 1;
		if (!flag)
		{
			return !flag2;
		}
		return false;
	}

	internal MessageBoxResult ShowMessageBox(string message, string title, MessageBoxAction action, MessageBoxIcon icon)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected I4, but got Unknown
		//IL_005a: Expected I4, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Guid requestId = Guid.NewGuid();
		MessageBoxResult result = (MessageBoxResult)0;
		MessageBoxReplyEventArgs response = null;
		AutoResetEvent responseEvent = new AutoResetEvent(initialState: false);
		CommunicationEventHandler<MessageBoxReplyEventArgs> val = (MessageBoxReplyEventArgs args) =>
		{
			if (args.RequestId == requestId)
			{
				response = args;
				responseEvent.Set();
			}
		};
		runtimeHost.MessageBoxReplyReceived += val;
		if (runtimeContext.ClientProxy.ShowMessage(message, title, (int)action, (int)icon, requestId).Success)
		{
			responseEvent.WaitOne();
			result = (MessageBoxResult)response.Result;
		}
		runtimeHost.MessageBoxReplyReceived -= val;
		return result;
	}

	internal bool TryAskForExamSelection(IEnumerable<Exam> exams, out Exam exam)
	{
		bool result = false;
		Guid requestId = Guid.NewGuid();
		ExamSelectionReplyEventArgs response = null;
		AutoResetEvent responseEvent = new AutoResetEvent(initialState: false);
		CommunicationEventHandler<ExamSelectionReplyEventArgs> val = (ExamSelectionReplyEventArgs a) =>
		{
			if (a.RequestId == requestId)
			{
				response = a;
				responseEvent.Set();
			}
		};
		exam = null;
		runtimeHost.ExamSelectionReceived += val;
		IEnumerable<(string, string, string, string)> enumerable = exams.Select((Exam e) => (Id: e.Id, LmsName: e.LmsName, Name: e.Name, Url: e.Url));
		if (runtimeContext.ClientProxy.RequestExamSelection(enumerable, requestId).Success)
		{
			responseEvent.WaitOne();
			exam = exams.First((Exam e) => e.Id == response.SelectedExamId);
			result = response.Success;
		}
		runtimeHost.ExamSelectionReceived -= val;
		return result;
	}

	internal void TryAskForServerFailureAction(string message, bool showFallback, out bool abort, out bool fallback, out bool retry)
	{
		Guid requestId = Guid.NewGuid();
		ServerFailureActionReplyEventArgs response = null;
		AutoResetEvent responseEvent = new AutoResetEvent(initialState: false);
		CommunicationEventHandler<ServerFailureActionReplyEventArgs> val = (ServerFailureActionReplyEventArgs a) =>
		{
			if (a.RequestId == requestId)
			{
				response = a;
				responseEvent.Set();
			}
		};
		runtimeHost.ServerFailureActionReceived += val;
		if (runtimeContext.ClientProxy.RequestServerFailureAction(message, showFallback, requestId).Success)
		{
			responseEvent.WaitOne();
			abort = response.Abort;
			fallback = response.Fallback;
			retry = response.Retry;
		}
		else
		{
			abort = true;
			fallback = false;
			retry = false;
		}
		runtimeHost.ServerFailureActionReceived -= val;
	}

	internal bool TryGetPassword(PasswordRequestPurpose purpose, out string password)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Guid requestId = Guid.NewGuid();
		PasswordReplyEventArgs response = null;
		AutoResetEvent responseEvent = new AutoResetEvent(initialState: false);
		CommunicationEventHandler<PasswordReplyEventArgs> val = (PasswordReplyEventArgs a) =>
		{
			if (a.RequestId == requestId)
			{
				response = a;
				responseEvent.Set();
			}
		};
		password = null;
		runtimeHost.PasswordReceived += val;
		if (runtimeContext.ClientProxy.RequestPassword(purpose, requestId).Success)
		{
			responseEvent.WaitOne();
			password = response.Password;
			result = response.Success;
		}
		runtimeHost.PasswordReceived -= val;
		return result;
	}
}
