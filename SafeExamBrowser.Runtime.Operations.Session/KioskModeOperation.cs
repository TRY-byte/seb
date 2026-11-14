using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Settings.Security;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class KioskModeOperation : SessionOperation
{
	private readonly IDesktopFactory desktopFactory;

	private readonly IDesktopMonitor desktopMonitor;

	private readonly IExplorerShell explorerShell;

	private readonly IProcessFactory processFactory;

	private Nullable activeMode;

	private IDesktop customDesktop;

	private IDesktop originalDesktop;

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

	public KioskModeOperation(Dependencies dependencies, IDesktopFactory desktopFactory, IDesktopMonitor desktopMonitor, IExplorerShell explorerShell, IProcessFactory processFactory)
		: base(dependencies)
	{
		this.desktopFactory = desktopFactory;
		this.desktopMonitor = desktopMonitor;
		this.explorerShell = explorerShell;
		this.processFactory = processFactory;
	}

	public override OperationResult Perform()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Invalid comparison between Unknown and I4
		Logger.Info($"Initializing kiosk mode '{Context.Next.Settings.Security.KioskMode}'...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)184);
		}
		activeMode = Context.Next.Settings.Security.KioskMode;
		KioskMode kioskMode = Context.Next.Settings.Security.KioskMode;
		if ((int)kioskMode != 1)
		{
			if ((int)kioskMode == 2)
			{
				TerminateExplorerShell();
			}
		}
		else
		{
			CreateCustomDesktop();
		}
		return (OperationResult)3;
	}

	public override OperationResult Repeat()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Invalid comparison between Unknown and I4
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Invalid comparison between Unknown and I4
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Invalid comparison between Unknown and I4
		KioskMode kioskMode = Context.Next.Settings.Security.KioskMode;
		if (activeMode == (Nullable)kioskMode)
		{
			Logger.Info($"New kiosk mode '{kioskMode}' is the same as the currently active mode, skipping re-initialization...");
		}
		else
		{
			Logger.Info($"Switching from kiosk mode '{activeMode}' to '{kioskMode}'...");
			StatusChangedEventHandler val = m_StatusChanged;
			if (val != null)
			{
				val.Invoke((TextKey)184);
			}
			Nullable val2 = activeMode;
			if (val2.HasValue)
			{
				KioskMode valueOrDefault = val2.GetValueOrDefault();
				if ((int)valueOrDefault != 1)
				{
					if ((int)valueOrDefault == 2)
					{
						RestartExplorerShell();
					}
				}
				else
				{
					CloseCustomDesktop();
				}
			}
			activeMode = kioskMode;
			if ((int)kioskMode != 1)
			{
				if ((int)kioskMode == 2)
				{
					TerminateExplorerShell();
				}
			}
			else
			{
				CreateCustomDesktop();
			}
		}
		return (OperationResult)3;
	}

	public override OperationResult Revert()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Invalid comparison between Unknown and I4
		Logger.Info($"Reverting kiosk mode '{activeMode}'...");
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)197);
		}
		Nullable val2 = activeMode;
		if (val2.HasValue)
		{
			KioskMode valueOrDefault = val2.GetValueOrDefault();
			if ((int)valueOrDefault != 1)
			{
				if ((int)valueOrDefault == 2)
				{
					RestartExplorerShell();
				}
			}
			else
			{
				CloseCustomDesktop();
			}
		}
		return (OperationResult)3;
	}

	private void CreateCustomDesktop()
	{
		originalDesktop = desktopFactory.GetCurrent();
		Logger.Info($"Current desktop is {originalDesktop}.");
		customDesktop = desktopFactory.CreateRandom();
		Logger.Info($"Created custom desktop {customDesktop}.");
		customDesktop.Activate();
		processFactory.StartupDesktop = customDesktop;
		Logger.Info("Successfully activated custom desktop.");
		desktopMonitor.Start(customDesktop);
	}

	private void CloseCustomDesktop()
	{
		desktopMonitor.Stop();
		if (originalDesktop != null)
		{
			originalDesktop.Activate();
			processFactory.StartupDesktop = originalDesktop;
			Logger.Info($"Switched back to original desktop {originalDesktop}.");
		}
		else
		{
			Logger.Warn("No original desktop found to activate!");
		}
		if (customDesktop != null)
		{
			customDesktop.Close();
			Logger.Info($"Closed custom desktop {customDesktop}.");
		}
		else
		{
			Logger.Warn("No custom desktop found to close!");
		}
	}

	private void TerminateExplorerShell()
	{
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)218);
		}
		explorerShell.HideAllWindows();
		explorerShell.Terminate();
	}

	private void RestartExplorerShell()
	{
		StatusChangedEventHandler val = m_StatusChanged;
		if (val != null)
		{
			val.Invoke((TextKey)217);
		}
		explorerShell.Start();
		explorerShell.RestoreAllWindows();
	}
}
