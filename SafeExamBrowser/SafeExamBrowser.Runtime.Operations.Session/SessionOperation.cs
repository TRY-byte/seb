using System.Collections.Generic;
using System.Linq;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Core.Contracts.OperationModel.Events;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Runtime.Communication;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal abstract class SessionOperation : IRepeatableOperation, IOperation
{
	private readonly IMessageBox messageBox;

	protected ClientBridge ClientBridge { get; }

	protected RuntimeContext Context { get; }

	protected ILogger Logger { get; }

	protected IRuntimeWindow RuntimeWindow { get; }

	protected IText Text { get; }

	public abstract event StatusChangedEventHandler StatusChanged;

	internal SessionOperation(Dependencies dependencies)
	{
		ClientBridge = dependencies.ClientBridge;
		Context = dependencies.RuntimeContext;
		Logger = dependencies.Logger;
		messageBox = dependencies.MessageBox;
		RuntimeWindow = dependencies.RuntimeWindow;
		Text = dependencies.Text;
	}

	public abstract OperationResult Perform();

	public abstract OperationResult Repeat();

	public abstract OperationResult Revert();

	protected MessageBoxResult ShowMessageBox(TextKey messageKey, TextKey titleKey, MessageBoxAction action = (MessageBoxAction)0, MessageBoxIcon icon = (MessageBoxIcon)1, IDictionary<string, string> messagePlaceholders = null, IDictionary<string, string> titlePlaceholders = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		string text = Text.Get(messageKey);
		MessageBoxResult val = (MessageBoxResult)0;
		string text2 = Text.Get(titleKey);
		IEnumerable<KeyValuePair<string, string>> enumerable = messagePlaceholders;
		foreach (KeyValuePair<string, string> item in enumerable ?? Enumerable.Empty<KeyValuePair<string, string>>())
		{
			text = text.Replace(item.Key, item.Value);
		}
		enumerable = titlePlaceholders;
		foreach (KeyValuePair<string, string> item2 in enumerable ?? Enumerable.Empty<KeyValuePair<string, string>>())
		{
			text2 = text2.Replace(item2.Key, item2.Value);
		}
		if (ClientBridge.IsRequired())
		{
			return ClientBridge.ShowMessageBox(text, text2, action, icon);
		}
		return messageBox.Show(text, text2, action, icon, (IWindow)(object)RuntimeWindow);
	}
}
