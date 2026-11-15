using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.Runtime.Responsibilities;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Runtime;

internal class RuntimeContext
{
	internal IProcess ClientProcess { get; set; }

	internal IClientProxy ClientProxy { get; set; }

	internal SessionConfiguration Current { get; set; }

	internal SessionConfiguration Next { get; set; }

	internal string ReconfigurationFilePath { get; set; }

	internal string ReconfigurationUrl { get; set; }

	internal IResponsibilityCollection<RuntimeTask> Responsibilities { get; set; }
}
