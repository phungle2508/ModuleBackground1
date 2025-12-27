using System;
using System.Collections.Generic;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CDrawLine
{
	public CSplitRoom split = new CSplitRoom();

	public CInsSymNearSquares square = new CInsSymNearSquares();

	public void cmdDrawline100(Document doc)
	{
		ObjectIdCollection idsCircle = new ObjectIdCollection();
		if (drawCircleTemp(doc, ref idsCircle))
		{
			drawLine100Select(doc);
			GlobalFunction.DeleteEntityS(doc.Database, idsCircle);
		}
	}

	public void drawLine100Select(Document doc)
	{
		Database database = doc.Database;
		Editor editor = doc.Editor;
		PromptPointOptions promptPointOptions = new PromptPointOptions("");
		promptPointOptions.Message = "\nSelect Base point:";
		promptPointOptions.AllowNone = true;
		PromptPointResult point = editor.GetPoint(promptPointOptions);
		if (point.Status != PromptStatus.OK)
		{
			return;
		}
		Point3d value = point.Value;
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		while (true)
		{
			promptPointOptions.Message = "\nSelect point next:";
			promptPointOptions.UseBasePoint = true;
			promptPointOptions.BasePoint = value;
			point = editor.GetPoint(promptPointOptions);
			if (point.Status == PromptStatus.Cancel)
			{
				GlobalFunction.DeleteEntityS(database, objectIdCollection);
				objectIdCollection.Clear();
				break;
			}
			if (point.Status != PromptStatus.OK)
			{
				break;
			}
			Line line = new Line();
			line.SetDatabaseDefaults(database);
			line.StartPoint = value;
			line.EndPoint = point.Value;
			if (line.Length <= 1.0)
			{
				line.Dispose();
				line = null;
				continue;
			}
			string value2 = GlobalFunction.AppendEntity(database, line, "0", 1);
			ObjectId objectId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value2, 16)), 0);
			if (!objectId.IsNull)
			{
				objectIdCollection.Add(objectId);
			}
			value = point.Value;
		}
		if (objectIdCollection.Count <= 0)
		{
			return;
		}
		double num = 150.0;
		while (true)
		{
			PromptDoubleOptions promptDoubleOptions = new PromptDoubleOptions("\nInput Distance <150>:");
			promptDoubleOptions.AllowNegative = false;
			promptDoubleOptions.AllowZero = false;
			promptDoubleOptions.AllowNone = true;
			PromptDoubleResult promptDoubleResult = editor.GetDouble(promptDoubleOptions);
			if (promptDoubleResult.Status == PromptStatus.Cancel)
			{
				GlobalFunction.DeleteEntityS(database, objectIdCollection);
				return;
			}
			if (promptDoubleResult.Status == PromptStatus.None)
			{
				break;
			}
			if (promptDoubleResult.Status == PromptStatus.OK && !(promptDoubleResult.Value <= 0.0))
			{
				num = promptDoubleResult.Value;
				break;
			}
		}
		deleteLine100Exited(doc, objectIdCollection);
		foreach (ObjectId item in objectIdCollection)
		{
			drawLine100(doc, item, num / 2.0);
		}
	}

	public bool drawCircleTemp(Document doc, ref ObjectIdCollection idsCircle)
	{
		Database database = doc.Database;
		Editor editor = doc.Editor;
		if (!split.CheckExitRoom(doc))
		{
			return false;
		}
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerLineSquare)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return true;
		}
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value = objectIds[i];
			Line line = (Line)value.GetObject(OpenMode.ForRead);
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				if (colorIndex == GlobalFunction.nColorLineSquare)
				{
					DBObject value2 = line.Clone() as DBObject;
					dBObjectCollection.Add(value2);
					objectIdCollection.Add(value);
				}
				line.Dispose();
				line = null;
			}
		}
		List<Region> lstRegion = new List<Region>();
		square.CreateRegionSquares(doc.Database, dBObjectCollection, ref lstRegion);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		if (lstRegion.Count == 0)
		{
			return true;
		}
		List<Region> lstRegionNotMax = new List<Region>();
		Region reBound = null;
		split.GetListRegion(doc, ref lstRegionNotMax, ref reBound);
		new List<CSquares>();
		new Point3dCollection();
		foreach (Region item2 in lstRegion)
		{
			if (!square.CheckObjectIsSquare(item2))
			{
				continue;
			}
			Point3d point3d = GlobalFunction.PointCenter(item2);
			if (split.CheckPointInsiheRegion(reBound, point3d))
			{
				Circle circle = new Circle();
				circle.SetDatabaseDefaults(doc.Database);
				circle.Center = point3d;
				circle.Radius = 25.0;
				string value3 = GlobalFunction.AppendEntity(database, circle, "0", 1);
				ObjectId objectId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value3, 16)), 0);
				if (!objectId.IsNull)
				{
					idsCircle.Add(objectId);
				}
			}
		}
		split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
		split.DeleteRegionS(doc.Database, lstRegion, null);
		editor.Regen();
		editor.UpdateScreen();
		return true;
	}

	public bool checkIntersectWithLine100(ObjectIdCollection idsLine100, Line line)
	{
		foreach (ObjectId item in idsLine100)
		{
			Line line2 = item.GetObject(OpenMode.ForRead) as Line;
			Point3dCollection point3dCollection = new Point3dCollection();
			line.IntersectWith(line2, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
			if (point3dCollection.Count != 0)
			{
				line2.Dispose();
				line2 = null;
				return true;
			}
			line2.Dispose();
			line2 = null;
		}
		return false;
	}

	public void deleteLine100Exited(Document doc, ObjectIdCollection idsLine100)
	{
		_ = doc.Database;
		_ = doc.Editor;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, "ST_LINE100")
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return;
		}
		new ObjectIdCollection();
		new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		foreach (ObjectId objectId in objectIds)
		{
			Line line = (Line)objectId.GetObject(OpenMode.ForRead);
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				if (colorIndex == 9 && checkIntersectWithLine100(idsLine100, line))
				{
					line.UpgradeOpen();
					line.Erase();
				}
				line.Dispose();
				line = null;
			}
		}
	}

	public void drawLine100(Document doc, ObjectId idLine, double dRDist)
	{
		Matrix3d currentUserCoordinateSystem = doc.Editor.CurrentUserCoordinateSystem;
		Line line = idLine.GetObject(OpenMode.ForRead) as Line;
		_ = line.StartPoint;
		_ = line.EndPoint;
		Point3d center = GlobalFunction.Point3dCenter(line);
		double angle = line.Angle;
		double length = line.Length;
		Matrix3d transform = Matrix3d.Rotation(Math.PI - angle, currentUserCoordinateSystem.CoordinateSystem3d.Zaxis, center);
		line.UpgradeOpen();
		line.TransformBy(transform);
		transform = Matrix3d.Rotation(angle, currentUserCoordinateSystem.CoordinateSystem3d.Zaxis, center);
		Point3d minPoint = line.GeometricExtents.MinPoint;
		for (double num = 0.0; num < length; num += 100.0)
		{
			Point3d startPoint = new Point3d(minPoint.X + num, minPoint.Y + dRDist, minPoint.Z);
			Point3d endPoint = new Point3d(minPoint.X + num, minPoint.Y - dRDist, minPoint.Z);
			Line line2 = new Line();
			line2.SetDatabaseDefaults(idLine.Database);
			line2.StartPoint = startPoint;
			line2.EndPoint = endPoint;
			line2.TransformBy(transform);
			GlobalFunction.AppendEntity(doc.Database, line2, "ST_LINE100", 9);
		}
		line.Erase();
		line.Dispose();
		line = null;
	}
}
