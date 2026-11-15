using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.DataFormats;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataFormats;

public class XmlSerializer : IDataSerializer
{
	private readonly ILogger logger;

	public XmlSerializer(ILogger logger)
	{
		this.logger = logger;
	}

	public bool CanSerialize(FormatType format)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		return (int)format == 2;
	}

	public SerializeResult TrySerialize(IDictionary<string, object> data, EncryptionParameters encryption = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		SerializeResult val = new SerializeResult();
		XmlWriterSettings settings = new XmlWriterSettings
		{
			Encoding = new UTF8Encoding(),
			Indent = true
		};
		MemoryStream memoryStream = new MemoryStream();
		logger.Debug("Starting to serialize data...");
		using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
		{
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteDocType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "https://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
			xmlWriter.WriteStartElement("plist");
			xmlWriter.WriteAttributeString("version", "1.0");
			val.Status = WriteDictionary(xmlWriter, data);
			val.Data = memoryStream;
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			xmlWriter.Close();
		}
		logger.Debug($"Finished serialization -> Result: {val.Status}.");
		return val;
	}

	private SaveStatus WriteArray(XmlWriter writer, List<object> array)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		SaveStatus val = (SaveStatus)2;
		writer.WriteStartElement("array");
		foreach (object item in array)
		{
			val = WriteElement(writer, item);
			if ((int)val != 2)
			{
				break;
			}
		}
		writer.WriteEndElement();
		return val;
	}

	private SaveStatus WriteDictionary(XmlWriter writer, IDictionary<string, object> data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		SaveStatus val = (SaveStatus)2;
		writer.WriteStartElement("dict");
		foreach (KeyValuePair<string, object> item in data.OrderBy((KeyValuePair<string, object> i) => i.Key))
		{
			val = WriteKeyValuePair(writer, item);
			if ((int)val != 2)
			{
				break;
			}
		}
		writer.WriteEndElement();
		return val;
	}

	private SaveStatus WriteKeyValuePair(XmlWriter writer, KeyValuePair<string, object> item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		SaveStatus result = (SaveStatus)0;
		if (item.Key != null)
		{
			writer.WriteElementString("key", item.Key);
			result = WriteElement(writer, item.Value);
		}
		else
		{
			logger.Error($"Key of item '{item}' is null!");
		}
		return result;
	}

	private SaveStatus WriteElement(XmlWriter writer, object element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		SaveStatus val = (SaveStatus)0;
		if (element is List<object> array)
		{
			return WriteArray(writer, array);
		}
		if (element is Dictionary<string, object> data)
		{
			return WriteDictionary(writer, data);
		}
		return WriteSimpleType(writer, element);
	}

	private SaveStatus WriteSimpleType(XmlWriter writer, object element)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Invalid comparison between Unknown and I4
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		string localName = null;
		string value = null;
		SaveStatus val = (SaveStatus)2;
		if (!(element is byte[] inArray))
		{
			if (!(element is DateTime dateTime))
			{
				if (!(element is bool flag))
				{
					if (!(element is int num))
					{
						if (!(element is double num2))
						{
							if (!(element is string text))
							{
								if (element != null)
								{
									goto IL_00ec;
								}
								localName = "string";
							}
							else
							{
								localName = "string";
								value = text;
							}
						}
						else
						{
							localName = "real";
							value = num2.ToString(NumberFormatInfo.InvariantInfo);
						}
					}
					else
					{
						localName = "integer";
						value = num.ToString(NumberFormatInfo.InvariantInfo);
					}
				}
				else if (!flag)
				{
					localName = "false";
				}
				else
				{
					if (!flag)
					{
						goto IL_00ec;
					}
					localName = "true";
				}
			}
			else
			{
				localName = "date";
				value = XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc);
			}
		}
		else
		{
			localName = "data";
			value = Convert.ToBase64String(inArray);
		}
		goto IL_00ee;
		IL_00ec:
		val = (SaveStatus)0;
		goto IL_00ee;
		IL_00ee:
		if ((int)val == 2)
		{
			writer.WriteElementString(localName, value);
		}
		else
		{
			logger.Error($"Element '{element}' is not supported!");
		}
		return val;
	}
}
