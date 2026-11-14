using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.Cryptography;

public class PublicKeySymmetricEncryption : PublicKeyEncryption
{
	private const int ENCRYPTION_KEY_LENGTH = 32;

	private const int KEY_LENGTH_SIZE = 4;

	private PasswordEncryption passwordEncryption;

	public PublicKeySymmetricEncryption(ICertificateStore store, ILogger logger, PasswordEncryption passwordEncryption)
		: base(store, logger)
	{
		this.passwordEncryption = passwordEncryption;
	}

	public override LoadStatus Decrypt(Stream data, out Stream decryptedData, out X509Certificate2 certificate)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = ParsePublicKeyHash(data);
		bool flag = store.TryGetCertificateWith(array, ref certificate);
		decryptedData = null;
		if (!flag)
		{
			return FailForMissingCertificate();
		}
		string password = ParseSymmetricKey(data, certificate);
		SubStream data2 = new SubStream(data, data.Position, data.Length - data.Position);
		return passwordEncryption.Decrypt(data2, password, out decryptedData);
	}

	public override SaveStatus Encrypt(Stream data, X509Certificate2 certificate, out Stream encryptedData)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		byte[] publicKeyHash = GeneratePublicKeyHash(certificate);
		byte[] array = GenerateSymmetricKey();
		string password = Convert.ToBase64String(array);
		SaveStatus val = passwordEncryption.Encrypt(data, password, out encryptedData);
		if ((int)val != 2)
		{
			return FailForUnsuccessfulPasswordEncryption(val);
		}
		encryptedData = WriteEncryptionParameters(encryptedData, certificate, publicKeyHash, array);
		return (SaveStatus)2;
	}

	private SaveStatus FailForUnsuccessfulPasswordEncryption(SaveStatus status)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		logger.Error($"Password encryption has failed with status '{status}'!");
		return (SaveStatus)3;
	}

	private byte[] GenerateSymmetricKey()
	{
		byte[] array = new byte[32];
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(array);
		return array;
	}

	private string ParseSymmetricKey(Stream data, X509Certificate2 certificate)
	{
		byte[] array = new byte[4];
		logger.Debug("Parsing symmetric key...");
		data.Seek(20L, SeekOrigin.Begin);
		data.Read(array, 0, array.Length);
		int num = BitConverter.ToInt32(array, 0);
		byte[] array2 = new byte[num];
		data.Read(array2, 0, array2.Length);
		SubStream data2 = new SubStream(data, 24L, num);
		return Convert.ToBase64String(Decrypt(data2, 0L, certificate).ToArray());
	}

	private Stream WriteEncryptionParameters(Stream encryptedData, X509Certificate2 certificate, byte[] publicKeyHash, byte[] symmetricKey)
	{
		MemoryStream memoryStream = new MemoryStream();
		MemoryStream data = new MemoryStream(symmetricKey);
		Stream stream = Encrypt(data, certificate);
		byte[] bytes = BitConverter.GetBytes((int)stream.Length);
		logger.Debug("Writing encryption parameters...");
		memoryStream.Write(publicKeyHash, 0, publicKeyHash.Length);
		memoryStream.Write(bytes, 0, bytes.Length);
		stream.Seek(0L, SeekOrigin.Begin);
		stream.CopyTo(memoryStream);
		encryptedData.Seek(0L, SeekOrigin.Begin);
		encryptedData.CopyTo(memoryStream);
		return memoryStream;
	}
}
