using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ModuleBackground;

public class FormSettingInputSurfaceCut : Form
{
	public int m_nSel;

	public string m_sText;

	private IContainer components;

	private Label label1;

	private ComboBox comboBox1;

	private Button button1;

	private Button button2;

	public FormSettingInputSurfaceCut()
	{
		InitializeComponent();
	}

	private void FormSettingInputSurfaceCut_Load(object sender, EventArgs e)
	{
		List<string> list = new List<string>();
		list.Add("FG2a");
		list.Add("FG2b");
		list.Add("FG2c");
		list.Add("FG2S");
		list.Add("FG2Sa");
		list.Add("FG2Sb");
		list.Add("FG2Sc");
		list.Add("FG3");
		list.Add("FG3a");
		list.Add("FG4");
		list.Add("FG4a");
		list.Add("FG4b");
		list.Add("FG5");
		list.Add("FG5a");
		list.Add("FG5b");
		list.Add("FG5c");
		list.Add("FG20 外部");
		list.Add("FG20 内部");
		list.Add("FG21");
		list.Add("配管貫通部（増し打ち）");
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
		this.label1.Location = new System.Drawing.Point(13, 27);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(64, 12);
		this.label1.TabIndex = 0;
		this.label1.Text = "Information:";
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(84, 24);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(171, 20);
		this.comboBox1.TabIndex = 1;
		this.button1.Location = new System.Drawing.Point(49, 63);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 2;
		this.button1.Text = "OK";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.Location = new System.Drawing.Point(130, 63);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(75, 23);
		this.button2.TabIndex = 2;
		this.button2.Text = "Cancel";
		this.button2.UseVisualStyleBackColor = true;
		base.AcceptButton = this.button1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.button2;
		base.ClientSize = new System.Drawing.Size(270, 96);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.comboBox1);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormSettingInputSurfaceCut";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Setting Information Surface Cut";
		base.TopMost = true;
		base.Load += new System.EventHandler(FormSettingInputSurfaceCut_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
