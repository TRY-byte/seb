using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.DataCompression;
using SafeExamBrowser.Configuration.Contracts.DataFormats;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataFormats;

public class XmlParser : IDataParser
{
	private const string XML_PREFIX = "<?xm";

	private readonly IDataCompressor compressor;

	private readonly ILogger logger;

	public XmlParser(IDataCompressor compressor, ILogger logger)
	{
		this.compressor = compressor;
		this.logger = logger;
	}

	public bool CanParse(Stream data)
	{
		try
		{
			if (data.Length > "<?xm".Length)
			{
				string text = ReadPrefix(data);
				bool flag = "<?xm".Equals(text, StringComparison.OrdinalIgnoreCase);
				logger.Debug(string.Format("'{0}' starting with '{1}' {2} the {3} format.", data, text, flag ? "matches" : "does not match", (object)(FormatType)2));
				return flag;
			}
			logger.Debug($"'{data}' is not long enough ({data.Length} bytes) to match the {(object)(FormatType)2} format.");
		}
		catch (Exception ex)
		{
			logger.Error($"Failed to determine whether '{data}' with {(double?)data?.Length / 1000.0} KB data matches the {(object)(FormatType)2} format!", ex);
		}
		return false;
	}

	public ParseResult TryParse(Stream data, PasswordParameters password = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		string text = ReadPrefix(data);
		bool flag = "<?xm".Equals(text, StringComparison.OrdinalIgnoreCase);
		ParseResult val = new ParseResult
		{
			Status = (LoadStatus)0
		};
		if (flag)
		{
			data = (compressor.IsCompressed(data) ? compressor.Decompress(data) : data);
			data.Seek(0L, SeekOrigin.Begin);
			using (XmlReader xmlReader = XmlReader.Create(data, new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Ignore
			}))
			{
				bool flag2 = xmlReader.ReadToFollowing("plist");
				bool flag3 = xmlReader.ReadToDescendant("dict");
				if (flag2 & flag3)
				{
					logger.Debug("Found root node, starting to parse data...");
					val.Status = ParseDictionary(xmlReader, out var dictionary);
					val.RawData = dictionary;
					logger.Debug($"Finished parsing -> Result: {val.Status}.");
				}
				else
				{
					logger.Error("Could not find root " + ((!flag2) ? "node 'plist'" : "dictionary 'dict'") + "!");
				}
			}
			val.Format = (FormatType)2;
		}
		else
		{
			logger.Error($"'{data}' starting with '{text}' does not match the {(object)(FormatType)2} format!");
		}
		return val;
	}

	private LoadStatus ParseArray(XmlReader reader, out List<object> array)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		array = new List<object>();
		if (reader.IsEmptyElement)
		{
			reader.Read();
			reader.MoveToContent();
			return (LoadStatus)4;
		}
		reader.Read();
		reader.MoveToContent();
		while (reader.NodeType == XmlNodeType.Element)
		{
			LoadStatus val = ParseElement(reader, out var element);
			if ((int)val == 4)
			{
				array.Add(element);
				reader.MoveToContent();
				continue;
			}
			return val;
		}
		if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "array")
		{
			reader.Read();
			reader.MoveToContent();
			return (LoadStatus)4;
		}
		logger.Error("Expected closing tag for 'array', but found '" + reader.Name + reader.Value + "'!");
		return (LoadStatus)0;
	}

	private LoadStatus ParseDictionary(XmlReader reader, out Dictionary<string, object> dictionary)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		dictionary = new Dictionary<string, object>();
		if (reader.IsEmptyElement)
		{
			reader.Read();
			reader.MoveToContent();
			return (LoadStatus)4;
		}
		reader.Read();
		reader.MoveToContent();
		while (reader.NodeType == XmlNodeType.Element)
		{
			LoadStatus val = ParseKeyValuePair(reader, out var pair);
			if ((int)val == 4)
			{
				dictionary[pair.Key] = pair.Value;
				reader.MoveToContent();
				continue;
			}
			return val;
		}
		if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "dict")
		{
			reader.Read();
			reader.MoveToContent();
			return (LoadStatus)4;
		}
		logger.Error("Expected closing tag for 'dict', but found '" + reader.Name + reader.Value + "'!");
		return (LoadStatus)0;
	}

	private LoadStatus ParseKeyValuePair(XmlReader reader, out KeyValuePair<string, object> pair)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		LoadStatus val = (LoadStatus)0;
		XElement xElement = XNode.ReadFrom(reader) as XElement;
		pair = default;
		if (xElement.Name.LocalName == "key")
		{
			reader.MoveToContent();
			val = ParseElement(reader, out var element);
			if ((int)val == 4)
			{
				pair = new KeyValuePair<string, object>(xElement.Value, element);
			}
		}
		else
		{
			logger.Error(string.Format("Expected element '{0}', but found '{1}'!", "key", xElement));
		}
		return val;
	}

	private LoadStatus ParseElement(XmlReader reader, out object element)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		List<object> array = null;
		Dictionary<string, object> dictionary = null;
		LoadStatus val = (LoadStatus)0;
		object value = null;
		if (reader.Name == "array")
		{
			val = ParseArray(reader, out array);
		}
		else
		{
			val = ((!(reader.Name == "dict")) ? ParseSimpleType(XNode.ReadFrom(reader) as XElement, out value) : ParseDictionary(reader, out dictionary));
		}
		element = array ?? dictionary ?? value;
		return val;
	}

	private LoadStatus ParseSimpleType(XElement element, out object value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Invalid comparison between Unknown and I4
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		LoadStatus val = (LoadStatus)4;
		value = null;
		switch (element.Name.LocalName)
		{
		case "data":
			value = Convert.FromBase64String(element.Value);
			break;
		case "date":
			value = XmlConvert.ToDateTime(element.Value, XmlDateTimeSerializationMode.Utc);
			break;
		case "false":
			value = false;
			break;
		case "integer":
			value = Convert.ToInt32(element.Value);
			break;
		case "real":
			value = Convert.ToDouble(element.Value, CultureInfo.InvariantCulture);
			break;
		case "string":
			value = (element.IsEmpty ? null : element.Value);
			break;
		case "true":
			value = true;
			break;
		default:
			val = (LoadStatus)0;
			break;
		}
		if ((int)val != 4)
		{
			logger.Error($"Element '{element}' is not a supported value type!");
		}
		return val;
	}

	private string ReadPrefix(Stream data)
	{
		byte[] array = new byte["<?xm".Length];
		if (compressor.IsCompressed(data))
		{
			array = compressor.Peek(data, "<?xm".Length);
		}
		else
		{
			data.Seek(0L, SeekOrigin.Begin);
			data.Read(array, 0, "<?xm".Length);
		}
		return Encoding.UTF8.GetString(array);
	}
}
