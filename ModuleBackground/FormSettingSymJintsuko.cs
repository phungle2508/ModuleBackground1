using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ModuleBackground;

public class FormSettingSymJintsuko : Form
{
	public int m_nSel;

	public string m_sText;

	private IContainer components;

	private Label label1;

	private ComboBox comboBox1;

	private Button button1;

	private Button button2;

	public FormSettingSymJintsuko()
	{
		InitializeComponent();
	}

	private void FormSettingSymJintsuko_Load(object sender, EventArgs e)
	{
		List<string> list = new List<string>();
		list.Add("285/550");
		list.Add("985/550");
		list.Add("375/550");
		list.Add("1075/550");
		comboBox1.DataSource = list;
		comboBox1.SelectedIndex = m_nSel;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		m_sText = comboBox1.Text;
		m_nSel = comboBox1.SelectedIndex;
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
		this.label1 = new System.Windows.Forms.Label();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.button1 = new System.Windows.Forms.Button();
		this.button2 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 23);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(64, 12);
		this.label1.TabIndex = 0;
		this.label1.Text = "Information:";
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(84, 23);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(174, 20);
		this.comboBox1.TabIndex = 1;
		this.button1.Location = new System.Drawing.Point(66, 67);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 2;
		this.button1.Text = "OK";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.button2.Location = new System.Drawing.Point(158, 67);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(75, 23);
		this.button2.TabIndex = 3;
		this.button2.Text = "Cancle";
		this.button2.UseVisualStyleBackColor = true;
		base.AcceptButton = this.button1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.button2;
		base.ClientSize = new System.Drawing.Size(292, 102);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.comboBox1);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormSettingSymJintsuko";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Select ventilation holes";
		base.Load += new System.EventHandler(FormSettingSymJintsuko_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
