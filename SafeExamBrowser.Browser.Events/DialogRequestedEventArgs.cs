using System.Runtime.CompilerServices;
using SafeExamBrowser.UserInterface.Contracts.FileSystemDialog;

namespace SafeExamBrowser.Browser.Events;

internal class DialogRequestedEventArgs
{
	internal FileSystemElement Element
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	internal string InitialPath { get; set; }

	internal FileSystemOperation Operation
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	internal string FullPath { get; set; }

	internal bool Success { get; set; }

	internal string Title { get; set; }
}
