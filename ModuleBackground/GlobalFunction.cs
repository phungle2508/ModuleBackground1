using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bricscad.ApplicationServices;
using Teigha.Colors;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public static class GlobalFunction
{
    public static string g_CurrentTextInput = "HD15KN";
    public const string S_Section = "SystemBackground";

	public static FormMenu dlg1 = null;

	public static double g_dTextHieght = 125.0;

	public static int g_nFullOsnap = 16383;

	public static string S_layerWall = "_0-0_デ__タ_001";

	public static int nColorWall = 3;

	public static string S_layerLineHatch = "_0-0_デ__タ_001";

	public static int nColorLineHatch = 5;

	public static string S_layerLineIntersect = "ST_LineIntersect";

	public static int nColorLineIntersect = 4;

	public static string S_layerLineSquare = "_0-0_デ__タ_001";

	public static int nColorLineSquare = 7;

	public static double g_dRadiusSquare = 285.0;

	public static void Swap<T>(ref T x, ref T y)
	{
		T val = y;
		y = x;
		x = val;
	}

	public static string GetPathFolderBinary()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		return Path.GetDirectoryName(executingAssembly.Location);
	}

	public static string GetPathFolderIni()
	{
		string pathFolderBinary = GetPathFolderBinary();
		return pathFolderBinary + "\\ini";
	}

	public static string GetPathFolderSymbol()
	{
		string pathFolderBinary = GetPathFolderBinary();
		return pathFolderBinary + "\\Symbol";
	}

	public static void InitIni()
	{
		string pathFolderIni = GetPathFolderIni();
		string text = pathFolderIni + "\\Background.ini";
		if (!File.Exists(text))
		{
			return;
		}
		CIniFile cIniFile = new CIniFile(text);
		List<string> lstValues = new List<string>();
		cIniFile.GetValues("SystemBackground", "LineWall", ref lstValues);
		if (lstValues.Count != 0)
		{
			S_layerWall = lstValues[0];
			try
			{
				if (lstValues.Count >= 2)
				{
					nColorWall = int.Parse(lstValues[1]);
				}
			}
			catch (Exception)
			{
			}
		}
		cIniFile.GetValues("SystemBackground", "LineHatch", ref lstValues);
		if (lstValues.Count != 0)
		{
			S_layerLineHatch = lstValues[0];
			try
			{
				if (lstValues.Count >= 2)
				{
					nColorLineHatch = int.Parse(lstValues[1]);
				}
			}
			catch (Exception)
			{
			}
		}
		cIniFile.GetValues("SystemBackground", "LineIntersect", ref lstValues);
		if (lstValues.Count != 0)
		{
			S_layerLineIntersect = lstValues[0];
			try
			{
				if (lstValues.Count >= 2)
				{
					nColorLineIntersect = int.Parse(lstValues[1]);
				}
			}
			catch (Exception)
			{
			}
		}
		cIniFile.GetValues("SystemBackground", "LineSquare", ref lstValues);
		if (lstValues.Count == 0)
		{
			return;
		}
		S_layerLineSquare = lstValues[0];
		try
		{
			if (lstValues.Count >= 2)
			{
				nColorLineSquare = int.Parse(lstValues[1]);
			}
		}
		catch (Exception)
		{
		}
	}

	public static void CreateLayer(Database db, string strLayerName, short nColor)
	{
		using LayerTable layerTable = db.LayerTableId.GetObject(OpenMode.ForRead) as LayerTable;
		if (layerTable.Has(strLayerName))
		{
			return;
		}
		using LayerTableRecord layerTableRecord = new LayerTableRecord();
		try
		{
			layerTableRecord.Color = Color.FromColorIndex(ColorMethod.ByAci, nColor);
		}
		catch (Exception)
		{
		}
		layerTableRecord.Name = strLayerName;
		layerTable.UpgradeOpen();
		layerTable.Add(layerTableRecord);
	}

	public static string AppendEntity(Database db, Entity ent, string sLayer, int ncolor)
	{
		CreateLayer(db, sLayer, (short)ncolor);
		string result = "";
		using (BlockTableRecord blockTableRecord = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord)
		{
			if (ent != null)
			{
				ObjectId objectId = blockTableRecord.AppendEntity(ent);
				ent.Layer = sLayer;
				ent.ColorIndex = ncolor;
				result = objectId.Handle.ToString();
				ent.Dispose();
			}
		}
		return result;
	}

	public static string AppendEntityWall(Database db, Entity ent)
	{
		return AppendEntity(db, ent, S_layerWall, nColorWall);
	}

	public static ObjectId AddWallToModelSpace(Database db, Entity ent)
	{
		CreateLayer(db, S_layerWall, (short)nColorWall);
		Teigha.DatabaseServices.TransactionManager transactionManager = db.TransactionManager;
		using Transaction transaction = transactionManager.StartTransaction();
		BlockTable blockTable = transaction.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
		if (blockTable == null)
		{
			throw new NullReferenceException("blockTable == null");
		}
		BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
		if (blockTableRecord == null)
		{
			throw new NullReferenceException("blockTableRecord == null");
		}
		ObjectId result = blockTableRecord.AppendEntity(ent);
		ent.Layer = S_layerWall;
		ent.ColorIndex = nColorWall;
		transaction.AddNewlyCreatedDBObject(ent, add: true);
		transaction.Commit();
		return result;
	}

	public static string AppendEntityLineHatch(Database db, Entity ent)
	{
		return AppendEntity(db, ent, S_layerLineHatch, nColorLineHatch);
	}

	public static string AppendBlockIntersect(Database db, Entity ent)
	{
		return AppendEntity(db, ent, S_layerLineIntersect, nColorLineIntersect);
	}

	public static string AppendBlockNearSquare(Database db, Entity ent)
	{
		return AppendEntity(db, ent, S_layerLineSquare, nColorLineSquare);
	}

	public static void DeleteEntityS(Database db, ObjectIdCollection ids)
	{
		foreach (ObjectId id in ids)
		{
			if (id.IsNull)
			{
				continue;
			}
			try
			{
				using Entity entity = id.GetObject(OpenMode.ForWrite) as Entity;
				entity.Erase();
			}
			catch (Exception)
			{
			}
		}
	}

	public static Point3d Point3dCenter(Line line)
	{
		double x = line.GeometricExtents.MinPoint.X + Math.Abs(line.GeometricExtents.MaxPoint.X - line.GeometricExtents.MinPoint.X) / 2.0;
		double y = line.GeometricExtents.MinPoint.Y + Math.Abs(line.GeometricExtents.MaxPoint.Y - line.GeometricExtents.MinPoint.Y) / 2.0;
		return new Point3d(x, y, 0.0);
	}

	public static Point3d GetMaxPointY(Point3dCollection ptsInterSect)
	{
		Point3d result = ptsInterSect[0];
		double y = result.Y;
		for (int i = 1; i < ptsInterSect.Count; i++)
		{
			Point3d point3d = ptsInterSect[i];
			double y2 = point3d.Y;
			if (y < y2)
			{
				result = point3d;
				y = result.Y;
			}
		}
		return result;
	}

	public static Point3d GetMinPointY(Point3dCollection ptsInterSect)
	{
		Point3d result = ptsInterSect[0];
		double y = result.Y;
		for (int i = 1; i < ptsInterSect.Count; i++)
		{
			Point3d point3d = ptsInterSect[i];
			double y2 = point3d.Y;
			if (y > y2)
			{
				result = point3d;
				y = result.Y;
			}
		}
		return result;
	}

	public static Point3d GetMaxPointX(Point3dCollection ptsInterSect)
	{
		Point3d result = ptsInterSect[0];
		double x = result.X;
		for (int i = 1; i < ptsInterSect.Count; i++)
		{
			Point3d point3d = ptsInterSect[i];
			double x2 = point3d.X;
			if (x < x2)
			{
				result = point3d;
				x = result.X;
			}
		}
		return result;
	}

	public static Point3d GetMinPointX(Point3dCollection ptsInterSect)
	{
		Point3d result = ptsInterSect[0];
		double x = result.X;
		for (int i = 1; i < ptsInterSect.Count; i++)
		{
			Point3d point3d = ptsInterSect[i];
			double x2 = point3d.X;
			if (x > x2)
			{
				result = point3d;
				x = result.X;
			}
		}
		return result;
	}

	public static Point2d asPoint2D(Point3d pt)
	{
		return new Point2d(pt.X, pt.Y);
	}

	public static Point3d PointCenter(Entity ent)
	{
		double x = ent.GeometricExtents.MinPoint.X + Math.Abs(ent.GeometricExtents.MaxPoint.X - ent.GeometricExtents.MinPoint.X) / 2.0;
		double y = ent.GeometricExtents.MinPoint.Y + Math.Abs(ent.GeometricExtents.MaxPoint.Y - ent.GeometricExtents.MinPoint.Y) / 2.0;
		return new Point3d(x, y, 0.0);
	}

	public static bool ptAboveLine(Point3d ptOrg, double dAngle, Point3d ptCheck)
	{
		return ptAboveLine(new Point2d(ptOrg.X, ptOrg.Y), dAngle, new Point2d(ptCheck.X, ptCheck.Y));
	}

	public static bool ptAboveLine(Point2d ptOrg, double dAngle, Point2d ptCheck)
	{
		if (dAngle == Math.PI / 2.0)
		{
			if (ptCheck.X > ptOrg.X)
			{
				return true;
			}
		}
		else
		{
			double num = Math.Tan(dAngle);
			if (ptCheck.Y > (ptCheck.X - ptOrg.X) * num + ptOrg.Y)
			{
				return true;
			}
		}
		return false;
	}

	public static double GetRotation(Vector3d vector, Vector3d normal)
	{
		Plane plane = new Plane(Point3d.Origin, normal);
		return Vector3d.XAxis.TransformBy(Matrix3d.PlaneToWorld(plane)).GetAngleTo(vector.ProjectTo(normal, normal), normal);
	}

	private static void AddRegAppTableRecord(string regAppName, Database db)
	{
		RegAppTable regAppTable = db.RegAppTableId.GetObject(OpenMode.ForRead, openErased: false) as RegAppTable;
		if (!regAppTable.Has(regAppName))
		{
			regAppTable.UpgradeOpen();
			RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
			regAppTableRecord.Name = regAppName;
			regAppTable.Add(regAppTableRecord);
			regAppTableRecord.Dispose();
			regAppTableRecord = null;
		}
		regAppTable.Dispose();
		regAppTable = null;
	}

	public static void SetXdata(ObjectId idObj, string sAppName, string sText)
	{
		DBObject dBObject = idObj.GetObject(OpenMode.ForWrite);
		AddRegAppTableRecord(sAppName, idObj.Database);
		ResultBuffer resultBuffer = (dBObject.XData = new ResultBuffer(new TypedValue(1001, sAppName), new TypedValue(1000, sText)));
		resultBuffer.Dispose();
		dBObject.Dispose();
		dBObject = null;
	}

	public static void GetXdata(ObjectId idObj, string sAppName, ref string sText)
	{
		sText = "";
		DBObject dBObject = idObj.GetObject(OpenMode.ForRead);
		Xrecord xrecord = new Xrecord();
		xrecord.XData = dBObject.GetXDataForApplication(sAppName);
		TypedValue[] array = xrecord.XData.AsArray();
		for (int i = 0; i < array.Length; i++)
		{
			sText = array[i].Value.ToString();
		}
		dBObject.Dispose();
		dBObject = null;
	}

	public static ObjectId GetArrowObjectId(string newArrName)
	{
		ObjectId objectId = ObjectId.Null;
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		Database database = mdiActiveDocument.Database;
		string text = Application.GetSystemVariable("DIMBLK") as string;
		Application.SetSystemVariable("DIMBLK", newArrName);
		if (text.Length != 0)
		{
			Application.SetSystemVariable("DIMBLK", text);
		}
		BlockTable blockTable = (BlockTable)database.BlockTableId.GetObject(OpenMode.ForRead);
		return blockTable[newArrName];
	}

	public static void setLTSCALE()
	{
		object systemVariable = Application.GetSystemVariable("LTSCALE");
		int num = 50;
		try
		{
			num = int.Parse(systemVariable.ToString());
		}
		catch (Exception)
		{
		}
		if (num <= 50)
		{
			Application.SetSystemVariable("LTSCALE", 50);
		}
	}

	public static void setOSNAP(int nOsnap, ref object curOsnap)
	{
		try
		{
			curOsnap = Application.GetSystemVariable("OSMODE");
			Application.SetSystemVariable("OSMODE", nOsnap);
			setLTSCALE();
			Application.DocumentManager.MdiActiveDocument.TransactionManager.QueueForGraphicsFlush();
		}
		catch (Exception)
		{
		}
	}

	public static void revertOSNAP(object curOsnap)
	{
		try
		{
			Application.SetSystemVariable("OSMODE", curOsnap);
			Application.DocumentManager.MdiActiveDocument.TransactionManager.QueueForGraphicsFlush();
		}
		catch (Exception)
		{
		}
	}

	public static bool existTSVConfigFile(out string keitenIniFile)
	{
		bool result = false;
		keitenIniFile = "";
		dynamic acadApplication = Application.AcadApplication;
		string text = acadApplication.Preferences.Files.SupportPath.ToString();
		string[] array = text.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			keitenIniFile = array[i] + "\\TSVConfig.ini";
			if (File.Exists(keitenIniFile))
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
