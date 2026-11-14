using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using patch_seb.Properties;

namespace patch_seb
{
	// Token: 0x02000004 RID: 4
	public partial class OfflinePatcher : Form
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002956 File Offset: 0x00000B56
		public OfflinePatcher()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002964 File Offset: 0x00000B64
		public void AddLog(string log)
		{
			TextBox textBox = this.textBox1;
			textBox.Text = textBox.Text + log + Environment.NewLine;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002984 File Offset: 0x00000B84
		private void button1_Click(object sender, EventArgs e)
		{
			Variables.installation = this.partitionComboBox.SelectedItem.ToString();
			this.selectedinstallation.Text = "Selected Installation: " + Variables.installation;
			this.AddLog("[INFO] Selected " + Variables.installation);
			this.groupBox2.Enabled = true;
			this.button2.Enabled = true;
			if (File.Exists(Variables.installation + "Windows\\SysWOW64\\cmd.exe"))
			{
				this.AddLog("[INFO] Detected x64 installation on " + Variables.installation + ".");
				Variables.is64bits = true;
			}
			else
			{
				this.AddLog("[INFO] Detected x86 installation on " + Variables.installation + ".");
			}
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Variables.installation + "Program Files\\SafeExamBrowser\\Application\\SafeExamBrowser.exe");
			FileVersionInfo versionInfo2 = FileVersionInfo.GetVersionInfo(Variables.installation + "Program Files\\SafeExamBrowser\\Application\\SafeExamBrowser.Configuration.dll");
			if (versionInfo.ProductVersion == Variables.SupportedSEB || versionInfo2.ProductVersion == "1.0.0.0")
			{
				Variables.alreadyPatched = true;
				Variables.isBackup = false;
				this.checkBox1.Enabled = false;
				this.button2.Text = "PATCH AGAIN/UPDATE";
				this.AddLog("READY TO UPDATE/PATCH AGAIN " + Variables.installation + ".");
			}
			else
			{
				this.AddLog("READY TO PATCH " + Variables.installation + ".");
			}
			Variables.started = false;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002AF0 File Offset: 0x00000CF0
		private void OfflinePatcher_Load(object sender, EventArgs e)
		{
			this.AddLog(string.Concat(new string[]
			{
				"Safe Exam Browser Offline Patch v",
				Application.ProductVersion,
				" (Safe Exam Browser v",
				Variables.SupportedSEB,
				")"
			}));
			this.AddLog("");
			this.partitionComboBox.Items.Clear();
			this.AddLog("[INFO] Searching for suitable partitions...");
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				if (driveInfo.IsReady && driveInfo.Name != "X:\\" && File.Exists(driveInfo.Name + "Program Files\\SafeExamBrowser\\Application\\SafeExamBrowser.exe"))
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(driveInfo.Name + "Program Files\\SafeExamBrowser\\Application\\SafeExamBrowser.exe");
					FileVersionInfo versionInfo2 = FileVersionInfo.GetVersionInfo(driveInfo.Name + "Program Files\\SafeExamBrowser\\Application\\SafeExamBrowser.Configuration.dll");
					if (versionInfo.FileVersion != Variables.SupportedSEB)
					{
						this.AddLog("[WARNING] Partition " + driveInfo.Name + " contains unsupported Safe Exam Browser version, not adding to the list.");
					}
					else if (driveInfo.Name + "Windows" == Environment.GetFolderPath(Environment.SpecialFolder.Windows) || driveInfo.Name + "windows" == Environment.GetFolderPath(Environment.SpecialFolder.Windows))
					{
						this.AddLog("[WARNING] Partition " + driveInfo.Name + " is an online image, not adding to the list.");
					}
					else if (versionInfo.ProductVersion == Variables.SupportedSEB || versionInfo2.ProductVersion == "1.0.0.0")
					{
						this.AddLog("[WARNING] Partition " + driveInfo.Name + " contains an already patched Safe Exam Browser, adding to the list anyway.");
						string item = driveInfo.Name ?? "";
						this.partitionComboBox.Items.Add(item);
					}
					else
					{
						this.AddLog("[INFO] Partition " + driveInfo.Name + " contains a supported Safe Exam Browser version, adding to the list.");
						string item2 = driveInfo.Name ?? "";
						this.partitionComboBox.Items.Add(item2);
					}
				}
				else
				{
					this.AddLog("[WARNING] Partition " + driveInfo.Name + " doesn't contain an installed Safe Exam Browser version or is not formatted correctly, not adding to the list.");
				}
			}
			if (this.partitionComboBox.Items.Count > 0)
			{
				this.AddLog(string.Format("[INFO] Found {0} partitions suitable for the patch.", this.partitionComboBox.Items.Count));
				this.partitionComboBox.SelectedIndex = 0;
				return;
			}
			this.AddLog("[ERROR] Found 0 partitions suitable for the patch.");
			this.button1.Enabled = false;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002D90 File Offset: 0x00000F90
		private void button2_Click(object sender, EventArgs e)
		{
			Variables.SEBPath = Variables.installation + "Program Files\\SafeExamBrowser\\Application\\";
			if (Variables.alreadyPatched && MessageBox.Show("An already patched Safe Exam Browser has been found. Are you sure to continue?", "Safe Exam Browser Offline Patch", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
			{
				return;
			}
			Variables.isBackup = this.checkBox1.Checked;
			Variables.isCert = this.checkBox2.Checked;
			if (!Variables.started)
			{
				Variables.started = true;
				new Thread(new ThreadStart(this.PatchThread)).Start();
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002E18 File Offset: 0x00001018
		private void PatchThread()
		{
			if (Variables.isBackup)
			{
				try
				{
					for (int i = 0; i < Variables.files.Length; i++)
					{
						Helpers.BackupFile(Variables.SEBPath + Variables.files[i], Variables.SEBPath + Variables.files[i] + ".backup");
					}
				}
				catch (Exception ex)
				{
					this.AddLog("[ERROR] " + ex.Message);
				}
			}
			try
			{
				if (this.autodetect.Checked)
				{
					if (Variables.is64bits)
					{
						for (int j = 0; j < Variables.files.Length; j++)
						{
							Helpers.PatchFile(Variables.SEBPath + Variables.files[j], Variables.patchedFiles64[j]);
						}
					}
					else
					{
						for (int k = 0; k < Variables.files.Length; k++)
						{
							Helpers.PatchFile(Variables.SEBPath + Variables.files[k], Variables.patchedFiles32[k]);
						}
					}
				}
				else if (this.x64.Checked)
				{
					for (int l = 0; l < Variables.files.Length; l++)
					{
						Helpers.PatchFile(Variables.SEBPath + Variables.files[l], Variables.patchedFiles64[l]);
					}
				}
				else
				{
					for (int m = 0; m < Variables.files.Length; m++)
					{
						Helpers.PatchFile(Variables.SEBPath + Variables.files[m], Variables.patchedFiles32[m]);
					}
				}
				if (Variables.isCert)
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\System32\\reg.exe",
						Arguments = "load HKLM\\OFFSOFTWARE " + Variables.installation + "Windows\\System32\\config\\software",
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden
					}).WaitForExit();
					File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Temp\\SEB.reg", Resources.cert_offline);
					Process.Start(new ProcessStartInfo
					{
						FileName = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\regedit.exe",
						Arguments = "/s " + Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Temp\\SEB.reg",
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden
					}).WaitForExit();
					File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Temp\\SEB.reg");
					Process.Start(new ProcessStartInfo
					{
						FileName = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\System32\\reg.exe",
						Arguments = "unload HKLM\\OFFSOFTWARE",
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden
					}).WaitForExit();
				}
				this.AddLog("PATCHING DONE ON " + Variables.installation + ".");
				this.button2.Text = "PATCH DONE";
			}
			catch (Exception ex2)
			{
				this.AddLog("[ERROR] " + ex2.Message);
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000310C File Offset: 0x0000130C
		private void label1_Click(object sender, EventArgs e)
		{
			if (Variables.something == 4)
			{
				Variables.something = 0;
				MessageBox.Show(string.Concat(new string[]
				{
					"Safe Exam Browser Offline Patch v",
					Application.ProductVersion,
					"\nFor Safe Exam Browser version ",
					Variables.SupportedSEB,
					"\nCreated with love by Vichingo455\n\nBecause Freedom is a right, respect it."
				}), "Safe Exam Browser Patch", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			Variables.something++;
		}
	}
}
