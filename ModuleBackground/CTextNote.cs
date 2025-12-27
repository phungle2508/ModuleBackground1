using System;
using System.Drawing;
using System.Windows.Forms;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CTextNote
{
	public void cmdTextNote(Document doc)
	{
		string keitenIniFile = "";
		bool flag = GlobalFunction.existTSVConfigFile(out keitenIniFile);
		Bricscad.ApplicationServices.Application.GetSystemVariable("OSMODE");
		ObjectId arrowObjectId = GlobalFunction.GetArrowObjectId("_OPEN30");
		Editor editor = doc.Editor;
		int nSel = 0;
		int num = 0;
		while (true)
		{
			FormSettingTextNote formSettingTextNote = new FormSettingTextNote();
			formSettingTextNote.m_nSel = nSel;
			formSettingTextNote.m_nCheck = num;
			int num2 = 4;
			if (flag)
			{
				CIniFile cIniFile = new CIniFile(keitenIniFile);
				string value = cIniFile.GetValue("NoteTexts", "Color", "");
				try
				{
					num2 = int.Parse(value);
				}
				catch (Exception)
				{
					num2 = 4;
				}
				int num3 = 1;
				while (true)
				{
					value = cIniFile.GetValue("NoteTexts", num3.ToString(), "");
					if (string.IsNullOrEmpty(value))
					{
						break;
					}
					formSettingTextNote.lstNameText.Add(value);
					num3++;
				}
			}
			Bricscad.ApplicationServices.Application.ShowModalDialog(Bricscad.ApplicationServices.Application.MainWindow, formSettingTextNote, persist: false);
			if (DialogResult.OK != formSettingTextNote.DialogResult)
			{
				break;
			}
			nSel = formSettingTextNote.m_nSel;
			num = formSettingTextNote.m_nCheck;
			string text = formSettingTextNote.m_sText;
			bool flag2 = false;
			if (text.IndexOf("SL+") == -1)
			{
				flag2 = true;
			}
			double num4 = 110.0;
			if (flag2)
			{
				string[] array = text.Split('/');
				if (array.Length >= 2)
				{
					text = array[1];
				}
				Font font = new Font("Courier New", 10f);
				System.Drawing.Image image = new Bitmap(1, 1);
				Graphics graphics = Graphics.FromImage(image);
				float width = graphics.MeasureString(text, font).Width;
				for (int i = 0; i < array.Length; i++)
				{
					SizeF sizeF = graphics.MeasureString(array[i], font);
					if (sizeF.Width > width)
					{
						width = sizeF.Width;
						text = array[i];
					}
				}
				PromptPointOptions promptPointOptions = new PromptPointOptions("");
				promptPointOptions.AllowNone = true;
				promptPointOptions.Message = "\nSelect start point:";
				PromptPointResult point = editor.GetPoint(promptPointOptions);
				if (point.Status != PromptStatus.OK)
				{
					continue;
				}
				Point3d value2 = point.Value;
				promptPointOptions.Message = "\nSelect next point:";
				promptPointOptions.UseBasePoint = true;
				promptPointOptions.BasePoint = value2;
				promptPointOptions.AllowNone = true;
				point = editor.GetPoint(promptPointOptions);
				if (point.Status != PromptStatus.OK)
				{
					continue;
				}
				Point3d value3 = point.Value;
				double rotation = 0.0;
				Point3d position = new Point3d(value3.X + 10.0, value3.Y + 50.0, value3.Z);
				if (num == 1)
				{
					position = new Point3d(value3.X - 50.0, value3.Y + 10.0, value3.Z);
					rotation = Math.PI / 2.0;
				}
				DBText dBText = new DBText();
				dBText.SetDatabaseDefaults(doc.Database);
				dBText.Position = position;
				dBText.TextString = text;
				dBText.Height = num4;
				dBText.Rotation = rotation;
				string value4 = GlobalFunction.AppendEntity(doc.Database, dBText, "ST_TextNote", num2);
				dBText = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value4, 16)), 0).GetObject(OpenMode.ForRead) as DBText;
				Point3d minPoint = dBText.GeometricExtents.MinPoint;
				Point3d maxPoint = dBText.GeometricExtents.MaxPoint;
				double num5 = Math.Abs(maxPoint.X - minPoint.X) + 20.0;
				double num6 = Math.Abs(maxPoint.Y - minPoint.Y) + 20.0;
				Point3d point3d = new Point3d(value3.X + num5, value3.Y, value3.Z);
				if (num != 1)
				{
					if (value3.X < value2.X)
					{
						point3d = new Point3d(value3.X - num5, value3.Y, value3.Z);
						position = new Point3d(point3d.X + 10.0, point3d.Y + 50.0, value3.Z);
					}
				}
				else
				{
					point3d = new Point3d(value3.X, value3.Y + num6, value3.Z);
					if (value3.Y < value2.Y)
					{
						point3d = new Point3d(value3.X, value3.Y - num6, value3.Z);
						position = new Point3d(point3d.X - 50.0, point3d.Y - 10.0, value3.Z);
					}
				}
				dBText.UpgradeOpen();
				dBText.Position = position;
				if (array.Length == 2)
				{
					dBText.TextString = array[0];
					Point3d position2 = new Point3d(position.X, position.Y - (GlobalFunction.g_dTextHieght + 100.0), position.Z);
					if (num == 1)
					{
						position2 = new Point3d(position.X + (GlobalFunction.g_dTextHieght + 100.0), position.Y, position.Z);
					}
					DBText dBText2 = new DBText();
					dBText2.SetDatabaseDefaults(doc.Database);
					dBText2.Position = position2;
					dBText2.TextString = array[1];
					dBText2.Height = num4;
					dBText2.Rotation = rotation;
					GlobalFunction.AppendEntity(doc.Database, dBText2, "ST_TextNote", num2);
				}
				if (array.Length >= 3)
				{
					int num7 = array.Length / 2;
					if (array.Length % 2 != 0)
					{
						num7++;
					}
					int num8 = 0;
					for (int num9 = num7 - 1; num9 >= 0; num9--)
					{
						Point3d position3 = new Point3d(position.X, position.Y + (double)num9 * (GlobalFunction.g_dTextHieght + 50.0), position.Z);
						if (num == 1)
						{
							position3 = new Point3d(position.X - (double)num9 * (GlobalFunction.g_dTextHieght + 50.0), position.Y, position.Z);
						}
						DBText dBText3 = new DBText();
						dBText3.SetDatabaseDefaults(doc.Database);
						dBText3.Position = position3;
						dBText3.TextString = array[num8];
						dBText3.Height = num4;
						dBText3.Rotation = rotation;
						GlobalFunction.AppendEntity(doc.Database, dBText3, "ST_TextNote", num2);
						num8++;
					}
					num8 = 1;
					for (int j = num7; j < array.Length; j++)
					{
						Point3d position4 = new Point3d(position.X, position.Y - (double)num8 * (GlobalFunction.g_dTextHieght + 100.0), position.Z);
						if (num == 1)
						{
							position4 = new Point3d(position.X + (double)num8 * (GlobalFunction.g_dTextHieght + 100.0), position.Y, position.Z);
						}
						DBText dBText4 = new DBText();
						dBText4.SetDatabaseDefaults(doc.Database);
						dBText4.Position = position4;
						dBText4.TextString = array[j];
						dBText4.Height = num4;
						dBText4.Rotation = rotation;
						GlobalFunction.AppendEntity(doc.Database, dBText4, "ST_TextNote", num2);
						num8++;
					}
					dBText.Erase();
				}
				dBText.Dispose();
				dBText = null;
				double num10 = 120.0;
				double num11 = value3.DistanceTo(point3d) / 2.0;
				if (num10 > num11)
				{
					num10 = num11;
				}
				Leader leader = new Leader();
				leader.SetDatabaseDefaults(doc.Database);
				leader.AppendVertex(value2);
				leader.AppendVertex(value3);
				leader.AppendVertex(point3d);
				leader.Dimldrblk = arrowObjectId;
				leader.Dimasz = num10;
				value4 = GlobalFunction.AppendEntity(doc.Database, leader, "ST_TextNote", num2);
				continue;
			}
			DBText dBText5 = new DBText();
			dBText5.Normal = editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
			dBText5.TextString = text;
			dBText5.ColorIndex = num2;
			dBText5.Height = num4;
			TextPlacementJig jig = new TextPlacementJig(dBText5, num4);
			PromptStatus promptStatus = PromptStatus.Keyword;
			while (promptStatus == PromptStatus.Keyword)
			{
				PromptResult promptResult = editor.Drag(jig);
				promptStatus = promptResult.Status;
				if (promptStatus != PromptStatus.OK && promptStatus != PromptStatus.Keyword)
				{
					dBText5.Dispose();
					dBText5 = null;
					break;
				}
			}
			GlobalFunction.AppendEntity(doc.Database, dBText5, "ST_TextNote", num2);
		}
	}
}
