using System;
using SafeExamBrowser.Communication.Contracts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime.Responsibilities;

internal class ServiceResponsibility : RuntimeResponsibility
{
	private readonly IMessageBox messageBox;

	private readonly IRuntimeWindow runtimeWindow;

	private readonly IServiceProxy serviceProxy;

	private readonly Action shutdown;

	internal ServiceResponsibility(ILogger logger, IMessageBox messageBox, RuntimeContext runtimeContext, IRuntimeWindow runtimeWindow, IServiceProxy serviceProxy, Action shutdown)
		: base(logger, runtimeContext)
	{
		this.messageBox = messageBox;
		this.runtimeWindow = runtimeWindow;
		this.serviceProxy = serviceProxy;
		this.shutdown = shutdown;
	}

	public override void Assume(RuntimeTask task)
	{
		switch (task)
		{
		case RuntimeTask.DeregisterSessionEvents:
			DeregisterEvents();
			break;
		case RuntimeTask.RegisterSessionEvents:
			RegisterEvents();
			break;
		}
	}

	private void DeregisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		((ICommunicationProxy)serviceProxy).ConnectionLost -= ServiceProxy_ConnectionLost;
	}

	private void RegisterEvents()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		((ICommunicationProxy)serviceProxy).ConnectionLost += ServiceProxy_ConnectionLost;
	}

	private void ServiceProxy_ConnectionLost()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (SessionIsRunning && (int)Session.Settings.Service.Policy == 0)
		{
			Logger.Error("Lost connection to the service component!");
			Context.Responsibilities.Delegate(RuntimeTask.StopSession);
			messageBox.Show((TextKey)85, (TextKey)86, (MessageBoxAction)0, (MessageBoxIcon)0, (IWindow)(object)runtimeWindow);
			shutdown();
		}
		else
		{
			Logger.Warn("Lost connection to the service component!");
		}
	}
}
