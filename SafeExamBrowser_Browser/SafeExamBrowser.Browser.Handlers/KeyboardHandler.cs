using System;
using System.Runtime.CompilerServices;
using System.Threading;
using CefSharp;
using SafeExamBrowser.Browser.Contracts.Events;
using SafeExamBrowser.UserInterface.Contracts;

namespace SafeExamBrowser.Browser.Handlers;

internal class KeyboardHandler : IKeyboardHandler
{
	[CompilerGenerated]
	private ActionRequestedEventHandler m_FindRequested;

	[CompilerGenerated]
	private ActionRequestedEventHandler m_HomeNavigationRequested;

	[CompilerGenerated]
	private ActionRequestedEventHandler m_ReloadRequested;

	[CompilerGenerated]
	private ActionRequestedEventHandler m_ZoomInRequested;

	[CompilerGenerated]
	private ActionRequestedEventHandler m_ZoomOutRequested;

	[CompilerGenerated]
	private ActionRequestedEventHandler m_ZoomResetRequested;

	[CompilerGenerated]
	private ActionRequestedEventHandler m_FocusAddressBarRequested;

	[CompilerGenerated]
	private TabPressedEventHandler m_TabPressed;

	private int? currentKeyDown;

	internal event ActionRequestedEventHandler FindRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_FindRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_FindRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_FindRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_FindRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event ActionRequestedEventHandler HomeNavigationRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_HomeNavigationRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_HomeNavigationRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_HomeNavigationRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_HomeNavigationRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event ActionRequestedEventHandler ReloadRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ReloadRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ReloadRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ReloadRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ReloadRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event ActionRequestedEventHandler ZoomInRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ZoomInRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoomInRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ZoomInRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoomInRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event ActionRequestedEventHandler ZoomOutRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ZoomOutRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoomOutRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ZoomOutRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoomOutRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event ActionRequestedEventHandler ZoomResetRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ZoomResetRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoomResetRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_ZoomResetRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoomResetRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event ActionRequestedEventHandler FocusAddressBarRequested
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_FocusAddressBarRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_FocusAddressBarRequested, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ActionRequestedEventHandler val = this.m_FocusAddressBarRequested;
			ActionRequestedEventHandler val2;
			do
			{
				val2 = val;
				ActionRequestedEventHandler value2 = (ActionRequestedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_FocusAddressBarRequested, value2, val2);
			}
			while (val != val2);
		}
	}

	internal event TabPressedEventHandler TabPressed
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TabPressedEventHandler val = this.m_TabPressed;
			TabPressedEventHandler val2;
			do
			{
				val2 = val;
				TabPressedEventHandler value2 = (TabPressedEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TabPressed, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			TabPressedEventHandler val = this.m_TabPressed;
			TabPressedEventHandler val2;
			do
			{
				val2 = val;
				TabPressedEventHandler value2 = (TabPressedEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
				val = Interlocked.CompareExchange(ref this.m_TabPressed, value2, val2);
			}
			while (val != val2);
		}
	}

	public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int keyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		bool flag = ((Enum)modifiers).HasFlag((Enum)(object)(CefEventFlags)4);
		bool flag2 = ((Enum)modifiers).HasFlag((Enum)(object)(CefEventFlags)2);
		if ((int)type == 2)
		{
			if (flag && keyCode == 70)
			{
				ActionRequestedEventHandler val = this.m_FindRequested;
				if (val != null)
				{
					val.Invoke();
				}
			}
			if (keyCode == 36)
			{
				ActionRequestedEventHandler val2 = this.m_HomeNavigationRequested;
				if (val2 != null)
				{
					val2.Invoke();
				}
			}
			if (flag && keyCode == 76)
			{
				ActionRequestedEventHandler val3 = this.m_FocusAddressBarRequested;
				if (val3 != null)
				{
					val3.Invoke();
				}
			}
			if ((flag && keyCode == 107) || (flag && keyCode == 187) || ((flag & flag2) && keyCode == 49))
			{
				ActionRequestedEventHandler val4 = this.m_ZoomInRequested;
				if (val4 != null)
				{
					val4.Invoke();
				}
			}
			if (flag && (keyCode == 109 || keyCode == 189))
			{
				ActionRequestedEventHandler val5 = this.m_ZoomOutRequested;
				if (val5 != null)
				{
					val5.Invoke();
				}
			}
			if (flag && (keyCode == 48 || keyCode == 96))
			{
				ActionRequestedEventHandler val6 = this.m_ZoomResetRequested;
				if (val6 != null)
				{
					val6.Invoke();
				}
			}
			if (keyCode == 9 && keyCode == currentKeyDown)
			{
				TabPressedEventHandler val7 = this.m_TabPressed;
				if (val7 != null)
				{
					val7.Invoke(flag2);
				}
			}
		}
		currentKeyDown = null;
		return false;
	}

	public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int keyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		if ((int)type == 2 && keyCode == 116)
		{
			ActionRequestedEventHandler val = this.m_ReloadRequested;
			if (val != null)
			{
				val.Invoke();
			}
			return true;
		}
		if ((int)type == 0 || (int)type == 1)
		{
			currentKeyDown = keyCode;
		}
		return false;
	}
}
