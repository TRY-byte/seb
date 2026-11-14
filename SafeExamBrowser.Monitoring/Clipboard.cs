using System.Timers;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts;
using SafeExamBrowser.Settings.Security;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Monitoring;

public class Clipboard : IClipboard
{
	private readonly ILogger logger;

	private readonly INativeMethods nativeMethods;

	private readonly Timer timer;

	private ClipboardPolicy policy;

	public Clipboard(ILogger logger, INativeMethods nativeMethods, int timeout_ms = 50)
	{
		this.logger = logger;
		this.nativeMethods = nativeMethods;
		timer = new Timer(timeout_ms);
	}

	public void Initialize(ClipboardPolicy policy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		this.policy = policy;
		nativeMethods.EmptyClipboard();
		logger.Debug("Cleared clipboard.");
		if ((int)policy != 0)
		{
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
			logger.Debug($"Started clipboard monitoring with interval {timer.Interval} ms.");
		}
		else
		{
			logger.Debug("Clipboard is allowed, not starting monitoring.");
		}
		logger.Info($"Initialized clipboard for policy '{policy}'.");
	}

	public void Terminate()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		nativeMethods.EmptyClipboard();
		logger.Debug("Cleared clipboard.");
		if ((int)policy != 0)
		{
			timer.Stop();
			logger.Debug("Stopped clipboard monitoring.");
		}
		else
		{
			logger.Debug("Clipboard monitoring was not active.");
		}
		logger.Info("Finalized clipboard.");
	}

	private void Timer_Elapsed(object sender, ElapsedEventArgs e)
	{
		nativeMethods.EmptyClipboard();
	}
}
