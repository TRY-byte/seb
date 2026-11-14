using System;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class ServiceOperation : SessionOperation
{
	private readonly IRuntimeHost runtimeHost;

	private readonly IServiceProxy serviceProxy;

	private readonly int timeout_ms;

	private readonly IUserInfo userInfo;

	private string serviceEventName;

	private Guid? sessionId;

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

	public ServiceOperation(Dependencies dependencies, IRuntimeHost runtimeHost, IServiceProxy serviceProxy, int timeout_ms, IUserInfo userInfo)
		: base(dependencies)
	{
		this.runtimeHost = runtimeHost;
		this.serviceProxy = serviceProxy;
		this.timeout_ms = timeout_ms;
		this.userInfo = userInfo;
	}

	public override OperationResult Perform()
	{
		Logger.Info("Initializing service...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)189);
		}
		bool flag = IgnoreService() || TryInitializeConnection();
		if (flag && ((ICommunicationProxy)serviceProxy).IsConnected)
		{
			flag = TryStartSession();
		}
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	public override OperationResult Repeat()
	{
		Logger.Info("Initializing service...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)189);
		}
		bool flag = true;
		if (((ICommunicationProxy)serviceProxy).IsConnected)
		{
			if (sessionId.HasValue)
			{
				flag = TryStopSession();
			}
			if (flag && IgnoreService())
			{
				flag = TryTerminateConnection();
			}
		}
		else
		{
			flag = IgnoreService() || TryInitializeConnection();
		}
		if (flag && ((ICommunicationProxy)serviceProxy).IsConnected)
		{
			flag = TryStartSession();
		}
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	public override OperationResult Revert()
	{
		Logger.Info("Finalizing service...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)178);
		}
		bool flag = true;
		if (((ICommunicationProxy)serviceProxy).IsConnected)
		{
			if (sessionId.HasValue)
			{
				flag = TryStopSession(isFinalSession: true);
			}
			flag &= TryTerminateConnection();
		}
		if (flag)
		{
			return (OperationResult)3;
		}
		return (OperationResult)2;
	}

	private bool IgnoreService()
	{
		if (Context.Next.Settings.Service.IgnoreService)
		{
			Logger.Info("The service will be ignored for the next session.");
			return true;
		}
		return false;
	}

	private bool TryInitializeConnection()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (int)Context.Next.Settings.Service.Policy == 0;
		bool flag2 = (int)Context.Next.Settings.Service.Policy == 2;
		bool flag3 = ((ICommunicationProxy)serviceProxy).Connect((Guid?)null, true);
		int num;
		if (!flag3)
		{
			num = ((!flag) ? 1 : 0);
			if (num == 0)
			{
				Logger.Error("The service is mandatory but no connection could be established!");
				ShowMessageBox((TextKey)142, (TextKey)143, (MessageBoxAction)0, (MessageBoxIcon)0);
				goto IL_00da;
			}
		}
		else
		{
			num = 1;
		}
		Logger.Info("The service is " + (flag ? "mandatory" : "optional") + " and " + (flag3 ? "connected." : "not connected."));
		if (!flag3 & flag2)
		{
			ShowMessageBox((TextKey)144, (TextKey)145, (MessageBoxAction)0, (MessageBoxIcon)3);
			return (byte)num != 0;
		}
		goto IL_00da;
		IL_00da:
		return (byte)num != 0;
	}

	private bool TryTerminateConnection()
	{
		bool flag = ((ICommunicationProxy)serviceProxy).Disconnect();
		if (flag)
		{
			Logger.Info("Successfully disconnected from service.");
			return flag;
		}
		Logger.Error("Failed to disconnect from service!");
		return flag;
	}

	private bool TryStartSession()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected Obj, but got Unknown
		ServiceConfiguration val = new ServiceConfiguration
		{
			AppConfig = Context.Next.AppConfig,
			SessionId = Context.Next.SessionId,
			Settings = Context.Next.Settings,
			UserName = userInfo.GetUserName(),
			UserSid = userInfo.GetUserSid()
		};
		bool flag = false;
		Logger.Info("Starting new service session...");
		if (serviceProxy.StartSession(val).Success)
		{
			flag = TryWaitForServiceEvent(Context.Next.AppConfig.ServiceEventName);
			if (flag)
			{
				sessionId = Context.Next.SessionId;
				serviceEventName = Context.Next.AppConfig.ServiceEventName;
				Logger.Info("Successfully started new service session.");
			}
			else
			{
				Logger.Error($"Failed to start new service session within {timeout_ms / 1000} seconds!");
			}
		}
		else
		{
			Logger.Error("Failed to communicate session start command to service!");
		}
		return flag;
	}

	private bool TryStopSession(bool isFinalSession = false)
	{
		bool flag = false;
		Logger.Info("Stopping current service session...");
		if (serviceProxy.StopSession(sessionId.Value).Success)
		{
			flag = TryWaitForServiceEvent(serviceEventName);
			if (flag)
			{
				sessionId = null;
				serviceEventName = null;
				Logger.Info("Successfully stopped service session.");
			}
			else
			{
				Logger.Error($"Failed to stop service session within {timeout_ms / 1000} seconds!");
			}
		}
		else
		{
			Logger.Error("Failed to communicate session stop command to service!");
		}
		if (flag & isFinalSession)
		{
			CommunicationResult val = serviceProxy.RunSystemConfigurationUpdate();
			flag = val.Success;
			if (val.Success)
			{
				Logger.Info("Instructed service to perform system configuration update.");
			}
			else
			{
				Logger.Error("Failed to communicate system configuration update command to service!");
			}
		}
		return flag;
	}

	private bool TryWaitForServiceEvent(string eventName)
	{
		EventWaitHandle result = null;
		DateTime now = DateTime.Now;
		while (!EventWaitHandle.TryOpenExisting(eventName, EventWaitHandleRights.Synchronize, out result) && now.AddMilliseconds(timeout_ms) > DateTime.Now)
		{
		}
		if (result != null)
		{
			using (result)
			{
				return result.WaitOne(timeout_ms);
			}
		}
		return false;
	}
}
