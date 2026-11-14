using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SafeExamBrowser.Configuration.ConfigurationData;

internal static class Json
{
	internal static void Serialize(IDictionary<string, object> dictionary, StreamWriter stream)
	{
		List<KeyValuePair<string, object>> list = dictionary.OrderBy((KeyValuePair<string, object> d) => d.Key, StringComparer.InvariantCulture).ToList();
		stream.Write('{');
		foreach (KeyValuePair<string, object> item in list)
		{
			if ((1u & ((!item.Key.Equals("originatorVersion", StringComparison.OrdinalIgnoreCase)) ? 1u : 0u) & ((!(item.Value is IDictionary<string, object> source) || source.Any()) ? 1u : 0u)) != 0)
			{
				stream.Write('"');
				stream.Write(item.Key);
				stream.Write('"');
				stream.Write(':');
				Serialize(item.Value, stream);
				if (item.Key != list.Last().Key)
				{
					stream.Write(',');
				}
			}
		}
		stream.Write('}');
	}

	private static void Serialize(IList<object> list, StreamWriter stream)
	{
		stream.Write('[');
		foreach (object item in list)
		{
			Serialize(item, stream);
			if (item != list.Last())
			{
				stream.Write(',');
			}
		}
		stream.Write(']');
	}

	private static void Serialize(object value, StreamWriter stream)
	{
		if (!(value is IDictionary<string, object> dictionary))
		{
			if (!(value is IList<object> list))
			{
				if (!(value is byte[] inArray))
				{
					if (!(value is DateTime dateTime))
					{
						if (!(value is bool flag))
						{
							if (!(value is int num))
							{
								if (!(value is double num2))
								{
									if (!(value is string value2))
									{
										if (value == null)
										{
											stream.Write('"');
											stream.Write('"');
										}
									}
									else
									{
										stream.Write('"');
										stream.Write(value2);
										stream.Write('"');
									}
								}
								else
								{
									stream.Write(num2.ToString(NumberFormatInfo.InvariantInfo));
								}
							}
							else
							{
								stream.Write(num.ToString(NumberFormatInfo.InvariantInfo));
							}
						}
						else
						{
							stream.Write(flag.ToString().ToLower());
						}
					}
					else
					{
						stream.Write(dateTime.ToString("o"));
					}
				}
				else
				{
					stream.Write('"');
					stream.Write(Convert.ToBase64String(inArray));
					stream.Write('"');
				}
			}
			else
			{
				Serialize(list, stream);
			}
		}
		else
		{
			Serialize(dictionary, stream);
		}
	}
}
