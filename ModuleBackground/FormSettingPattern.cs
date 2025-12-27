using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ModuleBackground;

public class FormSettingPattern : Form
{
	public double dDist1;

	public double dDist2;

	public int nCase;

	private IContainer components;

	private Label label1;

	private TextBox textBox1;

	private Button button1;

	private Button button2;

	private Label label2;

	private Button button3;

	private Button button4;

	private PictureBox pictureBox1;

	private TextBox textBox2;

	public FormSettingPattern()
	{
		InitializeComponent();
	}

	private void FormSettingPattern_Load(object sender, EventArgs e)
	{
		string text = "";
		string text2 = "";
		if (dDist1 != 0.0)
		{
			text = $"{Math.Round(dDist1, 2)}";
		}
		textBox1.Text = text;
		textBox1.Select(text.Length, 1);
		if (dDist2 != 0.0)
		{
			text2 = $"{Math.Round(dDist2, 2)}";
		}
		textBox2.Text = text2;
		if (nCase == 2)
		{
			textBox2.Select(text2.Length, 1);
		}
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string text3 = pathFolderSymbol + "\\common";
		pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
		try
		{
			pictureBox1.BackgroundImage = Image.FromFile(text3 + "\\PatternImage.JPG");
		}
		catch (Exception)
		{
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		nCase = 1;
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void button2_Click(object sender, EventArgs e)
	{
		nCase = 2;
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void button3_Click(object sender, EventArgs e)
	{
		string value = textBox1.Text;
		if (string.IsNullOrEmpty(value))
		{
			MessageBox.Show("Please input Horizontal Length");
			textBox1.Focus();
			return;
		}
		try
		{
			dDist1 = double.Parse(textBox1.Text);
		}
		catch (Exception)
		{
		}
		if (dDist1 <= 0.0)
		{
			MessageBox.Show("Please input Horizontal Length > 0");
			textBox1.Focus();
			return;
		}
		value = textBox2.Text;
		if (string.IsNullOrEmpty(value))
		{
			MessageBox.Show("Please input Vertical Length ");
			textBox2.Focus();
			return;
		}
		try
		{
			dDist2 = double.Parse(value);
		}
		catch (Exception)
		{
		}
		if (dDist2 <= 0.0)
		{
			MessageBox.Show("Please input Vertical Length  > 0");
			textBox2.Focus();
		}
		else
		{
			nCase = 0;
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void txt_KeyPress_Decimal(object sender, KeyPressEventArgs e)
	{
		if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
		{
			e.Handled = true;
			return;
		}
		if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
		{
			e.Handled = true;
			return;
		}
		string text = "１２３４５６７８９０";
		string value = e.KeyChar.ToString();
		int num = text.IndexOf(value);
		if (num != -1)
		{
			e.Handled = true;
		}
	}

	private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
	{
		txt_KeyPress_Decimal(sender, e);
	}

	private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
	{
		txt_KeyPress_Decimal(sender, e);
	}

	private void FormSettingPattern_Activated(object sender, EventArgs e)
	{
		if (nCase == 1)
		{
			textBox1.Focus();
		}
		if (nCase == 2)
		{
			textBox2.Focus();
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
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.textBox2 = new System.Windows.Forms.TextBox();
		this.button2 = new System.Windows.Forms.Button();
		this.label2 = new System.Windows.Forms.Label();
		this.button3 = new System.Windows.Forms.Button();
		this.button4 = new System.Windows.Forms.Button();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 17);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(94, 12);
		this.label1.TabIndex = 0;
		this.label1.Text = "Horizontal Length";
		this.textBox1.Location = new System.Drawing.Point(110, 13);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(113, 19);
		this.textBox1.TabIndex = 1;
		this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox1_KeyPress);
		this.button1.Location = new System.Drawing.Point(230, 13);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(39, 23);
		this.button1.TabIndex = 5;
		this.button1.Text = "Sel";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.textBox2.Location = new System.Drawing.Point(110, 54);
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size(113, 19);
		this.textBox2.TabIndex = 2;
		this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox2_KeyPress);
		this.button2.Location = new System.Drawing.Point(230, 54);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(39, 23);
		this.button2.TabIndex = 6;
		this.button2.Text = "Sel";
		this.button2.UseVisualStyleBackColor = true;
		this.button2.Click += new System.EventHandler(button2_Click);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(20, 58);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(87, 12);
		this.label2.TabIndex = 0;
		this.label2.Text = "Vertical Length ";
		this.button3.Location = new System.Drawing.Point(64, 102);
		this.button3.Name = "button3";
		this.button3.Size = new System.Drawing.Size(75, 23);
		this.button3.TabIndex = 3;
		this.button3.Text = "Ok";
		this.button3.UseVisualStyleBackColor = true;
		this.button3.Click += new System.EventHandler(button3_Click);
		this.button4.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.button4.Location = new System.Drawing.Point(158, 101);
		this.button4.Name = "button4";
		this.button4.Size = new System.Drawing.Size(75, 23);
		this.button4.TabIndex = 4;
		this.button4.Text = "Cancel";
		this.button4.UseVisualStyleBackColor = true;
		this.pictureBox1.Location = new System.Drawing.Point(289, 17);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(211, 108);
		this.pictureBox1.TabIndex = 8;
		this.pictureBox1.TabStop = false;
		base.AcceptButton = this.button3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.button4;
		base.ClientSize = new System.Drawing.Size(515, 135);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.button4);
		base.Controls.Add(this.button3);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.textBox2);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormSettingPattern";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Setting Pattern";
		base.Activated += new System.EventHandler(FormSettingPattern_Activated);
		base.Load += new System.EventHandler(FormSettingPattern_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
