using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;

namespace SafeExamBrowser.Client.Operations;

internal abstract class ClientOperation : IOperation
{
	protected ClientContext Context { get; }

	public abstract event StatusChangedEventHandler StatusChanged;

	public ClientOperation(ClientContext context)
	{
		Context = context;
	}

	public abstract OperationResult Perform();

	public abstract OperationResult Revert();
}
