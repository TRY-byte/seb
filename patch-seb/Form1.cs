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
	// Token: 0x02000002 RID: 2
	public partial class Form1 : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Form1()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205E File Offset: 0x0000025E
		public void AddLog(string log)
		{
			TextBox textBox = this.textBox1;
			textBox.Text = textBox.Text + log + Environment.NewLine;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000207C File Offset: 0x0000027C
		private void Form1_Load(object sender, EventArgs e)
		{
			this.AddLog(string.Concat(new string[]
			{
				"Safe Exam Browser Patch v",
				Application.ProductVersion,
				" (Safe Exam Browser v",
				Variables.SupportedSEB,
				")"
			}));
			this.AddLog("");
			if (Environment.Is64BitOperatingSystem)
			{
				this.AddLog("[INFO] Detected x64 operating system.");
			}
			else
			{
				this.AddLog("[INFO] Detected x86 operating system.");
			}
			if (Environment.OSVersion.Version.Major != 10)
			{
				this.AddLog("[ERROR] Supported Windows version not found.");
				this.button1.Enabled = false;
				return;
			}
			if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\SafeExamBrowser\\Application\\SafeExamBrowser.exe"))
			{
				this.AddLog("[ERROR] Safe Exam Browser not found.");
				this.button1.Enabled = false;
				return;
			}
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\SafeExamBrowser\\Application\\SafeExamBrowser.exe");
			FileVersionInfo versionInfo2 = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\SafeExamBrowser\\Application\\SafeExamBrowser.Configuration.dll");
			if (versionInfo.FileVersion != Variables.SupportedSEB)
			{
				this.AddLog("[ERROR] Found unsupported Safe Exam Browser version.");
				this.button1.Enabled = false;
				return;
			}
			if (versionInfo.ProductVersion == Variables.SupportedSEB || versionInfo2.ProductVersion == "1.0.0.0")
			{
				this.checkBox1.Checked = false;
				this.checkBox1.Enabled = false;
				Variables.isBackup = false;
				Variables.alreadyPatched = true;
				this.button1.Text = "PATCH AGAIN/UPDATE";
				this.AddLog("[WARNING] Found an already patched Safe Exam Browser.");
				this.AddLog("READY TO UPDATE PATCH");
				return;
			}
			this.AddLog("[INFO] Supported Safe Exam Browser version found.");
			this.AddLog("READY TO PATCH");
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002224 File Offset: 0x00000424
		private void button1_Click(object sender, EventArgs e)
		{
			if (Variables.alreadyPatched && MessageBox.Show("An already patched Safe Exam Browser has been found. Are you sure to continue?", "Safe Exam Browser Patch", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
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

		// Token: 0x06000005 RID: 5 RVA: 0x00002298 File Offset: 0x00000498
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
				if (Environment.Is64BitOperatingSystem)
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
				if (Variables.isCert)
				{
					File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Temp\\SEB.reg", Resources.cert);
					Process.Start(new ProcessStartInfo
					{
						FileName = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\regedit.exe",
						Arguments = "/s " + Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Temp\\SEB.reg",
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden
					}).WaitForExit();
					File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Temp\\SEB.reg");
				}
				this.AddLog("PATCHING DONE");
				this.button1.Text = "PATCH DONE";
			}
			catch (Exception ex2)
			{
				this.AddLog("[ERROR] " + ex2.Message);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002464 File Offset: 0x00000664
		private void label1_Click(object sender, EventArgs e)
		{
			if (Variables.something == 4)
			{
				Variables.something = 0;
				MessageBox.Show(string.Concat(new string[]
				{
					"Safe Exam Browser Patch v",
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
