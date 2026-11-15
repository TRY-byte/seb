using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.Cryptography;

public class CertificateStore : ICertificateStore
{
	private ILogger logger;

	private readonly X509Store[] stores = new X509Store[3]
	{
		new X509Store(StoreLocation.CurrentUser),
		new X509Store(StoreLocation.LocalMachine),
		new X509Store(StoreName.TrustedPeople)
	};

	public CertificateStore(ILogger logger)
	{
		this.logger = logger;
	}

	public bool TryGetCertificateWith(byte[] keyHash, out X509Certificate2 certificate)
	{
		certificate = null;
		using (SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider())
		{
			X509Store[] array = stores;
			foreach (X509Store x509Store in array)
			{
				try
				{
					x509Store.Open(OpenFlags.ReadOnly);
					foreach (X509Certificate2 certificate2 in x509Store.Certificates)
					{
						byte[] rawData = certificate2.PublicKey.EncodedKeyValue.RawData;
						if (sHA1CryptoServiceProvider.ComputeHash(rawData).SequenceEqual(keyHash))
						{
							certificate = certificate2;
							return true;
						}
					}
				}
				finally
				{
					x509Store.Close();
				}
			}
		}
		return false;
	}

	public void ExtractAndImportIdentities(IDictionary<string, object> data)
	{
		if (!data.TryGetValue("embeddedCertificates", out var value))
		{
			return;
		}
		IList<IDictionary<string, object>> certificates = value as IList<IDictionary<string, object>>;
		if (certificates == null)
		{
			return;
		}
		List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
		foreach (IDictionary<string, object> item in certificates)
		{
			bool flag = item.TryGetValue("certificateData", out var value2);
			bool flag2 = item.TryGetValue("type", out var value3);
			bool flag3 = value3 is int num && num == 1;
			if ((flag & flag2 & flag3) && value2 is byte[] certificateData)
			{
				ImportIdentityCertificate(certificateData, new X509Store(StoreLocation.CurrentUser));
				ImportIdentityCertificate(certificateData, new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine));
				list.Add(item);
			}
		}
		list.ForEach((IDictionary<string, object> c) =>
		{
			certificates.Remove(c);
		});
	}

	private void ImportIdentityCertificate(byte[] certificateData, X509Store store)
	{
		try
		{
			X509Certificate2 x509Certificate = new X509Certificate2();
			x509Certificate.Import(certificateData, "Di\ud834\ude2dl\ud834\ude16Ch\ud834\ude12ah\ud834\ude47t\ud834\ude01a\ud834\ude48Hai1972", X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet);
			store.Open(OpenFlags.ReadWrite);
			store.Add(x509Certificate);
			logger.Info($"Successfully imported identity certificate into {store.Location}.{store.Name}.");
		}
		catch (Exception ex)
		{
			logger.Error($"Failed to import identity certificate into {store.Location}.{store.Name}!", ex);
		}
		finally
		{
			store.Close();
		}
	}
}
