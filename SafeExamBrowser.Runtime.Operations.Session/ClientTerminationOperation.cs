using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class ClientTerminationOperation : ClientOperation
{
	public ClientTerminationOperation(Dependencies dependencies, IProcessFactory processFactory, IProxyFactory proxyFactory, IRuntimeHost runtimeHost, int timeout_ms)
		: base(dependencies, processFactory, proxyFactory, runtimeHost, timeout_ms)
	{
	}

	public override OperationResult Perform()
	{
		return (OperationResult)3;
	}

	public override OperationResult Repeat()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return base.Revert();
	}

	public override OperationResult Revert()
	{
		return (OperationResult)3;
	}
}
