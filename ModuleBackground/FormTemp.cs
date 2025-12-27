using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ModuleBackground;

public class FormTemp : Form
{
	private IContainer components;

	public ListView listView1;

	private GroupBox groupBox1;

	private ComboBox comboBox1;

	private Label label1;

	private RadioButton radioButton2;

	private RadioButton radioButton1;

	public FormTemp()
	{
		InitializeComponent();
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
		this.listView1 = new System.Windows.Forms.ListView();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.radioButton2 = new System.Windows.Forms.RadioButton();
		this.radioButton1 = new System.Windows.Forms.RadioButton();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.listView1.BackgroundImageTiled = true;
		this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.listView1.Location = new System.Drawing.Point(0, 106);
		this.listView1.Name = "listView1";
		this.listView1.Size = new System.Drawing.Size(693, 276);
		this.listView1.TabIndex = 3;
		this.listView1.UseCompatibleStateImageBehavior = false;
		this.groupBox1.Controls.Add(this.comboBox1);
		this.groupBox1.Controls.Add(this.label1);
		this.groupBox1.Controls.Add(this.radioButton2);
		this.groupBox1.Controls.Add(this.radioButton1);
		this.groupBox1.Location = new System.Drawing.Point(0, 0);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(406, 98);
		this.groupBox1.TabIndex = 2;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "尺 / Ｍ";
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(64, 52);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(217, 20);
		this.comboBox1.TabIndex = 3;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(25, 55);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(32, 12);
		this.label1.TabIndex = 2;
		this.label1.Text = "Type:";
		this.radioButton2.AutoSize = true;
		this.radioButton2.Location = new System.Drawing.Point(114, 26);
		this.radioButton2.Name = "radioButton2";
		this.radioButton2.Size = new System.Drawing.Size(32, 16);
		this.radioButton2.TabIndex = 1;
		this.radioButton2.TabStop = true;
		this.radioButton2.Text = "M";
		this.radioButton2.UseVisualStyleBackColor = true;
		this.radioButton1.AutoSize = true;
		this.radioButton1.Location = new System.Drawing.Point(25, 26);
		this.radioButton1.Name = "radioButton1";
		this.radioButton1.Size = new System.Drawing.Size(35, 16);
		this.radioButton1.TabIndex = 0;
		this.radioButton1.TabStop = true;
		this.radioButton1.Text = "尺";
		this.radioButton1.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(736, 491);
		base.Controls.Add(this.listView1);
		base.Controls.Add(this.groupBox1);
		base.Name = "FormTemp";
		this.Text = "FormTemp";
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		base.ResumeLayout(false);
	}
}
