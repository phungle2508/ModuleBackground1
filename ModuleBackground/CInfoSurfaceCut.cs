using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CInfoSurfaceCut
{
	public const string S_NameFolderSym = "\\SurfaceCut";

	public const double dPicth = 150.0;

	public double dHeightHacth = 175.0;

	public int m_nCaseMR = 1;

	public int m_nCaseS = 1;

	public void GetListInfo(ref List<CInfoObjCut> lstInfo)
	{
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string path = pathFolderSymbol + "\\SurfaceCut\\Image";
		string[] files = Directory.GetFiles(path, "*.JPG");
		Array.Sort(files, new AlphanumComparatorFast());
		string[] array = files;
		foreach (string text in array)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
			CInfoObjCut item = new CInfoObjCut(fileNameWithoutExtension, text);
			lstInfo.Add(item);
		}
	}

	public void UpdateLine(ObjectId idPoly, ObjectId idLine)
	{
		Polyline polyline = idPoly.GetObject(OpenMode.ForRead) as Polyline;
		Line line = idLine.GetObject(OpenMode.ForRead) as Line;
		Point3d startPoint = line.StartPoint;
		Point3d endPoint = line.EndPoint;
		Point3d closestPointTo = polyline.GetClosestPointTo(startPoint, extend: false);
		double num = startPoint.DistanceTo(closestPointTo);
		closestPointTo = polyline.GetClosestPointTo(endPoint, extend: false);
		double num2 = endPoint.DistanceTo(closestPointTo);
		if (num <= 0.001 && num2 <= 0.001)
		{
			line.Dispose();
			line = null;
			polyline.Dispose();
			polyline = null;
			return;
		}
		Point3dCollection point3dCollection = new Point3dCollection();
		line.IntersectWith(polyline, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
		if (point3dCollection.Count == 0)
		{
			line.Dispose();
			line = null;
			polyline.Dispose();
			polyline = null;
			return;
		}
		line.UpgradeOpen();
		if (point3dCollection.Count == 1)
		{
			if (point3dCollection.Contains(startPoint))
			{
				line.StartPoint = startPoint;
				line.EndPoint = point3dCollection[0];
			}
			if (point3dCollection.Contains(endPoint))
			{
				line.StartPoint = point3dCollection[0];
				line.EndPoint = endPoint;
			}
		}
		else
		{
			line.StartPoint = point3dCollection[0];
			line.EndPoint = point3dCollection[1];
		}
		line.Dispose();
		line = null;
		polyline.Dispose();
		polyline = null;
	}

	public void drawPolyBackGround(Document doc, ObjectId idLine, Point3d ptDirect, ref ObjectIdCollection idsObj, int nDraw = 0)
	{
		Matrix3d currentUserCoordinateSystem = doc.Editor.CurrentUserCoordinateSystem;
		Line line = idLine.GetObject(OpenMode.ForRead) as Line;
		_ = line.StartPoint;
		_ = line.EndPoint;
		Point3d point3d = GlobalFunction.Point3dCenter(line);
		double num = ((line.Angle >= Math.PI) ? (line.Angle - Math.PI) : line.Angle);
		_ = line.Length;
		Point3d center = point3d.TransformBy(currentUserCoordinateSystem);
		Matrix3d matrix3d = Matrix3d.Rotation(-1.0 * num, currentUserCoordinateSystem.CoordinateSystem3d.Zaxis, center);
		line.UpgradeOpen();
		line.TransformBy(matrix3d);
		ptDirect = ptDirect.TransformBy(matrix3d);
		Point3d minPoint = line.GeometricExtents.MinPoint;
		Point3d maxPoint = line.GeometricExtents.MaxPoint;
		line.Erase();
		line.Dispose();
		line = null;
		int num2 = 0;
		if (ptDirect.Y > minPoint.Y)
		{
			num2 = 1;
		}
		Polyline polyline = new Polyline();
		polyline.SetDatabaseDefaults(doc.Database);
		polyline.AddVertexAt(0, new Point2d(minPoint.X, minPoint.Y), 0.0, -1.0, -1.0);
		Point3d point3d2 = new Point3d(minPoint.X + 250.0, minPoint.Y - 250.0, minPoint.Z);
		if (num2 == 1)
		{
			point3d2 = new Point3d(minPoint.X + 250.0, minPoint.Y + 250.0, minPoint.Z);
		}
		polyline.AddVertexAt(1, new Point2d(point3d2.X, point3d2.Y), 0.0, -1.0, -1.0);
		Point3d point3d3 = new Point3d(maxPoint.X - 250.0, maxPoint.Y - 250.0, minPoint.Z);
		if (num2 == 1)
		{
			point3d3 = new Point3d(maxPoint.X - 250.0, maxPoint.Y + 250.0, minPoint.Z);
		}
		polyline.AddVertexAt(2, new Point2d(point3d3.X, point3d3.Y), 0.0, -1.0, -1.0);
		polyline.AddVertexAt(3, new Point2d(maxPoint.X, maxPoint.Y), 0.0, -1.0, -1.0);
		polyline.TransformBy(matrix3d.Inverse());
		string value = GlobalFunction.AppendEntity(doc.Database, polyline, "ST_InfoSufaceCut", 3);
		ObjectId objectId = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
		if (objectId.IsNull)
		{
			return;
		}
		idsObj.Add(objectId);
		if (nDraw != 1 && nDraw != 2)
		{
			return;
		}
		string[] array = new string[2] { "ST_DASHED_1", "dashed" };
		for (int i = 0; i < 2; i++)
		{
			bool flag = false;
			LinetypeTable linetypeTable = doc.Database.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
			if (!linetypeTable.Has(array[i]))
			{
				try
				{
					string pathFolderBinary = GlobalFunction.GetPathFolderBinary();
					string filename = $"{pathFolderBinary}\\ST.lin";
					doc.Database.LoadLineTypeFile(array[i], filename);
				}
				catch (Exception)
				{
					flag = true;
				}
			}
			linetypeTable.Dispose();
			linetypeTable = null;
			if (flag)
			{
				array[i] = "byBlock";
			}
		}
		if (nDraw == 2)
		{
			dHeightHacth = 150.0;
		}
		else
		{
			dHeightHacth = 175.0;
		}
		Polyline polyline2 = new Polyline();
		polyline2.SetDatabaseDefaults(doc.Database);
		polyline2.AddVertexAt(0, new Point2d(minPoint.X, minPoint.Y), 0.0, -1.0, -1.0);
		Point2d pt = new Point2d(minPoint.X, minPoint.Y + dHeightHacth);
		if (num2 == 1)
		{
			pt = new Point2d(minPoint.X, minPoint.Y - dHeightHacth);
		}
		polyline2.AddVertexAt(1, pt, 0.0, -1.0, -1.0);
		polyline2.AddVertexAt(2, new Point2d(maxPoint.X, pt.Y), 0.0, -1.0, -1.0);
		polyline2.AddVertexAt(3, new Point2d(maxPoint.X, minPoint.Y), 0.0, -1.0, -1.0);
		polyline2.Linetype = array[0];
		polyline2.TransformBy(matrix3d.Inverse());
		polyline2.Closed = true;
		string value2 = GlobalFunction.AppendEntity(doc.Database, polyline2, "ST_InfoSufaceCut", 3);
		ObjectId objectId2 = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value2, 16)), 0);
		if (objectId2.IsNull)
		{
			return;
		}
		idsObj.Add(objectId2);
		for (double num3 = minPoint.X + 150.0; num3 <= maxPoint.X + 150.0; num3 += 150.0)
		{
			Line line2 = new Line();
			line2.SetDatabaseDefaults(doc.Database);
			line2.StartPoint = new Point3d(num3, minPoint.Y, minPoint.Z);
			line2.EndPoint = new Point3d(num3 - 150.0, pt.Y, minPoint.Z);
			line2.Linetype = array[1];
			line2.TransformBy(matrix3d.Inverse());
			string value3 = GlobalFunction.AppendEntity(doc.Database, line2, "ST_InfoSufaceCut", 3);
			if (!string.IsNullOrEmpty(value3))
			{
				ObjectId objectId3 = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value3, 16)), 0);
				if (!objectId3.IsNull)
				{
					UpdateLine(objectId2, objectId3);
					idsObj.Add(objectId3);
				}
			}
		}
	}

	public void cmdInputSurfaceCutDlg(Document doc)
	{
		Editor editor = doc.Editor;
		List<CInfoObjCut> lstInfo = new List<CInfoObjCut>();
		GetListInfo(ref lstInfo);
		while (true)
		{
			FormSettingInputOther formSettingInputOther = new FormSettingInputOther();
			Bricscad.ApplicationServices.Application.ShowModalDialog(Bricscad.ApplicationServices.Application.MainWindow, formSettingInputOther, persist: false);
			if (DialogResult.OK != formSettingInputOther.DialogResult)
			{
				break;
			}
			string textString = "";
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nSelect start point:";
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				continue;
			}
			Point3d value = point.Value;
			Line line;
			while (true)
			{
				promptPointOptions.Message = "\nSelect end point:";
				promptPointOptions.UseBasePoint = true;
				promptPointOptions.BasePoint = value;
				promptPointOptions.AllowNone = true;
				point = editor.GetPoint(promptPointOptions);
				if (point.Status == PromptStatus.OK)
				{
					line = new Line();
					line.SetDatabaseDefaults(doc.Database);
					line.StartPoint = value;
					line.EndPoint = point.Value;
					if (!(line.Length <= 300.0))
					{
						break;
					}
					line.Dispose();
					line = null;
				}
			}
			string value2 = GlobalFunction.AppendEntity(doc.Database, line, "0", 1);
			doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value2, 16)), 0);
			promptPointOptions.Message = "\nSelect direction:";
			promptPointOptions.UseBasePoint = false;
			promptPointOptions.AllowNone = true;
			point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				continue;
			}
			_ = point.Value;
			DBText dBText = new DBText();
			dBText.Normal = editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
			dBText.TextString = textString;
			TextPlacementJig jig = new TextPlacementJig(dBText);
			PromptStatus promptStatus = PromptStatus.Keyword;
			while (promptStatus == PromptStatus.Keyword)
			{
				PromptResult promptResult = editor.Drag(jig);
				promptStatus = promptResult.Status;
				if (promptStatus != PromptStatus.OK && promptStatus != PromptStatus.Keyword)
				{
					return;
				}
			}
			GlobalFunction.AppendEntity(doc.Database, dBText, "ST_InfoSufaceCut", 4);
		}
	}

	public void cmdInputSurfaceCut(Document doc)
	{
		List<string> list = new List<string>();
		list.Add("FG2a");
		list.Add("FG2b");
		list.Add("FG2c");
		list.Add("FG2S");
		list.Add("FG2Sa");
		list.Add("FG2Sb");
		list.Add("FG2Sc");
		list.Add("FG3");
		list.Add("FG3a");
		list.Add("FG4");
		list.Add("FG4a");
		list.Add("FG4b");
		list.Add("FG5");
		list.Add("FG5a");
		list.Add("FG5b");
		list.Add("FG5c");
		list.Add("FG20 外部");
		list.Add("FG20 内部");
		list.Add("FG21");
		list.Add("配管貫通部（増し打ち）");
		object systemVariable = Bricscad.ApplicationServices.Application.GetSystemVariable("LTSCALE");
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
			Bricscad.ApplicationServices.Application.SetSystemVariable("LTSCALE", 50);
		}
		Bricscad.ApplicationServices.Application.GetSystemVariable("OSMODE");
		Editor editor = doc.Editor;
		int nSel = 0;
		while (true)
		{
			editor.WriteMessage("\n");
			FormSettingInputSurfaceCut formSettingInputSurfaceCut = new FormSettingInputSurfaceCut();
			formSettingInputSurfaceCut.m_nSel = nSel;
			Bricscad.ApplicationServices.Application.ShowModalDialog(Bricscad.ApplicationServices.Application.MainWindow, formSettingInputSurfaceCut, persist: true);
			if (DialogResult.OK != formSettingInputSurfaceCut.DialogResult)
			{
				break;
			}
			string text = formSettingInputSurfaceCut.m_sText;
			nSel = formSettingInputSurfaceCut.m_nSel;
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.AllowNone = true;
			while (true)
			{
				promptPointOptions.Message = "\nSelect start point:";
				PromptPointResult point = editor.GetPoint(promptPointOptions);
				if (point.Status != PromptStatus.OK)
				{
					break;
				}
				Point3d value = point.Value;
				string value2 = "";
				bool flag = false;
				while (true)
				{
					PromptPointOptions promptPointOptions2 = new PromptPointOptions("");
					promptPointOptions2.Message = "\nSelect end point:";
					promptPointOptions2.UseBasePoint = true;
					promptPointOptions2.BasePoint = value;
					point = editor.GetPoint(promptPointOptions2);
					if (point.Status != PromptStatus.OK)
					{
						flag = true;
						break;
					}
					Point3d value3 = point.Value;
					Line line = new Line();
					line.SetDatabaseDefaults(doc.Database);
					line.StartPoint = value;
					line.EndPoint = value3;
					if (line.Length <= 300.0)
					{
						line.Dispose();
						line = null;
						continue;
					}
					value2 = GlobalFunction.AppendEntity(doc.Database, line, "0", 1);
					break;
				}
				if (flag)
				{
					continue;
				}
				ObjectId objectId = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value2, 16)), 0);
				editor.Regen();
				promptPointOptions.Message = "\nSelect direction:";
				promptPointOptions.UseBasePoint = false;
				promptPointOptions.AllowNone = true;
				point = editor.GetPoint(promptPointOptions);
				if (point.Status != PromptStatus.OK)
				{
					ObjectIdCollection objectIdCollection = new ObjectIdCollection();
					objectIdCollection.Add(objectId);
					GlobalFunction.DeleteEntityS(doc.Database, objectIdCollection);
					continue;
				}
				Point3d value4 = point.Value;
				int num2 = 0;
				if (string.Compare(text, "配管貫通部（増し打ち）", ignoreCase: true) == 0)
				{
					num2 = 1;
				}
				else if (text.IndexOf("FG2S") != -1 || text.IndexOf("FG3") != -1)
				{
					num2 = 2;
				}
				ObjectIdCollection idsObj = new ObjectIdCollection();
				drawPolyBackGround(doc, objectId, value4, ref idsObj, num2);
				string text2 = "";
				int num3 = text.IndexOf("FG20");
				if (num3 != -1)
				{
					text2 = text;
					text = "FG20";
				}
				DBText dBText = new DBText();
				dBText.Normal = editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
				dBText.TextString = text;
				dBText.ColorIndex = 1;
				TextPlacementJig jig = new TextPlacementJig(dBText, 110.0);
				PromptStatus promptStatus = PromptStatus.Keyword;
				while (promptStatus == PromptStatus.Keyword)
				{
					PromptResult promptResult = editor.Drag(jig);
					promptStatus = promptResult.Status;
					if (promptStatus != PromptStatus.OK && promptStatus != PromptStatus.Keyword)
					{
						dBText.Dispose();
						dBText = null;
						GlobalFunction.DeleteEntityS(doc.Database, idsObj);
						return;
					}
				}
				if (num2 == 1 || num2 == 2)
				{
					Point3d position = dBText.Position;
					value2 = GlobalFunction.AppendEntity(doc.Database, dBText, "ST_InfoSufaceCut", 3);
					idsObj.Add(doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value2, 16)), 0));
					using (Database database = new Database(buildDefaultDrawing: true, noDocument: false))
					{
						doc.Database.Wblock(database, idsObj, Point3d.Origin, DuplicateRecordCloning.Ignore);
						ObjectId blockTableRecord = doc.Database.Insert("*U", database, preserveSourceDatabase: true);
						Matrix3d transform = Matrix3d.Displacement(Point3d.Origin - position);
						BlockTableRecord blockTableRecord2 = blockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord;
						foreach (ObjectId item in blockTableRecord2)
						{
							Entity entity = item.GetObject(OpenMode.ForWrite) as Entity;
							entity.TransformBy(transform);
							entity.Dispose();
						}
						blockTableRecord2.Dispose();
						BlockReference blockReference = new BlockReference(position, blockTableRecord);
						blockReference.SetDatabaseDefaults(doc.Database);
						GlobalFunction.AppendEntity(doc.Database, blockReference, "ST_InfoSufaceCut", 3);
					}
					GlobalFunction.DeleteEntityS(doc.Database, idsObj);
				}
				else
				{
					value2 = GlobalFunction.AppendEntity(doc.Database, dBText, "ST_InfoSufaceCut", 3);
					if (!string.IsNullOrEmpty(text2))
					{
						ObjectId objectId2 = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value2, 16)), 0);
						GlobalFunction.SetXdata(objectId2, "AppSufaceCut", text2);
					}
				}
			}
		}
	}

	public bool checkDrawingIsOpen(string pathFile)
	{
		DocumentCollection documentManager = Bricscad.ApplicationServices.Application.DocumentManager;
		foreach (Document item in documentManager)
		{
			if (string.Compare(pathFile, item.Name, ignoreCase: true) == 0)
			{
				return true;
			}
		}
		return false;
	}

	private static void zoom(Editor editor, Extents3d ext)
	{
		using ViewTableRecord viewTableRecord = editor.GetCurrentView();
		Matrix3d mat = (Matrix3d.Rotation(0.0 - viewTableRecord.ViewTwist, viewTableRecord.ViewDirection, viewTableRecord.Target) * Matrix3d.Displacement(viewTableRecord.Target - Point3d.Origin) * Matrix3d.PlaneToWorld(viewTableRecord.ViewDirection)).Inverse();
		ext.TransformBy(mat);
		Point3d minPoint = ext.MinPoint;
		Point3d maxPoint = ext.MaxPoint;
		viewTableRecord.Width = maxPoint.X - minPoint.X;
		viewTableRecord.Height = maxPoint.Y - minPoint.Y;
		viewTableRecord.CenterPoint = new Point2d((minPoint.X + maxPoint.X) / 2.0, (minPoint.Y + maxPoint.Y) / 2.0);
		editor.SetCurrentView(viewTableRecord);
	}

	public void GetTextInfoSurface(Document doc, ref List<string> lstNameFile)
	{
		Editor editor = doc.Editor;
		_ = doc.Database;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "text"),
			new TypedValue(8, "ST_InfoSufaceCut")
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return;
		}
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId idObj = objectIds[i];
			DBText dBText = (DBText)idObj.GetObject(OpenMode.ForRead);
			if (dBText == null)
			{
				continue;
			}
			string textString = dBText.TextString;
			dBText.Dispose();
			dBText = null;
			if (string.IsNullOrEmpty(textString))
			{
				break;
			}
			textString = textString.Replace("f", "F");
			textString = textString.Replace("g", "G");
			textString = textString.Replace("s", "S");
			int num = textString.IndexOf("FG20");
			if (num != -1)
			{
				string sText = "";
				GlobalFunction.GetXdata(idObj, "AppSufaceCut", ref sText);
				if (string.IsNullOrEmpty(sText))
				{
					continue;
				}
				textString = sText;
				textString = textString.Replace("f", "F");
				textString = textString.Replace("g", "G");
			}
			if (!lstNameFile.Contains(textString))
			{
				lstNameFile.Add(textString);
			}
		}
	}

	public static string CreateNewBlockDynamic(Database db, string strDwgFile)
	{
		Path.GetFileNameWithoutExtension(strDwgFile);
		if (File.Exists(strDwgFile))
		{
			using (Database database = new Database(buildDefaultDrawing: false, noDocument: true))
			{
				database.ReadDwgFile(strDwgFile, FileOpenMode.OpenForReadAndReadShare, allowCPConversion: true, "");
				database.CloseInput(closeFile: true);
				ObjectId objectId = db.Insert("*U", database, preserveSourceDatabase: true);
				database.Dispose();
				return objectId.Handle.ToString();
			}
		}
		return "";
	}

	public bool InsertBlockDynamicFromFile(Database db, string strDwgFile, Point3d ptIns, ref string sHandleRef)
	{
		string value = CreateNewBlockDynamic(db, strDwgFile);
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
		if (objectId.IsNull)
		{
			return false;
		}
		BlockReference blockReference = new BlockReference(ptIns, objectId);
		blockReference.SetDatabaseDefaults(db);
		sHandleRef = GlobalFunction.AppendBlockIntersect(db, blockReference);
		return true;
	}

	public bool GetFG2(List<string> lstNameText, ref List<string> listFG2)
	{
		foreach (string item in lstNameText)
		{
			if (string.Compare(item, "Fg2a", ignoreCase: true) == 0 || string.Compare(item, "Fg2b", ignoreCase: true) == 0 || string.Compare(item, "Fg2c", ignoreCase: true) == 0)
			{
				listFG2.Add(item);
			}
		}
		if (listFG2.Count <= 0)
		{
			return false;
		}
		return true;
	}

	public bool GetFG2S(List<string> lstNameText, ref string sText)
	{
		foreach (string item in lstNameText)
		{
			int num = item.IndexOf(sText);
			if (num != -1)
			{
				sText = item;
				return true;
			}
		}
		return false;
	}

	public void GetFG2S(List<string> lstNameText, ref List<string> lstRes)
	{
		foreach (string item in lstNameText)
		{
			int num = item.IndexOf("FG2S");
			if (num != -1)
			{
				lstRes.Add(item);
			}
		}
	}

	public void FillterGroup(List<string> lstNameText, ref List<string> lstRes)
	{
		foreach (string item in lstNameText)
		{
			int num = item.IndexOf("FG2");
			if (num == -1)
			{
				num = item.IndexOf("配管貫通部");
				if (num == -1)
				{
					lstRes.Add(item);
				}
			}
		}
	}

	public void explodeDwg(ref Database dbNew)
	{
		BlockTableRecord blockTableRecord = dbNew.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord;
		foreach (ObjectId item in blockTableRecord)
		{
			if (string.Compare(item.ObjectClass.DxfName, "INSERT", ignoreCase: true) == 0)
			{
				BlockReference blockReference = item.GetObject(OpenMode.ForWrite) as BlockReference;
				blockReference.ExplodeToOwnerSpace();
				blockReference.Erase();
				blockReference.Dispose();
				blockReference = null;
			}
		}
	}

	public void AddFG12Fix(ref Database dbNew, string sPathFolderSymbol, int nRegion, List<string> lstNameText)
	{
		string text = $"FG1_{nRegion}";
		string text2 = $"{sPathFolderSymbol}{text}" + ".dwg";
		string sHandleRef = "";
		if (!InsertBlockDynamicFromFile(dbNew, text2, Point3d.Origin, ref sHandleRef))
		{
			return;
		}
		BlockReference blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
		Point3d maxPoint = blockReference.GeometricExtents.MaxPoint;
		Point3d point3d = blockReference.GeometricExtents.MinPoint;
		blockReference.Dispose();
		blockReference = null;
		Point3d ptIns = new Point3d(maxPoint.X, 0.0, 0.0);
		text2 = text2.Replace(text, "FG2");
		if (!InsertBlockDynamicFromFile(dbNew, text2, ptIns, ref sHandleRef))
		{
			return;
		}
		blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
		maxPoint = blockReference.GeometricExtents.MaxPoint;
		Point3d point3d2 = maxPoint;
		Point3d minPoint = blockReference.GeometricExtents.MinPoint;
		blockReference.Dispose();
		blockReference = null;
		List<string> listFG = new List<string>();
		bool fG = GetFG2(lstNameText, ref listFG);
		string arg = "S1";
		string text3 = "用";
		if (m_nCaseS == 2)
		{
			arg = "S2";
			text3 = "用-多雪";
		}
		int i;
		if (fG)
		{
			for (i = 0; i < listFG.Count; i++)
			{
				string arg2 = listFG[i];
				text2 = $"{sPathFolderSymbol}{arg2}" + ".dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(maxPoint.X, 0.0, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					maxPoint = new Point3d(point3d2.X, maxPoint.Y, 0.0);
					point3d = point3d2;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
					arg2 = listFG[listFG.Count - i - 1];
					text2 = $"{sPathFolderSymbol}{arg2}{text3}" + ".dwg";
					Point3d ptIns2 = new Point3d(0.0, maxPoint.Y, 0.0);
					string sHandleRef2 = "";
					if (InsertBlockDynamicFromFile(dbNew, text2, ptIns2, ref sHandleRef2))
					{
						blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef2, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
						maxPoint = new Point3d(point3d2.X, blockReference.GeometricExtents.MaxPoint.Y, 0.0);
						blockReference.Dispose();
						blockReference = null;
					}
				}
			}
		}
		Point3d point3d3 = Point3d.Origin;
		Point3d point3d4 = Point3d.Origin;
		text2 = $"{sPathFolderSymbol}FG2{text3}.dwg";
		if (InsertBlockDynamicFromFile(dbNew, text2, new Point3d(0.0, maxPoint.Y, 0.0), ref sHandleRef))
		{
			blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
			maxPoint = blockReference.GeometricExtents.MaxPoint;
			if (!fG)
			{
				point3d = blockReference.GeometricExtents.MinPoint;
			}
			blockReference.Dispose();
			blockReference = null;
			text2 = $"{sPathFolderSymbol}配管貫通部_{nRegion}.dwg";
			if (InsertBlockDynamicFromFile(dbNew, text2, new Point3d(maxPoint.X, point3d.Y, 0.0), ref sHandleRef))
			{
				blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
				point3d3 = blockReference.GeometricExtents.MaxPoint;
				point3d4 = blockReference.GeometricExtents.MinPoint;
				blockReference.Dispose();
				blockReference = null;
			}
		}
		string arg3 = "尺";
		if (m_nCaseMR == 2)
		{
			arg3 = "M";
		}
		int num = 0;
		if (fG)
		{
			num = listFG.Count;
		}
		for (i = 0; i < lstNameText.Count; i++)
		{
			string text4 = lstNameText[i];
			if (string.Compare(text4, "Fg2a", ignoreCase: true) == 0 || string.Compare(text4, "Fg2b", ignoreCase: true) == 0 || string.Compare(text4, "Fg2c", ignoreCase: true) == 0 || string.Compare(text4, "配管貫通部", ignoreCase: true) == 0)
			{
				continue;
			}
			if (num == 3)
			{
				break;
			}
			string value = "FG2S";
			int num2 = text4.IndexOf(value);
			if (num2 != -1)
			{
				text2 = $"{sPathFolderSymbol}{text4}" + ".dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(point3d2.X, minPoint.Y, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					num++;
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
				}
				continue;
			}
			if (string.Compare(text4, "FG3a", ignoreCase: true) == 0)
			{
				text2 = $"{sPathFolderSymbol}{text4}" + ".dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(point3d2.X, minPoint.Y, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					num++;
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
				}
				continue;
			}
			num2 = text4.IndexOf("FG3");
			if (num2 != -1)
			{
				text2 = $"{sPathFolderSymbol}{text4}_{nRegion}.dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(point3d2.X, minPoint.Y, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					num++;
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
				}
				continue;
			}
			num2 = text4.IndexOf("FG4");
			if (num2 != -1)
			{
				text2 = $"{sPathFolderSymbol}{text4}_{nRegion}.dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(point3d2.X, minPoint.Y, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					num++;
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
				}
				continue;
			}
			num2 = text4.IndexOf("FG5");
			if (num2 != -1)
			{
				text2 = $"{sPathFolderSymbol}{text4}_{nRegion}.dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(point3d2.X, minPoint.Y, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					num++;
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
				}
				continue;
			}
			num2 = text4.IndexOf("FG20");
			if (num2 != -1)
			{
				text2 = $"{sPathFolderSymbol}{text4} {arg3}.dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(point3d2.X, minPoint.Y, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					num++;
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
				}
				continue;
			}
			num2 = text4.IndexOf("FG21");
			if (num2 != -1)
			{
				text2 = $"{sPathFolderSymbol}{text4} {arg3}.dwg";
				if (InsertBlockDynamicFromFile(ptIns: new Point3d(point3d2.X, minPoint.Y, 0.0), db: dbNew, strDwgFile: text2, sHandleRef: ref sHandleRef))
				{
					num++;
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d2 = blockReference.GeometricExtents.MaxPoint;
					minPoint = blockReference.GeometricExtents.MinPoint;
					blockReference.Dispose();
					blockReference = null;
				}
			}
		}
		if (num == 3 && lstNameText.Count >= 4)
		{
			Point3d ptIns3 = new Point3d(point3d4.X, point3d3.Y, 0.0);
			text2 = $"{sPathFolderSymbol}{arg}_m.dwg";
			string sHandleRef3 = "";
			if (InsertBlockDynamicFromFile(dbNew, text2, ptIns3, ref sHandleRef3))
			{
				blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef3, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
				point3d3 = blockReference.GeometricExtents.MaxPoint;
				point3d4 = blockReference.GeometricExtents.MinPoint;
				ptIns3 = new Point3d(point3d3.X, point3d4.Y, 0.0);
				blockReference.Dispose();
				blockReference = null;
			}
			int num3 = 0;
			for (int j = i; j < lstNameText.Count; j++)
			{
				string text5 = lstNameText[j];
				int num4 = text5.IndexOf("FG20");
				if (num4 != -1)
				{
					text2 = $"{sPathFolderSymbol}{text5} {arg3}_m.dwg";
				}
				else
				{
					num4 = text5.IndexOf("FG21");
					if (num4 != -1)
					{
						text2 = $"{sPathFolderSymbol}{text5} {arg3}_m.dwg";
					}
					else
					{
						num4 = text5.IndexOf("FG3a");
						text2 = ((num4 == -1) ? $"{sPathFolderSymbol}{text5}_{nRegion}_m.dwg" : ($"{sPathFolderSymbol}{text5}_m" + ".dwg"));
					}
				}
				if (InsertBlockDynamicFromFile(dbNew, text2, ptIns3, ref sHandleRef))
				{
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					point3d3 = blockReference.GeometricExtents.MaxPoint;
					point3d4 = blockReference.GeometricExtents.MinPoint;
					ptIns3 = new Point3d(point3d3.X, point3d4.Y, 0.0);
					blockReference.Dispose();
					blockReference = null;
					num3++;
				}
			}
			if (num3 == 0)
			{
				try
				{
					blockReference = dbNew.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRef3, 16)), 0).GetObject(OpenMode.ForRead) as BlockReference;
					ptIns3 = blockReference.Position;
					blockReference.UpgradeOpen();
					blockReference.Erase();
					blockReference.Dispose();
					blockReference = null;
					text2 = $"{sPathFolderSymbol}{arg}.dwg";
					InsertBlockDynamicFromFile(dbNew, text2, ptIns3, ref sHandleRef3);
				}
				catch (Exception)
				{
				}
			}
		}
		else
		{
			text2 = $"{sPathFolderSymbol}{arg}.dwg";
			InsertBlockDynamicFromFile(dbNew, text2, new Point3d(point3d4.X, point3d3.Y, 0.0), ref sHandleRef);
		}
	}

	public void cmdOutputSurfaceCut(Document doc)
	{
		List<string> lstNameFile = new List<string>();
		GetTextInfoSurface(doc, ref lstNameFile);
		if (lstNameFile.Count == 0)
		{
			doc.Editor.WriteMessage("\n The Information Surface Cut do not found");
			return;
		}
		string[] array = lstNameFile.ToArray();
		try
		{
			Array.Sort(array, new AlphanumComparatorFast());
		}
		catch (Exception)
		{
		}
		lstNameFile.Clear();
		lstNameFile.AddRange(array);
		_ = doc.Editor;
		int num = 1;
		bool bEnable = false;
		if (lstNameFile.Contains("FG20 外部") || lstNameFile.Contains("FG20 内部") || lstNameFile.Contains("FG21"))
		{
			bEnable = true;
		}
		FormSettingOutSurfaceCut formSettingOutSurfaceCut = new FormSettingOutSurfaceCut();
		formSettingOutSurfaceCut.m_bEnable = bEnable;
		Bricscad.ApplicationServices.Application.ShowModalDialog(Bricscad.ApplicationServices.Application.MainWindow, formSettingOutSurfaceCut, persist: true);
		if (DialogResult.OK == formSettingOutSurfaceCut.DialogResult)
		{
			num = int.Parse(formSettingOutSurfaceCut.m_sKey);
			m_nCaseMR = int.Parse(formSettingOutSurfaceCut.m_sKey1);
			m_nCaseS = int.Parse(formSettingOutSurfaceCut.m_sKey2);
			string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
			string sPathFolderSymbol = pathFolderSymbol + "\\SurfaceCut\\";
			string fileName = Path.Combine(pathFolderSymbol + "\\SurfaceCut", "TemplateOut.dwg");
			Database dbNew = new Database(buildDefaultDrawing: false, noDocument: true);
			dbNew.ReadDwgFile(fileName, FileOpenMode.OpenForReadAndReadShare, allowCPConversion: true, "");
			dbNew.CloseInput(closeFile: true);
			AddFG12Fix(ref dbNew, sPathFolderSymbol, num, lstNameFile);
			explodeDwg(ref dbNew);
			ObjectId blockTableRecord = doc.Database.Insert("*U", dbNew, preserveSourceDatabase: true);
			dbNew.Dispose();
			dbNew = null;
			BlockReference blockReference = new BlockReference(Point3d.Origin, blockTableRecord);
			blockReference.SetDatabaseDefaults(doc.Database);
			blockReference.ScaleFactors = new Scale3d(90.0, 90.0, 90.0);
			if (BlockMovingScaling.Jig(blockReference))
			{
				GlobalFunction.AppendEntity(doc.Database, blockReference, "ST_InfoSufaceCut", 4);
				return;
			}
			blockReference.Dispose();
			blockReference = null;
		}
	}

	public void changeBasePoint(ObjectId idBlockCur, Point3d ptChange, ref ObjectIdCollection idsBlock)
	{
		BlockTableRecord blockTableRecord = idBlockCur.GetObject(OpenMode.ForRead) as BlockTableRecord;
		BlockTableRecord blockTableRecord2 = new BlockTableRecord();
		blockTableRecord2.Name = "*U";
		BlockTable blockTable = idBlockCur.Database.BlockTableId.GetObject(OpenMode.ForWrite) as BlockTable;
		blockTable.Add(blockTableRecord2);
		Matrix3d transform = Matrix3d.Displacement(ptChange - Point3d.Origin);
		foreach (ObjectId item in blockTableRecord)
		{
			Entity entity = item.GetObject(OpenMode.ForWrite) as Entity;
			entity.TransformBy(transform);
			Entity entity2 = entity.Clone() as Entity;
			blockTableRecord2.AppendEntity(entity2);
			entity.Dispose();
			entity = null;
			entity2.Dispose();
			entity2 = null;
		}
		blockTableRecord.Dispose();
		blockTableRecord = null;
		idsBlock.Add(blockTableRecord2.ObjectId);
		blockTableRecord2.Dispose();
		blockTableRecord2 = null;
	}

	public void UpdateTextBlockSL200(ObjectId idBref)
	{
		BlockReference blockReference = idBref.GetObject(OpenMode.ForRead) as BlockReference;
		double num = blockReference.Rotation;
		if (num < Math.PI)
		{
			blockReference.Dispose();
			blockReference = null;
			return;
		}
		BlockTableRecord blockTableRecord = blockReference.BlockTableRecord.GetObject(OpenMode.ForWrite) as BlockTableRecord;
		foreach (ObjectId item in blockTableRecord)
		{
			DBObject dBObject = item.GetObject(OpenMode.ForWrite);
			DBText dBText = dBObject as DBText;
			if (dBText != null)
			{
				_ = dBText.TextString;
				Point3d minPoint = dBText.GeometricExtents.MinPoint;
				num += dBText.Rotation;
				num = Math.PI * 2.0 - num;
				if (num != Math.PI)
				{
					num = num + Math.PI * 2.0 - blockReference.Rotation;
				}
				dBText.TransformBy(Matrix3d.Rotation(num, Bricscad.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis, dBText.Position));
				Point3d minPoint2 = dBText.GeometricExtents.MinPoint;
				dBText.TransformBy(Matrix3d.Displacement(minPoint - minPoint2));
			}
			dBObject.Dispose();
			dBObject = null;
		}
		blockTableRecord.Dispose();
		blockTableRecord = null;
		blockReference.Dispose();
		blockReference = null;
	}

	public void cmdInputOther(Document doc)
	{
		int nSel = 0;
		while (true)
		{
			FormSettingInputOther formSettingInputOther = new FormSettingInputOther();
			formSettingInputOther.m_nSel = nSel;
			Bricscad.ApplicationServices.Application.ShowModalDialog(Bricscad.ApplicationServices.Application.MainWindow, formSettingInputOther, persist: true);
			if (DialogResult.OK != formSettingInputOther.DialogResult)
			{
				break;
			}
			nSel = formSettingInputOther.m_nSel;
			string sPathFileDwg = formSettingInputOther.sPathFileDwg;
			if (string.IsNullOrEmpty(sPathFileDwg))
			{
				continue;
			}
			string value = CreateNewBlockDynamic(doc.Database, sPathFileDwg);
			if (string.IsNullOrEmpty(value))
			{
				break;
			}
			ObjectId objectId = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
			if (objectId.IsNull)
			{
				break;
			}
			BlockReference blockReference = new BlockReference(Point3d.Origin, objectId);
			blockReference.SetDatabaseDefaults(doc.Database);
			blockReference.ScaleFactors = new Scale3d(50.0, 50.0, 50.0);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sPathFileDwg);
			if (fileNameWithoutExtension.IndexOf("SL50") != -1 || fileNameWithoutExtension.IndexOf("SL200") != -1)
			{
				bool flag = false;
				if (fileNameWithoutExtension.IndexOf("SL50") != -1)
				{
					flag = true;
				}
				if (!BlockMovingChangeBaseScaling.Jig(blockReference, flag))
				{
					continue;
				}
				value = GlobalFunction.AppendEntity(doc.Database, blockReference, "ST_InfoSufaceCut", 4);
				if (!flag)
				{
					ObjectId objectId2 = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
					if (!objectId.IsNull)
					{
						UpdateTextBlockSL200(objectId2);
						doc.Editor.Regen();
					}
				}
			}
			else if (fileNameWithoutExtension.IndexOf("床下点検口") != -1)
			{
				if (BlockMovingScaling.JigOnly(blockReference))
				{
					GlobalFunction.AppendEntity(doc.Database, blockReference, "ST_InfoSufaceCut", 4);
					continue;
				}
				blockReference.Dispose();
				blockReference = null;
			}
			else if (BlockMovingScaling.Jig(blockReference))
			{
				GlobalFunction.AppendEntity(doc.Database, blockReference, "ST_InfoSufaceCut", 4);
			}
			else
			{
				blockReference.Dispose();
				blockReference = null;
			}
		}
	}
}
