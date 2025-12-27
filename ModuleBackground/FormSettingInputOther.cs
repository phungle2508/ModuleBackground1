using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ModuleBackground;

public class FormSettingInputOther : Form
{
	private List<string> lstNameText = new List<string>();

	public int m_nSel;

	public string sPathFileDwg = "";

	private IContainer components;

	private Button button1;

	private Button button2;

	private Label label1;

	private ComboBox comboBox1;

	private PictureBox pictureBox1;

	public FormSettingInputOther()
	{
		InitializeComponent();
	}

	private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			string text = lstNameText[comboBox1.SelectedIndex];
			text = text.Replace(".DWG", ".JPG");
			text = text.Replace(".dwg", ".JPG");
			pictureBox1.BackgroundImage = Image.FromFile(text);
		}
		catch (Exception)
		{
		}
	}

	public Bitmap GetImage(string sPath)
	{
		Image bitmap = GetBitmapDwgFile.GetBitmap(sPath);
		if (bitmap != null)
		{
			Bitmap img = new Bitmap(bitmap);
			Bitmap image = GetBitmapDwgFile.MakeBlackAndWhite(img);
			double num = 200.0;
			_ = (double)bitmap.Height * num / (double)bitmap.Width;
			Bitmap bitmap2 = new Bitmap((int)num, 200);
			using Graphics graphics = Graphics.FromImage(bitmap2);
			graphics.DrawImage(image, 0, 0, (int)num, 200);
			return bitmap2;
		}
		return null;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		try
		{
			m_nSel = comboBox1.SelectedIndex;
			if (m_nSel != -1)
			{
				sPathFileDwg = lstNameText[m_nSel];
			}
			else
			{
				m_nSel = 0;
			}
		}
		catch (Exception)
		{
		}
		Close();
	}

	private void FormSettingInputOther_Load(object sender, EventArgs e)
	{
		pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string path = pathFolderSymbol + "\\common";
		string[] files = Directory.GetFiles(path, "*.dwg");
		if (files.Length == 0)
		{
			files = Directory.GetFiles(path, "*.DWG");
		}
		try
		{
			Array.Sort(files, new AlphanumComparatorFast());
		}
		catch (Exception)
		{
		}
		lstNameText.Clear();
		lstNameText.AddRange(files);
		foreach (string item in lstNameText)
		{
			string text = Path.GetFileNameWithoutExtension(item);
			int num = text.IndexOf("_");
			if (num != -1)
			{
				text = text.Substring(num + 1, text.Length - num - 1);
			}
			comboBox1.Items.Add(text);
		}
		if (lstNameText.Count != 0)
		{
			if (m_nSel >= lstNameText.Count)
			{
				m_nSel = 0;
			}
			comboBox1.SelectedIndex = m_nSel;
			string text2 = lstNameText[m_nSel];
			text2 = text2.Replace(".DWG", ".JPG");
			text2 = text2.Replace(".dwg", ".JPG");
			try
			{
				pictureBox1.BackgroundImage = Image.FromFile(text2);
			}
			catch (Exception)
			{
			}
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
		this.button1 = new System.Windows.Forms.Button();
		this.button2 = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.button1.Location = new System.Drawing.Point(212, 214);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 3;
		this.button1.Text = "Input";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.button2.Location = new System.Drawing.Point(293, 214);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(75, 23);
		this.button2.TabIndex = 4;
		this.button2.Text = "Close";
		this.button2.UseVisualStyleBackColor = true;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(25, 23);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(64, 12);
		this.label1.TabIndex = 1;
		this.label1.Text = "Information:";
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(25, 53);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(171, 20);
		this.comboBox1.TabIndex = 2;
		this.comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
		this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
		this.pictureBox1.InitialImage = null;
		this.pictureBox1.Location = new System.Drawing.Point(202, 2);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(385, 200);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 3;
		this.pictureBox1.TabStop = false;
		base.AcceptButton = this.button1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.button2;
		base.ClientSize = new System.Drawing.Size(613, 250);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.comboBox1);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.button1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormSettingInputOther";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "FormInputSurfaceCut";
		base.Load += new System.EventHandler(FormSettingInputOther_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
