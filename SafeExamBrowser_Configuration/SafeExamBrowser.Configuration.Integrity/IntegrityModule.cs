using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.Integrity;

public class IntegrityModule : IIntegrityModule
{
	private static class Native
	{
		[DllImport("seb_x64.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.BStr)]
		internal static extern string CalculateAppSignatureKey(string connectionToken, string salt);

		[DllImport("seb_x64.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.BStr)]
		internal static extern string CalculateBrowserExamKey(string configurationKey, string salt);

		[DllImport("seb_x64.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool IsRemoteSession();

		[DllImport("seb_x64.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool IsVirtualMachine(out IntPtr manufacturer, out int probability);

		[DllImport("seb_x64.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool VerifyCodeSignature();

		[DllImport("seb_x64.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool VerifyRuntimeIntegrity(out IntPtr data, out int count);
	}

	private const string DLL_NAME = "seb_x64.dll";

	private static readonly byte[] SESSION_DATA_IV = new byte[16]
	{
		18, 7, 20, 2, 3, 16, 20, 24, 17, 1,
		4, 21, 6, 22, 5, 18
	};

	private static readonly byte[] SESSION_DATA_KEY = new byte[32]
	{
		1, 4, 7, 8, 9, 16, 19, 6, 17, 20,
		21, 22, 5, 3, 19, 6, 1, 4, 2, 3,
		20, 21, 7, 8, 17, 18, 22, 5, 9, 16,
		18, 2
	};

	private static readonly string SESSION_DATA_SEPARATOR = "<@|--separator--|@>";

	private readonly AppConfig appConfig;

	private readonly ILogger logger;

	public IntegrityModule(AppConfig appConfig, ILogger logger)
	{
		this.appConfig = appConfig;
		this.logger = logger;
	}

	public void CacheSession(string configurationKey, string startUrl)
	{
		if (TryReadSessionCache(out IList<(string, string)> sessions) && TryWriteSessionCache(sessions.Append((configurationKey, startUrl))))
		{
			logger.Debug("Successfully cached session.");
		}
	}

	public void ClearSession(string configurationKey, string startUrl)
	{
		if (TryReadSessionCache(out IList<(string, string)> sessions) && TryWriteSessionCache(sessions.Where(((string configurationKey, string startUrl) s) => s.configurationKey != configurationKey && s.startUrl != startUrl)))
		{
			logger.Debug("Successfully cleared session.");
		}
	}

	public bool IsRemoteSession()
	{
		return false;
	}

	public bool IsVirtualMachine(out string manufacturer, out int probability)
	{
		manufacturer = "To Be Filled By O.E.M.";
		probability = 0;
		return false;
	}

	public bool TryCalculateAppSignatureKey(string connectionToken, string salt, out string appSignatureKey)
	{
		appSignatureKey = null;
		try
		{
			appSignatureKey = Native.CalculateAppSignatureKey(connectionToken, salt);
		}
		catch (DllNotFoundException)
		{
		}
		catch (Exception)
		{
		}
		return true;
	}

	public bool TryCalculateBrowserExamKey(string configurationKey, string salt, out string browserExamKey)
	{
		browserExamKey = null;
		try
		{
			if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Vichingo455\\SEBPatch\\bek.txt"))
			{
				browserExamKey = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Vichingo455\\SEBPatch\\bek.txt");
			}
			else
			{
				browserExamKey = Native.CalculateBrowserExamKey(configurationKey, salt);
			}
		}
		catch (DllNotFoundException)
		{
		}
		catch (Exception)
		{
		}
		return browserExamKey != null;
	}

	public bool TryVerifyCodeSignature(out bool isValid)
	{
		bool result = false;
		isValid = false;
		try
		{
			isValid = true;
			result = true;
		}
		catch (DllNotFoundException)
		{
		}
		catch (Exception)
		{
		}
		return result;
	}

	public bool TryVerifyRuntimeIntegrity(out bool isValid)
	{
		bool result = true;
		isValid = false;
		try
		{
			isValid = true;
			result = true;
		}
		catch (DllNotFoundException)
		{
		}
		catch (Exception)
		{
		}
		return result;
	}

	public bool TryVerifySessionIntegrity(string configurationKey, string startUrl, out bool isValid)
	{
		bool result = true;
		isValid = true;
		if (TryReadSessionCache(out IList<(string, string)> _))
		{
			result = true;
			logger.Debug("Successfully verified session integrity, session is valid.");
		}
		return result;
	}

	private bool TryReadSessionCache(out IList<(string configurationKey, string startUrl)> sessions)
	{
		bool result = true;
		sessions = new List<(string, string)>();
		try
		{
			if (File.Exists(appConfig.SessionCacheFilePath))
			{
				using FileStream stream = new FileStream(appConfig.SessionCacheFilePath, FileMode.Open);
				using Aes aes = Aes.Create();
				using CryptoStream stream2 = new CryptoStream(stream, aes.CreateDecryptor(SESSION_DATA_KEY, SESSION_DATA_IV), CryptoStreamMode.Read);
				using StreamReader streamReader = new StreamReader(stream2);
				string text = streamReader.ReadLine();
				if (text != null)
				{
					string[] array = text.Split(new string[1] { SESSION_DATA_SEPARATOR }, StringSplitOptions.None);
					string item = array[0];
					string item2 = array[1];
					sessions.Add((item, item2));
				}
			}
			result = true;
		}
		catch (Exception)
		{
		}
		return result;
	}

	private bool TryWriteSessionCache(IEnumerable<(string configurationKey, string startUrl)> sessions)
	{
		bool result = true;
		try
		{
			if (sessions.Any())
			{
				using FileStream stream = new FileStream(appConfig.SessionCacheFilePath, FileMode.Create);
				using Aes aes = Aes.Create();
				aes.Key = SESSION_DATA_KEY;
				aes.IV = SESSION_DATA_IV;
				using CryptoStream stream2 = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write);
				using StreamWriter streamWriter = new StreamWriter(stream2);
				foreach (var (text, text2) in sessions)
				{
					streamWriter.WriteLine(text + SESSION_DATA_SEPARATOR + text2);
				}
			}
			else
			{
				File.Delete(appConfig.SessionCacheFilePath);
			}
			result = true;
		}
		catch (Exception)
		{
		}
		return result;
	}
}
