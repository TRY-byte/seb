using System;
using System.Collections.Generic;
using SafeExamBrowser.Applications.Contracts;
using SafeExamBrowser.Browser.Contracts;
using SafeExamBrowser.Client.Responsibilities;
using SafeExamBrowser.Communication.Contracts.Hosts;
using SafeExamBrowser.Communication.Contracts.Proxies;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.Configuration.Contracts.Cryptography;
using SafeExamBrowser.Configuration.Contracts.Integrity;
using SafeExamBrowser.Core.Contracts.ResponsibilityModel;
using SafeExamBrowser.Proctoring.Contracts;
using SafeExamBrowser.Server.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.MessageBox;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client;

internal class ClientContext
{
	internal IList<IActivator> Activators { get; }

	internal IList<IApplication<IApplicationWindow>> Applications { get; }

	internal AppConfig AppConfig { get; set; }

	internal IBrowserApplication Browser { get; set; }

	internal IClientHost ClientHost { get; set; }

	internal IHashAlgorithm HashAlgorithm { get; set; }

	internal IIntegrityModule IntegrityModule { get; set; }

	internal ILockScreen LockScreen { get; set; }

	internal IMessageBox MessageBox { get; set; }

	internal IProctoringController Proctoring { get; set; }

	internal bool QuitPasswordValidated { get; set; }

	internal IResponsibilityCollection<ClientTask> Responsibilities { get; set; }

	internal IRuntimeProxy Runtime { get; set; }

	internal IServerProxy Server { get; set; }

	internal Guid SessionId { get; set; }

	internal AppSettings Settings { get; set; }

	internal IUserInterfaceFactory UserInterfaceFactory { get; set; }

	internal ClientContext()
	{
		Activators = new List<IActivator>();
		Applications = new List<IApplication<IApplicationWindow>>();
	}
}
