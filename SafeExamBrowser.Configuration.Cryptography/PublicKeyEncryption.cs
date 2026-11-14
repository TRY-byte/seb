using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.Cryptography;

public class PublicKeyEncryption : IPublicKeyEncryption
{
	protected const int PUBLIC_KEY_HASH_SIZE = 20;

	protected ICertificateStore store;

	protected ILogger logger;

	public PublicKeyEncryption(ICertificateStore store, ILogger logger)
	{
		this.logger = logger;
		this.store = store;
	}

	public virtual LoadStatus Decrypt(Stream data, out Stream decryptedData, out X509Certificate2 certificate)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = ParsePublicKeyHash(data);
		bool flag = store.TryGetCertificateWith(array, ref certificate);
		decryptedData = null;
		if (!flag)
		{
			return FailForMissingCertificate();
		}
		decryptedData = Decrypt(data, 20L, certificate);
		return (LoadStatus)4;
	}

	public virtual SaveStatus Encrypt(Stream data, X509Certificate2 certificate, out Stream encryptedData)
	{
		byte[] keyHash = GeneratePublicKeyHash(certificate);
		encryptedData = Encrypt(data, certificate);
		encryptedData = WriteEncryptionParameters(encryptedData, keyHash);
		return (SaveStatus)2;
	}

	protected LoadStatus FailForMissingCertificate()
	{
		logger.Error("Could not find certificate which matches the given public key hash!");
		return (LoadStatus)0;
	}

	protected byte[] GeneratePublicKeyHash(X509Certificate2 certificate)
	{
		byte[] rawData = certificate.PublicKey.EncodedKeyValue.RawData;
		using SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
		return sHA1CryptoServiceProvider.ComputeHash(rawData);
	}

	protected byte[] ParsePublicKeyHash(Stream data)
	{
		byte[] array = new byte[20];
		logger.Debug("Parsing public key hash...");
		data.Seek(0L, SeekOrigin.Begin);
		data.Read(array, 0, array.Length);
		return array;
	}

	protected MemoryStream Decrypt(Stream data, long offset, X509Certificate2 certificate)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = certificate.PrivateKey as RSACryptoServiceProvider;
		int num = rSACryptoServiceProvider.KeySize / 8;
		long num2 = (data.Length - offset) / num;
		MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[num];
		byte[] array2 = new byte[num];
		long num3 = data.Length - offset - num * num2;
		data.Seek(offset, SeekOrigin.Begin);
		logger.Debug("Decrypting data...");
		using (rSACryptoServiceProvider)
		{
			for (int i = 0; i < num2; i++)
			{
				data.Read(array2, 0, array2.Length);
				array = rSACryptoServiceProvider.Decrypt(array2, fOAEP: false);
				memoryStream.Write(array, 0, array.Length);
			}
			if (num3 > 0)
			{
				array2 = new byte[num3];
				data.Read(array2, 0, array2.Length);
				array = rSACryptoServiceProvider.Decrypt(array2, fOAEP: false);
				memoryStream.Write(array, 0, array.Length);
			}
		}
		return memoryStream;
	}

	protected Stream Encrypt(Stream data, X509Certificate2 certificate)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = certificate.PublicKey.Key as RSACryptoServiceProvider;
		int num = rSACryptoServiceProvider.KeySize / 8 - 32;
		long num2 = data.Length / num;
		byte[] array = new byte[num];
		MemoryStream memoryStream = new MemoryStream();
		byte[] array2 = new byte[num];
		long num3 = data.Length - num2 * num;
		data.Seek(0L, SeekOrigin.Begin);
		logger.Debug("Encrypting data...");
		using (rSACryptoServiceProvider)
		{
			for (int i = 0; i < num2; i++)
			{
				data.Read(array, 0, array.Length);
				array2 = rSACryptoServiceProvider.Encrypt(array, fOAEP: false);
				memoryStream.Write(array2, 0, array2.Length);
			}
			if (num3 > 0)
			{
				array = new byte[num3];
				data.Read(array, 0, array.Length);
				array2 = rSACryptoServiceProvider.Encrypt(array, fOAEP: false);
				memoryStream.Write(array2, 0, array2.Length);
			}
		}
		return memoryStream;
	}

	private Stream WriteEncryptionParameters(Stream encryptedData, byte[] keyHash)
	{
		MemoryStream memoryStream = new MemoryStream();
		logger.Debug("Writing encryption parameters...");
		memoryStream.Write(keyHash, 0, keyHash.Length);
		encryptedData.Seek(0L, SeekOrigin.Begin);
		encryptedData.CopyTo(memoryStream);
		return memoryStream;
	}
}
