using CefSharp;
using SafeExamBrowser.UserInterface.Contracts.FileSystemDialog;

namespace SafeExamBrowser.Browser.Wrapper;

internal static class Extensions
{
	internal static FileSystemElement ToElement(this CefFileDialogMode mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)mode != 2)
		{
			return (FileSystemElement)0;
		}
		return (FileSystemElement)1;
	}

	internal static FileSystemOperation ToOperation(this CefFileDialogMode mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)mode != 3)
		{
			return (FileSystemOperation)0;
		}
		return (FileSystemOperation)1;
	}
}
