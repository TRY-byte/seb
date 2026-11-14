using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SafeExamBrowser.Configuration.ConfigurationData;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.DataFormats;
using SafeExamBrowser.Configuration.Contracts.DataResources;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration;

public class ConfigurationRepository : IConfigurationRepository
{
	private readonly ICertificateStore certificateStore;

	private readonly IList<IDataParser> dataParsers;

	private readonly IList<IDataSerializer> dataSerializers;

	private readonly DataMapper dataMapper;

	private readonly DataProcessor dataProcessor;

	private readonly DataValues dataValues;

	private readonly ILogger logger;

	private readonly IList<IResourceLoader> resourceLoaders;

	private readonly IList<IResourceSaver> resourceSavers;

	public ConfigurationRepository(ICertificateStore certificateStore, IModuleLogger logger)
	{
		this.certificateStore = certificateStore;
		this.logger = (ILogger)(object)logger;
		dataParsers = new List<IDataParser>();
		dataSerializers = new List<IDataSerializer>();
		dataMapper = new DataMapper();
		dataProcessor = new DataProcessor();
		dataValues = new DataValues();
		resourceLoaders = new List<IResourceLoader>();
		resourceSavers = new List<IResourceSaver>();
	}

	public SaveStatus ConfigureClientWith(Uri resource, PasswordParameters password = null)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		logger.Info($"Attempting to configure local client with '{resource}'...");
		try
		{
			TryLoadData(resource, out var data);
			using (data)
			{
				TryParseData(data, out var encryption, out var format, out var rawData, password);
				certificateStore.ExtractAndImportIdentities(rawData);
				encryption = DetermineEncryptionForClientConfiguration(rawData, encryption);
				SaveStatus val = TrySerializeData(rawData, format, out var serialized, encryption);
				using (serialized)
				{
					if ((int)val == 2)
					{
						val = TrySaveData(new Uri(dataValues.GetAppDataFilePath()), serialized);
					}
					return val;
				}
			}
		}
		catch (Exception ex)
		{
			logger.Error($"Unexpected error while trying to configure local client with '{resource}'!", ex);
			return (SaveStatus)3;
		}
	}

	public AppConfig InitializeAppConfig()
	{
		return dataValues.InitializeAppConfig();
	}

	public SessionConfiguration InitializeSessionConfiguration()
	{
		return dataValues.InitializeSessionConfiguration();
	}

	public AppSettings LoadDefaultSettings()
	{
		return dataValues.LoadDefaultSettings();
	}

	public void Register(IDataParser parser)
	{
		dataParsers.Add(parser);
	}

	public void Register(IDataSerializer serializer)
	{
		dataSerializers.Add(serializer);
	}

	public void Register(IResourceLoader loader)
	{
		resourceLoaders.Add(loader);
	}

	public void Register(IResourceSaver saver)
	{
		resourceSavers.Add(saver);
	}

	public LoadStatus TryLoadSettings(Uri resource, out AppSettings settings, PasswordParameters password = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		LoadStatus val = (LoadStatus)0;
		settings = LoadDefaultSettings();
		dataProcessor.ProcessDefault(settings);
		logger.Info($"Initialized default settings, now attempting to load '{resource}'...");
		try
		{
			val = TryLoadData(resource, out var data);
			using (data)
			{
				if ((int)val == 4)
				{
					val = TryParseData(data, out var _, out var _, out var rawData, password);
					if ((int)val == 4)
					{
						dataMapper.Map(rawData, settings);
						dataProcessor.Process(rawData, settings);
					}
				}
			}
		}
		catch (Exception ex)
		{
			val = (LoadStatus)5;
			logger.Error($"Unexpected error while trying to load '{resource}'!", ex);
		}
		return val;
	}

	private EncryptionParameters DetermineEncryptionForClientConfiguration(IDictionary<string, object> data, EncryptionParameters encryption)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected Obj, but got Unknown
		bool flag = data.TryGetValue("clientConfigKeepEncryption", out var value);
		bool flag2 = value is bool flag3 && !flag3;
		if (!flag || (flag & flag2))
		{
			encryption = (EncryptionParameters)new PasswordParameters
			{
				Password = string.Empty,
				IsHash = true
			};
		}
		return encryption;
	}

	private LoadStatus TryLoadData(Uri resource, out Stream data)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		LoadStatus val = (LoadStatus)2;
		IResourceLoader val2 = resourceLoaders.FirstOrDefault((IResourceLoader l) => l.CanLoad(resource));
		data = null;
		if (val2 != null)
		{
			val = val2.TryLoad(resource, ref data);
			logger.Info($"Tried to load data from '{resource}' using {((object)val2).GetType().Name} -> Result: {val}.");
		}
		else
		{
			logger.Warn($"No resource loader found for '{resource}'!");
		}
		return val;
	}

	private LoadStatus TryParseData(Stream data, out EncryptionParameters encryption, out FormatType format, out IDictionary<string, object> rawData, PasswordParameters password = null)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected I4, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		IDataParser val = dataParsers.FirstOrDefault((IDataParser p) => p.CanParse(data));
		LoadStatus val2 = (LoadStatus)2;
		encryption = null;
		format = (FormatType)0;
		rawData = null;
		if (val != null)
		{
			ParseResult val3 = val.TryParse(data, password);
			encryption = val3.Encryption;
			format = (FormatType)(int)val3.Format;
			rawData = val3.RawData;
			val2 = val3.Status;
			logger.Info($"Tried to parse data from '{data}' using {((object)val).GetType().Name} -> Result: {val2}.");
		}
		else
		{
			logger.Warn($"No data parser found which can parse '{data}'!");
		}
		return val2;
	}

	private SaveStatus TrySaveData(Uri destination, Stream data)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		SaveStatus val = (SaveStatus)1;
		IResourceSaver val2 = resourceSavers.FirstOrDefault((IResourceSaver s) => s.CanSave(destination));
		if (val2 != null)
		{
			val = val2.TrySave(destination, data);
			logger.Info($"Tried to save data as '{destination}' using {((object)val2).GetType().Name} -> Result: {val}.");
		}
		else
		{
			logger.Warn($"No resource saver found for '{destination}'!");
		}
		return val;
	}

	private SaveStatus TrySerializeData(IDictionary<string, object> data, FormatType format, out Stream serialized, EncryptionParameters encryption = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		IDataSerializer val = dataSerializers.FirstOrDefault((IDataSerializer s) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return s.CanSerialize(format);
		});
		SaveStatus val2 = (SaveStatus)1;
		serialized = null;
		if (val != null)
		{
			SerializeResult val3 = val.TrySerialize(data, encryption);
			serialized = val3.Data;
			val2 = val3.Status;
			logger.Info($"Tried to serialize data as '{format}' using {((object)val).GetType().Name} -> Result: {val2}.");
		}
		else
		{
			logger.Error($"No data serializer found which can serialize '{format}'!");
		}
		return val2;
	}
}
