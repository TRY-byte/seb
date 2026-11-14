using System;
using patch_seb.Properties;

namespace patch_seb
{
	// Token: 0x02000006 RID: 6
	internal class Variables
	{
		// Token: 0x04000018 RID: 24
		public static readonly string SupportedSEB = "3.10.2.920";

		// Token: 0x04000019 RID: 25
		public static readonly string[] files = new string[]
		{
			"SafeExamBrowser.exe",
			"SafeExamBrowser.Client.exe",
			"SafeExamBrowser.Configuration.dll",
			"SafeExamBrowser.Monitoring.dll",
			"SafeExamBrowser.Browser.dll"
		};

		// Token: 0x0400001A RID: 26
		public static readonly byte[][] patchedFiles64 = new byte[][]
		{
			Resources.SafeExamBrowser,
			Resources.SafeExamBrowser_Client,
			Resources.SafeExamBrowser_Configuration,
			Resources.SafeExamBrowser_Monitoring,
			Resources.SafeExamBrowser_Browser
		};

		// Token: 0x0400001B RID: 27
		public static readonly byte[][] patchedFiles32 = new byte[][]
		{
			Resources.SafeExamBrowser1,
			Resources.SafeExamBrowser_Client1,
			Resources.SafeExamBrowser_Configuration1,
			Resources.SafeExamBrowser_Monitoring1,
			Resources.SafeExamBrowser_Browser1
		};

		// Token: 0x0400001C RID: 28
		public static bool isBackup;

		// Token: 0x0400001D RID: 29
		public static bool started = false;

		// Token: 0x0400001E RID: 30
		public static bool alreadyPatched = false;

		// Token: 0x0400001F RID: 31
		public static int something = 0;

		// Token: 0x04000020 RID: 32
		public static bool isCert;

		// Token: 0x04000021 RID: 33
		public static string SEBPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\SafeExamBrowser\\Application\\";

		// Token: 0x04000022 RID: 34
		public static string installation = "";

		// Token: 0x04000023 RID: 35
		public static bool is64bits = false;
	}
}
