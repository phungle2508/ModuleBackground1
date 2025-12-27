using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ModuleBackground;

public class FormSettingTextNote : Form
{
	public int m_nSel;

	public string m_sText;

	public int m_nCheck;

	private List<string> lstPathJPG = new List<string>();

	public List<string> lstNameText = new List<string>();

	private IContainer components;

	private Label label1;

	private ComboBox comboBox1;

	private GroupBox groupBox1;

	private RadioButton radioButton2;

	private RadioButton radioButton1;

	private PictureBox pictureBox1;

	private Button button1;

	private Button button2;

	public FormSettingTextNote()
	{
		InitializeComponent();
	}

	private void FormSettingTextNote_Load(object sender, EventArgs e)
	{
		pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
		if (lstNameText.Count == 0)
		{
			lstNameText.Add("SL+50");
			lstNameText.Add("SL+200");
			lstNameText.Add("SL+350");
			lstNameText.Add("SL+120");
			lstNameText.Add("SL+300");
			lstNameText.Add("SL+330");
			lstNameText.Add("SL+315");
			lstNameText.Add("SL+345");
			lstNameText.Add("勝手口階段寸法現場調整(後打ち)");
			lstNameText.Add("架台寸法現場調整(後打ち)");
			lstNameText.Add("気密パッキン");
			lstNameText.Add("盗み板t=20");
			lstNameText.Add("盗み板t=15");
			lstNameText.Add("鋼製束");
			lstNameText.Add("カネライトスーパーＥ－Ⅲ");
			lstNameText.Add("カネライトフォームＦＸ");
			lstNameText.Add("ユニットバス / 基礎開口用断熱材 / Joto製 SPK-150設置");
			lstNameText.Add("押出法 / ポリスチレンフォーム3種 / t=50");
			lstNameText.Add("基礎隅角部補強 / D16 定着L=480以上");
			lstNameText.Add("土間断熱材敷き込み / 押出法ポリスチレソフォーム3種t20");
		}
		comboBox1.DataSource = lstNameText;
		comboBox1.SelectedIndex = m_nSel;
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string text = pathFolderSymbol + "\\common";
		lstPathJPG.Clear();
		lstPathJPG.Add(text + "\\LeaderH.JPG");
		lstPathJPG.Add(text + "\\LeaderV.JPG");
		groupBox1.Enabled = false;
		string text2 = lstNameText[m_nSel];
		if (text2.IndexOf("SL+") == -1)
		{
			groupBox1.Enabled = true;
		}
		radioButton1.Checked = true;
		if (m_nCheck == 1)
		{
			radioButton2.Checked = true;
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		m_sText = comboBox1.Text;
		m_nSel = comboBox1.SelectedIndex;
		m_nCheck = 0;
		if (radioButton2.Checked)
		{
			m_nCheck = 1;
		}
		Close();
	}

	private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		groupBox1.Enabled = false;
		string text = comboBox1.Text;
		if (text.IndexOf("SL+") == -1)
		{
			groupBox1.Enabled = true;
		}
	}

	public void LoadImage(string sName)
	{
		try
		{
			pictureBox1.BackgroundImage = Image.FromFile(sName);
		}
		catch (Exception)
		{
		}
	}

	private void radioButton1_CheckedChanged(object sender, EventArgs e)
	{
		if (radioButton1.Checked)
		{
			LoadImage(lstPathJPG[0]);
		}
	}

	private void radioButton2_CheckedChanged(object sender, EventArgs e)
	{
		if (radioButton2.Checked)
		{
			LoadImage(lstPathJPG[1]);
		}
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
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.radioButton1 = new System.Windows.Forms.RadioButton();
		this.radioButton2 = new System.Windows.Forms.RadioButton();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.button1 = new System.Windows.Forms.Button();
		this.button2 = new System.Windows.Forms.Button();
		this.groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(22, 23);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(30, 12);
		this.label1.TabIndex = 0;
		this.label1.Text = "Text:";
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(59, 19);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(367, 20);
		this.comboBox1.TabIndex = 1;
		this.comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
		this.groupBox1.Controls.Add(this.pictureBox1);
		this.groupBox1.Controls.Add(this.radioButton2);
		this.groupBox1.Controls.Add(this.radioButton1);
		this.groupBox1.Location = new System.Drawing.Point(24, 57);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(402, 270);
		this.groupBox1.TabIndex = 2;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Select type of Leader Line";
		this.radioButton1.AutoSize = true;
		this.radioButton1.Location = new System.Drawing.Point(38, 26);
		this.radioButton1.Name = "radioButton1";
		this.radioButton1.Size = new System.Drawing.Size(74, 16);
		this.radioButton1.TabIndex = 3;
		this.radioButton1.TabStop = true;
		this.radioButton1.Text = "Horizontal";
		this.radioButton1.UseVisualStyleBackColor = true;
		this.radioButton1.CheckedChanged += new System.EventHandler(radioButton1_CheckedChanged);
		this.radioButton2.AutoSize = true;
		this.radioButton2.Location = new System.Drawing.Point(178, 26);
		this.radioButton2.Name = "radioButton2";
		this.radioButton2.Size = new System.Drawing.Size(63, 16);
		this.radioButton2.TabIndex = 4;
		this.radioButton2.TabStop = true;
		this.radioButton2.Text = "Vertical";
		this.radioButton2.UseVisualStyleBackColor = true;
		this.radioButton2.CheckedChanged += new System.EventHandler(radioButton2_CheckedChanged);
		this.pictureBox1.Location = new System.Drawing.Point(35, 48);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(336, 203);
		this.pictureBox1.TabIndex = 2;
		this.pictureBox1.TabStop = false;
		this.button1.Location = new System.Drawing.Point(149, 345);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 5;
		this.button1.Text = "Ok";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.button2.Location = new System.Drawing.Point(240, 345);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(75, 23);
		this.button2.TabIndex = 6;
		this.button2.Text = "Cancel";
		this.button2.UseVisualStyleBackColor = true;
		base.AcceptButton = this.button1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.button2;
		base.ClientSize = new System.Drawing.Size(451, 380);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.comboBox1);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormSettingTextNote";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Setting Text Note";
		base.Load += new System.EventHandler(FormSettingTextNote_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
