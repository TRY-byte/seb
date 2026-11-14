using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace SafeExamBrowser.Runtime;

public class App : Application
{
	private static readonly Mutex Mutex = new Mutex(initiallyOwned: true, "safe_exam_browser_runtime_mutex");

	private readonly CompositionRoot instances = new CompositionRoot();

	[STAThread]
	public static void Main()
	{
		try
		{
			if (!CheckRegistry(RegistryHive.CurrentUser, "Software\\Vichingo455\\SEBPatch", "FirstOpeningDialogShown"))
			{
				MessageBox.Show("This patch has been created by Vichingo455.\nIt patches Safe Exam Browser functions to escape its kiosk mode, as well as allowing copy paste and more.\nAfter you finished using Safe Exam Browser, you can quit it using CTRL+Q or the button at the bottom.\nRemember to never run explorer.exe or you will mess up the mask (I will laugh at you if you get caught this way).\n\nThis message is only shown once, click OK when you finished reading.", "Safe Exam Browser Patch by Vichingo455", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				try
				{
					RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Vichingo455\\SEBPatch");
					registryKey.SetValue("FirstOpeningDialogShown", 1, RegistryValueKind.DWord);
					registryKey.Close();
				}
				catch
				{
				}
			}
			StartApplication();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		finally
		{
			Mutex.Close();
		}
	}

	private static void StartApplication()
	{
		if (NoInstanceRunning())
		{
			new App().Run();
		}
		else
		{
			MessageBox.Show("You can only run one instance of SEB at a time.", "Startup Not Allowed", MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
	}

	private static bool NoInstanceRunning()
	{
		return Mutex.WaitOne(TimeSpan.Zero, exitContext: true);
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		ShutdownMode = ShutdownMode.OnExplicitShutdown;
		instances.BuildObjectGraph(Shutdown);
		instances.LogStartupInformation();
		Task.Run((Action)TryStart);
	}

	private void TryStart()
	{
		if (!instances.RuntimeController.TryStart())
		{
			Shutdown();
		}
	}

	public new void Shutdown()
	{
		Task.Run((Action)ShutdownInternal);
	}

	private void ShutdownInternal()
	{
		instances.RuntimeController.Terminate();
		instances.LogShutdownInformation();
		Dispatcher.Invoke(base.Shutdown);
	}

	private static bool CheckRegistry(RegistryHive hive, string subKeyPath, string valueName)
	{
		try
		{
			using RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
			using RegistryKey registryKey2 = registryKey.OpenSubKey(subKeyPath, writable: false);
			if (registryKey2 == null)
			{
				return false;
			}
			object value = registryKey2.GetValue(valueName, null);
			registryKey2.Close();
			return value is int num && num == 1;
		}
		catch
		{
			return false;
		}
	}
}
