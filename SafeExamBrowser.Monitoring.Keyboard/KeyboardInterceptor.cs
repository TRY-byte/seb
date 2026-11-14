using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Keyboard;
using SafeExamBrowser.Settings.Monitoring;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Contracts.Events;

namespace SafeExamBrowser.Monitoring.Keyboard;

public class KeyboardInterceptor : IKeyboardInterceptor
{
	private Guid? hookId;

	private readonly ILogger logger;

	private readonly INativeMethods nativeMethods;

	private readonly KeyboardSettings settings;

	public KeyboardInterceptor(ILogger logger, INativeMethods nativeMethods, KeyboardSettings settings)
	{
		this.logger = logger;
		this.nativeMethods = nativeMethods;
		this.settings = settings;
	}

	public void Start()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected Obj, but got Unknown
		hookId = nativeMethods.RegisterKeyboardHook((KeyboardHookCallback)KeyboardHookCallback);
	}

	public void Stop()
	{
		if (hookId.HasValue)
		{
			nativeMethods.DeregisterKeyboardHook(hookId.Value);
		}
	}

	private bool KeyboardHookCallback(int keyCode, KeyModifier modifier, KeyState state)
	{
		return false;
	}

	private void Log(Key key, int keyCode, KeyModifier modifier, KeyState state)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<KeyModifier> enumerable = Enum.GetValues(typeof(KeyModifier)).OfType<KeyModifier>().Where((KeyModifier m) =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			return (int)m != 0 && ((Enum)modifier).HasFlag((Enum)(object)m);
		});
		string text = (enumerable.Any() ? (string.Join(" + ", enumerable) + " + ") : string.Empty);
		logger.Info($"Blocked '{text}{key}' ({key} = {keyCode}) when {((object)state/*cast due to constrained. prefix*/).ToString().ToLower()}.");
	}
}
