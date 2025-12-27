using System;
using System.Collections.Generic;
using System.IO;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CInsSymIntersect
{
	public const string S_NameFolderSym = "\\SymBackGround";

	public CSplitRoom split = new CSplitRoom();

	public CInsSymNearSquares symNear = new CInsSymNearSquares();

	public CSymbol symbol = new CSymbol();

	public bool CheckState2Line(ObjectId idLine1, ObjectId idLine2, ref Point3dCollection points)
	{
		if (idLine1.Equals(idLine2))
		{
			return false;
		}
		Line line = idLine1.GetObject(OpenMode.ForRead, openErased: false, forceOpenOnLockedLayer: true) as Line;
		if (line == null)
		{
			return false;
		}
		Line line2 = idLine2.GetObject(OpenMode.ForRead, openErased: false, forceOpenOnLockedLayer: true) as Line;
		if (line2 == null)
		{
			return false;
		}
		double angle = line.Angle;
		double angle2 = line2.Angle;
		if (Math.Abs(angle - angle2) >= 0.1)
		{
			line.Dispose();
			line = null;
			line2.Dispose();
			line2 = null;
			return false;
		}
		Point3d minPoint = line.GeometricExtents.MinPoint;
		points.Add(minPoint);
		_ = line.GeometricExtents.MaxPoint;
		_ = line2.GeometricExtents.MinPoint;
		Point3d maxPoint = line2.GeometricExtents.MaxPoint;
		points.Add(maxPoint);
		line.Dispose();
		line = null;
		line2.Dispose();
		line2 = null;
		return true;
	}

	public bool CheckXinIntersect(double x, Point3dCollection ptsInterSect)
	{
		foreach (Point3d item in ptsInterSect)
		{
			if (Math.Abs(x - item.X) <= 0.001)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckYinIntersect(double y, Point3dCollection ptsInterSect)
	{
		foreach (Point3d item in ptsInterSect)
		{
			if (Math.Abs(y - item.Y) <= 0.001)
			{
				return true;
			}
		}
		return false;
	}

	public void GetPostionInsert(ObjectId idLine1, ObjectId idLine2, double dOffset, List<CInterSectRegion> lstRegionInter, ref Point3dCollection ptsOffset)
	{
		Line line = idLine1.GetObject(OpenMode.ForRead, openErased: false, forceOpenOnLockedLayer: true) as Line;
		if (line == null)
		{
			return;
		}
		Line line2 = idLine2.GetObject(OpenMode.ForRead, openErased: false, forceOpenOnLockedLayer: true) as Line;
		if (line2 == null)
		{
			return;
		}
		Point3d minPoint = line.GeometricExtents.MinPoint;
		Point3d maxPoint = line.GeometricExtents.MaxPoint;
		Point3d minPoint2 = line2.GeometricExtents.MinPoint;
		_ = line2.GeometricExtents.MaxPoint;
		if (Math.Abs(minPoint.X - maxPoint.X) <= 0.001)
		{
			foreach (CInterSectRegion item in lstRegionInter)
			{
				Point3d maxPointY = GlobalFunction.GetMaxPointY(item.m_ptsInterSect);
				Point3d minPointY = GlobalFunction.GetMinPointY(item.m_ptsInterSect);
				if (minPoint.X < minPoint2.X)
				{
					for (double num = minPoint.X + dOffset; num < minPoint2.X; num += dOffset)
					{
						if (CheckXinIntersect(num, item.m_ptsInterSect))
						{
							continue;
						}
						for (double num2 = maxPointY.Y; num2 >= minPointY.Y; num2 -= dOffset)
						{
							Point3d point3d = new Point3d(num, num2, 0.0);
							if (!ptsOffset.Contains(point3d) && split.CheckPointInsiheRegion(item.m_ReObj, point3d))
							{
								ptsOffset.Add(point3d);
							}
						}
					}
					continue;
				}
				for (double num3 = minPoint.X - dOffset; num3 > minPoint2.X; num3 -= dOffset)
				{
					if (!CheckXinIntersect(num3, item.m_ptsInterSect))
					{
						for (double num4 = maxPointY.Y; num4 >= minPointY.Y; num4 -= dOffset)
						{
							Point3d point3d2 = new Point3d(num3, num4, 0.0);
							if (!ptsOffset.Contains(point3d2) && split.CheckPointInsiheRegion(item.m_ReObj, point3d2))
							{
								ptsOffset.Add(point3d2);
							}
						}
					}
				}
			}
		}
		if (Math.Abs(minPoint.Y - maxPoint.Y) <= 0.001)
		{
			foreach (CInterSectRegion item2 in lstRegionInter)
			{
				Point3d maxPointX = GlobalFunction.GetMaxPointX(item2.m_ptsInterSect);
				Point3d minPointX = GlobalFunction.GetMinPointX(item2.m_ptsInterSect);
				if (minPoint.Y < minPoint2.Y)
				{
					for (double num5 = minPoint.Y + dOffset; num5 < minPoint2.Y; num5 += dOffset)
					{
						if (CheckYinIntersect(num5, item2.m_ptsInterSect))
						{
							continue;
						}
						for (double num6 = minPointX.X; num6 <= maxPointX.X; num6 += dOffset)
						{
							Point3d point3d3 = new Point3d(num6, num5, 0.0);
							if (!ptsOffset.Contains(point3d3) && split.CheckPointInsiheRegion(item2.m_ReObj, point3d3))
							{
								ptsOffset.Add(point3d3);
							}
						}
					}
					continue;
				}
				for (double num7 = minPoint.Y - dOffset; num7 > minPoint2.Y; num7 -= dOffset)
				{
					if (!CheckYinIntersect(num7, item2.m_ptsInterSect))
					{
						for (double num8 = minPointX.X; num8 <= maxPointX.X; num8 += dOffset)
						{
							Point3d point3d4 = new Point3d(num8, num7, 0.0);
							if (!ptsOffset.Contains(point3d4) && split.CheckPointInsiheRegion(item2.m_ReObj, point3d4))
							{
								ptsOffset.Add(point3d4);
							}
						}
					}
				}
			}
		}
		line.Dispose();
		line = null;
		line2.Dispose();
		line2 = null;
	}

	public void GetPosInsertFromLine(ObjectId idLine1, double dOffset, List<CInterSectRegion> lstRegionInter, ref Point3dCollection ptsOffset)
	{
		Line line = idLine1.GetObject(OpenMode.ForRead, openErased: false, forceOpenOnLockedLayer: true) as Line;
		if (line == null)
		{
			return;
		}
		Point3d minPoint = line.GeometricExtents.MinPoint;
		Point3d maxPoint = line.GeometricExtents.MaxPoint;
		if (Math.Abs(minPoint.X - maxPoint.X) <= 0.001)
		{
			foreach (CInterSectRegion item in lstRegionInter)
			{
				foreach (Point3d item2 in item.m_ptsInterSect)
				{
					Point3d point3d2 = new Point3d(item2.X - dOffset, item2.Y, item2.Z);
					if (!ptsOffset.Contains(point3d2) && split.CheckPointInsiheRegion(item.m_ReObj, point3d2))
					{
						ptsOffset.Add(point3d2);
					}
					Point3d point3d3 = new Point3d(item2.X + dOffset, item2.Y, item2.Z);
					if (!ptsOffset.Contains(point3d3) && split.CheckPointInsiheRegion(item.m_ReObj, point3d3))
					{
						ptsOffset.Add(point3d3);
					}
				}
			}
		}
		if (Math.Abs(minPoint.Y - maxPoint.Y) <= 0.001)
		{
			foreach (CInterSectRegion item3 in lstRegionInter)
			{
				foreach (Point3d item4 in item3.m_ptsInterSect)
				{
					Point3d point3d5 = new Point3d(item4.X, item4.Y + dOffset, item4.Z);
					if (!ptsOffset.Contains(point3d5) && split.CheckPointInsiheRegion(item3.m_ReObj, point3d5))
					{
						ptsOffset.Add(point3d5);
					}
					Point3d point3d6 = new Point3d(item4.X, item4.Y - dOffset, item4.Z);
					if (!ptsOffset.Contains(point3d6) && split.CheckPointInsiheRegion(item3.m_ReObj, point3d6))
					{
						ptsOffset.Add(point3d6);
					}
				}
			}
		}
		line.Dispose();
		line = null;
	}

	public void GetPointFromAB(ObjectId idLineTruc, double dOffset, Point3dCollection ptsColIntersect, ref Point3dCollection ptsColInsert)
	{
		Line line = idLineTruc.GetObject(OpenMode.ForRead, openErased: false, forceOpenOnLockedLayer: true) as Line;
		if (line == null)
		{
			return;
		}
		Point3d minPoint = line.GeometricExtents.MinPoint;
		Point3d maxPoint = line.GeometricExtents.MaxPoint;
		double num = Math.Abs(maxPoint.X - minPoint.X);
		double num2 = Math.Abs(maxPoint.Y - minPoint.Y);
		if (num2 > num)
		{
			foreach (Point3d item in ptsColIntersect)
			{
				if (item.Y >= minPoint.Y && item.Y <= maxPoint.Y)
				{
					Point3d p = new Point3d(minPoint.X + dOffset, item.Y, item.Z);
					if (!ptsColInsert.Contains(p))
					{
						ptsColInsert.Add(p);
					}
				}
			}
		}
		else
		{
			foreach (Point3d item2 in ptsColIntersect)
			{
				if (item2.X >= minPoint.X && item2.X <= maxPoint.X)
				{
					Point3d p2 = new Point3d(item2.X, minPoint.Y + dOffset, item2.Z);
					if (!ptsColInsert.Contains(p2))
					{
						ptsColInsert.Add(p2);
					}
				}
			}
		}
		line.Dispose();
		line = null;
	}

	public void cmdInsSymIntersect()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		Database database = mdiActiveDocument.Database;
		Editor editor = mdiActiveDocument.Editor;
		if (!split.CheckExitRoom(mdiActiveDocument))
		{
			return;
		}
		string text;
		while (true)
		{
			Point3dCollection point3dCollection = new Point3dCollection();
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nSelect point 1:";
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				return;
			}
			Point3d value = point.Value;
			point3dCollection.Add(value);
			point = editor.GetCorner("\nSelect point 2:", value);
			if (point.Status != PromptStatus.OK)
			{
				return;
			}
			Point3d value2 = point.Value;
			point3dCollection.Add(new Point3d(value.X, value2.Y, 0.0));
			point3dCollection.Add(value2);
			point3dCollection.Add(new Point3d(value2.X, value.Y, 0.0));
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			try
			{
				PromptSelectionResult promptSelectionResult = editor.SelectCrossingWindow(value, value2);
				if (promptSelectionResult.Status != PromptStatus.OK)
				{
					continue;
				}
				ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
				for (int i = 0; i < objectIds.Length; i++)
				{
					ObjectId value3 = objectIds[i];
					Line line = value3.GetObject(OpenMode.ForRead) as Line;
					if (!(line == null))
					{
						string layer = line.Layer;
						int colorIndex = line.ColorIndex;
						if (layer.Equals(GlobalFunction.S_layerLineIntersect, StringComparison.CurrentCultureIgnoreCase) && colorIndex == GlobalFunction.nColorLineIntersect)
						{
							objectIdCollection.Add(value3);
						}
						line.Dispose();
						line = null;
					}
				}
				goto IL_01ac;
			}
			catch (Exception)
			{
				goto IL_01ac;
			}
			IL_01ac:
			if (objectIdCollection.Count == 0)
			{
				Application.ShowAlertDialog("The Object Line has not found in select Window.\nYou can not setting correct. Please setting it.");
				continue;
			}
			string sHandle = "";
			symNear.DrawPolySelect(database, point3dCollection, ref sHandle);
			double num = 0.0;
			while (true)
			{
				PromptDoubleOptions promptDoubleOptions = new PromptDoubleOptions("\nInput Distance:");
				promptDoubleOptions.AllowNegative = false;
				promptDoubleOptions.AllowZero = true;
				promptDoubleOptions.AllowNone = true;
				PromptDoubleResult promptDoubleResult = editor.GetDouble(promptDoubleOptions);
				if (promptDoubleResult.Status == PromptStatus.Cancel)
				{
					symNear.DeleteObject(database, sHandle);
					return;
				}
				if (promptDoubleResult.Status == PromptStatus.OK)
				{
					if (!(promptDoubleResult.Value < 0.0))
					{
						num = promptDoubleResult.Value;
						break;
					}
				}
				else if (promptDoubleResult.Status == PromptStatus.None)
				{
					num = 0.0;
					break;
				}
			}
			List<Region> lstRegionNotMax = new List<Region>();
			Region reBound = new Region();
			split.GetListRegion(mdiActiveDocument, ref lstRegionNotMax, ref reBound);
			editor.Regen();
			editor.UpdateScreen();
			Point3dCollection point3dCollection2 = new Point3dCollection();
			List<CInterSectRegion> list = new List<CInterSectRegion>();
			new List<Region>();
			for (int j = 0; j < objectIdCollection.Count - 1; j++)
			{
				Entity entity = objectIdCollection[j].GetObject(OpenMode.ForRead) as Entity;
				for (int k = j + 1; k < objectIdCollection.Count; k++)
				{
					Entity entity2 = objectIdCollection[k].GetObject(OpenMode.ForRead) as Entity;
					Point3dCollection point3dCollection3 = new Point3dCollection();
					entity.IntersectWith(entity2, Intersect.OnBothOperands, point3dCollection3, IntPtr.Zero, IntPtr.Zero);
					entity2.Dispose();
					entity2 = null;
					if (point3dCollection3.Count == 0)
					{
						continue;
					}
					foreach (Point3d item in point3dCollection3)
					{
						Region res = new Region();
						if (split.CheckPointIntersectIsOnRegion(lstRegionNotMax, item, ref res))
						{
							bool flag = false;
							int num2 = 0;
							for (num2 = 0; num2 < list.Count; num2++)
							{
								CInterSectRegion cInterSectRegion = list[num2];
								if (cInterSectRegion.m_ReObj.Equals(res))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								CInterSectRegion cInterSectRegion2 = new CInterSectRegion();
								cInterSectRegion2.m_ReObj = res;
								cInterSectRegion2.m_ptsInterSect.Add(item);
								list.Add(cInterSectRegion2);
							}
							else if (!list[num2].m_ptsInterSect.Contains(item))
							{
								list[num2].m_ptsInterSect.Add(item);
							}
						}
						else
						{
							res.Dispose();
							res = null;
						}
					}
				}
				entity.Dispose();
				entity = null;
			}
			if (num != 0.0)
			{
				if (list.Count == 0)
				{
					num = 0.0;
				}
				else
				{
					bool flag2 = false;
					PromptEntityOptions promptEntityOptions = new PromptEntityOptions("\n Select Line 1:");
					promptEntityOptions.SetRejectMessage("\nSelect object Line");
					promptEntityOptions.AddAllowedClass(typeof(Line), match: true);
					promptEntityOptions.AllowNone = true;
					PromptEntityResult entity3 = editor.GetEntity(promptEntityOptions);
					if (entity3.Status != PromptStatus.OK)
					{
						flag2 = true;
					}
					else
					{
						ObjectId objectId = entity3.ObjectId;
						Point3dCollection ptsOffset = new Point3dCollection();
						GetPosInsertFromLine(objectId, num, list, ref ptsOffset);
						point3dCollection2.Clear();
						point3dCollection2 = ptsOffset;
					}
					if (flag2)
					{
						symNear.DeleteObject(database, sHandle);
						split.DeleteRegionS(database, lstRegionNotMax, reBound);
						editor.Regen();
						editor.UpdateScreen();
						return;
					}
				}
			}
			if (num == 0.0)
			{
				foreach (CInterSectRegion item2 in list)
				{
					foreach (Point3d item3 in item2.m_ptsInterSect)
					{
						point3dCollection2.Add(item3);
					}
				}
			}
			symNear.DeleteObject(database, sHandle);
			split.DeleteRegionS(database, lstRegionNotMax, reBound);
			if (point3dCollection2.Count == 0)
			{
				Application.ShowAlertDialog("The command has not found intersect of Object.");
				editor.Regen();
				editor.UpdateScreen();
				continue;
			}
			string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
			text = pathFolderSymbol + "\\SymBackGround\\symIntersect.dwg";
			if (!File.Exists(text))
			{
				break;
			}
			ObjectIdCollection idsLine = new ObjectIdCollection();
			symbol.DeleteBlockRefSame(mdiActiveDocument, "symIntersect", point3dCollection2, ref idsLine);
			foreach (Point3d item4 in point3dCollection2)
			{
				if (!symbol.InsertBlockFromFile(database, text, item4))
				{
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
			}
			GlobalFunction.DeleteEntityS(database, idsLine);
			editor.Regen();
			editor.UpdateScreen();
		}
		string message = text + "do not found.";
		editor.WriteMessage("\n");
		editor.WriteMessage(message);
		editor.Regen();
		editor.UpdateScreen();
	}

	public void cmdInsSymIntersect2()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		Database database = mdiActiveDocument.Database;
		Editor editor = mdiActiveDocument.Editor;
		if (!split.CheckExitRoom(mdiActiveDocument))
		{
			return;
		}
		object systemVariable = Application.GetSystemVariable("ORTHOMODE");
		string text2;
		while (true)
		{
			Point3dCollection point3dCollection = new Point3dCollection();
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nSelect point A:";
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				Application.SetSystemVariable("ORTHOMODE", systemVariable);
				return;
			}
			Point3d value = point.Value;
			Application.SetSystemVariable("ORTHOMODE", 1);
			point3dCollection.Add(value);
			promptPointOptions.Message = "\nSelect point B:";
			promptPointOptions.UseBasePoint = true;
			promptPointOptions.BasePoint = value;
			promptPointOptions.AllowNone = true;
			point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				Application.SetSystemVariable("ORTHOMODE", systemVariable);
				return;
			}
			_ = point.Value;
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
			string text = GlobalFunction.AppendEntityWall(database, line);
			ObjectId objectId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(text, 16)), 0);
			double num = 0.0;
			while (true)
			{
				PromptDoubleOptions promptDoubleOptions = new PromptDoubleOptions("\nInput Distance:");
				promptDoubleOptions.AllowNegative = true;
				promptDoubleOptions.AllowZero = false;
				promptDoubleOptions.AllowNone = false;
				PromptDoubleResult promptDoubleResult = editor.GetDouble(promptDoubleOptions);
				if (promptDoubleResult.Status == PromptStatus.Cancel)
				{
					symNear.DeleteObject(database, text);
					Application.SetSystemVariable("ORTHOMODE", systemVariable);
					return;
				}
				if (promptDoubleResult.Status == PromptStatus.OK)
				{
					num = promptDoubleResult.Value;
					break;
				}
				if (promptDoubleResult.Status == PromptStatus.None)
				{
					num = 0.0;
					break;
				}
			}
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			try
			{
				TypedValue[] typedVals = new TypedValue[2]
				{
					new TypedValue(0, "line"),
					new TypedValue(8, GlobalFunction.S_layerLineIntersect)
				};
				SelectionFilter filter = new SelectionFilter(typedVals);
				PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
				if (promptSelectionResult.Status != PromptStatus.OK)
				{
					symNear.DeleteObject(database, text);
					Application.SetSystemVariable("ORTHOMODE", systemVariable);
					return;
				}
				ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
				for (int i = 0; i < objectIds.Length; i++)
				{
					ObjectId value2 = objectIds[i];
					Line line2 = value2.GetObject(OpenMode.ForRead) as Line;
					if (!(line2 == null))
					{
						string layer = line2.Layer;
						int colorIndex = line2.ColorIndex;
						if (layer.Equals(GlobalFunction.S_layerLineIntersect, StringComparison.CurrentCultureIgnoreCase) && colorIndex == GlobalFunction.nColorLineIntersect)
						{
							objectIdCollection.Add(value2);
						}
						line2.Dispose();
						line2 = null;
					}
				}
			}
			catch (Exception)
			{
			}
			if (objectIdCollection.Count == 0)
			{
				symNear.DeleteObject(database, text);
				Application.ShowAlertDialog("The Object Line has not found in Drawing.\nYou can not setting correct. Please setting it.");
				continue;
			}
			Point3dCollection ptsColInsert = new Point3dCollection();
			for (int j = 0; j < objectIdCollection.Count - 1; j++)
			{
				Entity entity = objectIdCollection[j].GetObject(OpenMode.ForRead) as Entity;
				for (int k = j + 1; k < objectIdCollection.Count; k++)
				{
					Entity entity2 = objectIdCollection[k].GetObject(OpenMode.ForRead) as Entity;
					Point3dCollection point3dCollection2 = new Point3dCollection();
					entity.IntersectWith(entity2, Intersect.OnBothOperands, point3dCollection2, IntPtr.Zero, IntPtr.Zero);
					entity2.Dispose();
					entity2 = null;
					if (point3dCollection2.Count != 0)
					{
						GetPointFromAB(objectId, num, point3dCollection2, ref ptsColInsert);
					}
				}
				entity.Dispose();
				entity = null;
			}
			symNear.DeleteObject(database, text);
			if (ptsColInsert.Count == 0)
			{
				Application.ShowAlertDialog("The command has not found intersect of Objects.");
				editor.Regen();
				editor.UpdateScreen();
				continue;
			}
			List<Region> lstRegionNotMax = new List<Region>();
			Region reBound = new Region();
			split.GetListRegion(mdiActiveDocument, ref lstRegionNotMax, ref reBound);
			editor.Regen();
			editor.UpdateScreen();
			Point3dCollection point3dCollection3 = new Point3dCollection();
			foreach (Point3d item in ptsColInsert)
			{
				if (split.CheckPointIsOnRegionAll(lstRegionNotMax, item))
				{
					point3dCollection3.Add(item);
				}
			}
			split.DeleteRegionS(database, lstRegionNotMax, reBound);
			if (point3dCollection3.Count == 0)
			{
				Application.ShowAlertDialog("The command has not found intersect of Object.");
				editor.Regen();
				editor.UpdateScreen();
				continue;
			}
			string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
			text2 = pathFolderSymbol + "\\SymBackGround\\symIntersect.dwg";
			if (!File.Exists(text2))
			{
				break;
			}
			ObjectIdCollection idsLine = new ObjectIdCollection();
			symbol.DeleteBlockRefSame(mdiActiveDocument, "symIntersect", point3dCollection3, ref idsLine);
			foreach (Point3d item2 in point3dCollection3)
			{
				if (!symbol.InsertBlockFromFile(database, text2, item2))
				{
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
			}
			GlobalFunction.DeleteEntityS(database, idsLine);
			editor.Regen();
			editor.UpdateScreen();
		}
		string message = text2 + "do not found.";
		editor.WriteMessage("\n");
		editor.WriteMessage(message);
		editor.Regen();
		editor.UpdateScreen();
	}

	public void cmdInsSymIntersect_sel2()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		Database database = mdiActiveDocument.Database;
		Editor editor = mdiActiveDocument.Editor;
		if (!split.CheckExitRoom(mdiActiveDocument))
		{
			return;
		}
		string text;
		while (true)
		{
			Point3dCollection points = new Point3dCollection();
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nSelect point 1:";
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				return;
			}
			Point3d value = point.Value;
			points.Add(value);
			point = editor.GetCorner("\nSelect point 2:", value);
			if (point.Status != PromptStatus.OK)
			{
				return;
			}
			Point3d value2 = point.Value;
			points.Add(new Point3d(value.X, value2.Y, 0.0));
			points.Add(value2);
			points.Add(new Point3d(value2.X, value.Y, 0.0));
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			try
			{
				PromptSelectionResult promptSelectionResult = editor.SelectCrossingWindow(value, value2);
				if (promptSelectionResult.Status != PromptStatus.OK)
				{
					continue;
				}
				ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
				for (int i = 0; i < objectIds.Length; i++)
				{
					ObjectId value3 = objectIds[i];
					Line line = value3.GetObject(OpenMode.ForRead) as Line;
					if (!(line == null))
					{
						string layer = line.Layer;
						int colorIndex = line.ColorIndex;
						if (layer.Equals(GlobalFunction.S_layerLineIntersect, StringComparison.CurrentCultureIgnoreCase) && colorIndex == GlobalFunction.nColorLineIntersect)
						{
							objectIdCollection.Add(value3);
						}
						line.Dispose();
						line = null;
					}
				}
				goto IL_01ac;
			}
			catch (Exception)
			{
				goto IL_01ac;
			}
			IL_01ac:
			if (objectIdCollection.Count == 0)
			{
				Application.ShowAlertDialog("The Object Line has not found in select Window.\nYou can not setting correct. Please setting it.");
				continue;
			}
			string sHandle = "";
			symNear.DrawPolySelect(database, points, ref sHandle);
			double num = 0.0;
			while (true)
			{
				PromptDoubleOptions promptDoubleOptions = new PromptDoubleOptions("\nInput Distance:");
				promptDoubleOptions.AllowNegative = false;
				promptDoubleOptions.AllowZero = true;
				promptDoubleOptions.AllowNone = true;
				PromptDoubleResult promptDoubleResult = editor.GetDouble(promptDoubleOptions);
				if (promptDoubleResult.Status == PromptStatus.Cancel)
				{
					symNear.DeleteObject(database, sHandle);
					return;
				}
				if (promptDoubleResult.Status == PromptStatus.OK)
				{
					if (!(promptDoubleResult.Value < 0.0))
					{
						num = promptDoubleResult.Value;
						break;
					}
				}
				else if (promptDoubleResult.Status == PromptStatus.None)
				{
					num = 0.0;
					break;
				}
			}
			List<Region> lstRegionNotMax = new List<Region>();
			Region reBound = new Region();
			split.GetListRegion(mdiActiveDocument, ref lstRegionNotMax, ref reBound);
			editor.Regen();
			editor.UpdateScreen();
			Point3dCollection point3dCollection = new Point3dCollection();
			List<CInterSectRegion> list = new List<CInterSectRegion>();
			new List<Region>();
			for (int j = 0; j < objectIdCollection.Count - 1; j++)
			{
				Entity entity = objectIdCollection[j].GetObject(OpenMode.ForRead) as Entity;
				for (int k = j + 1; k < objectIdCollection.Count; k++)
				{
					Entity entity2 = objectIdCollection[k].GetObject(OpenMode.ForRead) as Entity;
					Point3dCollection point3dCollection2 = new Point3dCollection();
					entity.IntersectWith(entity2, Intersect.OnBothOperands, point3dCollection2, IntPtr.Zero, IntPtr.Zero);
					entity2.Dispose();
					entity2 = null;
					if (point3dCollection2.Count == 0)
					{
						continue;
					}
					foreach (Point3d item in point3dCollection2)
					{
						Region res = new Region();
						if (split.CheckPointIntersectIsOnRegion(lstRegionNotMax, item, ref res))
						{
							bool flag = false;
							int num2 = 0;
							for (num2 = 0; num2 < list.Count; num2++)
							{
								CInterSectRegion cInterSectRegion = list[num2];
								if (cInterSectRegion.m_ReObj.Equals(res))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								CInterSectRegion cInterSectRegion2 = new CInterSectRegion();
								cInterSectRegion2.m_ReObj = res;
								cInterSectRegion2.m_ptsInterSect.Add(item);
								list.Add(cInterSectRegion2);
							}
							else if (!list[num2].m_ptsInterSect.Contains(item))
							{
								list[num2].m_ptsInterSect.Add(item);
							}
						}
						else
						{
							res.Dispose();
							res = null;
						}
					}
				}
				entity.Dispose();
				entity = null;
			}
			if (num != 0.0)
			{
				if (list.Count == 0)
				{
					num = 0.0;
				}
				else
				{
					bool flag2 = false;
					PromptEntityOptions promptEntityOptions = new PromptEntityOptions("\n Select Line 1:");
					promptEntityOptions.SetRejectMessage("\nSelect object Line");
					promptEntityOptions.AddAllowedClass(typeof(Line), match: true);
					promptEntityOptions.AllowNone = true;
					PromptEntityResult entity3 = editor.GetEntity(promptEntityOptions);
					if (entity3.Status != PromptStatus.OK)
					{
						flag2 = true;
					}
					else
					{
						ObjectId objectId = entity3.ObjectId;
						while (true)
						{
							promptEntityOptions.Message = "\n Select Line 2:";
							entity3 = editor.GetEntity(promptEntityOptions);
							if (entity3.Status == PromptStatus.Cancel)
							{
								flag2 = true;
								break;
							}
							if (entity3.Status == PromptStatus.OK)
							{
								ObjectId objectId2 = entity3.ObjectId;
								if (CheckState2Line(objectId, objectId2, ref points))
								{
									Point3dCollection ptsOffset = new Point3dCollection();
									GetPostionInsert(objectId, objectId2, num, list, ref ptsOffset);
									point3dCollection.Clear();
									point3dCollection = ptsOffset;
									break;
								}
								editor.WriteMessage("\n Select Line 2 other and same direction with Line 1");
							}
						}
					}
					if (flag2)
					{
						symNear.DeleteObject(database, sHandle);
						split.DeleteRegionS(database, lstRegionNotMax, reBound);
						editor.Regen();
						editor.UpdateScreen();
						return;
					}
				}
			}
			if (num == 0.0)
			{
				foreach (CInterSectRegion item2 in list)
				{
					foreach (Point3d item3 in item2.m_ptsInterSect)
					{
						point3dCollection.Add(item3);
					}
				}
			}
			symNear.DeleteObject(database, sHandle);
			split.DeleteRegionS(database, lstRegionNotMax, reBound);
			if (point3dCollection.Count == 0)
			{
				Application.ShowAlertDialog("The command has not found intersect of Object.");
				editor.Regen();
				editor.UpdateScreen();
				continue;
			}
			string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
			text = pathFolderSymbol + "\\SymBackGround\\symIntersect.dwg";
			if (!File.Exists(text))
			{
				break;
			}
			ObjectIdCollection idsLine = new ObjectIdCollection();
			symbol.DeleteBlockRefSame(mdiActiveDocument, "symIntersect", point3dCollection, ref idsLine);
			foreach (Point3d item4 in point3dCollection)
			{
				if (!symbol.InsertBlockFromFile(database, text, item4))
				{
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
			}
			GlobalFunction.DeleteEntityS(database, idsLine);
			editor.Regen();
			editor.UpdateScreen();
		}
		string message = text + "do not found.";
		editor.WriteMessage("\n");
		editor.WriteMessage(message);
		editor.Regen();
		editor.UpdateScreen();
	}

	public void cmdInsSymIntersect_Poly()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		Database database = mdiActiveDocument.Database;
		Editor editor = mdiActiveDocument.Editor;
		CSplitRoom cSplitRoom = new CSplitRoom();
		if (!cSplitRoom.CheckExitRoom(mdiActiveDocument))
		{
			return;
		}
		CSymbol cSymbol = new CSymbol();
		string text;
		while (true)
		{
			Point3dCollection point3dCollection = new Point3dCollection();
			PromptPointOptions promptPointOptions = new PromptPointOptions("");
			promptPointOptions.Message = "\nSelect point 1:";
			promptPointOptions.AllowNone = true;
			PromptPointResult point = editor.GetPoint(promptPointOptions);
			if (point.Status != PromptStatus.OK)
			{
				return;
			}
			Point3d value = point.Value;
			point3dCollection.Add(value);
			point = editor.GetCorner("\nSelect point 2:", value);
			if (point.Status != PromptStatus.OK)
			{
				return;
			}
			Point3d value2 = point.Value;
			point3dCollection.Add(value2);
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			try
			{
				PromptSelectionResult promptSelectionResult = editor.SelectCrossingWindow(point3dCollection[0], point3dCollection[1]);
				if (promptSelectionResult.Status != PromptStatus.OK)
				{
					continue;
				}
				ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
				for (int i = 0; i < objectIds.Length; i++)
				{
					ObjectId value3 = objectIds[i];
					Line line = value3.GetObject(OpenMode.ForRead) as Line;
					if (!(line == null))
					{
						string layer = line.Layer;
						int colorIndex = line.ColorIndex;
						if (layer.Equals(GlobalFunction.S_layerLineIntersect, StringComparison.CurrentCultureIgnoreCase) && colorIndex == GlobalFunction.nColorLineIntersect)
						{
							objectIdCollection.Add(value3);
						}
						line.Dispose();
						line = null;
					}
				}
				goto IL_017d;
			}
			catch (Exception)
			{
				goto IL_017d;
			}
			IL_017d:
			if (objectIdCollection.Count == 0)
			{
				Application.ShowAlertDialog("The Object Line has not found in select Window.\nYou can not setting correct. Please setting it.");
				continue;
			}
			List<Region> lstRegionNotMax = new List<Region>();
			Region reBound = new Region();
			cSplitRoom.GetListRegion(mdiActiveDocument, ref lstRegionNotMax, ref reBound);
			editor.Regen();
			editor.UpdateScreen();
			Point3dCollection point3dCollection2 = new Point3dCollection();
			for (int j = 0; j < objectIdCollection.Count - 1; j++)
			{
				Entity entity = objectIdCollection[j].GetObject(OpenMode.ForRead) as Entity;
				for (int k = j + 1; k < objectIdCollection.Count; k++)
				{
					Entity entity2 = objectIdCollection[k].GetObject(OpenMode.ForRead) as Entity;
					Point3dCollection point3dCollection3 = new Point3dCollection();
					entity.IntersectWith(entity2, Intersect.OnBothOperands, point3dCollection3, IntPtr.Zero, IntPtr.Zero);
					entity2.Dispose();
					entity2 = null;
					if (point3dCollection3.Count == 0)
					{
						continue;
					}
					foreach (Point3d item in point3dCollection3)
					{
						if (!point3dCollection2.Contains(item) && cSplitRoom.CheckPointIsOnRegionAll(lstRegionNotMax, item))
						{
							point3dCollection2.Add(item);
						}
					}
				}
				entity.Dispose();
				entity = null;
			}
			cSplitRoom.DeleteRegionS(database, lstRegionNotMax, reBound);
			if (point3dCollection2.Count == 0)
			{
				Application.ShowAlertDialog("The command has not found intersect of Object.");
				editor.Regen();
				editor.UpdateScreen();
				continue;
			}
			string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
			text = pathFolderSymbol + "\\SymBackGround\\symIntersect.dwg";
			if (!File.Exists(text))
			{
				break;
			}
			ObjectIdCollection idsLine = new ObjectIdCollection();
			cSymbol.DeleteBlockRefSame(mdiActiveDocument, "symIntersect", point3dCollection2, ref idsLine);
			foreach (Point3d item2 in point3dCollection2)
			{
				if (!cSymbol.InsertBlockFromFile(database, text, item2))
				{
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
			}
			GlobalFunction.DeleteEntityS(database, idsLine);
			editor.Regen();
			editor.UpdateScreen();
		}
		string message = text + "do not found.";
		editor.WriteMessage("\n");
		editor.WriteMessage(message);
		editor.Regen();
		editor.UpdateScreen();
	}

	public int DeleteGridsIntersect(Document doc)
	{
		Editor editor = doc.Editor;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerLineIntersect)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return 0;
		}
		int num = 0;
		new CSplitRoom();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		foreach (ObjectId objectId in objectIds)
		{
			Line line = objectId.GetObject(OpenMode.ForRead) as Line;
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				if (colorIndex == GlobalFunction.nColorLineIntersect)
				{
					line.UpgradeOpen();
					line.Erase();
					num++;
				}
				line.Dispose();
				line = null;
			}
		}
		return num;
	}

	public void cmdDrawGridsIntersect(Document doc)
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
		Editor editor = doc.Editor;
		double num2 = 0.0;
		PromptDoubleResult promptDoubleResult;
		do
		{
			PromptDoubleOptions promptDoubleOptions = new PromptDoubleOptions("\nInput Distance:");
			promptDoubleOptions.AllowNegative = false;
			promptDoubleOptions.AllowZero = false;
			promptDoubleOptions.AllowNone = true;
			promptDoubleResult = editor.GetDouble(promptDoubleOptions);
			if (promptDoubleResult.Status == PromptStatus.Cancel)
			{
				return;
			}
		}
		while (promptDoubleResult.Status != PromptStatus.OK || promptDoubleResult.Value <= 0.0);
		num2 = promptDoubleResult.Value;
		PromptIntegerResult integer;
		do
		{
			PromptIntegerOptions promptIntegerOptions = new PromptIntegerOptions("\nInput N Row:");
			promptIntegerOptions.AllowNegative = false;
			promptIntegerOptions.AllowZero = false;
			promptIntegerOptions.AllowNone = true;
			integer = editor.GetInteger(promptIntegerOptions);
			if (integer.Status == PromptStatus.Cancel)
			{
				return;
			}
		}
		while (integer.Status != PromptStatus.OK || integer.Value <= 0);
		int value = integer.Value;
		PromptIntegerResult integer2;
		do
		{
			PromptIntegerOptions promptIntegerOptions2 = new PromptIntegerOptions("\nInput N Column:");
			promptIntegerOptions2.AllowNegative = false;
			promptIntegerOptions2.AllowZero = false;
			promptIntegerOptions2.AllowNone = true;
			integer2 = editor.GetInteger(promptIntegerOptions2);
			if (integer2.Status == PromptStatus.Cancel)
			{
				return;
			}
		}
		while (integer2.Status != PromptStatus.OK || integer2.Value <= 0);
		int value2 = integer2.Value;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerLineSquare)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			Application.ShowAlertDialog("The Drawing has not found Wall.\nYou can not setting correct. Please setting it.");
			return;
		}
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value3 = objectIds[i];
			Line line = (Line)value3.GetObject(OpenMode.ForRead);
			if (!(line == null))
			{
				int colorIndex = line.ColorIndex;
				if (colorIndex == GlobalFunction.nColorLineSquare)
				{
					DBObject value4 = line.Clone() as DBObject;
					dBObjectCollection.Add(value4);
					objectIdCollection.Add(value3);
				}
				line.Dispose();
				line = null;
			}
		}
		CInsSymNearSquares cInsSymNearSquares = new CInsSymNearSquares();
		List<Region> lstRegion = new List<Region>();
		cInsSymNearSquares.CreateRegionSquares(doc.Database, dBObjectCollection, ref lstRegion);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		if (lstRegion.Count == 0)
		{
			Application.ShowAlertDialog("The Drawing has not found one Room.");
			return;
		}
		List<Region> lstRegionNotMax = new List<Region>();
		Region reBound = null;
		split.GetListRegion(doc, ref lstRegionNotMax, ref reBound);
		Point3dCollection point3dCollection = new Point3dCollection();
		foreach (Region item2 in lstRegion)
		{
			if (cInsSymNearSquares.CheckObjectIsSquare(item2))
			{
				Point3d point3d = GlobalFunction.PointCenter(item2);
				if (split.CheckPointInsiheRegion(reBound, point3d))
				{
					point3dCollection.Add(point3d);
				}
			}
		}
		split.DeleteRegionS(doc.Database, lstRegion, null);
		split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
		if (point3dCollection.Count == 0)
		{
			editor.WriteMessage("\nYou can not setting column, Please setting it.");
			return;
		}
		Point3d minPointX = GlobalFunction.GetMinPointX(point3dCollection);
		Point3d minPointY = GlobalFunction.GetMinPointY(point3dCollection);
		Point3d maxPointX = GlobalFunction.GetMaxPointX(point3dCollection);
		Point3d maxPointY = GlobalFunction.GetMaxPointY(point3dCollection);
		bool flag = false;
		string text = "chain";
		LinetypeTable linetypeTable = doc.Database.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable;
		if (!linetypeTable.Has(text))
		{
			try
			{
				string pathFolderBinary = GlobalFunction.GetPathFolderBinary();
				string filename = $"{pathFolderBinary}\\ST.lin";
				doc.Database.LoadLineTypeFile(text, filename);
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
			text = "byBlock";
		}
		DeleteGridsIntersect(doc);
		double num3 = maxPointX.X;
		for (int j = 1; j <= value2; j++)
		{
			Point3d stPoint = new Point3d(num3, maxPointY.Y + num2, 0.0);
			Point3d endPoint = new Point3d(num3, minPointY.Y - num2, 0.0);
			Line line2 = new Line(stPoint, endPoint);
			line2.SetDatabaseDefaults(doc.Database);
			line2.Linetype = text;
			GlobalFunction.AppendBlockIntersect(doc.Database, line2);
			num3 -= num2;
		}
		double num4 = maxPointY.Y;
		for (int k = 1; k <= value; k++)
		{
			Point3d stPoint2 = new Point3d(minPointX.X - num2, num4, 0.0);
			Point3d endPoint2 = new Point3d(maxPointX.X + num2, num4, 0.0);
			Line line3 = new Line(stPoint2, endPoint2);
			line3.SetDatabaseDefaults(doc.Database);
			line3.Linetype = text;
			GlobalFunction.AppendBlockIntersect(doc.Database, line3);
			num4 -= num2;
		}
		editor.Regen();
		editor.UpdateScreen();
	}
}
