using System.IO;
using System.Linq;
using System.Security.Cryptography;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.Cryptography;

public class PasswordEncryption : IPasswordEncryption
{
	private const int BLOCK_SIZE = 16;

	private const int HEADER_SIZE = 2;

	private const int ITERATIONS = 10000;

	private const int KEY_SIZE = 32;

	private const int OPTIONS = 1;

	private const int SALT_SIZE = 8;

	private const int VERSION = 2;

	private ILogger logger;

	public PasswordEncryption(ILogger logger)
	{
		this.logger = logger;
	}

	public LoadStatus Decrypt(Stream data, string password, out Stream decrypted)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		decrypted = null;
		if (password == null)
		{
			return (LoadStatus)3;
		}
		ParseHeader(data);
		(byte[] authenticationKey, byte[] encryptionKey) tuple = GenerateKeysForDecryption(data, password);
		byte[] item = tuple.authenticationKey;
		byte[] item2 = tuple.encryptionKey;
		(byte[] originalHmac, byte[] computedHmac) tuple2 = GenerateHmacForDecryption(item, data);
		var (array, _) = tuple2;
		if (!tuple2.computedHmac.SequenceEqual(array))
		{
			return FailForInvalidHmac();
		}
		decrypted = Decrypt(data, item2, array.Length);
		return (LoadStatus)4;
	}

	public SaveStatus Encrypt(Stream data, string password, out Stream encrypted)
	{
		(byte[] authKey, byte[] authSalt, byte[] encrKey, byte[] encrSalt) tuple = GenerateKeysForEncryption(password);
		byte[] item = tuple.authKey;
		byte[] item2 = tuple.authSalt;
		byte[] item3 = tuple.encrKey;
		byte[] item4 = tuple.encrSalt;
		encrypted = Encrypt(data, item3, out var initializationVector);
		encrypted = WriteEncryptionParameters(item, item2, item4, initializationVector, encrypted);
		return (SaveStatus)2;
	}

	private (int version, int options) ParseHeader(Stream data)
	{
		data.Seek(0L, SeekOrigin.Begin);
		logger.Debug("Parsing encryption header...");
		int num = data.ReadByte();
		int num2 = data.ReadByte();
		if (num != 2 || num2 != 1)
		{
			logger.Debug($"Unknown encryption header! Expected: [{2},{1},...] - Actual: [{num},{num2},...]");
		}
		return (version: num, options: num2);
	}

	private (byte[] authenticationKey, byte[] encryptionKey) GenerateKeysForDecryption(Stream data, string password)
	{
		byte[] array = new byte[8];
		byte[] array2 = new byte[8];
		logger.Debug("Generating keys for authentication and decryption...");
		data.Seek(2L, SeekOrigin.Begin);
		data.Read(array2, 0, 8);
		data.Read(array, 0, 8);
		using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, array, 10000);
		using Rfc2898DeriveBytes rfc2898DeriveBytes2 = new Rfc2898DeriveBytes(password, array2, 10000);
		byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
		byte[] bytes2 = rfc2898DeriveBytes2.GetBytes(32);
		return (authenticationKey: bytes, encryptionKey: bytes2);
	}

	private (byte[] authKey, byte[] authSalt, byte[] encrKey, byte[] encrSalt) GenerateKeysForEncryption(string password)
	{
		logger.Debug("Generating keys for authentication and encryption...");
		using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, 8, 10000);
		using Rfc2898DeriveBytes rfc2898DeriveBytes2 = new Rfc2898DeriveBytes(password, 8, 10000);
		byte[] salt = rfc2898DeriveBytes.Salt;
		byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
		byte[] salt2 = rfc2898DeriveBytes2.Salt;
		byte[] bytes2 = rfc2898DeriveBytes2.GetBytes(32);
		return (authKey: bytes, authSalt: salt, encrKey: bytes2, encrSalt: salt2);
	}

	private (byte[] originalHmac, byte[] computedHmac) GenerateHmacForDecryption(byte[] authenticationKey, Stream data)
	{
		logger.Debug("Generating HMACs for authentication...");
		using HMACSHA256 hMACSHA = new HMACSHA256(authenticationKey);
		byte[] array = new byte[hMACSHA.HashSize / 8];
		SubStream inputStream = new SubStream(data, 0L, data.Length - array.Length);
		byte[] item = hMACSHA.ComputeHash(inputStream);
		data.Seek(-array.Length, SeekOrigin.End);
		data.Read(array, 0, array.Length);
		return (originalHmac: array, computedHmac: item);
	}

	private byte[] GenerateHmacForEncryption(byte[] authenticationKey, Stream data)
	{
		data.Seek(0L, SeekOrigin.Begin);
		logger.Debug("Generating HMAC for authentication...");
		using HMACSHA256 hMACSHA = new HMACSHA256(authenticationKey);
		return hMACSHA.ComputeHash(data);
	}

	private LoadStatus FailForInvalidHmac()
	{
		logger.Debug("The authentication failed due to an invalid password or corrupted data!");
		return (LoadStatus)3;
	}

	private Stream Decrypt(Stream data, byte[] encryptionKey, int hmacLength)
	{
		byte[] array = new byte[16];
		data.Seek(18L, SeekOrigin.Begin);
		data.Read(array, 0, 16);
		MemoryStream memoryStream = new MemoryStream();
		SubStream stream = new SubStream(data, data.Position, data.Length - data.Position - hmacLength);
		logger.Debug("Decrypting data...");
		using AesManaged aesManaged = new AesManaged
		{
			KeySize = 256,
			BlockSize = 128,
			Mode = CipherMode.CBC,
			Padding = PaddingMode.PKCS7
		};
		using ICryptoTransform transform = aesManaged.CreateDecryptor(encryptionKey, array);
		using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
		cryptoStream.CopyTo(memoryStream);
		return memoryStream;
	}

	private Stream Encrypt(Stream data, byte[] encryptionKey, out byte[] initializationVector)
	{
		MemoryStream memoryStream = new MemoryStream();
		logger.Debug("Encrypting data...");
		using AesManaged aesManaged = new AesManaged
		{
			KeySize = 256,
			BlockSize = 128,
			Mode = CipherMode.CBC,
			Padding = PaddingMode.PKCS7
		};
		aesManaged.GenerateIV();
		data.Seek(0L, SeekOrigin.Begin);
		initializationVector = aesManaged.IV;
		using (ICryptoTransform transform = aesManaged.CreateEncryptor(encryptionKey, initializationVector))
		{
			using CryptoStream cryptoStream = new CryptoStream(data, transform, CryptoStreamMode.Read);
			cryptoStream.CopyTo(memoryStream);
		}
		return memoryStream;
	}

	private Stream WriteEncryptionParameters(byte[] authKey, byte[] authSalt, byte[] encrSalt, byte[] initVector, Stream encryptedData)
	{
		MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[2] { 2, 1 };
		logger.Debug("Writing encryption parameters...");
		memoryStream.Write(array, 0, array.Length);
		memoryStream.Write(encrSalt, 0, encrSalt.Length);
		memoryStream.Write(authSalt, 0, authSalt.Length);
		memoryStream.Write(initVector, 0, initVector.Length);
		encryptedData.Seek(0L, SeekOrigin.Begin);
		encryptedData.CopyTo(memoryStream);
		byte[] array2 = GenerateHmacForEncryption(authKey, memoryStream);
		memoryStream.Seek(0L, SeekOrigin.End);
		memoryStream.Write(array2, 0, array2.Length);
		return memoryStream;
	}
}
