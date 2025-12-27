using System;
using System.Collections.Generic;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CSplitRoom
{
	public void ExplodeRegion(Region regi, ref DBObjectCollection colExp)
	{
		if (regi == null || regi.Area <= 0.0)
		{
			return;
		}
		List<Region> list = new List<Region>();
		list.Add(regi.Clone() as Region);
		int num = 0;
		while (true)
		{
			regi = list[num];
			if (regi.Area <= 0.0)
			{
				if (num >= list.Count - 1)
				{
					break;
				}
				num++;
				continue;
			}
			DBObjectCollection dBObjectCollection = new DBObjectCollection();
			regi.Explode(dBObjectCollection);
			foreach (DBObject item2 in dBObjectCollection)
			{
				if (item2 is Region)
				{
					Region item = item2 as Region;
					list.Add(item);
				}
				else if (item2 is Line)
				{
					colExp.Add(item2);
				}
				else
				{
					item2.Dispose();
				}
			}
			dBObjectCollection.Clear();
			if (num >= list.Count - 1)
			{
				break;
			}
			num++;
		}
		foreach (Region item3 in list)
		{
			item3.Dispose();
		}
	}

	public void ExplodeRegion(Region regi, ref ObjectIdCollection colExp)
	{
		if (regi == null || regi.Area <= 0.0)
		{
			return;
		}
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		List<Region> list = new List<Region>();
		list.Add(regi.Clone() as Region);
		int num = 0;
		while (true)
		{
			regi = list[num];
			if (regi.Area <= 0.0)
			{
				if (num >= list.Count - 1)
				{
					break;
				}
				num++;
				continue;
			}
			DBObjectCollection dBObjectCollection = new DBObjectCollection();
			regi.Explode(dBObjectCollection);
			foreach (DBObject item in dBObjectCollection)
			{
				Entity ent = item as Entity;
				ObjectId value = GlobalFunction.AddWallToModelSpace(mdiActiveDocument.Database, ent);
				colExp.Add(value);
			}
			dBObjectCollection.Clear();
			if (num >= list.Count - 1)
			{
				break;
			}
			num++;
		}
		foreach (Region item2 in list)
		{
			item2.Dispose();
		}
	}

	public bool Check2Region(Region res1, Region res2)
	{
		if (res1 == null || res2 == null)
		{
			return false;
		}
		DBObjectCollection colExp = new DBObjectCollection();
		ExplodeRegion(res1, ref colExp);
		DBObjectCollection colExp2 = new DBObjectCollection();
		ExplodeRegion(res2, ref colExp2);
		bool result = false;
		foreach (DBObject item in colExp)
		{
			Entity entity = item as Entity;
			foreach (DBObject item2 in colExp2)
			{
				Entity entityPointer = item2 as Entity;
				Point3dCollection point3dCollection = new Point3dCollection();
				entity.IntersectWith(entityPointer, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
				if (point3dCollection.Count != 0)
				{
					result = true;
					break;
				}
			}
		}
		foreach (DBObject item3 in colExp)
		{
			item3.Dispose();
		}
		foreach (DBObject item4 in colExp2)
		{
			item4.Dispose();
		}
		return result;
	}

	public bool UnionWall(ref DBObjectCollection colRegions)
	{
		Region region = null;
		bool flag = false;
		for (int i = 0; i < colRegions.Count - 1; i++)
		{
			if (!(colRegions[i] is Region))
			{
				continue;
			}
			Region region2 = colRegions[i] as Region;
			Region region3 = null;
			int j;
			for (j = i + 1; j < colRegions.Count; j++)
			{
				if (colRegions[j] is Region)
				{
					region3 = colRegions[j] as Region;
					if (Check2Region(region2, region3))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				double area = region2.Area;
				double area2 = region3.Area;
				if (area > area2)
				{
					region2.BooleanOperation(BooleanOperationType.BoolSubtract, region3);
					region = region2;
				}
				else
				{
					region3.BooleanOperation(BooleanOperationType.BoolSubtract, region2);
					region = region3;
				}
				colRegions.RemoveAt(i);
				colRegions.RemoveAt(j);
				colRegions.Add(region);
				return true;
			}
		}
		return false;
	}

	public void GetWall(ref DBObjectCollection colRegions)
	{
		while (UnionWall(ref colRegions))
		{
		}
	}

	public void GetRegionInterSec(DBObjectCollection colRegions, ref DBObjectCollection colRegionsNotInterSec, ref DBObjectCollection colRegionsInterSec)
	{
		bool flag = false;
		for (int i = 0; i < colRegions.Count; i++)
		{
			Region region = colRegions[i] as Region;
			Region region2 = null;
			for (int j = i + 1; j < colRegions.Count; j++)
			{
				region2 = colRegions[j] as Region;
				if (Check2Region(region, region2))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				colRegionsInterSec.Add(region);
			}
			else
			{
				colRegionsNotInterSec.Add(region);
			}
		}
	}

	public bool IsInPolygon(Point3d point, Point3dCollection polygon)
	{
		bool flag = false;
		Point3d point3d = polygon[polygon.Count - 1];
		foreach (Point3d item in polygon)
		{
			if (item.X == point.X && item.Y == point.Y)
			{
				return true;
			}
			if (item.Y == point3d.Y && point.Y == point3d.Y && point3d.X <= point.X && point.X <= item.X)
			{
				return true;
			}
			if (((item.Y < point.Y && point3d.Y >= point.Y) || (point3d.Y < point.Y && item.Y >= point.Y)) && item.X + (point.Y - item.Y) / (point3d.Y - item.Y) * (point3d.X - item.X) <= point.X)
			{
				flag = !flag;
			}
			point3d = item;
		}
		return flag;
	}

	public bool IsOnPolygon(Point3d point, Point3dCollection polygon)
	{
		bool flag = false;
		Point3d point3d = polygon[polygon.Count - 1];
		foreach (Point3d item in polygon)
		{
			if (item.X == point.X && item.Y == point.Y)
			{
				return false;
			}
			if (item.Y == point3d.Y && point.Y == point3d.Y && point3d.X <= point.X && point.X <= item.X)
			{
				return false;
			}
			if (((item.Y < point.Y && point3d.Y >= point.Y) || (point3d.Y < point.Y && item.Y >= point.Y)) && item.X + (point.Y - item.Y) / (point3d.Y - item.Y) * (point3d.X - item.X) <= point.X)
			{
				flag = !flag;
			}
			point3d = item;
		}
		return flag;
	}

	public bool CheckPointInsiheRegion(Region regi, Point3d ptSel, bool bOut = false)
	{
		DBObjectCollection colExp = new DBObjectCollection();
		ExplodeRegion(regi, ref colExp);
		Point3dCollection point3dCollection = new Point3dCollection();
		foreach (DBObject item in colExp)
		{
			if (item is Line)
			{
				Line line = item as Line;
				Point3d startPoint = line.StartPoint;
				Point3d endPoint = line.EndPoint;
				if (startPoint.DistanceTo(endPoint) <= 1.0)
				{
					item.Dispose();
					continue;
				}
				point3dCollection.Add(startPoint);
			}
			item.Dispose();
		}
		if (point3dCollection.Count == 0)
		{
			return false;
		}
		bool result = IsInPolygon(ptSel, point3dCollection);
		if (bOut)
		{
			result = IsOnPolygon(ptSel, point3dCollection);
		}
		return result;
	}

	public Region GetRegionMax(DBObjectCollection colRegions)
	{
		double num = 0.0;
		Region result = null;
		foreach (DBObject colRegion in colRegions)
		{
			if (colRegion is Region)
			{
				Region region = colRegion as Region;
				if (region.Area > num)
				{
					num = region.Area;
					result = region;
				}
			}
		}
		return result;
	}

	public DBObjectCollection FindRegionInterSec(Region reSrc, ref DBObjectCollection colRegions)
	{
		return new DBObjectCollection();
	}

	public string AppendEntity(Database db, Entity ent, string sLayer, int ncolor)
	{
		string result = "";
		using (BlockTableRecord blockTableRecord = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord)
		{
			if (ent != null)
			{
				ent.Layer = sLayer;
				ent.ColorIndex = ncolor;
				result = blockTableRecord.AppendEntity(ent).Handle.ToString();
				ent.Dispose();
			}
		}
		return result;
	}

	public string AppendEntityWall(Database db, Entity ent)
	{
		return GlobalFunction.AppendEntityWall(db, ent);
	}

	public string AppendEntityLineHatch(Database db, Entity ent)
	{
		return GlobalFunction.AppendEntityLineHatch(db, ent);
	}

	public void DeleteEntityS(Database db, ObjectIdCollection ids)
	{
		GlobalFunction.DeleteEntityS(db, ids);
	}

	public bool SelectPoint(Document doc, Point3d ptSel, ref ObjectIdCollection ids)
	{
		Database database = doc.Database;
		Editor editor = doc.Editor;
		while (true)
		{
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nSelect point next:";
			promptPointOptions.UseBasePoint = true;
			promptPointOptions.BasePoint = ptSel;
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status == PromptStatus.Cancel)
			{
				DeleteEntityS(database, ids);
				return false;
			}
			if (point.Status != PromptStatus.OK)
			{
				break;
			}

			// Check if point is in same wall (not on same horizontal or vertical line)
			bool bInSameWall = CheckPointInSameWall(doc, ptSel, point.Value);

			Point3d ptNew = point.Value;

			if (bInSameWall)
			{
				// Prompt for direction
				string direction = "";
				do
				{
					PromptStringOptions promptStringOptions = new PromptStringOptions("\nInput direction horizontal with Squares [Left(L)/Right(R)/Top(T)/Bottom(B)] <Left(L)>:");
					promptStringOptions.AllowSpaces = false;
					PromptResult promptResult = editor.GetString(promptStringOptions);
					if (promptResult.Status == PromptStatus.Cancel)
					{
						DeleteEntityS(database, ids);
						return false;
					}
					direction = promptResult.StringResult;
					if (string.IsNullOrEmpty(direction))
					{
						direction = "L";
					}
					else
					{
						direction = direction.Substring(0, 1).ToUpper();
					}
				}
				while (direction != "L" && direction != "R" && direction != "T" && direction != "B");

                // Adjust the point based on direction
                ptNew = AdjustPointByDirection(doc, ptSel, point.Value, direction);

                // Prompt for thickness after direction is selected
                PromptIntegerOptions promptThicknessOpts = new PromptIntegerOptions("\nInput thickness:");
                promptThicknessOpts.AllowNone = true;
                PromptIntegerResult thicknessResult = editor.GetInteger(promptThicknessOpts);
                if (thicknessResult.Status != PromptStatus.OK)
                {
                    DeleteEntityS(database, ids);
                    return false;
                }
                int thicknessValue = thicknessResult.Value;
                double dThickness = (double)thicknessValue * 1.0 / 2.0;

                Vector3d dirVector = (point.Value - ptSel).GetNormal();
                Vector3d perpVector = new Vector3d(-dirVector.Y, dirVector.X, 0.0); // Perpendicular direction

                // Determine offset direction based on T/B direction
                Vector3d offsetDirection = perpVector;

                if (direction == "B")
                {
                    // For bottom, we want to draw BELOW the line
                    if (point.Value.Y < ptSel.Y)
                    {
                        offsetDirection = perpVector;
                    }
                    else
                    {
                        offsetDirection = -perpVector;
                    }
                }
                else if (direction == "T")
                {
                    // For top, we want to draw ABOVE the line
                    if (point.Value.Y > ptSel.Y)
                    {
                        offsetDirection = -perpVector;
                    }
                    else
                    {
                        offsetDirection = perpVector;
                    }
                }

                // Calculate offset points - center line and one offset line only
                Point3d ptSelCenter = ptSel;
                Point3d ptNewCenter = point.Value;
                Point3d ptSelOffset = ptSel + offsetDirection * (dThickness * 2);
                Point3d ptNewOffset = point.Value + offsetDirection * (dThickness * 2);

                // Create the rectangle as a collection of lines
                DBObjectCollection dbObjs = new DBObjectCollection();

                // Draw first line (center line along the wall)
                Line thickLine1 = new Line();
                thickLine1.SetDatabaseDefaults(database);
                thickLine1.StartPoint = ptSelCenter;
                thickLine1.EndPoint = ptNewCenter;
                dbObjs.Add(thickLine1);

                // Draw second line (offset line)
                Line thickLine2 = new Line();
                thickLine2.SetDatabaseDefaults(database);
                thickLine2.StartPoint = ptSelOffset;
                thickLine2.EndPoint = ptNewOffset;
                dbObjs.Add(thickLine2);

                // Draw connecting lines to close the rectangle
                Line connectLine1 = new Line();
                connectLine1.SetDatabaseDefaults(database);
                connectLine1.StartPoint = ptSelCenter;
                connectLine1.EndPoint = ptSelOffset;
                dbObjs.Add(connectLine1);

                Line connectLine2 = new Line();
                connectLine2.SetDatabaseDefaults(database);
                connectLine2.StartPoint = ptNewCenter;
                connectLine2.EndPoint = ptNewOffset;
                dbObjs.Add(connectLine2);

                // Create region from the 4 lines
                ids.Clear();
                try
                {
                    DBObjectCollection finalRegions = Region.CreateFromCurves(dbObjs);

                    // Dispose the temporary lines
                    foreach (DBObject item in dbObjs)
                    {
                        item.Dispose();
                    }

                    // Add the region to the database
                    foreach (DBObject item4 in finalRegions)
                    {
                        if (item4 is Region)
                        {
                            Region finalRegion = item4 as Region;
                            if (finalRegion.Area > 0.0)
                            {
                                string finalHandle = AppendEntityWall(database, finalRegion);
                                ObjectId finalId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(finalHandle, 16)), 0);
                                if (!finalId.IsNull)
                                {
                                    ids.Add(finalId);
                                }
                            }
                        }
                        item4.Dispose();
                    }
                }
                catch (Exception)
                {
                    // Dispose objects on error
                    foreach (DBObject item in dbObjs)
                    {
                        item.Dispose();
                    }
                    return false;
                }

                return true;
            }

            Line line = new Line();
			line.SetDatabaseDefaults(database);
			line.StartPoint = ptSel;
			line.EndPoint = ptNew;
			if (line.Length <= 1.0)
			{
				line.Dispose();
				line = null;
				continue;
			}
			string value = AppendEntityWall(database, line);
			ObjectId objectId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
			if (!objectId.IsNull)
			{
				ids.Add(objectId);
			}
			ptSel = ptNew;
		}
		if (ids.Count <= 0)
		{
			return false;
		}
		PromptIntegerOptions promptIntegerOptions = new PromptIntegerOptions("\nInput thickness:");
		promptIntegerOptions.AllowNone = true;
		PromptIntegerResult integer = editor.GetInteger(promptIntegerOptions);
		if (integer.Status != PromptStatus.OK)
		{
			DeleteEntityS(database, ids);
			return false;
		}
		int value2 = integer.Value;
		double num = (double)value2 * 1.0 / 2.0;
		int num2 = 0;
		Point3dCollection point3dCollection = new Point3dCollection();
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		for (num2 = 0; num2 < ids.Count; num2++)
		{
			Line line2 = ids[num2].GetObject(OpenMode.ForRead) as Line;
			dBObjectCollection.Add(line2.Clone() as DBObject);
			Point3d startPoint = line2.StartPoint;
			Point3d endPoint = line2.EndPoint;
			if (startPoint.DistanceTo(endPoint) <= 1.0)
			{
				line2.Dispose();
				continue;
			}
			point3dCollection.Add(startPoint);
			if (num2 == ids.Count - 1)
			{
				point3dCollection.Add(endPoint);
			}
			line2.Dispose();
			line2 = null;
		}
		DeleteEntityS(database, ids);
		ids.Clear();
		DBObjectCollection dBObjectCollection2 = Region.CreateFromCurves(dBObjectCollection);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		if (dBObjectCollection2.Count != 0)
		{
			bool flag = false;
			foreach (DBObject item2 in dBObjectCollection2)
			{
				if (item2 is Region)
				{
					Region region = item2 as Region;
					if (region.Area > 0.0 && !flag)
					{
						point3dCollection.Clear();
						DBObjectCollection colExp = new DBObjectCollection();
						ExplodeRegion(region, ref colExp);
						for (int i = 0; i < colExp.Count; i++)
						{
							DBObject dBObject3 = colExp[i];
							if (dBObject3 is Line)
							{
								Line line3 = dBObject3 as Line;
								point3dCollection.Add(line3.StartPoint);
							}
							dBObject3.Dispose();
						}
						flag = true;
					}
				}
				item2.Dispose();
			}
		}
		Polyline polyline = new Polyline();
		polyline.SetDatabaseDefaults(database);
		for (num2 = 0; num2 < point3dCollection.Count; num2++)
		{
			polyline.AddVertexAt(num2, new Point2d(point3dCollection[num2].X, point3dCollection[num2].Y), 0.0, -1.0, -1.0);
		}
		dBObjectCollection = polyline.GetOffsetCurves(num);
		Polyline polyline2 = dBObjectCollection[0] as Polyline;
		int num3 = point3dCollection.Count;
		dBObjectCollection = polyline.GetOffsetCurves(-1.0 * num);
		if (dBObjectCollection[0] is Polyline)
		{
			Polyline polyline3 = dBObjectCollection[0] as Polyline;
			for (int num4 = polyline3.NumberOfVertices - 1; num4 >= 0; num4--)
			{
				try
				{
					polyline2.AddVertexAt(num3, polyline3.GetPoint2dAt(num4), 0.0, -1.0, -1.0);
					num3++;
				}
				catch (Exception)
				{
				}
			}
		}
		polyline2.Closed = true;
		dBObjectCollection.Clear();
		polyline2.Explode(dBObjectCollection);
		polyline2.Dispose();
		polyline2 = null;
		ids.Clear();
		try
		{
			DBObjectCollection dBObjectCollection3 = Region.CreateFromCurves(dBObjectCollection);
			foreach (DBObject item3 in dBObjectCollection)
			{
				item3.Dispose();
			}
			foreach (DBObject item4 in dBObjectCollection3)
			{
				if (item4 is Region)
				{
					Region region2 = item4 as Region;
					if (region2.Area > 0.0)
					{
						string value3 = AppendEntityWall(database, region2);
						ObjectId objectId2 = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value3, 16)), 0);
						if (!objectId2.IsNull)
						{
							ids.Add(objectId2);
						}
						continue;
					}
				}
				item4.Dispose();
			}
		}
		catch (Exception)
		{
			return false;
		}
		polyline.Dispose();
		polyline = null;
		return true;
	}

	public bool CheckPointInSameWall(Document doc, Point3d ptPrev, Point3d ptNew)
	{
		// Check if both points are on the SAME wall line entity
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return false;
		}

		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		foreach (ObjectId objectId in objectIds)
		{
			Line line = objectId.GetObject(OpenMode.ForRead) as Line;
			if (line == null)
			{
				continue;
			}
			int colorIndex = line.ColorIndex;
			if (colorIndex == GlobalFunction.nColorWall)
			{
				Point3d closestPointToPrev = line.GetClosestPointTo(ptPrev, extend: false);
				Point3d closestPointToNew = line.GetClosestPointTo(ptNew, extend: false);

				double distPrev = ptPrev.DistanceTo(closestPointToPrev);
				double distNew = ptNew.DistanceTo(closestPointToNew);

				// Check if both points are on the same wall line entity
				if (distPrev < 1.0 && distNew < 1.0)
				{
					line.Dispose();
					return true; // Both points are on the same wall line
				}
			}
			line.Dispose();
		}

		// Points are not on the same wall line
		return false;
	}

    public Point3d AdjustPointByDirection(Document doc, Point3d ptPrev, Point3d ptNew, string direction)
    {
        // Adjust point based on direction relative to previous point
        // Determine if this is a horizontal wall (Y is same) or vertical wall (X is same)
        bool isHorizontalWall = Math.Abs(ptPrev.Y - ptNew.Y) < 0.001;

        switch (direction)
        {
            case "L": // Left - keep same Y, use new X if it's to the left
                return new Point3d(ptNew.X, ptPrev.Y, 0.0);

            case "R": // Right - keep same Y, use new X
                return new Point3d(ptNew.X, ptPrev.Y, 0.0);

            case "T": // Top - draw above the wall line
                if (isHorizontalWall)
                {
                    // For horizontal wall, top means +Y direction (above)
                    // Always use the NEW Y value which is the click point
                    return new Point3d(ptPrev.X, ptNew.Y, 0.0);
                }
                else
                {
                    // For vertical wall, keep same X
                    return new Point3d(ptPrev.X, ptNew.Y, 0.0);
                }

            case "B": // Bottom - draw below the wall line
                if (isHorizontalWall)
                {
                    // For horizontal wall, bottom means -Y direction (below)
                    // Always use the NEW Y value which is the click point
                    return new Point3d(ptPrev.X, ptNew.Y, 0.0);
                }
                else
                {
                    // For vertical wall, keep same X
                    return new Point3d(ptPrev.X, ptNew.Y, 0.0);
                }

            default:
                return ptNew;
        }
    }

    public Region GetRegionMax(List<Region> lstRegion)
	{
		double num = 0.0;
		Region result = null;
		foreach (Region item in lstRegion)
		{
			if ((object)item != null && item.Area > num)
			{
				num = item.Area;
				result = item;
			}
		}
		return result;
	}

	public Region GetRegionBound(List<Region> lstRegion)
	{
		Region regionMax = GetRegionMax(lstRegion);
		foreach (Region item in lstRegion)
		{
			if (item.Area != regionMax.Area)
			{
				if (!regionMax.IsWriteEnabled)
				{
					regionMax.UpgradeOpen();
				}
				regionMax.BooleanOperation(BooleanOperationType.BoolUnite, item);
			}
		}
		return regionMax;
	}

	public Region CreateRegionSelected(List<Region> lstRegion, List<Polyline> lstPoly, ref List<Region> lstRegionIntersect)
	{
		return null;
	}

	public void GetAllRegion(Database db, DBObjectCollection dbColsObj, ref List<Region> lstRegion)
	{
		DBObjectCollection dBObjectCollection = Region.CreateFromCurves(dbColsObj);
		for (int i = 0; i < dBObjectCollection.Count; i++)
		{
			DBObject dBObject = dBObjectCollection[i];
			if (!(dBObject is Region))
			{
				dBObject.Dispose();
				continue;
			}
			Region region = dBObject as Region;
			if (region.Area <= 0.0)
			{
				dBObject.Dispose();
				continue;
			}
			region = GlobalFunction.AddWallToModelSpace(db, region).GetObject(OpenMode.ForRead) as Region;
			lstRegion.Add(region);
		}
	}

	private static void Swap<T>(ref T x, ref T y)
	{
		T val = y;
		y = x;
		x = val;
	}

	public bool InsertHatch(Database db, List<Region> lstRegionNotMax)
	{
		foreach (Region item in lstRegionNotMax)
		{
			try
			{
				ObjectIdCollection objectIdCollection = new ObjectIdCollection();
				objectIdCollection.Add(item.Id);
				Hatch hatch = new Hatch();
				hatch.SetDatabaseDefaults();
				hatch.Normal = new Vector3d(0.0, 0.0, 1.0);
				hatch.Elevation = 0.0;
				hatch.PatternScale = 1.0;
				hatch.PatternAngle = 0.0;
				hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
				hatch.ColorIndex = 5;
				hatch.Layer = "_0-5_図面";
				hatch.Associative = false;
				hatch.AppendLoop(HatchLoopTypes.Default, objectIdCollection);
				hatch.EvaluateHatch(underEstimateNumLines: true);
				AppendEntityLineHatch(db, hatch);
				item.Dispose();
			}
			catch (Exception)
			{
			}
		}
		return true;
	}

	public void UpdateHatCh(Database db, ObjectId id)
	{
		try
		{
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			objectIdCollection.Add(id);
			Hatch hatch = new Hatch();
			hatch.SetDatabaseDefaults();
			hatch.Normal = new Vector3d(0.0, 0.0, 1.0);
			hatch.Elevation = 0.0;
			hatch.PatternScale = 1.0;
			hatch.PatternAngle = 0.0;
			hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
			hatch.ColorIndex = 5;
			hatch.Layer = "_0-5_図面";
			hatch.Associative = false;
			hatch.AppendLoop(HatchLoopTypes.Default, objectIdCollection);
			hatch.EvaluateHatch(underEstimateNumLines: true);
			AppendEntityLineHatch(db, hatch);
		}
		catch (Exception)
		{
		}
	}

	public bool InsertWallTmp(ObjectId idRegionSel, List<Region> lstRegionNotMax, ref ObjectIdCollection idsLineRoom)
	{
		Database database = idRegionSel.Database;
		Region region = idRegionSel.GetObject(OpenMode.ForRead) as Region;
		List<Region> list = new List<Region>();
		foreach (Region item in lstRegionNotMax)
		{
			if (Check2Region(region, item) && !list.Contains(item))
			{
				list.Add(item);
			}
		}
		if (list.Count != 1)
		{
			region.UpgradeOpen();
			region.Erase();
			region.Dispose();
			region = null;
			return false;
		}
		Region x = region.Clone() as Region;
		Region otherRegion = list[0].Clone() as Region;
		x.BooleanOperation(BooleanOperationType.BoolIntersect, otherRegion);
		otherRegion = list[0].Clone() as Region;
		double area = otherRegion.Area;
		double area2 = x.Area;
		if (area > area2)
		{
			Swap(ref x, ref otherRegion);
		}
		x.BooleanOperation(BooleanOperationType.BoolSubtract, otherRegion);
		otherRegion.Dispose();
		otherRegion = null;
		DBObjectCollection colExp = new DBObjectCollection();
		ExplodeRegion(x, ref colExp);
		x.Dispose();
		x = null;
		foreach (DBObject item2 in colExp)
		{
			string value = AppendEntityWall(database, item2 as Entity);
			ObjectId objectId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
			idsLineRoom.Add(objectId);
		}
		foreach (Region item3 in lstRegionNotMax)
		{
			if (list.Contains(item3))
			{
				continue;
			}
			DBObjectCollection colExp2 = new DBObjectCollection();
			ExplodeRegion(item3, ref colExp2);
			foreach (DBObject item4 in colExp2)
			{
				AppendEntityWall(database, item4 as Entity);
			}
			if (!item3.IsWriteEnabled)
			{
				item3.UpgradeOpen();
			}
			item3.Erase();
			item3.Dispose();
		}
		list[0].UpgradeOpen();
		list[0].Erase();
		list[0].Dispose();
		list[0] = null;
		region.UpgradeOpen();
		region.Erase();
		region.Dispose();
		region = null;
		return true;
	}

	public bool InsertWall(ObjectId idRegionSel, List<Region> lstRegionNotMax, ref ObjectIdCollection idsLineRoom)
	{
		Database database = idRegionSel.Database;
		Region region = idRegionSel.GetObject(OpenMode.ForRead) as Region;
		List<Region> list = new List<Region>();
		foreach (Region item in lstRegionNotMax)
		{
			if (Check2Region(region, item) && !list.Contains(item))
			{
				list.Add(item);
			}
		}
		if (list.Count != 0)
		{
			foreach (Region item2 in lstRegionNotMax)
			{
				if (list.Contains(item2))
				{
					continue;
				}
				DBObjectCollection colExp = new DBObjectCollection();
				ExplodeRegion(item2, ref colExp);
				foreach (DBObject item3 in colExp)
				{
					AppendEntityWall(database, item3 as Entity);
				}
				if (!item2.IsWriteEnabled)
				{
					item2.UpgradeOpen();
				}
				item2.Erase();
				item2.Dispose();
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			Region x = region.Clone() as Region;
			Region otherRegion = list[i].Clone() as Region;
			x.BooleanOperation(BooleanOperationType.BoolIntersect, otherRegion);
			otherRegion = list[i].Clone() as Region;
			double area = otherRegion.Area;
			double area2 = x.Area;
			if (area > area2)
			{
				Swap(ref x, ref otherRegion);
			}
			x.BooleanOperation(BooleanOperationType.BoolSubtract, otherRegion);
			otherRegion.Dispose();
			otherRegion = null;
			DBObjectCollection colExp2 = new DBObjectCollection();
			ExplodeRegion(x, ref colExp2);
			x.Dispose();
			x = null;
			foreach (DBObject item4 in colExp2)
			{
				string value = AppendEntityWall(database, item4 as Entity);
				ObjectId objectId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
				idsLineRoom.Add(objectId);
			}
			list[i].UpgradeOpen();
			list[i].Erase();
			list[i].Dispose();
			list[i] = null;
		}
		region.UpgradeOpen();
		region.Erase();
		region.Dispose();
		region = null;
		return true;
	}

	public int UpdateHacthRoom(Document doc)
	{
		Editor editor = doc.Editor;
		Database database = doc.Database;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, "_0-5_図面")
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return -1;
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
				if (colorIndex == 3)
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
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		Region regionBound = GetRegionBound(lstRegion);
		Point3d minPoint = regionBound.GeometricExtents.MinPoint;
		Point3d maxPoint = regionBound.GeometricExtents.MaxPoint;
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region = lstRegion[j];
			if (!region.Id.Equals(regionBound.Id))
			{
				if (!region.IsWriteEnabled)
				{
					region.UpgradeOpen();
				}
				region.Erase();
				region.Dispose();
				region = null;
			}
		}
		lstRegion.Clear();
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		List<Region> list = new List<Region>();
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region2 = lstRegion[j];
			if (region2.Area <= 0.0)
			{
				lstRegion.RemoveAt(j);
				j--;
				if (!region2.IsWriteEnabled)
				{
					region2.UpgradeOpen();
				}
				region2.Erase();
				region2.Dispose();
				region2 = null;
			}
			else if (!(maxPoint.DistanceTo(region2.GeometricExtents.MaxPoint) <= 0.001) || !(minPoint.DistanceTo(region2.GeometricExtents.MinPoint) <= 0.001))
			{
				list.Add(region2);
			}
		}
		bool flag = InsertHatch(database, list);
		for (int j = 0; j < list.Count; j++)
		{
			Region region3 = list[j];
			DBObjectCollection colExp = new DBObjectCollection();
			ExplodeRegion(region3, ref colExp);
			foreach (DBObject item2 in colExp)
			{
				AppendEntityWall(database, item2 as Entity);
			}
			if (!region3.IsWriteEnabled)
			{
				region3.UpgradeOpen();
			}
			region3.Erase();
			region3.Dispose();
			region3 = null;
		}
		DBObjectCollection colExp2 = new DBObjectCollection();
		ExplodeRegion(regionBound, ref colExp2);
		foreach (DBObject item3 in colExp2)
		{
			AppendEntityWall(database, item3 as Entity);
		}
		if (!regionBound.IsWriteEnabled)
		{
			regionBound.UpgradeOpen();
		}
		regionBound.Erase();
		regionBound.Dispose();
		regionBound = null;
		foreach (ObjectId item4 in objectIdCollection)
		{
			Line line2 = item4.GetObject(OpenMode.ForWrite) as Line;
			line2.Erase();
			line2.Dispose();
			line2 = null;
		}
		if (!flag)
		{
			return 0;
		}
		return 1;
	}

	public bool CheckLineHasHatch(Line line, ObjectIdCollection idsLineInterSect)
	{
		Point3d startPoint = line.StartPoint;
		Point3d endPoint = line.EndPoint;
		foreach (ObjectId item in idsLineInterSect)
		{
			Line line2 = item.GetObject(OpenMode.ForRead) as Line;
			if (line2 == null)
			{
				continue;
			}
			Point3d closestPointTo = line2.GetClosestPointTo(startPoint, extend: false);
			double num = startPoint.DistanceTo(closestPointTo);
			if (num > 0.001)
			{
				line2.Dispose();
				line2 = null;
				continue;
			}
			closestPointTo = line2.GetClosestPointTo(endPoint, extend: false);
			num = endPoint.DistanceTo(closestPointTo);
			if (num > 0.001)
			{
				line2.Dispose();
				line2 = null;
				continue;
			}
			line2.Dispose();
			line2 = null;
			return true;
		}
		return false;
	}

	public bool Compare2Line(Line line1, Line line2)
	{
		if (line1.StartPoint.DistanceTo(line2.StartPoint) > 0.001 && line1.StartPoint.DistanceTo(line2.EndPoint) > 0.001)
		{
			return false;
		}
		if (line1.EndPoint.DistanceTo(line2.StartPoint) > 0.001 && line1.EndPoint.DistanceTo(line2.EndPoint) > 0.001)
		{
			return false;
		}
		return true;
	}

	public void AppendHatchFromRegion(Database db, Region region, DBObjectCollection dbColsObj, ObjectIdCollection idsLineInterSect)
	{
		List<Line> list = new List<Line>();
		DBObjectCollection dBObjectCollection = Region.CreateFromCurves(dbColsObj);
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		for (int i = 0; i < dBObjectCollection.Count; i++)
		{
			DBObject dBObject = dBObjectCollection[i];
			if (!(dBObject is Region))
			{
				dBObject.Dispose();
				continue;
			}
			Region region2 = dBObject as Region;
			if (region2.Area <= 0.0)
			{
				dBObject.Dispose();
				continue;
			}
			DBObjectCollection colExp = new DBObjectCollection();
			ExplodeRegion(region2, ref colExp);
			ObjectId value = GlobalFunction.AddWallToModelSpace(db, region2);
			objectIdCollection.Add(value);
			foreach (DBObject item in colExp)
			{
				if (item is Line)
				{
					Line line = item as Line;
					Point3d startPoint = line.StartPoint;
					Point3d endPoint = line.EndPoint;
					if (startPoint.DistanceTo(endPoint) <= 1.0)
					{
						item.Dispose();
					}
					else if (CheckLineHasHatch(line, idsLineInterSect))
					{
						double x = line.GeometricExtents.MinPoint.X + Math.Abs(line.GeometricExtents.MaxPoint.X - line.GeometricExtents.MinPoint.X) / 2.0;
						double y = line.GeometricExtents.MinPoint.Y + Math.Abs(line.GeometricExtents.MaxPoint.Y - line.GeometricExtents.MinPoint.Y) / 2.0;
						Point3d ptSel = new Point3d(x, y, 0.0);
						if (!CheckPointInsiheRegion(region, ptSel))
						{
							item.Dispose();
							continue;
						}
						bool flag = false;
						foreach (Line item2 in list)
						{
							if (Compare2Line(line, item2))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							list.Add(line);
						}
						else
						{
							item.Dispose();
						}
					}
					else
					{
						item.Dispose();
					}
				}
				else
				{
					item.Dispose();
				}
			}
			dBObject.Dispose();
		}
		DeleteEntityS(db, objectIdCollection);
		foreach (Line item3 in list)
		{
			AppendEntityLineHatch(db, item3);
		}
	}

	public void UpdateLineHatchOfRegion(Database db, Region region, ObjectIdCollection idsLine, ref ObjectIdCollection idsLineInterSect)
	{
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		foreach (ObjectId item in idsLine)
		{
			Line line = item.GetObject(OpenMode.ForRead) as Line;
			if (line == null)
			{
				continue;
			}
			Point3dCollection point3dCollection = new Point3dCollection();
			region.IntersectWith(line, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
			if (point3dCollection.Count != 0)
			{
				if (!idsLineInterSect.Contains(item))
				{
					idsLineInterSect.Add(item);
				}
				dBObjectCollection.Add(line.Clone() as DBObject);
			}
			line.Dispose();
			line = null;
		}
		if (dBObjectCollection.Count == 0)
		{
			return;
		}
		DBObjectCollection colExp = new DBObjectCollection();
		ExplodeRegion(region, ref colExp);
		foreach (DBObject item2 in colExp)
		{
			if (item2 is Line)
			{
				Line line2 = item2 as Line;
				Point3d startPoint = line2.StartPoint;
				Point3d endPoint = line2.EndPoint;
				if (startPoint.DistanceTo(endPoint) <= 1.0)
				{
					item2.Dispose();
				}
				else
				{
					dBObjectCollection.Add(item2);
				}
			}
			else
			{
				item2.Dispose();
			}
		}
		AppendHatchFromRegion(db, region, dBObjectCollection, idsLineInterSect);
		foreach (DBObject item3 in dBObjectCollection)
		{
			item3.Dispose();
		}
	}

	public void UpdateHatchWithLine(Document doc, ObjectIdCollection idsLineRoom)
	{
		Editor editor = doc.Editor;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerLineHatch)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return;
		}
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value = objectIds[i];
			Line line = (Line)value.GetObject(OpenMode.ForRead);
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				if (colorIndex == GlobalFunction.nColorLineHatch)
				{
					objectIdCollection.Add(value);
				}
				line.Dispose();
				line = null;
			}
		}
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		foreach (ObjectId item in idsLineRoom)
		{
			Line line2 = item.GetObject(OpenMode.ForRead) as Line;
			if (!(line2 == null))
			{
				DBObject value2 = line2.Clone() as DBObject;
				dBObjectCollection.Add(value2);
				line2.Dispose();
				line2 = null;
			}
		}
		ObjectIdCollection idsLineInterSect = new ObjectIdCollection();
		ObjectIdCollection objectIdCollection2 = new ObjectIdCollection();
		DBObjectCollection dBObjectCollection2 = Region.CreateFromCurves(dBObjectCollection);
		foreach (DBObject item2 in dBObjectCollection)
		{
			item2.Dispose();
		}
		for (int j = 0; j < dBObjectCollection2.Count; j++)
		{
			DBObject dBObject2 = dBObjectCollection2[j];
			if (!(dBObject2 is Region))
			{
				dBObject2.Dispose();
				continue;
			}
			Region region = dBObject2 as Region;
			if (region.Area <= 0.0)
			{
				dBObject2.Dispose();
				continue;
			}
			UpdateLineHatchOfRegion(doc.Database, region, objectIdCollection, ref idsLineInterSect);
			ObjectId value3 = GlobalFunction.AddWallToModelSpace(doc.Database, region);
			objectIdCollection2.Add(value3);
		}
		DeleteEntityS(doc.Database, idsLineInterSect);
		DeleteEntityS(doc.Database, objectIdCollection2);
	}

	public bool CheckPointIsOnRegionAll(List<Region> lstRegionNotMax, Point3d pt, bool bOut = false)
	{
		foreach (Region item in lstRegionNotMax)
		{
			if (CheckPointInsiheRegion(item, pt, bOut))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckPointIntersectIsOnRegion(List<Region> lstRegionNotMax, Point3d pt, ref Region res)
	{
		foreach (Region item in lstRegionNotMax)
		{
			if (CheckPointInsiheRegion(item, pt))
			{
				res = item;
				return true;
			}
		}
		return false;
	}

	public void DeleteRegionS(Database db, List<Region> lstRegionNotMax, Region reBound)
	{
		if (reBound != null)
		{
			if (!reBound.IsWriteEnabled)
			{
				reBound.UpgradeOpen();
			}
			reBound.Erase();
			reBound.Dispose();
			reBound = null;
		}
		for (int i = 0; i < lstRegionNotMax.Count; i++)
		{
			Region region = lstRegionNotMax[i];
			if (!(region == null))
			{
				if (!region.IsWriteEnabled)
				{
					region.UpgradeOpen();
				}
				region.Erase();
				region.Dispose();
				region = null;
			}
		}
	}

	public int GetListRegion(Document doc, ref List<Region> lstRegionNotMax, ref Region reBound)
	{
		Editor editor = doc.Editor;
		Database database = doc.Database;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return -1;
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
				if (colorIndex == GlobalFunction.nColorWall)
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
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		reBound = GetRegionBound(lstRegion);
		Point3d minPoint = reBound.GeometricExtents.MinPoint;
		Point3d maxPoint = reBound.GeometricExtents.MaxPoint;
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region = lstRegion[j];
			if (!region.Id.Equals(reBound.Id))
			{
				if (!region.IsWriteEnabled)
				{
					region.UpgradeOpen();
				}
				region.Erase();
				region.Dispose();
				region = null;
			}
		}
		lstRegion.Clear();
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region2 = lstRegion[j];
			if (region2.Area <= 0.0)
			{
				lstRegion.RemoveAt(j);
				j--;
				if (!region2.IsWriteEnabled)
				{
					region2.UpgradeOpen();
				}
				region2.Erase();
				region2.Dispose();
				region2 = null;
			}
			else if (maxPoint.DistanceTo(region2.GeometricExtents.MaxPoint) <= 0.001 && minPoint.DistanceTo(region2.GeometricExtents.MinPoint) <= 0.001)
			{
				if (!region2.IsWriteEnabled)
				{
					region2.UpgradeOpen();
				}
				region2.Erase();
				region2.Dispose();
				region2 = null;
			}
			else
			{
				lstRegionNotMax.Add(region2);
			}
		}
		return 1;
	}

	public bool getPointMin(Document doc, Point3d ptSel, ref Point3d ptRes)
	{
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			Application.ShowAlertDialog("The Drawing has not found Wall.\nYou can not setting correct. Please setting it.");
			return false;
		}
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value = objectIds[i];
			Line line = (Line)value.GetObject(OpenMode.ForRead);
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				line.Dispose();
				line = null;
				if (colorIndex == GlobalFunction.nColorWall)
				{
					objectIdCollection.Add(value);
				}
			}
		}
		if (objectIdCollection.Count == 0)
		{
			return false;
		}
		Line line2 = objectIdCollection[0].GetObject(OpenMode.ForRead) as Line;
		ptRes = line2.GetClosestPointTo(ptSel, extend: false);
		double num = ptSel.DistanceTo(ptRes);
		line2.Dispose();
		line2 = null;
		for (int j = 1; j < objectIdCollection.Count; j++)
		{
			line2 = objectIdCollection[j].GetObject(OpenMode.ForRead) as Line;
			Point3d closestPointTo = line2.GetClosestPointTo(ptSel, extend: false);
			double num2 = ptSel.DistanceTo(closestPointTo);
			line2.Dispose();
			line2 = null;
			if (num2 < num)
			{
				num = num2;
				ptRes = closestPointTo;
			}
		}
		return true;
	}

	public bool CheckExitRoom(Document doc)
	{
		TypedValue[] typedVals = new TypedValue[3]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall),
			new TypedValue(62, GlobalFunction.nColorWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			Application.ShowAlertDialog("The Drawing has not found Wall.\nYou can not setting correct. Please setting it.");
			return false;
		}
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		foreach (ObjectId objectId in objectIds)
		{
			using Line line = objectId.GetObject(OpenMode.ForRead) as Line;
			DBObject value = line.Clone() as DBObject;
			dBObjectCollection.Add(value);
		}
		bool flag = false;
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		DBObjectCollection dBObjectCollection2 = Region.CreateFromCurves(dBObjectCollection);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		foreach (DBObject item2 in dBObjectCollection2)
		{
			if (!(item2 is Region))
			{
				item2.Dispose();
				continue;
			}
			Region region = item2 as Region;
			if (region.Area > 0.0)
			{
				flag = true;
			}
			ObjectId value2 = GlobalFunction.AddWallToModelSpace(doc.Database, region);
			objectIdCollection.Add(value2);
		}
		DeleteEntityS(doc.Database, objectIdCollection);
		if (!flag)
		{
			Application.ShowAlertDialog("The Drawing has not found one Room.");
		}
		return flag;
	}

	public bool CheckPointSelectOutRoom(Document doc, Point3d ptSel)
	{
		List<Region> lstRegionNotMax = new List<Region>();
		Region reBound = new Region();
		GetListRegion(doc, ref lstRegionNotMax, ref reBound);
		doc.Editor.Regen();
		doc.Editor.UpdateScreen();
		bool result = CheckPointIsOnRegionAll(lstRegionNotMax, ptSel, bOut: true);
		DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
		doc.Editor.Regen();
		doc.Editor.UpdateScreen();
		return result;
	}

	public void SplitRoom(Document doc)
	{
		Database database = doc.Database;
		Editor editor = doc.Editor;
		if (!CheckExitRoom(doc))
		{
			return;
		}
		ObjectIdCollection ids;
		while (true)
		{
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nSelect Base point:";
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				return;
			}
			Point3d value = point.Value;
			Point3d ptRes = Point3d.Origin;
			if (!getPointMin(doc, value, ref ptRes))
			{
				continue;
			}
			value = ptRes;
			ids = new ObjectIdCollection();
			if (!SelectPoint(doc, value, ref ids))
			{
				break;
			}
			if (ids.Count >= 2)
			{
				DeleteEntityS(database, ids);
				editor.Regen();
				editor.UpdateScreen();
				continue;
			}
			editor.Regen();
			editor.UpdateScreen();
			ObjectId idWallSel = ids[0];
			bool flag = true;
			string text;
			do
			{
				text = "";
				PromptStringOptions promptStringOptions = new PromptStringOptions("\nAre you want split Room ?［Yes(Y)/No(N)] <Yes>:");
				promptStringOptions.AllowSpaces = true;
				PromptResult promptResult = editor.GetString(promptStringOptions);
				if (promptResult.Status == PromptStatus.Cancel)
				{
					DeleteEntityS(database, ids);
					return;
				}
				text = promptResult.StringResult;
				if (string.IsNullOrEmpty(text))
				{
					text = "Y";
				}
				text = text.Substring(0, 1);
			}
			while (string.Compare(text, "Y", ignoreCase: true) != 0 && string.Compare(text, "N", ignoreCase: true) != 0);
			if (string.Compare(text, "N", ignoreCase: true) == 0)
			{
				flag = false;
			}
			if (!flag)
			{
				DeleteEntityS(database, ids);
				editor.Regen();
				editor.UpdateScreen();
				continue;
			}
			int num = OperatorSplitRoom(doc, idWallSel);
			editor.Regen();
			editor.UpdateScreen();
			switch (num)
			{
			case -1:
				return;
			case 0:
				Application.ShowAlertDialog("The command has not split Room");
				break;
			}
		}
		DeleteEntityS(database, ids);
	}

	public void RemoveLineWallNotConnect(List<Region> lstRegionAll, ref ObjectIdCollection idsLine)
	{
		new DBObjectCollection();
		for (int i = 0; i < idsLine.Count; i++)
		{
			Line line = idsLine[i].GetObject(OpenMode.ForRead) as Line;
			if (line == null)
			{
				continue;
			}
			bool flag = true;
			foreach (Region item in lstRegionAll)
			{
				Point3dCollection point3dCollection = new Point3dCollection();
				item.IntersectWith(line, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
				if (point3dCollection.Count >= 2)
				{
					flag = false;
					break;
				}
			}
			line.Dispose();
			line = null;
			if (flag)
			{
				idsLine.RemoveAt(i);
				i--;
			}
		}
	}

	public int OperatorSplitRoom(Document doc, ObjectId idWallSel)
	{
		Editor editor = doc.Editor;
		Database database = doc.Database;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return -1;
		}
		ObjectIdCollection idsLine = new ObjectIdCollection();
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value = objectIds[i];
			Line line = (Line)value.GetObject(OpenMode.ForRead);
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				if (colorIndex == GlobalFunction.nColorWall)
				{
					DBObject value2 = line.Clone() as DBObject;
					dBObjectCollection.Add(value2);
					idsLine.Add(value);
				}
				line.Dispose();
				line = null;
			}
		}
		List<Region> lstRegion = new List<Region>();
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		RemoveLineWallNotConnect(lstRegion, ref idsLine);
		Region regionBound = GetRegionBound(lstRegion);
		Point3d minPoint = regionBound.GeometricExtents.MinPoint;
		Point3d maxPoint = regionBound.GeometricExtents.MaxPoint;
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region = lstRegion[j];
			if (!region.Id.Equals(regionBound.Id))
			{
				if (!region.IsWriteEnabled)
				{
					region.UpgradeOpen();
				}
				region.Erase();
				region.Dispose();
				region = null;
			}
		}
		lstRegion.Clear();
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		List<Region> list = new List<Region>();
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region2 = lstRegion[j];
			if (region2.Area <= 0.0)
			{
				lstRegion.RemoveAt(j);
				j--;
				if (!region2.IsWriteEnabled)
				{
					region2.UpgradeOpen();
				}
				region2.Erase();
				region2.Dispose();
				region2 = null;
			}
			else if (!(maxPoint.DistanceTo(region2.GeometricExtents.MaxPoint) <= 0.001) || !(minPoint.DistanceTo(region2.GeometricExtents.MinPoint) <= 0.001))
			{
				list.Add(region2);
			}
		}
		ObjectIdCollection idsLineRoom = new ObjectIdCollection();
		bool flag = InsertWall(idWallSel, list, ref idsLineRoom);
		if (!flag)
		{
			for (int j = 0; j < list.Count; j++)
			{
				Region region3 = list[j];
				DBObjectCollection colExp = new DBObjectCollection();
				ExplodeRegion(region3, ref colExp);
				foreach (DBObject item2 in colExp)
				{
					AppendEntityWall(database, item2 as Entity);
				}
				if (!region3.IsWriteEnabled)
				{
					region3.UpgradeOpen();
				}
				region3.Erase();
				region3.Dispose();
				region3 = null;
			}
		}
		DBObjectCollection colExp2 = new DBObjectCollection();
		ExplodeRegion(regionBound, ref colExp2);
		foreach (DBObject item3 in colExp2)
		{
			AppendEntityWall(database, item3 as Entity);
		}
		if (!regionBound.IsWriteEnabled)
		{
			regionBound.UpgradeOpen();
		}
		regionBound.Erase();
		regionBound.Dispose();
		regionBound = null;
		foreach (ObjectId item4 in idsLine)
		{
			Line line2 = item4.GetObject(OpenMode.ForWrite) as Line;
			line2.Erase();
			line2.Dispose();
			line2 = null;
		}
		if (!flag)
		{
			return 0;
		}
		UpdateHatchWithLine(doc, idsLineRoom);
		return 1;
	}

	public bool CreateRegionFromLine()
	{
		new List<ObjectId>();
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return false;
		}
		Database database = mdiActiveDocument.Database;
		Editor editor = mdiActiveDocument.Editor;
		PromptPointOptions promptPointOptions = new PromptPointOptions("");
		promptPointOptions.Message = "\nSelect Base point:";
		promptPointOptions.AllowNone = true;
		PromptPointResult point = editor.GetPoint(promptPointOptions);
		if (point.Status != PromptStatus.OK)
		{
			return false;
		}
		Point3d value = point.Value;
		ObjectIdCollection ids = null;
		if (!SelectPoint(mdiActiveDocument, value, ref ids))
		{
			DeleteEntityS(database, ids);
			return false;
		}
		if (ids.Count >= 2)
		{
			DeleteEntityS(database, ids);
			return false;
		}
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, "_0-5_図面")
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return false;
		}
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value2 = objectIds[i];
			Line line = (Line)value2.GetObject(OpenMode.ForRead);
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				if (colorIndex == 3)
				{
					DBObject value3 = line.Clone() as DBObject;
					dBObjectCollection.Add(value3);
					objectIdCollection.Add(value2);
				}
				line.Dispose();
				line = null;
			}
		}
		List<Region> lstRegion = new List<Region>();
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		Region regionBound = GetRegionBound(lstRegion);
		Point3d minPoint = regionBound.GeometricExtents.MinPoint;
		Point3d maxPoint = regionBound.GeometricExtents.MaxPoint;
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region = lstRegion[j];
			if (!region.Id.Equals(regionBound.Id))
			{
				if (!region.IsWriteEnabled)
				{
					region.UpgradeOpen();
				}
				region.Erase();
				region.Dispose();
				region = null;
			}
		}
		lstRegion.Clear();
		GetAllRegion(database, dBObjectCollection, ref lstRegion);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		List<Region> list = new List<Region>();
		for (int j = 0; j < lstRegion.Count; j++)
		{
			Region region2 = lstRegion[j];
			if (region2.Area <= 0.0)
			{
				lstRegion.RemoveAt(j);
				j--;
				if (!region2.IsWriteEnabled)
				{
					region2.UpgradeOpen();
				}
				region2.Erase();
				region2.Dispose();
				region2 = null;
			}
			else if (!(maxPoint.DistanceTo(region2.GeometricExtents.MaxPoint) <= 0.001) || !(minPoint.DistanceTo(region2.GeometricExtents.MinPoint) <= 0.001))
			{
				list.Add(region2);
			}
		}
		if (ids.Count == 1)
		{
			ObjectIdCollection idsLineRoom = new ObjectIdCollection();
			if (!InsertWall(ids[0], list, ref idsLineRoom))
			{
				for (int j = 0; j < list.Count; j++)
				{
					Region region3 = list[j];
					DBObjectCollection colExp = new DBObjectCollection();
					ExplodeRegion(region3, ref colExp);
					foreach (DBObject item2 in colExp)
					{
						AppendEntityWall(database, item2 as Entity);
					}
					if (!region3.IsWriteEnabled)
					{
						region3.UpgradeOpen();
					}
					region3.Erase();
					region3.Dispose();
					region3 = null;
				}
			}
		}
		DBObjectCollection colExp2 = new DBObjectCollection();
		ExplodeRegion(regionBound, ref colExp2);
		foreach (DBObject item3 in colExp2)
		{
			AppendEntityWall(database, item3 as Entity);
		}
		if (!regionBound.IsWriteEnabled)
		{
			regionBound.UpgradeOpen();
		}
		regionBound.Erase();
		regionBound.Dispose();
		regionBound = null;
		foreach (ObjectId item4 in objectIdCollection)
		{
			Line line2 = item4.GetObject(OpenMode.ForWrite) as Line;
			line2.Erase();
			line2.Dispose();
			line2 = null;
		}
		return true;
	}
}
