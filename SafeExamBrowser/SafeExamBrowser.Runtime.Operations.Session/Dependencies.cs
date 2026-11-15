using System;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Runtime.Communication;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Runtime.Operations.Session;

internal class Dependencies
{
	public ClientBridge ClientBridge { get; }

	internal ILogger Logger { get; }

	internal IMessageBox MessageBox { get; }

	internal RuntimeContext RuntimeContext { get; }

	internal IRuntimeWindow RuntimeWindow { get; }

	internal IText Text { get; }

	internal Dependencies(ClientBridge clientBridge, ILogger logger, IMessageBox messageBox, IRuntimeWindow runtimeWindow, RuntimeContext runtimeContext, IText text)
	{
		ClientBridge = clientBridge ?? throw new ArgumentNullException("clientBridge");
		Logger = logger ?? throw new ArgumentNullException("logger");
		MessageBox = messageBox ?? throw new ArgumentNullException("messageBox");
		RuntimeWindow = runtimeWindow ?? throw new ArgumentNullException("runtimeWindow");
		RuntimeContext = runtimeContext ?? throw new ArgumentNullException("runtimeContext");
		Text = text ?? throw new ArgumentNullException("text");
	}
}
