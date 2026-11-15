using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Data;
using SafeExamBrowser.Communication.Contracts.Events;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Hosts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Runtime.Communication;

internal class RuntimeHost : BaseHost, IRuntimeHost, ICommunicationHost
{
	[CompilerGenerated]
	private CommunicationEventHandler m_ClientDisconnected;

	[CompilerGenerated]
	private CommunicationEventHandler m_ClientReady;

	[CompilerGenerated]
	private CommunicationEventHandler m_ShutdownRequested;

	public bool AllowConnection { get; set; }

	public Guid? AuthenticationToken { private get; set; }

	public event CommunicationEventHandler ClientDisconnected
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ClientDisconnected;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ClientDisconnected, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ClientDisconnected;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ClientDisconnected, value2, val2);
			}
			while (val != val2);
		}
	}

	public event CommunicationEventHandler ClientReady
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ClientReady;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ClientReady, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ClientReady;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ClientReady, value2, val2);
			}
			while (val != val2);
		}
	}

	public event CommunicationEventHandler<ClientConfigurationEventArgs> ClientConfigurationNeeded;

	public event CommunicationEventHandler<ExamSelectionReplyEventArgs> ExamSelectionReceived;

	public event CommunicationEventHandler<MessageBoxReplyEventArgs> MessageBoxReplyReceived;

	public event CommunicationEventHandler<PasswordReplyEventArgs> PasswordReceived;

	public event CommunicationEventHandler<ReconfigurationEventArgs> ReconfigurationRequested;

	public event CommunicationEventHandler<ServerFailureActionReplyEventArgs> ServerFailureActionReceived;

	public event CommunicationEventHandler ShutdownRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ShutdownRequested;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ShutdownRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CommunicationEventHandler val = this.m_ShutdownRequested;
			CommunicationEventHandler val2;
			do
			{
				val2 = val;
				CommunicationEventHandler value2 = (CommunicationEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ShutdownRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	public RuntimeHost(string address, IHostObjectFactory factory, ILogger logger, int timeout_ms)
		: base(address, factory, logger, timeout_ms)
	{
	}

	protected override bool OnConnect(Guid? token = null)
	{
		bool flag = AuthenticationToken.HasValue && AuthenticationToken == token;
		bool num = AllowConnection & flag;
		if (num)
		{
			AllowConnection = false;
		}
		return num;
	}

	protected override void OnDisconnect(Interlocutor interlocutor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)interlocutor == 1)
		{
			CommunicationEventHandler val = this.m_ClientDisconnected;
			if (val != null)
			{
				val.Invoke();
			}
		}
	}

	protected override Response OnReceive(Message message)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected Obj, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected Obj, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected Obj, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected Obj, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected Obj, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected Obj, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected Obj, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected Obj, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected Obj, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected Obj, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected Obj, but got Unknown
		ExamSelectionReplyMessage val = (ExamSelectionReplyMessage)(object)((message is ExamSelectionReplyMessage) ? message : null);
		if (val == null)
		{
			MessageBoxReplyMessage val2 = (MessageBoxReplyMessage)(object)((message is MessageBoxReplyMessage) ? message : null);
			if (val2 == null)
			{
				PasswordReplyMessage val3 = (PasswordReplyMessage)(object)((message is PasswordReplyMessage) ? message : null);
				if (val3 == null)
				{
					ReconfigurationMessage val4 = (ReconfigurationMessage)(object)((message is ReconfigurationMessage) ? message : null);
					if (val4 == null)
					{
						ServerFailureActionReplyMessage val5 = (ServerFailureActionReplyMessage)(object)((message is ServerFailureActionReplyMessage) ? message : null);
						if (val5 != null)
						{
							CommunicationEventHandler<ServerFailureActionReplyEventArgs> val6 = ServerFailureActionReceived;
							if (val6 != null)
							{
								CommunicationEventHandlerExtensions.InvokeAsync<ServerFailureActionReplyEventArgs>(val6, new ServerFailureActionReplyEventArgs
								{
									Abort = val5.Abort,
									Fallback = val5.Fallback,
									RequestId = val5.RequestId,
									Retry = val5.Retry
								});
							}
							return (Response)new SimpleResponse((SimpleResponsePurport)1);
						}
						return (Response)new SimpleResponse((SimpleResponsePurport)3);
					}
					CommunicationEventHandler<ReconfigurationEventArgs> val7 = ReconfigurationRequested;
					if (val7 != null)
					{
						CommunicationEventHandlerExtensions.InvokeAsync<ReconfigurationEventArgs>(val7, new ReconfigurationEventArgs
						{
							ConfigurationPath = val4.ConfigurationPath,
							ResourceUrl = val4.ResourceUrl
						});
					}
					return (Response)new SimpleResponse((SimpleResponsePurport)1);
				}
				CommunicationEventHandler<PasswordReplyEventArgs> val8 = PasswordReceived;
				if (val8 != null)
				{
					CommunicationEventHandlerExtensions.InvokeAsync<PasswordReplyEventArgs>(val8, new PasswordReplyEventArgs
					{
						Password = val3.Password,
						RequestId = val3.RequestId,
						Success = val3.Success
					});
				}
				return (Response)new SimpleResponse((SimpleResponsePurport)1);
			}
			CommunicationEventHandler<MessageBoxReplyEventArgs> val9 = MessageBoxReplyReceived;
			if (val9 != null)
			{
				CommunicationEventHandlerExtensions.InvokeAsync<MessageBoxReplyEventArgs>(val9, new MessageBoxReplyEventArgs
				{
					RequestId = val2.RequestId,
					Result = val2.Result
				});
			}
			return (Response)new SimpleResponse((SimpleResponsePurport)1);
		}
		CommunicationEventHandler<ExamSelectionReplyEventArgs> val10 = ExamSelectionReceived;
		if (val10 != null)
		{
			CommunicationEventHandlerExtensions.InvokeAsync<ExamSelectionReplyEventArgs>(val10, new ExamSelectionReplyEventArgs
			{
				RequestId = val.RequestId,
				SelectedExamId = val.SelectedExamId,
				Success = val.Success
			});
		}
		return (Response)new SimpleResponse((SimpleResponsePurport)1);
	}

	protected override Response OnReceive(SimpleMessagePurport message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected I4, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected Obj, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected Obj, but got Unknown
		switch (message - 2)
		{
		case 0:
		{
			CommunicationEventHandler val2 = this.m_ClientReady;
			if (val2 != null)
			{
				val2.Invoke();
			}
			return (Response)new SimpleResponse((SimpleResponsePurport)1);
		}
		case 1:
			return HandleConfigurationRequest();
		case 4:
		{
			CommunicationEventHandler val = this.m_ShutdownRequested;
			if (val != null)
			{
				val.Invoke();
			}
			return (Response)new SimpleResponse((SimpleResponsePurport)1);
		}
		default:
			return (Response)new SimpleResponse((SimpleResponsePurport)3);
		}
	}

	private Response HandleConfigurationRequest()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		ClientConfigurationEventArgs e = new ClientConfigurationEventArgs();
		ClientConfigurationNeeded?.Invoke(e);
		return (Response)new ConfigurationResponse
		{
			Configuration = e.ClientConfiguration
		};
	}
}
