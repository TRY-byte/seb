using System;
using System.IO;
using System.Windows.Forms;

namespace patch_seb
{
	// Token: 0x02000005 RID: 5
	internal static class Program
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00003C00 File Offset: 0x00001E00
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\System32\\wpeinit.exe"))
			{
				Application.Run(new OfflinePatcher());
				return;
			}
			if (args.Length != 1)
			{
				Application.Run(new Form1());
				return;
			}
			if (args[0] == "/offline" || args[0] == "/Offline")
			{
				Application.Run(new OfflinePatcher());
				return;
			}
			Application.Run(new Form1());
		}
	}
}
