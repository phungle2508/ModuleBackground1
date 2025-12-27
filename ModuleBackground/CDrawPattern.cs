using System;
using System.IO;
using System.Windows.Forms;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CDrawPattern
{
	public const string S_NameFolderSym = "\\SurfaceCut";

	public void cmdDrawPattern(Document doc)
	{
		Editor editor = doc.Editor;
		double dDist = 0.0;
		double dDist2 = 0.0;
		int nCase = 0;
		while (true)
		{
			editor.WriteMessage("\n");
			FormSettingPattern formSettingPattern = new FormSettingPattern();
			formSettingPattern.dDist1 = dDist;
			formSettingPattern.dDist2 = dDist2;
			formSettingPattern.nCase = nCase;
			Bricscad.ApplicationServices.Application.ShowModalDialog(Bricscad.ApplicationServices.Application.MainWindow, formSettingPattern, persist: false);
			if (DialogResult.OK != formSettingPattern.DialogResult)
			{
				break;
			}
			nCase = formSettingPattern.nCase;
			if (formSettingPattern.nCase == 1)
			{
				double dDist3 = 0.0;
				if (GetDistance(doc, ref dDist3))
				{
					dDist = dDist3;
				}
				continue;
			}
			if (formSettingPattern.nCase == 2)
			{
				double dDist4 = 0.0;
				if (GetDistance(doc, ref dDist4))
				{
					dDist2 = dDist4;
				}
				continue;
			}
			dDist = formSettingPattern.dDist1;
			dDist2 = formSettingPattern.dDist2;
			string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
			_ = pathFolderSymbol + "\\SurfaceCut\\";
			string fileName = Path.Combine(pathFolderSymbol + "\\SurfaceCut", "TemplateOut.dwg");
			Database db = new Database(buildDefaultDrawing: false, noDocument: true);
			db.ReadDwgFile(fileName, FileOpenMode.OpenForReadAndReadShare, allowCPConversion: true, "");
			db.CloseInput(closeFile: true);
			Bricscad.ApplicationServices.Application.SetSystemVariable("LWDISPLAY", 1);
			AddPattern(ref db, dDist, dDist2);
			ObjectId blockTableRecord = doc.Database.Insert("*U", db, preserveSourceDatabase: true);
			db.Dispose();
			db = null;
			BlockReference blockReference = new BlockReference(Point3d.Origin, blockTableRecord);
			blockReference.SetDatabaseDefaults(doc.Database);
			blockReference.Normal = editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
			if (BlockMoveRotateJig.JigOnly(blockReference, "\nSelect base point Pattern"))
			{
				string value = GlobalFunction.AppendEntity(doc.Database, blockReference, "0", 256);
				ObjectId objectId = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
				if (objectId.IsNull)
				{
					break;
				}
				blockReference = objectId.GetObject(OpenMode.ForWrite) as BlockReference;
				blockReference.ExplodeToOwnerSpace();
				blockReference.Erase();
				blockReference.Dispose();
				blockReference = null;
			}
			else
			{
				blockReference.Dispose();
				blockReference = null;
			}
		}
	}

	public bool GetDistance(Document doc, ref double dDist)
	{
		_ = doc.Database;
		Editor editor = doc.Editor;
		bool result = false;
		try
		{
			PromptPointResult point = editor.GetPoint("\nSelect Point Base: ");
			if (point.Status != PromptStatus.OK)
			{
				return result;
			}
			PromptDistanceOptions promptDistanceOptions = new PromptDistanceOptions("\nDistance: ");
			promptDistanceOptions.BasePoint = point.Value;
			promptDistanceOptions.UseBasePoint = true;
			PromptDoubleResult distance = editor.GetDistance(promptDistanceOptions);
			if (distance.Status != PromptStatus.OK)
			{
				return result;
			}
			dDist = distance.Value;
			if (dDist > 0.0)
			{
				result = true;
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public void AddPattern(ref Database db, double dDist1, double dDist2)
	{
		Point3d startPoint = new Point3d(-130.0, 130.0, 0.0);
		Line line = new Line();
		line.SetDatabaseDefaults(db);
		line.StartPoint = startPoint;
		line.EndPoint = new Point3d(startPoint.X + dDist1, startPoint.Y, startPoint.Z);
		string value = GlobalFunction.AppendEntity(db, line, "0", 3);
		line = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0).GetObject(OpenMode.ForWrite) as Line;
		line.LineWeight = LineWeight.LineWeight035;
		line.Dispose();
		Line line2 = new Line();
		line2.SetDatabaseDefaults(db);
		line2.StartPoint = startPoint;
		line2.EndPoint = new Point3d(startPoint.X, startPoint.Y - dDist2, startPoint.Z);
		line2.LineWeight = LineWeight.LineWeight035;
		value = GlobalFunction.AppendEntity(db, line2, "0", 3);
		line2 = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0).GetObject(OpenMode.ForWrite) as Line;
		line2.LineWeight = LineWeight.LineWeight035;
		line2.Dispose();
	}
}
