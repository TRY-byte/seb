using System.Collections.Generic;
using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData.DataMapping;

internal abstract class BaseDataMapper
{
	internal abstract void Map(string key, object value, AppSettings settings);

	internal virtual void MapGlobal(IDictionary<string, object> rawData, AppSettings settings)
	{
	}
}
