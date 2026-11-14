using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.DataCompression;
using SafeExamBrowser.Configuration.Contracts.DataFormats;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataFormats;

public class BinaryParser : IDataParser
{
	private const int PREFIX_LENGTH = 4;

	private IDataCompressor compressor;

	private IHashAlgorithm hashAlgorithm;

	private ILogger logger;

	private IPasswordEncryption passwordEncryption;

	private IPublicKeyEncryption publicKeyEncryption;

	private IPublicKeyEncryption publicKeySymmetricEncryption;

	private readonly IDataParser xmlParser;

	public BinaryParser(IDataCompressor compressor, IHashAlgorithm hashAlgorithm, ILogger logger, IPasswordEncryption passwordEncryption, IPublicKeyEncryption publicKeyEncryption, IPublicKeyEncryption publicKeySymmetricEncryption, IDataParser xmlParser)
	{
		this.compressor = compressor;
		this.hashAlgorithm = hashAlgorithm;
		this.logger = logger;
		this.passwordEncryption = passwordEncryption;
		this.publicKeyEncryption = publicKeyEncryption;
		this.publicKeySymmetricEncryption = publicKeySymmetricEncryption;
		this.xmlParser = xmlParser;
	}

	public bool CanParse(Stream data)
	{
		try
		{
			if (data.Length > 4)
			{
				string text = ReadPrefix(data);
				bool flag = IsValid(text);
				logger.Debug(string.Format("'{0}' starting with '{1}' {2} the {3} format.", data, text, flag ? "matches" : "does not match", (object)(FormatType)1));
				return flag;
			}
			logger.Debug($"'{data}' is not long enough ({data.Length} bytes) to match the {(object)(FormatType)1} format.");
		}
		catch (Exception ex)
		{
			logger.Error($"Failed to determine whether '{data}' with {(double?)data?.Length / 1000.0} KB data matches the {(object)(FormatType)1} format!", ex);
		}
		return false;
	}

	public ParseResult TryParse(Stream data, PasswordParameters password = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected Obj, but got Unknown
		string text = ReadPrefix(data);
		bool flag = IsValid(text);
		ParseResult val = new ParseResult
		{
			Status = (LoadStatus)0
		};
		if (flag)
		{
			data = (compressor.IsCompressed(data) ? compressor.Decompress(data) : data);
			data = new SubStream(data, 4L, data.Length - 4);
			switch (text)
			{
			case "pswd":
			case "pwcc":
				val = ParsePasswordBlock(data, text, password);
				break;
			case "plnd":
				val = ParsePlainDataBlock(data);
				break;
			case "pkhs":
			case "phsk":
				val = ParsePublicKeyBlock(data, text, password);
				break;
			}
			val.Format = (FormatType)1;
		}
		else
		{
			logger.Error($"'{data}' starting with '{text}' does not match the {(object)(FormatType)1} format!");
		}
		return val;
	}

	private ParseResult ParsePasswordBlock(Stream data, string prefix, PasswordParameters password = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		ParseResult val = new ParseResult();
		if (password != null)
		{
			PasswordParameters val2 = DetermineEncryptionParametersFor(prefix, password);
			logger.Debug("Attempting to parse password block with prefix '" + prefix + "'...");
			Stream data2 = default;
			val.Status = passwordEncryption.Decrypt(data, val2.Password, ref data2);
			if ((int)val.Status == 4)
			{
				val = ParsePlainDataBlock(data2);
				val.Encryption = (EncryptionParameters)(object)val2;
			}
		}
		else
		{
			val.Status = (LoadStatus)3;
		}
		return val;
	}

	private ParseResult ParsePlainDataBlock(Stream data)
	{
		data = (compressor.IsCompressed(data) ? compressor.Decompress(data) : data);
		logger.Debug("Attempting to parse plain data block...");
		return xmlParser.TryParse(data, (PasswordParameters)null);
	}

	private ParseResult ParsePublicKeyBlock(Stream data, string prefix, PasswordParameters password = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected Obj, but got Unknown
		IPublicKeyEncryption val = ((prefix == "pkhs") ? publicKeyEncryption : publicKeySymmetricEncryption);
		ParseResult val2 = new ParseResult();
		logger.Debug("Attempting to parse public key hash block with prefix '" + prefix + "'...");
		Stream data2 = default;
		X509Certificate2 certificate = default;
		val2.Status = val.Decrypt(data, ref data2, ref certificate);
		if ((int)val2.Status == 4)
		{
			val2 = TryParse(data2, password);
			ParseResult val3 = val2;
			PublicKeyParameters val4 = new PublicKeyParameters
			{
				Certificate = certificate
			};
			EncryptionParameters encryption = val2.Encryption;
			val4.InnerEncryption = (PasswordParameters)(object)((encryption is PasswordParameters) ? encryption : null);
			val4.SymmetricEncryption = prefix == "phsk";
			val3.Encryption = (EncryptionParameters)val4;
		}
		return val2;
	}

	private PasswordParameters DetermineEncryptionParametersFor(string prefix, PasswordParameters password)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		PasswordParameters val = new PasswordParameters();
		if (prefix == "pwcc")
		{
			val.Password = (password.IsHash ? password.Password : hashAlgorithm.GenerateHashFor(password.Password));
			val.IsHash = true;
		}
		else
		{
			val.Password = password.Password;
			val.IsHash = password.IsHash;
		}
		return val;
	}

	private string ReadPrefix(Stream data)
	{
		byte[] array = new byte[4];
		if (compressor.IsCompressed(data))
		{
			array = compressor.Peek(data, 4);
		}
		else
		{
			data.Seek(0L, SeekOrigin.Begin);
			data.Read(array, 0, 4);
		}
		return Encoding.UTF8.GetString(array);
	}

	private bool IsValid(string prefix)
	{
		BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		return typeof(BinaryBlock).GetFields(bindingAttr).Any((FieldInfo f) => f.GetRawConstantValue() as string == prefix);
	}
}
