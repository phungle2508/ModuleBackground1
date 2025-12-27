using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Bricscad.ApplicationServices;

namespace ModuleBackground;

public class FormMenu : Form
{
	private IContainer components;

	private Button button1;

	private Button button2;

	private Button button4;

	private Button button5;

	private Button button6;

	private Button button8;

	private Button button7;

	private Button button9;

	private Button button10;

	private Button button11;

	private Button button12;

	private Button button13;

	private Button button14;

	private Button button15;

	private Button button17;

	private Button button19;

	private Button button18;

	private GroupBox groupBox1;

	private Button button3;

	public FormMenu()
	{
		InitializeComponent();
	}

	private void SendCommandMenu(string sNameCommand)
	{
		Document mdiActiveDocument = Bricscad.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
		if (!(mdiActiveDocument == null))
		{
			mdiActiveDocument.SendStringToExecute(sNameCommand + "\n", activate: true, wrapUpInactiveDoc: false, echoCommand: false);
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_DrawGridsIntersect");
	}

	private void button2_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_SettingBackground");
	}

	private void button3_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_SplitRoom");
	}

	private void button4_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InputSurfaceCut");
	}

	private void button5_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InsSymJintsuko");
	}

	private void button6_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InsSymIntersect1");
	}

	private void button7_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InsSymIntersect2");
	}

	private void button8_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InsSymNearSquares1");
	}

	private void button9_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InsSymNearSquares2");
	}

	private void button10_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InsSymNearSquares3");
	}

	private void button11_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InsSymNearSquares4");
	}

	private void button12_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InputUB");
	}

	private void button13_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_DrawLine100");
	}

	private void button14_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_DrawPattern");
	}

	private void button15_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_DrawLine75");
	}

	private void button16_Click(object sender, EventArgs e)
	{
	}

	private void button19_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_TextNote");
	}

	private void button17_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_InputOther");
	}

	private void button18_Click(object sender, EventArgs e)
	{
		SendCommandMenu("ST_OutputSurfaceCut");
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
		this.button4 = new System.Windows.Forms.Button();
		this.button5 = new System.Windows.Forms.Button();
		this.button6 = new System.Windows.Forms.Button();
		this.button8 = new System.Windows.Forms.Button();
		this.button7 = new System.Windows.Forms.Button();
		this.button9 = new System.Windows.Forms.Button();
		this.button10 = new System.Windows.Forms.Button();
		this.button11 = new System.Windows.Forms.Button();
		this.button12 = new System.Windows.Forms.Button();
		this.button13 = new System.Windows.Forms.Button();
		this.button14 = new System.Windows.Forms.Button();
		this.button15 = new System.Windows.Forms.Button();
		this.button17 = new System.Windows.Forms.Button();
		this.button19 = new System.Windows.Forms.Button();
		this.button18 = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.button3 = new System.Windows.Forms.Button();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.button1.AutoSize = true;
		this.button1.Location = new System.Drawing.Point(9, 13);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(61, 23);
		this.button1.TabIndex = 0;
		this.button1.Text = "グリッド";
		this.button1.UseVisualStyleBackColor = false;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.AutoSize = true;
		this.button2.Location = new System.Drawing.Point(75, 13);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(65, 23);
		this.button2.TabIndex = 1;
		this.button2.Text = "ライン設定";
		this.button2.UseVisualStyleBackColor = false;
		this.button2.Click += new System.EventHandler(button2_Click);
		this.button4.AutoSize = true;
		this.button4.Location = new System.Drawing.Point(227, 12);
		this.button4.Name = "button4";
		this.button4.Size = new System.Drawing.Size(97, 23);
		this.button4.TabIndex = 3;
		this.button4.Text = "立上登録";
		this.button4.UseVisualStyleBackColor = false;
		this.button4.Click += new System.EventHandler(button4_Click);
		this.button5.AutoSize = true;
		this.button5.Location = new System.Drawing.Point(330, 13);
		this.button5.Name = "button5";
		this.button5.Size = new System.Drawing.Size(52, 23);
		this.button5.TabIndex = 4;
		this.button5.Text = "人通口";
		this.button5.UseVisualStyleBackColor = false;
		this.button5.Click += new System.EventHandler(button5_Click);
		this.button6.AutoSize = true;
		this.button6.Location = new System.Drawing.Point(388, 13);
		this.button6.Name = "button6";
		this.button6.Size = new System.Drawing.Size(85, 23);
		this.button6.TabIndex = 5;
		this.button6.Text = "鋼製束自動";
		this.button6.UseVisualStyleBackColor = false;
		this.button6.Click += new System.EventHandler(button6_Click);
		this.button8.AutoSize = true;
		this.button8.Location = new System.Drawing.Point(560, 13);
		this.button8.Name = "button8";
		this.button8.Size = new System.Drawing.Size(76, 23);
		this.button8.TabIndex = 7;
		this.button8.Text = "アンカー自動";
		this.button8.UseVisualStyleBackColor = false;
		this.button8.Click += new System.EventHandler(button8_Click);
		this.button7.AutoSize = true;
		this.button7.Location = new System.Drawing.Point(479, 12);
		this.button7.Name = "button7";
		this.button7.Size = new System.Drawing.Size(75, 23);
		this.button7.TabIndex = 6;
		this.button7.Text = "鋼製束自由";
		this.button7.UseVisualStyleBackColor = false;
		this.button7.Click += new System.EventHandler(button7_Click);
		this.button9.AutoSize = true;
		this.button9.Location = new System.Drawing.Point(642, 12);
		this.button9.Name = "button9";
		this.button9.Size = new System.Drawing.Size(75, 23);
		this.button9.TabIndex = 8;
		this.button9.Text = "アンカー200";
		this.button9.UseVisualStyleBackColor = false;
		this.button9.Click += new System.EventHandler(button9_Click);
		this.button10.AutoSize = true;
		this.button10.Location = new System.Drawing.Point(388, 43);
		this.button10.Name = "button10";
		this.button10.Size = new System.Drawing.Size(85, 23);
		this.button10.TabIndex = 15;
		this.button10.Text = "アンカーHD";
		this.button10.UseVisualStyleBackColor = false;
		this.button10.Click += new System.EventHandler(button10_Click);
		this.button11.AutoSize = true;
		this.button11.Location = new System.Drawing.Point(8, 43);
		this.button11.Name = "button11";
		this.button11.Size = new System.Drawing.Size(62, 23);
		this.button11.TabIndex = 10;
		this.button11.Text = "アンカー0 ";
		this.button11.UseVisualStyleBackColor = false;
		this.button11.Click += new System.EventHandler(button11_Click);
		this.button12.AutoSize = true;
		this.button12.Location = new System.Drawing.Point(75, 43);
		this.button12.Name = "button12";
		this.button12.Size = new System.Drawing.Size(65, 23);
		this.button12.TabIndex = 11;
		this.button12.Text = "浴室";
		this.button12.UseVisualStyleBackColor = false;
		this.button12.Click += new System.EventHandler(button12_Click);
		this.button13.AutoSize = true;
		this.button13.Location = new System.Drawing.Point(146, 43);
		this.button13.Name = "button13";
		this.button13.Size = new System.Drawing.Size(75, 23);
		this.button13.TabIndex = 12;
		this.button13.Text = "気密パッキン";
		this.button13.UseVisualStyleBackColor = false;
		this.button13.Click += new System.EventHandler(button13_Click);
		this.button14.AutoSize = true;
		this.button14.Location = new System.Drawing.Point(227, 43);
		this.button14.Name = "button14";
		this.button14.Size = new System.Drawing.Size(97, 23);
		this.button14.TabIndex = 13;
		this.button14.Text = "隅角部の補強筋";
		this.button14.UseVisualStyleBackColor = false;
		this.button14.Click += new System.EventHandler(button14_Click);
		this.button15.AutoSize = true;
		this.button15.Location = new System.Drawing.Point(330, 43);
		this.button15.Name = "button15";
		this.button15.Size = new System.Drawing.Size(52, 23);
		this.button15.TabIndex = 14;
		this.button15.Text = "線75";
		this.button15.UseVisualStyleBackColor = false;
		this.button15.Click += new System.EventHandler(button15_Click);
		this.button17.AutoSize = true;
		this.button17.Location = new System.Drawing.Point(560, 43);
		this.button17.Name = "button17";
		this.button17.Size = new System.Drawing.Size(76, 23);
		this.button17.TabIndex = 17;
		this.button17.Text = "凡例";
		this.button17.UseVisualStyleBackColor = false;
		this.button17.Click += new System.EventHandler(button17_Click);
		this.button19.AutoSize = true;
		this.button19.Location = new System.Drawing.Point(479, 43);
		this.button19.Name = "button19";
		this.button19.Size = new System.Drawing.Size(75, 23);
		this.button19.TabIndex = 16;
		this.button19.Text = "注記";
		this.button19.UseVisualStyleBackColor = false;
		this.button19.Click += new System.EventHandler(button19_Click);
		this.button18.AutoSize = true;
		this.button18.Location = new System.Drawing.Point(642, 43);
		this.button18.Name = "button18";
		this.button18.Size = new System.Drawing.Size(75, 23);
		this.button18.TabIndex = 18;
		this.button18.Text = "断面出力";
		this.button18.UseVisualStyleBackColor = false;
		this.button18.Click += new System.EventHandler(button18_Click);
		this.groupBox1.Controls.Add(this.button6);
		this.groupBox1.Controls.Add(this.button10);
		this.groupBox1.Controls.Add(this.button1);
		this.groupBox1.Controls.Add(this.button19);
		this.groupBox1.Controls.Add(this.button2);
		this.groupBox1.Controls.Add(this.button18);
		this.groupBox1.Controls.Add(this.button11);
		this.groupBox1.Controls.Add(this.button8);
		this.groupBox1.Controls.Add(this.button3);
		this.groupBox1.Controls.Add(this.button17);
		this.groupBox1.Controls.Add(this.button12);
		this.groupBox1.Controls.Add(this.button9);
		this.groupBox1.Controls.Add(this.button4);
		this.groupBox1.Controls.Add(this.button13);
		this.groupBox1.Controls.Add(this.button7);
		this.groupBox1.Controls.Add(this.button5);
		this.groupBox1.Controls.Add(this.button15);
		this.groupBox1.Controls.Add(this.button14);
		this.groupBox1.Location = new System.Drawing.Point(4, 0);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(727, 75);
		this.groupBox1.TabIndex = 2;
		this.groupBox1.TabStop = false;
		this.button3.AutoSize = true;
		this.button3.Location = new System.Drawing.Point(146, 13);
		this.button3.Name = "button3";
		this.button3.Size = new System.Drawing.Size(75, 23);
		this.button3.TabIndex = 2;
		this.button3.Text = "立上";
		this.button3.UseVisualStyleBackColor = false;
		this.button3.Click += new System.EventHandler(button3_Click);
		this.AllowDrop = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		base.ClientSize = new System.Drawing.Size(736, 76);
		base.ControlBox = false;
		base.Controls.Add(this.groupBox1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FormMenu";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "タマホーム基礎";
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		base.ResumeLayout(false);
	}
}
