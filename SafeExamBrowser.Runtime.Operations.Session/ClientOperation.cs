using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Data;
using SafeExamBrowser.Communication.Contracts.Events;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Contracts.Events;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class ClientOperation : SessionOperation
{
	private readonly IProcessFactory processFactory;

	private readonly IProxyFactory proxyFactory;

	private readonly IRuntimeHost runtimeHost;

	private readonly int timeout_ms;

	[CompilerGenerated]
	private new StatusChangedEventHandler m_StatusChanged;

	private IProcess ClientProcess
	{
		get
		{
			return Context.ClientProcess;
		}
		set
		{
			Context.ClientProcess = value;
		}
	}

	private IClientProxy ClientProxy
	{
		get
		{
			return Context.ClientProxy;
		}
		set
		{
			Context.ClientProxy = value;
		}
	}

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

	public ClientOperation(Dependencies dependencies, IProcessFactory processFactory, IProxyFactory proxyFactory, IRuntimeHost runtimeHost, int timeout_ms)
		: base(dependencies)
	{
		this.processFactory = processFactory;
		this.proxyFactory = proxyFactory;
		this.runtimeHost = runtimeHost;
		this.timeout_ms = timeout_ms;
	}

	public override OperationResult Perform()
	{
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)198);
		}
		bool flag = TryStartClient();
		if (flag)
		{
			Logger.Info("Successfully started new client instance.");
		}
		else
		{
			Logger.Error("Failed to start new client instance! Aborting procedure...");
		}
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	public override OperationResult Repeat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return Perform();
	}

	public override OperationResult Revert()
	{
		bool flag = true;
		if (ClientProcess != null && !ClientProcess.HasTerminated)
		{
			StatusChangedEventHandler val = m_StatusChanged;
			if (val != null)
			{
				val.Invoke((TextKey)202);
			}
			flag = TryStopClient();
		}
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	private bool TryStartClient()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected Obj, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected Obj, but got Unknown
		bool result = false;
		string text = Context.Next.ClientAuthenticationToken.ToString("D");
		string clientExecutablePath = Context.Next.AppConfig.ClientExecutablePath;
		string text2 = "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(Context.Next.AppConfig.ClientLogFilePath)) + "\"";
		string text3 = ((object)Context.Next.Settings.LogLevel/*cast due to constrained. prefix*/).ToString();
		string runtimeAddress = Context.Next.AppConfig.RuntimeAddress;
		string text4 = ((object)Context.Next.Settings.UserInterface.Mode/*cast due to constrained. prefix*/).ToString();
		AutoResetEvent clientReadyEvent = new AutoResetEvent(initialState: false);
		CommunicationEventHandler val = () =>
		{
			clientReadyEvent.Set();
		};
		bool clientTerminated = false;
		ProcessTerminatedEventHandler val2 = (int _) =>
		{
			clientTerminated = true;
			clientReadyEvent.Set();
		};
		Logger.Info("Starting new client process...");
		runtimeHost.AllowConnection = true;
		runtimeHost.AuthenticationToken = Context.Next.ClientAuthenticationToken;
		runtimeHost.ClientReady += val;
		ClientProcess = processFactory.StartNew(clientExecutablePath, new string[5] { text2, text3, runtimeAddress, text, text4 });
		ClientProcess.Terminated += val2;
		Logger.Info("Waiting for client to complete initialization...");
		bool flag = clientReadyEvent.WaitOne();
		runtimeHost.AllowConnection = false;
		runtimeHost.AuthenticationToken = null;
		runtimeHost.ClientReady -= val;
		ClientProcess.Terminated -= val2;
		if (flag && !clientTerminated)
		{
			result = TryStartCommunication();
		}
		else
		{
			Logger.Error("Client instance terminated unexpectedly during initialization!");
		}
		return result;
	}

	private bool TryStartCommunication()
	{
		bool flag = false;
		Logger.Info("Client has been successfully started and initialized. Creating communication proxy for client host...");
		ClientProxy = proxyFactory.CreateClientProxy(Context.Next.AppConfig.ClientAddress, (Interlocutor)2);
		if (((ICommunicationProxy)ClientProxy).Connect((Guid?)Context.Next.ClientAuthenticationToken, true))
		{
			Logger.Info("Connection with client has been established. Requesting authentication...");
			CommunicationResult<AuthenticationResponse> val = ClientProxy.RequestAuthentication();
			AuthenticationResponse value = val.Value;
			flag = ((CommunicationResult)val).Success && ClientProcess.Id == ((value != null) ? new int?(value.ProcessId) : ((int?)null));
			if (flag)
			{
				Logger.Info("Authentication of client has been successful, client is ready to operate.");
			}
			else
			{
				Logger.Error("Failed to verify client integrity!");
			}
		}
		else
		{
			Logger.Error("Failed to connect to client!");
		}
		return flag;
	}

	private bool TryStopClient()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		bool flag = false;
		bool flag2 = false;
		AutoResetEvent disconnectedEvent = new AutoResetEvent(initialState: false);
		CommunicationEventHandler val = () =>
		{
			disconnectedEvent.Set();
		};
		bool flag3 = false;
		AutoResetEvent terminatedEvent = new AutoResetEvent(initialState: false);
		ProcessTerminatedEventHandler val2 = (int _) =>
		{
			terminatedEvent.Set();
		};
		if (ClientProxy != null)
		{
			runtimeHost.ClientDisconnected += val;
			ClientProcess.Terminated += val2;
			Logger.Info("Instructing client to initiate shutdown procedure.");
			ClientProxy.InitiateShutdown();
			Logger.Info("Disconnecting from client communication host.");
			((ICommunicationProxy)ClientProxy).Disconnect();
			Logger.Info("Waiting for client to disconnect from runtime communication host...");
			flag2 = disconnectedEvent.WaitOne(timeout_ms / 2);
			if (!flag2)
			{
				Logger.Error($"Client failed to disconnect within {timeout_ms / 2 / 1000} seconds!");
			}
			Logger.Info("Waiting for client process to terminate...");
			flag3 = terminatedEvent.WaitOne(timeout_ms / 2);
			if (!flag3)
			{
				Logger.Error($"Client failed to terminate within {timeout_ms / 2 / 1000} seconds!");
			}
			runtimeHost.ClientDisconnected -= val;
			ClientProcess.Terminated -= val2;
		}
		if (flag2 & flag3)
		{
			Logger.Info("Client has been successfully terminated.");
			flag = true;
		}
		else
		{
			Logger.Warn("Attempting to kill client process since graceful termination failed!");
			flag = TryKillClient();
		}
		if (flag)
		{
			ClientProcess = null;
			ClientProxy = null;
		}
		return flag;
	}

	private bool TryKillClient()
	{
		for (int i = 1; i <= 5; i++)
		{
			Logger.Info($"Attempt {i}/{5} to kill client process with ID = {ClientProcess.Id}.");
			if (ClientProcess.TryKill(500))
			{
				break;
			}
		}
		if (ClientProcess.HasTerminated)
		{
			Logger.Info("Client process has terminated.");
		}
		else
		{
			Logger.Error($"Failed to kill client process within {5} attempts!");
		}
		return ClientProcess.HasTerminated;
	}
}
