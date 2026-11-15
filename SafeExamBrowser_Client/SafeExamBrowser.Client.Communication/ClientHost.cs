using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Data;
using SafeExamBrowser.Communication.Contracts.Events;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Hosts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Client.Communication;

internal class ClientHost : BaseHost, IClientHost, ICommunicationHost
{
	private bool allowConnection;

	private int processId;

	[CompilerGenerated]
	private CommunicationEventHandler m_ReconfigurationAborted;

	[CompilerGenerated]
	private CommunicationEventHandler m_RuntimeDisconnected;

	[CompilerGenerated]
	private CommunicationEventHandler m_Shutdown;

	public Guid AuthenticationToken { private get; set; }

	public bool IsConnected { get; private set; }

	public event CommunicationEventHandler<ExamSelectionRequestEventArgs> ExamSelectionRequested;

	public event CommunicationEventHandler<MessageBoxRequestEventArgs> MessageBoxRequested;

	public event CommunicationEventHandler<PasswordRequestEventArgs> PasswordRequested;

	public event CommunicationEventHandler ReconfigurationAborted
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ReconfigurationAborted;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ReconfigurationAborted, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ReconfigurationAborted;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ReconfigurationAborted, value2, val2);
			}
			while (val != val2);
		}
	}

	public event CommunicationEventHandler<ReconfigurationEventArgs> ReconfigurationDenied;

	public event CommunicationEventHandler RuntimeDisconnected
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_RuntimeDisconnected;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_RuntimeDisconnected, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_RuntimeDisconnected;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_RuntimeDisconnected, value2, val2);
			}
			while (val != val2);
		}
	}

	public event CommunicationEventHandler<ServerFailureActionRequestEventArgs> ServerFailureActionRequested;

	public event CommunicationEventHandler Shutdown
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_Shutdown;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_Shutdown, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_Shutdown;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_Shutdown, value2, val2);
			}
			while (val != val2);
		}
	}

	public ClientHost(string address, IHostObjectFactory factory, ILogger logger, int processId, int timeout_ms)
		: base(address, factory, logger, timeout_ms)
	{
		allowConnection = true;
		this.processId = processId;
	}

	protected override bool OnConnect(Guid? token)
	{
		bool flag = AuthenticationToken == token;
		bool num = allowConnection & flag;
		if (num)
		{
			allowConnection = false;
			IsConnected = true;
		}
		return num;
	}

	protected override void OnDisconnect(Interlocutor interlocutor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)interlocutor == 2)
		{
			CommunicationEventHandler val = this.m_RuntimeDisconnected;
			if (val != null)
			{
				val.Invoke();
			}
			IsConnected = false;
		}
	}

	protected override Response OnReceive(Message message)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected Obj, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected Obj, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected Obj, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected Obj, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected Obj, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected Obj, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected Obj, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected Obj, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected Obj, but got Unknown
		ExamSelectionRequestMessage val = (ExamSelectionRequestMessage)(object)((message is ExamSelectionRequestMessage) ? message : null);
		if (val == null)
		{
			MessageBoxRequestMessage val2 = (MessageBoxRequestMessage)(object)((message is MessageBoxRequestMessage) ? message : null);
			if (val2 == null)
			{
				PasswordRequestMessage val3 = (PasswordRequestMessage)(object)((message is PasswordRequestMessage) ? message : null);
				if (val3 == null)
				{
					ReconfigurationDeniedMessage val4 = (ReconfigurationDeniedMessage)(object)((message is ReconfigurationDeniedMessage) ? message : null);
					if (val4 == null)
					{
						ServerFailureActionRequestMessage val5 = (ServerFailureActionRequestMessage)(object)((message is ServerFailureActionRequestMessage) ? message : null);
						if (val5 != null)
						{
							CommunicationEventHandler<ServerFailureActionRequestEventArgs> val6 = ServerFailureActionRequested;
							if (val6 != null)
							{
								CommunicationEventHandlerExtensions.InvokeAsync<ServerFailureActionRequestEventArgs>(val6, new ServerFailureActionRequestEventArgs
								{
									Message = val5.Message,
									RequestId = val5.RequestId,
									ShowFallback = val5.ShowFallback
								});
							}
							return (Response)new SimpleResponse((SimpleResponsePurport)1);
						}
						return (Response)new SimpleResponse((SimpleResponsePurport)3);
					}
					CommunicationEventHandler<ReconfigurationEventArgs> val7 = ReconfigurationDenied;
					if (val7 != null)
					{
						CommunicationEventHandlerExtensions.InvokeAsync<ReconfigurationEventArgs>(val7, new ReconfigurationEventArgs
						{
							ConfigurationPath = val4.FilePath
						});
					}
					return (Response)new SimpleResponse((SimpleResponsePurport)1);
				}
				CommunicationEventHandler<PasswordRequestEventArgs> val8 = PasswordRequested;
				if (val8 != null)
				{
					CommunicationEventHandlerExtensions.InvokeAsync<PasswordRequestEventArgs>(val8, new PasswordRequestEventArgs
					{
						Purpose = val3.Purpose,
						RequestId = val3.RequestId
					});
				}
				return (Response)new SimpleResponse((SimpleResponsePurport)1);
			}
			CommunicationEventHandler<MessageBoxRequestEventArgs> val9 = MessageBoxRequested;
			if (val9 != null)
			{
				CommunicationEventHandlerExtensions.InvokeAsync<MessageBoxRequestEventArgs>(val9, new MessageBoxRequestEventArgs
				{
					Action = val2.Action,
					Icon = val2.Icon,
					Message = val2.Message,
					RequestId = val2.RequestId,
					Title = val2.Title
				});
			}
			return (Response)new SimpleResponse((SimpleResponsePurport)1);
		}
		CommunicationEventHandler<ExamSelectionRequestEventArgs> val10 = ExamSelectionRequested;
		if (val10 != null)
		{
			CommunicationEventHandlerExtensions.InvokeAsync<ExamSelectionRequestEventArgs>(val10, new ExamSelectionRequestEventArgs
			{
				Exams = val.Exams,
				RequestId = val.RequestId
			});
		}
		return (Response)new SimpleResponse((SimpleResponsePurport)1);
	}

	protected override Response OnReceive(SimpleMessagePurport message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected Obj, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected Obj, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected Obj, but got Unknown
		if ((int)message != 1)
		{
			if ((int)message != 5)
			{
				if ((int)message == 7)
				{
					CommunicationEventHandler val = this.m_Shutdown;
					if (val != null)
					{
						val.Invoke();
					}
					return (Response)new SimpleResponse((SimpleResponsePurport)1);
				}
				return (Response)new SimpleResponse((SimpleResponsePurport)3);
			}
			CommunicationEventHandler val2 = this.m_ReconfigurationAborted;
			if (val2 != null)
			{
				CommunicationEventHandlerExtensions.InvokeAsync(val2);
			}
			return (Response)new SimpleResponse((SimpleResponsePurport)1);
		}
		return (Response)new AuthenticationResponse
		{
			ProcessId = processId
		};
	}
}
