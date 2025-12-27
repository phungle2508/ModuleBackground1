using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Microsoft.Win32;

namespace ModuleBackground;

public class FormSettingRoomUB : Form
{
	public bool bCheck = true;

	public int nSel;

	public string sFolderSymUB = "";

	private IContainer components;

	private SplitContainer splitContainer1;

	private GroupBox groupBox1;

	private ComboBox comboBox1;

	private Label label1;

	private RadioButton radioButton2;

	private RadioButton radioButton1;

	public ListView listView1;

	public FormSettingRoomUB()
	{
		InitializeComponent();
	}

	private void FormSettingRoomUB_Load(object sender, EventArgs e)
	{
		string[] array = new string[4] { "UB2x2", "UB2.5x2", "UB2x2.5", "UB2.5x2.5" };
		string[] array2 = array;
		foreach (string item in array2)
		{
			comboBox1.Items.Add(item);
		}
		comboBox1.SelectedIndex = nSel;
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		sFolderSymUB = $"{pathFolderSymbol}\\UB\\";
		string sPathFolderSym = $"{sFolderSymUB}{array[nSel]}";
		string sPitch = "910";
		if (!bCheck)
		{
			sPitch = "1000";
		}
		LoadListview(sPathFolderSym, sPitch);
		radioButton1.Checked = bCheck;
		GetRegistryUB();
	}

	public void LoadListview(string sPathFolderSym, string sPitch)
	{
		listView1.Clear();
		string[] files = Directory.GetFiles(sPathFolderSym, sPitch + "_*.dwg");
		if (files.Length == 0)
		{
			files = Directory.GetFiles(sPathFolderSym, sPitch + "_*.DWG");
		}
		try
		{
			Array.Sort(files, new AlphanumComparatorFast());
		}
		catch (Exception)
		{
		}
		int num = 0;
		ImageList imageList = new ImageList();
		string[] array = files;
		foreach (string text in array)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
			fileNameWithoutExtension = fileNameWithoutExtension.Replace("910_", "");
			fileNameWithoutExtension = fileNameWithoutExtension.Replace("1000_", "");
			ListViewItem listViewItem = new ListViewItem(fileNameWithoutExtension, num);
			listViewItem.Tag = text;
			Image bitmap = GetBitmapDwgFile.GetBitmap(text);
			if (bitmap != null)
			{
				Bitmap img = new Bitmap(bitmap);
				Bitmap value = GetBitmapDwgFile.MakeBlackAndWhite(img);
				double num2 = 200.0;
				double num3 = 0.0;
				num3 = (double)bitmap.Height * num2 / (double)bitmap.Width;
				imageList.ImageSize = new Size((int)num2, (int)num3);
				imageList.Images.Add(value);
				num++;
				listView1.Items.Add(listViewItem);
				img = null;
			}
		}
		listView1.LargeImageList = imageList;
	}

	private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		Editor editor = Bricscad.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
		using (editor.StartUserInteraction(this))
		{
			string sPathFile = listView1.SelectedItems[0].Tag as string;
			CInputRoomUB.InsertUb(editor.Document, sPathFile);
			editor.WriteMessage("\n: ");
		}
	}

	private void radioButton1_CheckedChanged(object sender, EventArgs e)
	{
		if (radioButton1.Checked)
		{
			string sPathFolderSym = $"{sFolderSymUB}{comboBox1.Text}";
			string sPitch = "910";
			try
			{
				LoadListview(sPathFolderSym, sPitch);
			}
			catch (Exception)
			{
			}
		}
	}

	private void radioButton2_CheckedChanged(object sender, EventArgs e)
	{
		if (radioButton2.Checked)
		{
			string sPathFolderSym = $"{sFolderSymUB}{comboBox1.Text}";
			string sPitch = "1000";
			try
			{
				LoadListview(sPathFolderSym, sPitch);
			}
			catch (Exception)
			{
			}
		}
	}

	private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			string sPathFolderSym = $"{sFolderSymUB}{comboBox1.Text}";
			string sPitch = "910";
			if (!radioButton1.Checked)
			{
				sPitch = "1000";
			}
			LoadListview(sPathFolderSym, sPitch);
		}
		catch (Exception)
		{
		}
	}

	public void SaveRegistryUB()
	{
		try
		{
			RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\ST\\ModuleMong\\Dialogs\\ST_InputUB");
			if (registryKey != null)
			{
				registryKey.SetValue(radioButton1.Name, radioButton1.Checked.ToString());
				registryKey.SetValue("FolderSelect", comboBox1.Text);
				registryKey.Close();
			}
		}
		catch (Exception)
		{
		}
	}

	public void GetRegistryUB()
	{
		try
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\ST\\ModuleMong\\Dialogs\\ST_InputUB");
			if (registryKey != null)
			{
				string strA = registryKey.GetValue(radioButton1.Name).ToString();
				if (string.Compare(strA, "true", ignoreCase: true) == 0)
				{
					radioButton1.Checked = true;
				}
				else
				{
					radioButton2.Checked = true;
				}
				try
				{
					comboBox1.Text = registryKey.GetValue("FolderSelect").ToString();
				}
				catch (Exception)
				{
					comboBox1.SelectedIndex = 0;
				}
				registryKey.Close();
			}
		}
		catch (Exception)
		{
		}
	}

	private void FormSettingRoomUB_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveRegistryUB();
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
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.radioButton2 = new System.Windows.Forms.RadioButton();
		this.radioButton1 = new System.Windows.Forms.RadioButton();
		this.listView1 = new System.Windows.Forms.ListView();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
		this.splitContainer1.Panel2.Controls.Add(this.listView1);
		this.splitContainer1.Size = new System.Drawing.Size(1092, 418);
		this.splitContainer1.SplitterDistance = 115;
		this.splitContainer1.TabIndex = 0;
		this.groupBox1.Controls.Add(this.comboBox1);
		this.groupBox1.Controls.Add(this.label1);
		this.groupBox1.Controls.Add(this.radioButton2);
		this.groupBox1.Controls.Add(this.radioButton1);
		this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.groupBox1.Location = new System.Drawing.Point(0, 12);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(1092, 103);
		this.groupBox1.TabIndex = 3;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "尺 / Ｍ";
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(64, 59);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(271, 20);
		this.comboBox1.TabIndex = 3;
		this.comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(25, 62);
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
		this.radioButton2.CheckedChanged += new System.EventHandler(radioButton2_CheckedChanged);
		this.radioButton1.AutoSize = true;
		this.radioButton1.Location = new System.Drawing.Point(25, 26);
		this.radioButton1.Name = "radioButton1";
		this.radioButton1.Size = new System.Drawing.Size(35, 16);
		this.radioButton1.TabIndex = 0;
		this.radioButton1.TabStop = true;
		this.radioButton1.Text = "尺";
		this.radioButton1.UseVisualStyleBackColor = true;
		this.radioButton1.CheckedChanged += new System.EventHandler(radioButton1_CheckedChanged);
		this.listView1.BackgroundImageTiled = true;
		this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.listView1.Location = new System.Drawing.Point(0, 0);
		this.listView1.Name = "listView1";
		this.listView1.Size = new System.Drawing.Size(1092, 299);
		this.listView1.TabIndex = 4;
		this.listView1.UseCompatibleStateImageBehavior = false;
		this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(listView1_MouseDoubleClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1092, 418);
		base.Controls.Add(this.splitContainer1);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(560, 440);
		base.Name = "FormSettingRoomUB";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Setting Room UB";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FormSettingRoomUB_FormClosing);
		base.Load += new System.EventHandler(FormSettingRoomUB_Load);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		base.ResumeLayout(false);
	}
}
