using System.Collections.Generic;
using SafeExamBrowser.Configuration.ConfigurationData.DataMapping;
using SafeExamBrowser.Settings;

namespace SafeExamBrowser.Configuration.ConfigurationData;

internal class DataMapper
{
	private readonly BaseDataMapper[] mappers = new BaseDataMapper[13]
	{
		new ApplicationDataMapper(),
		new AudioDataMapper(),
		new BrowserDataMapper(),
		new ConfigurationFileDataMapper(),
		new DisplayDataMapper(),
		new GeneralDataMapper(),
		new InputDataMapper(),
		new ProctoringDataMapper(),
		new SecurityDataMapper(),
		new ServerDataMapper(),
		new ServiceDataMapper(),
		new SystemDataMapper(),
		new UserInterfaceDataMapper()
	};

	internal void Map(IDictionary<string, object> rawData, AppSettings settings)
	{
		BaseDataMapper[] array;
		foreach (KeyValuePair<string, object> rawDatum in rawData)
		{
			array = mappers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Map(rawDatum.Key, rawDatum.Value, settings);
			}
		}
		array = mappers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MapGlobal(rawData, settings);
		}
	}
}
