using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace SafeExamBrowser.Client;

public class App : Application
{
	private const int ILMCM_CHECKLAYOUTANDTIPENABLED = 1;

	private const int ILMCM_LANGUAGEBAROFF = 2;

	private static readonly Mutex Mutex = new Mutex(initiallyOwned: true, "safe_exam_browser_client_mutex");

	private readonly CompositionRoot instances = new CompositionRoot();

	[STAThread]
	public static void Main()
	{
		try
		{
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
		InitLocalMsCtfMonitor(3);
		instances.BuildObjectGraph(Shutdown);
		instances.LogStartupInformation();
		if (!instances.ClientController.TryStart())
		{
			Shutdown();
		}
	}

	public new void Shutdown()
	{
		Dispatcher.InvokeAsync(shutdown);
		void shutdown()
		{
			instances.ClientController.Terminate();
			instances.LogShutdownInformation();
			UninitLocalMsCtfMonitor();
			base.Shutdown();
		}
	}

	[DllImport("MsCtfMonitor.dll", SetLastError = true)]
	private static extern IntPtr InitLocalMsCtfMonitor(int dwFlags);

	[DllImport("MsCtfMonitor.dll", SetLastError = true)]
	private static extern IntPtr UninitLocalMsCtfMonitor();
}
