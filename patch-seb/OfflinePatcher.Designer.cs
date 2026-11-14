namespace patch_seb
{
	// Token: 0x02000004 RID: 4
	public partial class OfflinePatcher : global::System.Windows.Forms.Form
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00003175 File Offset: 0x00001375
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00003194 File Offset: 0x00001394
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::patch_seb.OfflinePatcher));
			this.label1 = new global::System.Windows.Forms.Label();
			this.groupBox1 = new global::System.Windows.Forms.GroupBox();
			this.button1 = new global::System.Windows.Forms.Button();
			this.label2 = new global::System.Windows.Forms.Label();
			this.partitionComboBox = new global::System.Windows.Forms.ComboBox();
			this.groupBox2 = new global::System.Windows.Forms.GroupBox();
			this.checkBox2 = new global::System.Windows.Forms.CheckBox();
			this.checkBox1 = new global::System.Windows.Forms.CheckBox();
			this.x86 = new global::System.Windows.Forms.RadioButton();
			this.x64 = new global::System.Windows.Forms.RadioButton();
			this.autodetect = new global::System.Windows.Forms.RadioButton();
			this.selectedinstallation = new global::System.Windows.Forms.Label();
			this.label3 = new global::System.Windows.Forms.Label();
			this.button2 = new global::System.Windows.Forms.Button();
			this.groupBox3 = new global::System.Windows.Forms.GroupBox();
			this.textBox1 = new global::System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			base.SuspendLayout();
			this.label1.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.label1.AutoSize = true;
			this.label1.Font = new global::System.Drawing.Font("Segoe UI", 10f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 0);
			this.label1.Location = new global::System.Drawing.Point(215, 9);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(273, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Safe Exam Browser Offline Patch";
			this.label1.Click += new global::System.EventHandler(this.label1_Click);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.partitionComboBox);
			this.groupBox1.Location = new global::System.Drawing.Point(12, 54);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new global::System.Drawing.Size(716, 100);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Safe Exam Browser installations";
			this.button1.Location = new global::System.Drawing.Point(583, 44);
			this.button1.Name = "button1";
			this.button1.Size = new global::System.Drawing.Size(116, 29);
			this.button1.TabIndex = 2;
			this.button1.Text = "USE";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new global::System.EventHandler(this.button1_Click);
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(6, 47);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(134, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "Installation to patch:";
			this.partitionComboBox.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.partitionComboBox.FormattingEnabled = true;
			this.partitionComboBox.Location = new global::System.Drawing.Point(178, 44);
			this.partitionComboBox.Name = "partitionComboBox";
			this.partitionComboBox.Size = new global::System.Drawing.Size(381, 25);
			this.partitionComboBox.TabIndex = 0;
			this.groupBox2.Controls.Add(this.checkBox2);
			this.groupBox2.Controls.Add(this.checkBox1);
			this.groupBox2.Controls.Add(this.x86);
			this.groupBox2.Controls.Add(this.x64);
			this.groupBox2.Controls.Add(this.autodetect);
			this.groupBox2.Controls.Add(this.selectedinstallation);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Enabled = false;
			this.groupBox2.Location = new global::System.Drawing.Point(12, 171);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new global::System.Drawing.Size(716, 158);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Patch settings";
			this.checkBox2.AutoSize = true;
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.checkBox2.Location = new global::System.Drawing.Point(26, 111);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new global::System.Drawing.Size(410, 23);
			this.checkBox2.TabIndex = 6;
			this.checkBox2.Text = "Install custom certificate on the offline system (recommended)";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new global::System.Drawing.Point(26, 82);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new global::System.Drawing.Size(75, 23);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Text = "Backup";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.x86.AutoSize = true;
			this.x86.Location = new global::System.Drawing.Point(607, 50);
			this.x86.Name = "x86";
			this.x86.Size = new global::System.Drawing.Size(52, 23);
			this.x86.TabIndex = 4;
			this.x86.Text = "x86";
			this.x86.UseVisualStyleBackColor = true;
			this.x64.AutoSize = true;
			this.x64.Location = new global::System.Drawing.Point(520, 50);
			this.x64.Name = "x64";
			this.x64.Size = new global::System.Drawing.Size(52, 23);
			this.x64.TabIndex = 3;
			this.x64.Text = "x64";
			this.x64.UseVisualStyleBackColor = true;
			this.autodetect.AutoSize = true;
			this.autodetect.Checked = true;
			this.autodetect.Location = new global::System.Drawing.Point(260, 50);
			this.autodetect.Name = "autodetect";
			this.autodetect.Size = new global::System.Drawing.Size(206, 23);
			this.autodetect.TabIndex = 2;
			this.autodetect.TabStop = true;
			this.autodetect.Text = "Auto-Detect (recommended)";
			this.autodetect.UseVisualStyleBackColor = true;
			this.selectedinstallation.AutoSize = true;
			this.selectedinstallation.Location = new global::System.Drawing.Point(21, 25);
			this.selectedinstallation.Name = "selectedinstallation";
			this.selectedinstallation.Size = new global::System.Drawing.Size(168, 19);
			this.selectedinstallation.TabIndex = 1;
			this.selectedinstallation.Text = "Selected Installation: none";
			this.label3.AutoSize = true;
			this.label3.Location = new global::System.Drawing.Point(21, 50);
			this.label3.Name = "label3";
			this.label3.Size = new global::System.Drawing.Size(204, 19);
			this.label3.TabIndex = 0;
			this.label3.Text = "Safe Exam Browser Architecture:";
			this.button2.Enabled = false;
			this.button2.Location = new global::System.Drawing.Point(235, 608);
			this.button2.Name = "button2";
			this.button2.Size = new global::System.Drawing.Size(270, 62);
			this.button2.TabIndex = 3;
			this.button2.Text = "PATCH";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new global::System.EventHandler(this.button2_Click);
			this.groupBox3.Controls.Add(this.textBox1);
			this.groupBox3.Location = new global::System.Drawing.Point(12, 345);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new global::System.Drawing.Size(716, 245);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Patch logs";
			this.textBox1.Location = new global::System.Drawing.Point(3, 25);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = global::System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new global::System.Drawing.Size(707, 204);
			this.textBox1.TabIndex = 0;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(7f, 17f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(742, 691);
			base.Controls.Add(this.groupBox3);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.label1);
			this.Font = new global::System.Drawing.Font("Segoe UI", 8f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.Name = "OfflinePatcher";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Safe Exam Browser Offline Patcher";
			base.Load += new global::System.EventHandler(this.OfflinePatcher_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000007 RID: 7
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000008 RID: 8
		private global::System.Windows.Forms.Label label1;

		// Token: 0x04000009 RID: 9
		private global::System.Windows.Forms.GroupBox groupBox1;

		// Token: 0x0400000A RID: 10
		private global::System.Windows.Forms.Button button1;

		// Token: 0x0400000B RID: 11
		private global::System.Windows.Forms.Label label2;

		// Token: 0x0400000C RID: 12
		private global::System.Windows.Forms.ComboBox partitionComboBox;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.GroupBox groupBox2;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.RadioButton x86;

		// Token: 0x0400000F RID: 15
		private global::System.Windows.Forms.RadioButton x64;

		// Token: 0x04000010 RID: 16
		private global::System.Windows.Forms.RadioButton autodetect;

		// Token: 0x04000011 RID: 17
		private global::System.Windows.Forms.Label selectedinstallation;

		// Token: 0x04000012 RID: 18
		private global::System.Windows.Forms.Label label3;

		// Token: 0x04000013 RID: 19
		private global::System.Windows.Forms.CheckBox checkBox2;

		// Token: 0x04000014 RID: 20
		private global::System.Windows.Forms.CheckBox checkBox1;

		// Token: 0x04000015 RID: 21
		private global::System.Windows.Forms.Button button2;

		// Token: 0x04000016 RID: 22
		private global::System.Windows.Forms.GroupBox groupBox3;

		// Token: 0x04000017 RID: 23
		private global::System.Windows.Forms.TextBox textBox1;
	}
}
