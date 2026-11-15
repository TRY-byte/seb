using System;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Mouse;
using SafeExamBrowser.Settings.Monitoring;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Contracts.Events;

namespace SafeExamBrowser.Monitoring.Mouse;

public class MouseInterceptor : IMouseInterceptor
{
	private Guid? hookId;

	private ILogger logger;

	private INativeMethods nativeMethods;

	private MouseSettings settings;

	public MouseInterceptor(ILogger logger, INativeMethods nativeMethods, MouseSettings settings)
	{
		this.logger = logger;
		this.nativeMethods = nativeMethods;
		this.settings = settings;
	}

	public void Start()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected Obj, but got Unknown
		hookId = nativeMethods.RegisterMouseHook((MouseHookCallback)MouseHookCallback);
	}

	public void Stop()
	{
		if (hookId.HasValue)
		{
			nativeMethods.DeregisterMouseHook(hookId.Value);
		}
	}

	private bool MouseHookCallback(MouseButton button, MouseButtonState state, MouseInformation info)
	{
		return false;
	}
}
