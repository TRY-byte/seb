using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Server.Contracts;
using SafeExamBrowser.Server.Contracts.Data;
using SafeExamBrowser.Settings;
using SafeExamBrowser.Settings.Browser;
using SafeExamBrowser.Settings.Server;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Data;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class ServerOperation : ConfigurationBaseOperation
{
	private readonly IFileSystem fileSystem;

	private readonly IServerProxy server;

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

	public ServerOperation(Dependencies dependencies, IFileSystem fileSystem, IConfigurationRepository repository, IServerProxy server, IUserInterfaceFactory uiFactory)
		: base(dependencies, repository, uiFactory)
	{
		this.fileSystem = fileSystem;
		this.server = server;
	}

	public override OperationResult Perform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Invalid comparison between Unknown and I4
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Invalid comparison between Unknown and I4
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Invalid comparison between Unknown and I4
		OperationResult val = (OperationResult)3;
		if ((int)Context.Next.Settings.SessionMode == 1)
		{
			string value = null;
			Exam exam = null;
			IEnumerable<Exam> value2 = null;
			Uri value3 = null;
			Logger.Info("Initializing server...");
			StatusChangedEventHandler val2 = m_StatusChanged;
			if (val2 != null)
			{
				val2.Invoke((TextKey)188);
			}
			server.Initialize(Context.Next.Settings.Server);
			bool flag;
			bool flag2;
			bool flag3;
			(flag, flag2, flag3) = TryPerformWithFallback(() => server.Connect());
			if (flag3)
			{
				(flag, flag2, flag3) = TryPerformWithFallback(() => server.GetAvailableExams(Context.Next.Settings.Server.ExamId), out value2);
			}
			if (flag3)
			{
				flag3 = TrySelectExam(value2, out exam);
			}
			if (flag3)
			{
				(flag, flag2, flag3) = TryPerformWithFallback(() => server.GetConfigurationFor(exam), out value3);
			}
			if (flag3)
			{
				val = TryLoadServerSettings(exam, value3);
			}
			if (flag3 && (int)val == 3)
			{
				(flag, flag2, flag3) = TryPerformWithFallback(() => server.SendSelectedExam(exam), out value);
			}
			if (value != null)
			{
				Context.Next.Settings.Browser.CustomBrowserExamKey = value;
			}
			if (flag)
			{
				val = (OperationResult)1;
				Logger.Info("The user aborted the server operation.");
			}
			if (flag2)
			{
				Context.Next.Settings.SessionMode = (SessionMode)0;
				val = (OperationResult)3;
				Logger.Info("The user chose to fallback and start a normal session.");
			}
			if ((int)val == 3)
			{
				Logger.Info("Successfully initialized server.");
			}
			else if ((int)val == 2)
			{
				Logger.Error("Failed to initialize server!");
			}
		}
		return val;
	}

	public override OperationResult Repeat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		OperationResult val = (OperationResult)3;
		if ((int)Context.Current.Settings.SessionMode == 1 && (int)Context.Next.Settings.SessionMode == 1)
		{
			val = Revert();
			if ((int)val == 3)
			{
				val = Perform();
			}
			else
			{
				Logger.Error("Cannot start new server session due to failed finalization of current server session! Terminating...");
			}
		}
		else if ((int)Context.Current.Settings.SessionMode == 1)
		{
			val = Revert();
		}
		else if ((int)Context.Next.Settings.SessionMode == 1)
		{
			val = Perform();
		}
		return val;
	}

	public override OperationResult Revert()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		OperationResult result = (OperationResult)3;
		SessionConfiguration current = Context.Current;
		if (current == null || (int)current.Settings.SessionMode != 1)
		{
			SessionConfiguration next = Context.Next;
			if (next == null || (int)next.Settings.SessionMode != 1)
			{
				goto IL_00a2;
			}
		}
		Logger.Info("Finalizing server...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)177);
		}
		if (server.Disconnect().Success)
		{
			result = (OperationResult)3;
			Logger.Info("Successfully finalized server.");
		}
		else
		{
			result = (OperationResult)2;
			Logger.Error("Failed to finalize server!");
		}
		goto IL_00a2;
		IL_00a2:
		return result;
	}

	private OperationResult TryLoadServerSettings(Exam exam, Uri uri)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		ConnectionInfo connectionInfo = server.GetConnectionInfo();
		OperationResult result = (OperationResult)2;
		Nullable val = TryLoadSettings(uri, UriSource.Server, out var _, out var settings);
		fileSystem.Delete(uri.LocalPath);
		if ((int)val.GetValueOrDefault() == 4)
		{
			BrowserSettings browser = Context.Next.Settings.Browser;
			InvigilationSettings invigilation = settings.Server.Invigilation;
			ServerSettings val2 = Context.Next.Settings.Server;
			Context.Next.AppConfig.ServerApi = connectionInfo.Api;
			Context.Next.AppConfig.ServerConnectionToken = connectionInfo.ConnectionToken;
			Context.Next.AppConfig.ServerExamId = exam.Id;
			Context.Next.AppConfig.ServerOauth2Token = connectionInfo.Oauth2Token;
			Context.Next.Settings = settings;
			Context.Next.Settings.Browser.StartUrl = exam.Url;
			Context.Next.Settings.Browser.StartUrlQuery = browser.StartUrlQuery;
			Context.Next.Settings.Server = val2;
			Context.Next.Settings.Server.Invigilation = invigilation;
			Context.Next.Settings.SessionMode = (SessionMode)1;
			result = (OperationResult)3;
		}
		return result;
	}

	private (bool abort, bool fallback, bool success) TryPerformWithFallback(Func<ServerResponse> request)
	{
		bool abort = false;
		bool fallback = false;
		bool flag = false;
		while (!flag)
		{
			ServerResponse val = request();
			flag = val.Success;
			if (!flag && !Retry(val.Message, out abort, out fallback))
			{
				break;
			}
		}
		return (abort: abort, fallback: fallback, success: flag);
	}

	private (bool abort, bool fallback, bool success) TryPerformWithFallback<T>(Func<ServerResponse<T>> request, out T value)
	{
		bool abort = false;
		bool fallback = false;
		bool flag = false;
		value = default;
		while (!flag)
		{
			ServerResponse<T> val = request();
			flag = ((ServerResponse)val).Success;
			value = val.Value;
			if (!flag && !Retry(((ServerResponse)val).Message, out abort, out fallback))
			{
				break;
			}
		}
		return (abort: abort, fallback: fallback, success: flag);
	}

	private bool Retry(string message, out bool abort, out bool fallback)
	{
		AskForServerFailureAction(message, out abort, out fallback, out var retry);
		if (retry)
		{
			Logger.Debug("The user chose to retry the current server request.");
		}
		return retry;
	}

	private void AskForServerFailureAction(string message, out bool abort, out bool fallback, out bool retry)
	{
		bool performFallback = Context.Next.Settings.Server.PerformFallback;
		if (ClientBridge.IsRequired())
		{
			ClientBridge.TryAskForServerFailureAction(message, performFallback, out abort, out fallback, out retry);
			return;
		}
		ServerFailureDialogResult val = uiFactory.CreateServerFailureDialog(message, performFallback).Show((IWindow)(object)RuntimeWindow);
		abort = val.Abort;
		fallback = val.Fallback;
		retry = val.Retry;
	}

	private bool TrySelectExam(IEnumerable<Exam> exams, out Exam exam)
	{
		bool result = true;
		if (string.IsNullOrWhiteSpace(Context.Next.Settings.Server.ExamId))
		{
			result = TryAskForExamSelection(exams, out exam);
		}
		else
		{
			exam = exams.First();
			Logger.Info("Automatically selected exam as defined in configuration.");
		}
		return result;
	}

	private bool TryAskForExamSelection(IEnumerable<Exam> exams, out Exam exam)
	{
		bool flag = false;
		if (ClientBridge.IsRequired())
		{
			return ClientBridge.TryAskForExamSelection(exams, out exam);
		}
		ExamSelectionDialogResult val = uiFactory.CreateExamSelectionDialog(exams).Show((IWindow)(object)RuntimeWindow);
		exam = val.SelectedExam;
		return val.Success;
	}
}
