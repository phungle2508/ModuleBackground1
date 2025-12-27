using System;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CDrawLeader
{
	public void cmdDrawLeader(string sNameSym)
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		ObjectId arrowObjectId = GlobalFunction.GetArrowObjectId("_OPEN30");
		Editor editor = mdiActiveDocument.Editor;
		string keitenIniFile = "";
		bool flag = GlobalFunction.existTSVConfigFile(out keitenIniFile);
		int ncolor = 4;
		if (flag)
		{
			CIniFile cIniFile = new CIniFile(keitenIniFile);
			string value = cIniFile.GetValue("NoteTexts", "Color", "");
			try
			{
				ncolor = int.Parse(value);
			}
			catch (Exception)
			{
				ncolor = 4;
			}
		}
		while (true)
		{
			PromptEntityOptions promptEntityOptions = new PromptEntityOptions("\nSelect Symbol:");
			promptEntityOptions.SetRejectMessage("\nSelect Symbol");
			promptEntityOptions.AddAllowedClass(typeof(BlockReference), match: true);
			promptEntityOptions.AllowNone = true;
			PromptEntityResult entity = editor.GetEntity(promptEntityOptions);
			if (entity.Status == PromptStatus.Cancel)
			{
				break;
			}
			if (entity.Status != PromptStatus.OK)
			{
				continue;
			}
			BlockReference blockReference = entity.ObjectId.GetObject(OpenMode.ForRead) as BlockReference;
			if (string.Compare(sNameSym, blockReference.Name, ignoreCase: true) != 0)
			{
				blockReference.Dispose();
				blockReference = null;
				string message = "\nPlease select symbol " + sNameSym;
				editor.WriteMessage(message);
				continue;
			}
			Point3d point3d = GlobalFunction.PointCenter(blockReference);
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nDraw Leader:";
			promptPointOptions.UseBasePoint = true;
			promptPointOptions.BasePoint = point3d;
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status == PromptStatus.OK)
			{
				Point3d value2 = point.Value;
				DBText dBText = new DBText();
				dBText.SetDatabaseDefaults(mdiActiveDocument.Database);
				dBText.Position = new Point3d(value2.X + 100.0, value2.Y + 50.0, value2.Z);
                dBText.TextString = GlobalFunction.g_CurrentTextInput;
                dBText.Height = 110.0;
				string value3 = GlobalFunction.AppendEntity(mdiActiveDocument.Database, dBText, "STDIM", ncolor);
				dBText = mdiActiveDocument.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value3, 16)), 0).GetObject(OpenMode.ForRead) as DBText;
				Point3d minPoint = dBText.GeometricExtents.MinPoint;
				Point3d maxPoint = dBText.GeometricExtents.MaxPoint;
				double num = Math.Abs(maxPoint.X - minPoint.X) + 200.0;
				Point3d point3d2 = new Point3d(value2.X + num, value2.Y, value2.Z);
				if (value2.X < point3d.X)
				{
					point3d2 = new Point3d(value2.X - num, value2.Y, value2.Z);
					dBText.UpgradeOpen();
					dBText.Position = new Point3d(point3d2.X + 100.0, point3d2.Y + 50.0, value2.Z);
				}
				dBText.Dispose();
				dBText = null;
				double num2 = 1.0;
				double num3 = 0.18;
				num2 = (double)Application.GetSystemVariable("DIMSCALE");
				num3 = (double)Application.GetSystemVariable("DIMASZ");
				double num4 = num3 * num2;
				double num5 = value2.DistanceTo(point3d2) / 2.0;
				if (num4 > num5)
				{
					num4 = num5;
				}
				Leader leader = new Leader();
				leader.SetDatabaseDefaults(mdiActiveDocument.Database);
				leader.AppendVertex(point3d);
				leader.AppendVertex(value2);
				leader.AppendVertex(point3d2);
				leader.Dimldrblk = arrowObjectId;
				leader.Dimasz = 120.0;
				value3 = GlobalFunction.AppendEntity(mdiActiveDocument.Database, leader, "STDIM", ncolor);
				double num6 = Math.Abs(maxPoint.Y - minPoint.Y) + 100.0;
				Line line = new Line();
				line.SetDatabaseDefaults(mdiActiveDocument.Database);
				line.StartPoint = value2;
				line.EndPoint = new Point3d(value2.X, value2.Y + num6, value2.Z);
				GlobalFunction.AppendEntity(mdiActiveDocument.Database, line, "STDIM", ncolor);
				Line line2 = new Line();
				line2.SetDatabaseDefaults(mdiActiveDocument.Database);
				line2.StartPoint = point3d2;
				line2.EndPoint = new Point3d(point3d2.X, point3d2.Y + num6, point3d2.Z);
				GlobalFunction.AppendEntity(mdiActiveDocument.Database, line2, "STDIM", ncolor);
				Line line3 = new Line();
				line3.SetDatabaseDefaults(mdiActiveDocument.Database);
				line3.StartPoint = new Point3d(value2.X, value2.Y + num6, value2.Z);
				line3.EndPoint = new Point3d(point3d2.X, point3d2.Y + num6, point3d2.Z);
				GlobalFunction.AppendEntity(mdiActiveDocument.Database, line3, "STDIM", ncolor);
			}
		}
	}
}
