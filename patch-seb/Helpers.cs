using System;
using System.IO;

namespace patch_seb
{
	// Token: 0x02000003 RID: 3
	internal class Helpers
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002927 File Offset: 0x00000B27
		public static void PatchFile(string filePath, byte[] patchedFile)
		{
			File.Delete(filePath);
			File.WriteAllBytes(filePath, patchedFile);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002936 File Offset: 0x00000B36
		public static void BackupFile(string filePath, string backupPath)
		{
			if (!File.Exists(backupPath))
			{
				File.Delete(backupPath);
			}
			File.Copy(filePath, backupPath, true);
		}
	}
}
