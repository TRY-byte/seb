using System.Collections.Generic;
using System.IO;
using System.Text;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.DataCompression;
using SafeExamBrowser.Configuration.Contracts.DataFormats;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataFormats;

public class BinarySerializer : IDataSerializer
{
	private IDataCompressor compressor;

	private ILogger logger;

	private IPasswordEncryption passwordEncryption;

	private IPublicKeyEncryption publicKeyEncryption;

	private IPublicKeyEncryption symmetricEncryption;

	private IDataSerializer xmlSerializer;

	public BinarySerializer(IDataCompressor compressor, ILogger logger, IPasswordEncryption passwordEncryption, IPublicKeyEncryption publicKeyEncryption, IPublicKeyEncryption symmetricEncryption, IDataSerializer xmlSerializer)
	{
		this.compressor = compressor;
		this.logger = logger;
		this.passwordEncryption = passwordEncryption;
		this.publicKeyEncryption = publicKeyEncryption;
		this.symmetricEncryption = symmetricEncryption;
		this.xmlSerializer = xmlSerializer;
	}

	public bool CanSerialize(FormatType format)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		return (int)format == 1;
	}

	public SerializeResult TrySerialize(IDictionary<string, object> data, EncryptionParameters encryption = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		SerializeResult val = new SerializeResult();
		PasswordParameters val2 = (PasswordParameters)(object)((encryption is PasswordParameters) ? encryption : null);
		if (val2 == null)
		{
			PublicKeyParameters val3 = (PublicKeyParameters)(object)((encryption is PublicKeyParameters) ? encryption : null);
			val = ((val3 == null) ? SerializePlainDataBlock(data, writePrefix: true) : SerializePublicKeyHashBlock(data, val3));
		}
		else
		{
			val = SerializePasswordBlock(data, val2);
		}
		if ((int)val.Status == 2)
		{
			val.Data = compressor.Compress(val.Data);
		}
		return val;
	}

	private SerializeResult SerializePasswordBlock(IDictionary<string, object> data, PasswordParameters password)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		SerializeResult val = SerializePlainDataBlock(data);
		if ((int)val.Status == 2)
		{
			string prefix = (password.IsHash ? "pwcc" : "pswd");
			logger.Debug("Attempting to serialize password block...");
			Stream data2 = default;
			SaveStatus val2 = passwordEncryption.Encrypt(val.Data, password.Password, ref data2);
			if ((int)val2 == 2)
			{
				val.Data = WritePrefix(prefix, data2);
			}
			val.Status = val2;
		}
		return val;
	}

	private SerializeResult SerializePlainDataBlock(IDictionary<string, object> data, bool writePrefix = false)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		logger.Debug("Attempting to serialize plain data block...");
		SerializeResult val = xmlSerializer.TrySerialize(data, (EncryptionParameters)null);
		if ((int)val.Status == 2)
		{
			if (writePrefix)
			{
				val.Data = WritePrefix("plnd", val.Data);
			}
			val.Data = compressor.Compress(val.Data);
		}
		return val;
	}

	private SerializeResult SerializePublicKeyHashBlock(IDictionary<string, object> data, PublicKeyParameters parameters)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		SerializeResult val = SerializePublicKeyHashInnerBlock(data, parameters);
		if ((int)val.Status == 2)
		{
			IPublicKeyEncryption val2 = (parameters.SymmetricEncryption ? symmetricEncryption : publicKeyEncryption);
			string prefix = (parameters.SymmetricEncryption ? "phsk" : "pkhs");
			logger.Debug("Attempting to serialize public key hash block...");
			Stream data2 = default;
			if ((int)val2.Encrypt(val.Data, parameters.Certificate, ref data2) == 2)
			{
				val.Data = WritePrefix(prefix, data2);
			}
		}
		return val;
	}

	private SerializeResult SerializePublicKeyHashInnerBlock(IDictionary<string, object> data, PublicKeyParameters parameters)
	{
		PasswordParameters innerEncryption = parameters.InnerEncryption;
		if (innerEncryption != null)
		{
			return SerializePasswordBlock(data, innerEncryption);
		}
		return SerializePlainDataBlock(data, writePrefix: true);
	}

	private Stream WritePrefix(string prefix, Stream data)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(prefix);
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(bytes, 0, bytes.Length);
		data.Seek(0L, SeekOrigin.Begin);
		data.CopyTo(memoryStream);
		return memoryStream;
	}
}
