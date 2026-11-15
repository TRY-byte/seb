using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.Cryptography;

public class KeyGenerator : IKeyGenerator
{
	private readonly object @lock = new object();

	private readonly ThreadLocal<SHA256Managed> algorithm;

	private readonly AppConfig appConfig;

	private readonly IIntegrityModule integrityModule;

	private readonly ILogger logger;

	private string browserExamKey;

	public KeyGenerator(AppConfig appConfig, IIntegrityModule integrityModule, ILogger logger)
	{
		algorithm = new ThreadLocal<SHA256Managed>(() => new SHA256Managed());
		this.appConfig = appConfig;
		this.integrityModule = integrityModule;
		this.logger = logger;
	}

	public string CalculateAppSignatureKey(string connectionToken, string salt)
	{
		string result = default;
		if (integrityModule.TryCalculateAppSignatureKey(connectionToken, salt, ref result))
		{
			logger.Debug("Successfully calculated app signature key using integrity module.");
		}
		else
		{
			logger.Error("Failed to calculate app signature key using integrity module!");
		}
		return result;
	}

	public string CalculateBrowserExamKeyHash(string configurationKey, byte[] salt, string url)
	{
		string text = url.Split('#')[0];
		byte[] array = algorithm.Value.ComputeHash(Encoding.UTF8.GetBytes(text + (browserExamKey ?? ComputeBrowserExamKey(configurationKey, salt))));
		return ToString(array);
	}

	public string CalculateConfigurationKeyHash(string configurationKey, string url)
	{
		string text = url.Split('#')[0];
		byte[] array = algorithm.Value.ComputeHash(Encoding.UTF8.GetBytes(text + configurationKey));
		return ToString(array);
	}

	public void UseCustomBrowserExamKey(string browserExamKey)
	{
		if (browserExamKey != null)
		{
			this.browserExamKey = browserExamKey;
			logger.Debug("Initialized custom browser exam key.");
		}
	}

	private string ComputeBrowserExamKey(string configurationKey, byte[] salt)
	{
		lock (@lock)
		{
			if (browserExamKey == null)
			{
				logger.Debug("Initializing browser exam key...");
				if (configurationKey == null)
				{
					configurationKey = "";
					logger.Warn("The current configuration does not contain a value for the configuration key!");
				}
				if (salt == null || salt.Length == 0)
				{
					salt = new byte[0];
					logger.Warn("The current configuration does not contain a salt value for the browser exam key!");
				}
				if (integrityModule.TryCalculateBrowserExamKey(configurationKey, ToString(salt), ref browserExamKey))
				{
					logger.Debug("Successfully calculated browser exam key using integrity module.");
				}
				else
				{
					logger.Warn("Failed to calculate browser exam key using integrity module! Falling back to simplified calculation...");
					using (HMACSHA256 hMACSHA = new HMACSHA256(salt))
					{
						byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(appConfig.CodeSignatureHash + appConfig.ProgramBuildVersion + configurationKey));
						string text = ToString(array);
						browserExamKey = text;
					}
					logger.Debug("Successfully calculated browser exam key using simplified calculation.");
				}
			}
		}
		return browserExamKey;
	}

	private string ToString(byte[] bytes)
	{
		return BitConverter.ToString(bytes).ToLower().Replace("-", string.Empty);
	}
}
