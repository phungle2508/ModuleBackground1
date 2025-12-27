using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ModuleBackground;

public class FormSettingOutSurfaceCut : Form
{
	public string m_sKey = "1";

	public string m_sKey1 = "1";

	public string m_sKey2 = "1";

	public bool m_bEnable;

	private IContainer components;

	private GroupBox groupBox1;

	private RadioButton radioButton8;

	private RadioButton radioButton7;

	private RadioButton radioButton6;

	private RadioButton radioButton5;

	private RadioButton radioButton4;

	private RadioButton radioButton3;

	private RadioButton radioButton2;

	private RadioButton radioButton1;

	private GroupBox groupBox3;

	private GroupBox groupBox2;

	private Button button2;

	private Button button1;

	public FormSettingOutSurfaceCut()
	{
		InitializeComponent();
	}

	private void FormSettingOutSurfaceCut_Load(object sender, EventArgs e)
	{
		if (m_sKey == "1")
		{
			radioButton1.Checked = true;
		}
		if (m_sKey == "2")
		{
			radioButton2.Checked = true;
		}
		if (m_sKey == "3")
		{
			radioButton3.Checked = true;
		}
		if (m_sKey == "4")
		{
			radioButton4.Checked = true;
		}
		if (m_sKey1 == "1")
		{
			radioButton5.Checked = true;
		}
		if (m_sKey1 == "2")
		{
			radioButton6.Checked = true;
		}
		if (m_sKey2 == "1")
		{
			radioButton7.Checked = true;
		}
		if (m_sKey2 == "2")
		{
			radioButton8.Checked = true;
		}
		groupBox2.Enabled = m_bEnable;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		m_sKey = "1";
		if (radioButton2.Checked)
		{
			m_sKey = "2";
		}
		if (radioButton3.Checked)
		{
			m_sKey = "3";
		}
		if (radioButton4.Checked)
		{
			m_sKey = "4";
		}
		m_sKey1 = "1";
		if (radioButton6.Checked)
		{
			m_sKey1 = "2";
		}
		m_sKey2 = "1";
		if (radioButton8.Checked)
		{
			m_sKey2 = "2";
		}
		Close();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.radioButton4 = new System.Windows.Forms.RadioButton();
		this.radioButton3 = new System.Windows.Forms.RadioButton();
		this.radioButton2 = new System.Windows.Forms.RadioButton();
		this.radioButton1 = new System.Windows.Forms.RadioButton();
		this.radioButton5 = new System.Windows.Forms.RadioButton();
		this.radioButton6 = new System.Windows.Forms.RadioButton();
		this.radioButton7 = new System.Windows.Forms.RadioButton();
		this.radioButton8 = new System.Windows.Forms.RadioButton();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.button2 = new System.Windows.Forms.Button();
		this.button1 = new System.Windows.Forms.Button();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox3.SuspendLayout();
		base.SuspendLayout();
		this.groupBox1.Controls.Add(this.radioButton4);
		this.groupBox1.Controls.Add(this.radioButton3);
		this.groupBox1.Controls.Add(this.radioButton2);
		this.groupBox1.Controls.Add(this.radioButton1);
		this.groupBox1.Location = new System.Drawing.Point(12, 13);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(380, 60);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "地域";
		this.radioButton4.AutoSize = true;
		this.radioButton4.Location = new System.Drawing.Point(280, 26);
		this.radioButton4.Name = "radioButton4";
		this.radioButton4.Size = new System.Drawing.Size(79, 17);
		this.radioButton4.TabIndex = 4;
		this.radioButton4.Text = "5・6・7地域";
		this.radioButton4.UseVisualStyleBackColor = true;
		this.radioButton3.AutoSize = true;
		this.radioButton3.Location = new System.Drawing.Point(195, 26);
		this.radioButton3.Name = "radioButton3";
		this.radioButton3.Size = new System.Drawing.Size(55, 17);
		this.radioButton3.TabIndex = 3;
		this.radioButton3.Text = "4地域";
		this.radioButton3.UseVisualStyleBackColor = true;
		this.radioButton2.AutoSize = true;
		this.radioButton2.Location = new System.Drawing.Point(110, 26);
		this.radioButton2.Name = "radioButton2";
		this.radioButton2.Size = new System.Drawing.Size(55, 17);
		this.radioButton2.TabIndex = 2;
		this.radioButton2.Text = "3地域";
		this.radioButton2.UseVisualStyleBackColor = true;
		this.radioButton1.AutoSize = true;
		this.radioButton1.Checked = true;
		this.radioButton1.Location = new System.Drawing.Point(24, 26);
		this.radioButton1.Name = "radioButton1";
		this.radioButton1.Size = new System.Drawing.Size(67, 17);
		this.radioButton1.TabIndex = 1;
		this.radioButton1.TabStop = true;
		this.radioButton1.Text = "1・2地域";
		this.radioButton1.UseVisualStyleBackColor = true;
		this.radioButton5.AutoSize = true;
		this.radioButton5.Checked = true;
		this.radioButton5.Location = new System.Drawing.Point(24, 20);
		this.radioButton5.Name = "radioButton5";
		this.radioButton5.Size = new System.Drawing.Size(37, 17);
		this.radioButton5.TabIndex = 9;
		this.radioButton5.TabStop = true;
		this.radioButton5.Text = "尺";
		this.radioButton5.UseVisualStyleBackColor = true;
		this.radioButton6.AutoSize = true;
		this.radioButton6.Location = new System.Drawing.Point(110, 20);
		this.radioButton6.Name = "radioButton6";
		this.radioButton6.Size = new System.Drawing.Size(34, 17);
		this.radioButton6.TabIndex = 10;
		this.radioButton6.Text = "M";
		this.radioButton6.UseVisualStyleBackColor = true;
		this.radioButton7.AutoSize = true;
		this.radioButton7.Checked = true;
		this.radioButton7.Location = new System.Drawing.Point(24, 23);
		this.radioButton7.Name = "radioButton7";
		this.radioButton7.Size = new System.Drawing.Size(68, 17);
		this.radioButton7.TabIndex = 6;
		this.radioButton7.TabStop = true;
		this.radioButton7.Text = "スラブＳ１";
		this.radioButton7.UseVisualStyleBackColor = true;
		this.radioButton8.AutoSize = true;
		this.radioButton8.Location = new System.Drawing.Point(110, 23);
		this.radioButton8.Name = "radioButton8";
		this.radioButton8.Size = new System.Drawing.Size(68, 17);
		this.radioButton8.TabIndex = 7;
		this.radioButton8.Text = "スラブＳ２";
		this.radioButton8.UseVisualStyleBackColor = true;
		this.groupBox2.Controls.Add(this.radioButton5);
		this.groupBox2.Controls.Add(this.radioButton6);
		this.groupBox2.Enabled = false;
		this.groupBox2.Location = new System.Drawing.Point(12, 139);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(200, 49);
		this.groupBox2.TabIndex = 8;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "尺 / Ｍ";
		this.groupBox3.Controls.Add(this.radioButton7);
		this.groupBox3.Controls.Add(this.radioButton8);
		this.groupBox3.Location = new System.Drawing.Point(12, 81);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(200, 49);
		this.groupBox3.TabIndex = 5;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "一般地域 / 多雪地域";
		this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.button2.Location = new System.Drawing.Point(203, 200);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(75, 25);
		this.button2.TabIndex = 12;
		this.button2.Text = "Cancel";
		this.button2.UseVisualStyleBackColor = true;
		this.button1.Location = new System.Drawing.Point(122, 200);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 25);
		this.button1.TabIndex = 11;
		this.button1.Text = "OK";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		base.AcceptButton = this.button1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.button2;
		base.ClientSize = new System.Drawing.Size(404, 229);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormSettingOutSurfaceCut";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "FormSettingOutSurfaceCut";
		base.Load += new System.EventHandler(FormSettingOutSurfaceCut_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		this.groupBox3.ResumeLayout(false);
		this.groupBox3.PerformLayout();
		base.ResumeLayout(false);
	}
}
