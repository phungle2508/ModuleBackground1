using System;
using System.Collections.Generic;
using System.IO;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CInsSymNearSquares
{
	public const string S_NameFolderSym = "\\SymBackGround";

	public CSplitRoom split = new CSplitRoom();

	public CSymbol symbol = new CSymbol();

	public bool CheckIsSquare(DBObjectCollection colObjExp)
	{
		if (colObjExp.Count != 4)
		{
			return false;
		}
		Line line = colObjExp[0] as Line;
		if (line == null)
		{
			return false;
		}
		double length = line.Length;
		line = colObjExp[1] as Line;
		if (line == null)
		{
			return false;
		}
		double length2 = line.Length;
		if (Math.Abs(length - length2) >= 0.001)
		{
			return false;
		}
		line = colObjExp[2] as Line;
		if (line == null)
		{
			return false;
		}
		length2 = line.Length;
		if (Math.Abs(length - length2) >= 0.001)
		{
			return false;
		}
		line = colObjExp[3] as Line;
		if (line == null)
		{
			return false;
		}
		length2 = line.Length;
		if (Math.Abs(length - length2) >= 0.001)
		{
			return false;
		}
		return true;
	}

	public bool CheckObjectIsSquare(Region regi)
	{
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		regi.Explode(dBObjectCollection);
		bool result = CheckIsSquare(dBObjectCollection);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		return result;
	}

	public void GetPointOutDistanceBy105(Point3dCollection ptColsCenter, ref Point3dCollection ptsCenterRes, ref Point3dCollection ptsCenterRes105)
	{
		Point3dCollection point3dCollection = new Point3dCollection();
		point3dCollection = ptColsCenter;
		for (int i = 0; i < ptColsCenter.Count; i++)
		{
			Point3d p = ptColsCenter[i];
			bool flag = false;
			for (int j = 0; j < point3dCollection.Count; j++)
			{
				if (i != j)
				{
					Point3d point = point3dCollection[j];
					double num = p.DistanceTo(point);
					if (Math.Abs(num - 105.0) <= 0.1)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				ptsCenterRes.Add(p);
			}
			else if (!ptsCenterRes105.Contains(p))
			{
				ptsCenterRes105.Add(p);
			}
		}
	}

	public void GetPointsCenterSquare(Database db, DBObjectCollection colDbObj, ref Point3dCollection ptColsCenter)
	{
		CSplitRoom cSplitRoom = new CSplitRoom();
		List<Region> lstRegion = new List<Region>();
		cSplitRoom.GetAllRegion(db, colDbObj, ref lstRegion);
		Point3dCollection point3dCollection = new Point3dCollection();
		foreach (Region item in lstRegion)
		{
			if (CheckObjectIsSquare(item))
			{
				point3dCollection.Add(GlobalFunction.PointCenter(item));
			}
		}
		cSplitRoom.DeleteRegionS(db, lstRegion, null);
		Point3dCollection ptsCenterRes = new Point3dCollection();
		GetPointOutDistanceBy105(point3dCollection, ref ptColsCenter, ref ptsCenterRes);
	}

	public bool CheckInterSect(BlockReference blockref, Entity region)
	{
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		blockref.Explode(dBObjectCollection);
		bool result = false;
		foreach (DBObject item in dBObjectCollection)
		{
			Point3dCollection point3dCollection = new Point3dCollection();
			region.IntersectWith(item as Entity, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
			if (point3dCollection.Count != 0)
			{
				result = true;
				break;
			}
		}
		foreach (DBObject item2 in dBObjectCollection)
		{
			item2.Dispose();
		}
		return result;
	}

	public bool CheckInterSect(ObjectId idRef, ObjectIdCollection ids)
	{
		BlockReference blockReference = idRef.GetObject(OpenMode.ForRead) as BlockReference;
		foreach (ObjectId id in ids)
		{
			BlockReference blockReference2 = id.GetObject(OpenMode.ForRead) as BlockReference;
			DBObjectCollection dBObjectCollection = new DBObjectCollection();
			blockReference2.Explode(dBObjectCollection);
			bool flag = false;
			foreach (DBObject item in dBObjectCollection)
			{
				Entity region = item as Entity;
				if (CheckInterSect(blockReference, region))
				{
					flag = true;
					break;
				}
			}
			foreach (DBObject item2 in dBObjectCollection)
			{
				item2.Dispose();
			}
			blockReference2.Dispose();
			blockReference2 = null;
			if (flag)
			{
				blockReference.Dispose();
				blockReference = null;
				return true;
			}
		}
		blockReference.Dispose();
		blockReference = null;
		return false;
	}

	public void GetSquaresDistanceBy105(List<CSquares> lstSquares, ref List<CSquares> lstSquaresNo105, ref List<CSquares> lstSquares105)
	{
		List<CSquares> list = new List<CSquares>();
		list.AddRange(lstSquares);
		for (int i = 0; i < lstSquares.Count; i++)
		{
			CSquares cSquares = lstSquares[i];
			Point3d ptCenter = cSquares.m_ptCenter;
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (i != j)
				{
					CSquares cSquares2 = lstSquares[j];
					Point3d ptCenter2 = cSquares2.m_ptCenter;
					double num = ptCenter.DistanceTo(ptCenter2);
					if (Math.Abs(num - 105.0) <= 0.1)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				if (!lstSquaresNo105.Contains(cSquares))
				{
					lstSquaresNo105.Add(cSquares);
				}
			}
			else if (!lstSquares105.Contains(cSquares))
			{
				lstSquares105.Add(cSquares);
			}
		}
	}

	public void GetGroupSquaresBy105(List<CSquares> lstSquares105, ref List<List<CSquares>> lstGroup)
	{
		for (int i = 0; i < lstSquares105.Count; i++)
		{
			CSquares cSquares = lstSquares105[i];
			bool flag = false;
			for (int j = 0; j < lstGroup.Count; j++)
			{
				List<CSquares> list = lstGroup[j];
				for (int k = 0; k < list.Count; k++)
				{
					CSquares cSquares2 = list[k];
					if (split.Check2Region(cSquares.m_Region, cSquares2.m_Region))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					list.Add(cSquares);
					lstGroup[j] = list;
					break;
				}
			}
			if (!flag)
			{
				List<CSquares> list2 = new List<CSquares>();
				list2.Add(cSquares);
				lstGroup.Add(list2);
			}
		}
	}

	public bool CheckSamePostion(List<CSquares> lstSquares105, int nY)
	{
		for (int i = 0; i < lstSquares105.Count; i++)
		{
			CSquares cSquares = lstSquares105[i];
			for (int j = i + 1; j < lstSquares105.Count; j++)
			{
				CSquares cSquares2 = lstSquares105[j];
				if (nY == 1)
				{
					double num = Math.Abs(cSquares.m_ptCenter.Y - cSquares2.m_ptCenter.Y);
					if (num > 0.1)
					{
						return false;
					}
				}
				else
				{
					double num2 = Math.Abs(cSquares.m_ptCenter.X - cSquares2.m_ptCenter.X);
					if (num2 > 0.1)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public void DeleteSymGroup105(Database db, List<CSquares> lstSquares105, ObjectIdCollection idsSym)
	{
		double num = 0.0;
		for (int i = 0; i < idsSym.Count; i++)
		{
			BlockReference blockReference = idsSym[i].GetObject(OpenMode.ForRead) as BlockReference;
			foreach (CSquares item in lstSquares105)
			{
				double num2 = blockReference.Position.DistanceTo(item.m_ptCenter);
				if (num2 > num)
				{
					num = num2;
				}
			}
			blockReference.Dispose();
			blockReference = null;
		}
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		for (int j = 0; j < idsSym.Count; j++)
		{
			bool flag = false;
			ObjectId value = idsSym[j];
			BlockReference blockReference2 = value.GetObject(OpenMode.ForRead) as BlockReference;
			foreach (CSquares item2 in lstSquares105)
			{
				double num3 = blockReference2.Position.DistanceTo(item2.m_ptCenter);
				if (Math.Abs(num3 - num) <= 0.1)
				{
					flag = true;
					break;
				}
			}
			blockReference2.Dispose();
			blockReference2 = null;
			if (!flag)
			{
				objectIdCollection.Add(value);
			}
		}
		GlobalFunction.DeleteEntityS(db, objectIdCollection);
	}

	public void InsertSymGroup105(Database db, List<Region> lstRegionNotMax, Region reBound, double dLeftRight, double dTopBottom, string sPathFileSym, List<CSquares> lstSquares, List<List<CSquares>> lstGroup)
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		foreach (List<CSquares> item in lstGroup)
		{
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			for (int i = 0; i < item.Count; i++)
			{
				Point3d pt = default(Point3d);
				CSquares cSquares = item[i];
				double num = dLeftRight;
				int num2 = 1;
				if (dLeftRight < 0.0)
				{
					num2 = 2;
				}
				while (true)
				{
					_ = cSquares.m_ptCenter;
					int num3 = num2;
					if (!Insert1SquaseCmd(mdiActiveDocument, cSquares, lstRegionNotMax, reBound, Math.Abs(num), num3, sPathFileSym, ref pt))
					{
						num3 = ((num3 != 1) ? 1 : 2);
						if (!Insert1SquaseCmd(mdiActiveDocument, cSquares, lstRegionNotMax, reBound, Math.Abs(num), num3, sPathFileSym, ref pt))
						{
							if (num == 80.0)
							{
								break;
							}
							num = 80.0;
							continue;
						}
					}
					string sHandleRes = "";
					if (!symbol.InsertBlockFromFile(db, sPathFileSym, pt, ref sHandleRes))
					{
						break;
					}
					try
					{
						ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes, 16)), 0);
						if (objectId.IsNull)
						{
							break;
						}
						BlockReference blockReference = objectId.GetObject(OpenMode.ForRead) as BlockReference;
						bool flag = false;
						foreach (CSquares lstSquare in lstSquares)
						{
							if (!(cSquares.m_ptCenter.DistanceTo(lstSquare.m_ptCenter) <= 0.1) && CheckInterSect(blockReference, lstSquare.m_Region))
							{
								flag = true;
								break;
							}
						}
						blockReference.Dispose();
						blockReference = null;
						if (flag)
						{
							DeleteObject(db, sHandleRes);
							if (num != 80.0)
							{
								num = 80.0;
								continue;
							}
						}
						else if (CheckInterSect(objectId, objectIdCollection))
						{
							DeleteObject(db, sHandleRes);
							if (num != 80.0)
							{
								num = 80.0;
								continue;
							}
						}
						else if (!objectIdCollection.Contains(objectId))
						{
							objectIdCollection.Add(objectId);
						}
					}
					catch (Exception)
					{
					}
					break;
				}
			}
			if (CheckSamePostion(item, 1))
			{
				DeleteSymGroup105(db, item, objectIdCollection);
			}
			objectIdCollection.Clear();
			for (int j = 0; j < item.Count; j++)
			{
				Point3d pt2 = default(Point3d);
				CSquares cSquares2 = item[j];
				double num4 = dTopBottom;
				int num5 = 3;
				if (num4 < 0.0)
				{
					num5 = 4;
				}
				while (true)
				{
					_ = cSquares2.m_ptCenter;
					int num6 = num5;
					if (!Insert1SquaseCmd(mdiActiveDocument, cSquares2, lstRegionNotMax, reBound, Math.Abs(num4), num6, sPathFileSym, ref pt2))
					{
						num6 = ((num6 != 3) ? 3 : 4);
						if (!Insert1SquaseCmd(mdiActiveDocument, cSquares2, lstRegionNotMax, reBound, Math.Abs(num4), num6, sPathFileSym, ref pt2))
						{
							if (num4 == 80.0)
							{
								break;
							}
							num4 = 80.0;
							continue;
						}
					}
					string sHandleRes2 = "";
					if (!symbol.InsertBlockFromFile(db, sPathFileSym, pt2, ref sHandleRes2))
					{
						break;
					}
					try
					{
						ObjectId objectId2 = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes2, 16)), 0);
						if (objectId2.IsNull)
						{
							break;
						}
						BlockReference blockReference2 = objectId2.GetObject(OpenMode.ForRead) as BlockReference;
						bool flag2 = false;
						foreach (CSquares lstSquare2 in lstSquares)
						{
							if (!(cSquares2.m_ptCenter.DistanceTo(lstSquare2.m_ptCenter) <= 0.1) && CheckInterSect(blockReference2, lstSquare2.m_Region))
							{
								flag2 = true;
								break;
							}
						}
						blockReference2.Dispose();
						blockReference2 = null;
						if (flag2)
						{
							DeleteObject(db, sHandleRes2);
							if (num4 != 80.0)
							{
								num4 = 80.0;
								continue;
							}
						}
						else if (CheckInterSect(objectId2, objectIdCollection))
						{
							DeleteObject(db, sHandleRes2);
							if (num4 != 80.0)
							{
								num4 = 80.0;
								continue;
							}
						}
						else if (!objectIdCollection.Contains(objectId2))
						{
							objectIdCollection.Add(objectId2);
						}
					}
					catch (Exception)
					{
					}
					break;
				}
			}
			if (CheckSamePostion(item, 0))
			{
				DeleteSymGroup105(db, item, objectIdCollection);
			}
		}
	}

	public void InsertSymGroup105_Bk(Database db, List<Region> lstRegionNotMax, Region reBound, double dLeftRight, double dTopBottom, string sPathFileSym, List<CSquares> lstSquares, List<List<CSquares>> lstGroup)
	{
		foreach (List<CSquares> item in lstGroup)
		{
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			for (int i = 0; i < item.Count; i++)
			{
				Point3d point3d = default(Point3d);
				CSquares cSquares = item[i];
				double num = dLeftRight;
				while (true)
				{
					Point3d ptCenter = cSquares.m_ptCenter;
					Point3d point3d2 = new Point3d(ptCenter.X - num, ptCenter.Y, ptCenter.Z);
					if (split.CheckPointInsiheRegion(reBound, point3d2) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d2))
					{
						point3d = point3d2;
					}
					else
					{
						Point3d point3d3 = new Point3d(ptCenter.X + num, ptCenter.Y, ptCenter.Z);
						if (!split.CheckPointInsiheRegion(reBound, point3d3) || split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d3))
						{
							break;
						}
						point3d = point3d3;
					}
					string sHandleRes = "";
					if (!symbol.InsertBlockFromFile(db, sPathFileSym, point3d, ref sHandleRes))
					{
						break;
					}
					try
					{
						ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes, 16)), 0);
						if (objectId.IsNull)
						{
							break;
						}
						BlockReference blockReference = objectId.GetObject(OpenMode.ForRead) as BlockReference;
						bool flag = false;
						foreach (CSquares lstSquare in lstSquares)
						{
							if (!(cSquares.m_ptCenter.DistanceTo(lstSquare.m_ptCenter) <= 0.1) && CheckInterSect(blockReference, lstSquare.m_Region))
							{
								flag = true;
								break;
							}
						}
						blockReference.Dispose();
						blockReference = null;
						if (flag)
						{
							DeleteObject(db, sHandleRes);
							if (num != 80.0)
							{
								num = 80.0;
								continue;
							}
						}
						else if (CheckInterSect(objectId, objectIdCollection))
						{
							DeleteObject(db, sHandleRes);
							if (num != 80.0)
							{
								num = 80.0;
								continue;
							}
						}
						else if (!objectIdCollection.Contains(objectId))
						{
							objectIdCollection.Add(objectId);
						}
					}
					catch (Exception)
					{
					}
					break;
				}
			}
			if (CheckSamePostion(item, 1))
			{
				DeleteSymGroup105(db, item, objectIdCollection);
			}
			objectIdCollection.Clear();
			for (int j = 0; j < item.Count; j++)
			{
				Point3d ptIns = default(Point3d);
				CSquares cSquares2 = item[j];
				double num2 = dTopBottom;
				while (true)
				{
					Point3d ptCenter2 = cSquares2.m_ptCenter;
					Point3d point3d4 = new Point3d(ptCenter2.X, ptCenter2.Y + num2, ptCenter2.Z);
					if (split.CheckPointInsiheRegion(reBound, point3d4) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d4))
					{
						ptIns = point3d4;
					}
					else
					{
						Point3d point3d5 = new Point3d(ptCenter2.X, ptCenter2.Y - num2, ptCenter2.Z);
						if (split.CheckPointInsiheRegion(reBound, point3d5) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d5))
						{
							ptIns = point3d5;
						}
					}
					string sHandleRes2 = "";
					if (!symbol.InsertBlockFromFile(db, sPathFileSym, ptIns, ref sHandleRes2))
					{
						break;
					}
					try
					{
						ObjectId objectId2 = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes2, 16)), 0);
						if (objectId2.IsNull)
						{
							break;
						}
						BlockReference blockReference2 = objectId2.GetObject(OpenMode.ForRead) as BlockReference;
						bool flag2 = false;
						foreach (CSquares lstSquare2 in lstSquares)
						{
							if (!(cSquares2.m_ptCenter.DistanceTo(lstSquare2.m_ptCenter) <= 0.1) && CheckInterSect(blockReference2, lstSquare2.m_Region))
							{
								flag2 = true;
								break;
							}
						}
						blockReference2.Dispose();
						blockReference2 = null;
						if (flag2)
						{
							DeleteObject(db, sHandleRes2);
							if (num2 != 80.0)
							{
								num2 = 80.0;
								continue;
							}
						}
						else if (CheckInterSect(objectId2, objectIdCollection))
						{
							DeleteObject(db, sHandleRes2);
							if (num2 != 80.0)
							{
								num2 = 80.0;
								continue;
							}
						}
						else if (!objectIdCollection.Contains(objectId2))
						{
							objectIdCollection.Add(objectId2);
						}
					}
					catch (Exception)
					{
					}
					break;
				}
			}
			if (CheckSamePostion(item, 0))
			{
				DeleteSymGroup105(db, item, objectIdCollection);
			}
		}
	}

	public void InsertSymGroupNot105_Bk1(Database db, List<Region> lstRegionNotMax, Region reBound, double dLeftRight, double dTopBottom, string sPathFileSym, List<CSquares> lstSquares, List<CSquares> lstSquaresNo105)
	{
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		for (int i = 0; i < lstSquaresNo105.Count; i++)
		{
			Point3d ptIns = default(Point3d);
			CSquares cSquares = lstSquaresNo105[i];
			double num = dLeftRight;
			while (true)
			{
				Point3d ptCenter = cSquares.m_ptCenter;
				Point3d point3d = new Point3d(ptCenter.X - num, ptCenter.Y, ptCenter.Z);
				if (split.CheckPointInsiheRegion(reBound, point3d) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d))
				{
					ptIns = point3d;
				}
				else
				{
					Point3d point3d2 = new Point3d(ptCenter.X + num, ptCenter.Y, ptCenter.Z);
					if (split.CheckPointInsiheRegion(reBound, point3d2) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d2))
					{
						ptIns = point3d2;
					}
				}
				string sHandleRes = "";
				if (!symbol.InsertBlockFromFile(db, sPathFileSym, ptIns, ref sHandleRes))
				{
					break;
				}
				try
				{
					ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes, 16)), 0);
					if (objectId.IsNull)
					{
						break;
					}
					BlockReference blockReference = objectId.GetObject(OpenMode.ForRead) as BlockReference;
					bool flag = false;
					foreach (CSquares lstSquare in lstSquares)
					{
						if (!(cSquares.m_ptCenter.DistanceTo(lstSquare.m_ptCenter) <= 0.1) && CheckInterSect(blockReference, lstSquare.m_Region))
						{
							flag = true;
							break;
						}
					}
					blockReference.Dispose();
					blockReference = null;
					if (flag)
					{
						DeleteObject(db, sHandleRes);
						if (num != 80.0)
						{
							num = 80.0;
							continue;
						}
					}
					else if (CheckInterSect(objectId, objectIdCollection))
					{
						DeleteObject(db, sHandleRes);
						if (num != 80.0)
						{
							num = 80.0;
							continue;
						}
					}
					else if (!objectIdCollection.Contains(objectId))
					{
						objectIdCollection.Add(objectId);
					}
				}
				catch (Exception)
				{
				}
				break;
			}
		}
		objectIdCollection.Clear();
		for (int j = 0; j < lstSquaresNo105.Count; j++)
		{
			Point3d ptIns2 = default(Point3d);
			CSquares cSquares2 = lstSquaresNo105[j];
			double num2 = dTopBottom;
			while (true)
			{
				Point3d ptCenter2 = cSquares2.m_ptCenter;
				Point3d point3d3 = new Point3d(ptCenter2.X, ptCenter2.Y + num2, ptCenter2.Z);
				if (split.CheckPointInsiheRegion(reBound, point3d3) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d3))
				{
					ptIns2 = point3d3;
				}
				else
				{
					Point3d point3d4 = new Point3d(ptCenter2.X, ptCenter2.Y - num2, ptCenter2.Z);
					if (split.CheckPointInsiheRegion(reBound, point3d4) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d4))
					{
						ptIns2 = point3d4;
					}
				}
				string sHandleRes2 = "";
				if (!symbol.InsertBlockFromFile(db, sPathFileSym, ptIns2, ref sHandleRes2))
				{
					break;
				}
				try
				{
					ObjectId objectId2 = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes2, 16)), 0);
					if (objectId2.IsNull)
					{
						break;
					}
					BlockReference blockReference2 = objectId2.GetObject(OpenMode.ForRead) as BlockReference;
					bool flag2 = false;
					foreach (CSquares lstSquare2 in lstSquares)
					{
						if (!(cSquares2.m_ptCenter.DistanceTo(lstSquare2.m_ptCenter) <= 0.1) && CheckInterSect(blockReference2, lstSquare2.m_Region))
						{
							flag2 = true;
							break;
						}
					}
					blockReference2.Dispose();
					blockReference2 = null;
					if (flag2)
					{
						DeleteObject(db, sHandleRes2);
						if (num2 != 80.0)
						{
							num2 = 80.0;
							continue;
						}
					}
					else if (CheckInterSect(objectId2, objectIdCollection))
					{
						DeleteObject(db, sHandleRes2);
						if (num2 != 80.0)
						{
							num2 = 80.0;
							continue;
						}
					}
					else if (!objectIdCollection.Contains(objectId2))
					{
						objectIdCollection.Add(objectId2);
					}
				}
				catch (Exception)
				{
				}
				break;
			}
		}
	}

	public void InsertSymGroupNot105_Bk(Database db, List<Region> lstRegionNotMax, Region reBound, double dLeftRight, double dTopBottom, string sPathFileSym, List<CSquares> lstSquares, List<CSquares> lstSquaresNo105)
	{
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		for (int i = 0; i < lstSquaresNo105.Count; i++)
		{
			Point3d ptIns = default(Point3d);
			CSquares cSquares = lstSquaresNo105[i];
			double num = dLeftRight;
			while (true)
			{
				Point3d ptCenter = cSquares.m_ptCenter;
				Point3d point3d = new Point3d(ptCenter.X - num, ptCenter.Y, ptCenter.Z);
				if (split.CheckPointInsiheRegion(reBound, point3d) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d))
				{
					ptIns = point3d;
				}
				else
				{
					Point3d point3d2 = new Point3d(ptCenter.X + num, ptCenter.Y, ptCenter.Z);
					if (split.CheckPointInsiheRegion(reBound, point3d2) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d2))
					{
						ptIns = point3d2;
					}
				}
				string sHandleRes = "";
				if (!symbol.InsertBlockFromFile(db, sPathFileSym, ptIns, ref sHandleRes))
				{
					break;
				}
				try
				{
					ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes, 16)), 0);
					if (objectId.IsNull)
					{
						break;
					}
					BlockReference blockReference = objectId.GetObject(OpenMode.ForRead) as BlockReference;
					bool flag = false;
					foreach (CSquares lstSquare in lstSquares)
					{
						if (!(cSquares.m_ptCenter.DistanceTo(lstSquare.m_ptCenter) <= 0.1) && CheckInterSect(blockReference, lstSquare.m_Region))
						{
							flag = true;
							break;
						}
					}
					blockReference.Dispose();
					blockReference = null;
					if (flag)
					{
						DeleteObject(db, sHandleRes);
						if (num != 80.0)
						{
							num = 80.0;
							continue;
						}
					}
					else if (CheckInterSect(objectId, objectIdCollection))
					{
						DeleteObject(db, sHandleRes);
						if (num != 80.0)
						{
							num = 80.0;
							continue;
						}
					}
					else if (!objectIdCollection.Contains(objectId))
					{
						objectIdCollection.Add(objectId);
					}
				}
				catch (Exception)
				{
				}
				break;
			}
		}
		objectIdCollection.Clear();
		for (int j = 0; j < lstSquaresNo105.Count; j++)
		{
			Point3d ptIns2 = default(Point3d);
			CSquares cSquares2 = lstSquaresNo105[j];
			double num2 = dTopBottom;
			while (true)
			{
				Point3d ptCenter2 = cSquares2.m_ptCenter;
				Point3d point3d3 = new Point3d(ptCenter2.X, ptCenter2.Y + num2, ptCenter2.Z);
				if (split.CheckPointInsiheRegion(reBound, point3d3) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d3))
				{
					ptIns2 = point3d3;
				}
				else
				{
					Point3d point3d4 = new Point3d(ptCenter2.X, ptCenter2.Y - num2, ptCenter2.Z);
					if (split.CheckPointInsiheRegion(reBound, point3d4) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d4))
					{
						ptIns2 = point3d4;
					}
				}
				string sHandleRes2 = "";
				if (!symbol.InsertBlockFromFile(db, sPathFileSym, ptIns2, ref sHandleRes2))
				{
					break;
				}
				try
				{
					ObjectId objectId2 = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes2, 16)), 0);
					if (objectId2.IsNull)
					{
						break;
					}
					BlockReference blockReference2 = objectId2.GetObject(OpenMode.ForRead) as BlockReference;
					bool flag2 = false;
					foreach (CSquares lstSquare2 in lstSquares)
					{
						if (!(cSquares2.m_ptCenter.DistanceTo(lstSquare2.m_ptCenter) <= 0.1) && CheckInterSect(blockReference2, lstSquare2.m_Region))
						{
							flag2 = true;
							break;
						}
					}
					blockReference2.Dispose();
					blockReference2 = null;
					if (flag2)
					{
						DeleteObject(db, sHandleRes2);
						if (num2 != 80.0)
						{
							num2 = 80.0;
							continue;
						}
					}
					else if (CheckInterSect(objectId2, objectIdCollection))
					{
						DeleteObject(db, sHandleRes2);
						if (num2 != 80.0)
						{
							num2 = 80.0;
							continue;
						}
					}
					else if (!objectIdCollection.Contains(objectId2))
					{
						objectIdCollection.Add(objectId2);
					}
				}
				catch (Exception)
				{
				}
				break;
			}
		}
	}

	public void InsertWithCenterSquare(Document doc, DBObjectCollection colDbObj, double dLeftRight, double dTopBottom, string sPathFileSym)
	{
		Database database = doc.Database;
		CSplitRoom cSplitRoom = new CSplitRoom();
		List<Region> lstRegionNotMax = new List<Region>();
		Region reBound = null;
		cSplitRoom.GetListRegion(doc, ref lstRegionNotMax, ref reBound);
		List<Region> lstRegion = new List<Region>();
		cSplitRoom.GetAllRegion(database, colDbObj, ref lstRegion);
		try
		{
			List<CSquares> list = new List<CSquares>();
			Point3dCollection point3dCollection = new Point3dCollection();
			foreach (Region item in lstRegion)
			{
				if (CheckObjectIsSquare(item))
				{
					CSquares cSquares = new CSquares();
					cSquares.m_Region = item;
					cSquares.m_ptCenter = GlobalFunction.PointCenter(item);
					if (cSplitRoom.CheckPointInsiheRegion(reBound, cSquares.m_ptCenter))
					{
						list.Add(cSquares);
						point3dCollection.Add(GlobalFunction.PointCenter(item));
					}
				}
			}
			List<CSquares> lstSquaresNo = new List<CSquares>();
			List<CSquares> lstSquares = new List<CSquares>();
			GetSquaresDistanceBy105(list, ref lstSquaresNo, ref lstSquares);
			List<List<CSquares>> lstGroup = new List<List<CSquares>>();
			GetGroupSquaresBy105(lstSquares, ref lstGroup);
			InsertSymGroup105(database, lstRegionNotMax, reBound, dLeftRight, dTopBottom, sPathFileSym, list, lstGroup);
			InsertSymGroupNot105(database, lstRegionNotMax, reBound, dLeftRight, dTopBottom, sPathFileSym, list, lstSquaresNo);
		}
		catch (Exception)
		{
		}
		cSplitRoom.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
		cSplitRoom.DeleteRegionS(database, lstRegion, null);
	}

	public void DrawPolySelect(Database db, Point3dCollection arrPointcolect, ref string sHandle)
	{
		try
		{
			Polyline polyline = new Polyline();
			polyline.SetDatabaseDefaults(db);
			for (int i = 0; i < arrPointcolect.Count; i++)
			{
				polyline.AddVertexAt(i, new Point2d(arrPointcolect[i].X, arrPointcolect[i].Y), 0.0, 0.0, 0.0);
			}
			polyline.Closed = true;
			sHandle = GlobalFunction.AppendEntityWall(db, polyline);
		}
		catch (Exception)
		{
		}
	}

	public void DeleteObject(Database db, string sHandle)
	{
		try
		{
			ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandle, 16)), 0);
			if (objectId.IsNull)
			{
				return;
			}
			using Entity entity = objectId.GetObject(OpenMode.ForWrite) as Entity;
			entity.Erase();
			entity.Dispose();
		}
		catch (Exception)
		{
		}
	}

	public void GetPointInsertSquare(Document doc, Point3dCollection ptsCenterSquare, double dDistance, ref Point3dCollection ptsInser)
	{
		CSplitRoom cSplitRoom = new CSplitRoom();
		List<Region> lstRegionNotMax = new List<Region>();
		Region reBound = null;
		cSplitRoom.GetListRegion(doc, ref lstRegionNotMax, ref reBound);
		foreach (Point3d item in ptsCenterSquare)
		{
			Point3d point3d2 = new Point3d(item.X - dDistance, item.Y, item.Z);
			if (cSplitRoom.CheckPointInsiheRegion(reBound, point3d2) && !cSplitRoom.CheckPointIsOnRegionAll(lstRegionNotMax, point3d2))
			{
				ptsInser.Add(point3d2);
			}
			else
			{
				Point3d point3d3 = new Point3d(item.X + dDistance, item.Y, item.Z);
				if (cSplitRoom.CheckPointInsiheRegion(reBound, point3d3) && !cSplitRoom.CheckPointIsOnRegionAll(lstRegionNotMax, point3d3))
				{
					ptsInser.Add(point3d3);
				}
			}
			Point3d point3d4 = new Point3d(item.X, item.Y + dDistance, item.Z);
			if (cSplitRoom.CheckPointInsiheRegion(reBound, point3d4) && !cSplitRoom.CheckPointIsOnRegionAll(lstRegionNotMax, point3d4))
			{
				ptsInser.Add(point3d4);
				continue;
			}
			Point3d point3d5 = new Point3d(item.X, item.Y - dDistance, item.Z);
			if (cSplitRoom.CheckPointInsiheRegion(reBound, point3d5) && !cSplitRoom.CheckPointIsOnRegionAll(lstRegionNotMax, point3d5))
			{
				ptsInser.Add(point3d5);
			}
		}
		cSplitRoom.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
	}

	public void VisibaleEntity(ObjectIdCollection idsLine, bool bVisibale = true)
	{
		for (int i = 0; i < idsLine.Count; i++)
		{
			using Entity entity = idsLine[i].GetObject(OpenMode.ForWrite) as Entity;
			entity.Visible = bVisibale;
		}
	}

	public void CreateRegionSquares(Database db, DBObjectCollection dbColsObj, ref List<Region> lstRegion)
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
			string value = GlobalFunction.AppendBlockNearSquare(db, region);
			region = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0).GetObject(OpenMode.ForRead) as Region;
			lstRegion.Add(region);
		}
	}

	public void getAllVector(Document doc, ObjectId idCir, List<Region> lstRegionNotMax, Region reBound, int nDirect)
	{
		Circle circle = idCir.GetObject(OpenMode.ForRead) as Circle;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
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
			if (line == null)
			{
				continue;
			}
			int colorIndex = line.ColorIndex;
			if (colorIndex == GlobalFunction.nColorWall)
			{
				Point3dCollection point3dCollection = new Point3dCollection();
				line.IntersectWith(circle, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
				if (point3dCollection.Count != 0)
				{
					objectIdCollection.Add(value);
				}
			}
			line.Dispose();
			line = null;
		}
		Point3d center = circle.Center;
		double radius = circle.Radius;
		Point3dCollection point3dCollection2 = new Point3dCollection();
		for (int j = 0; j < objectIdCollection.Count; j++)
		{
			Line line2 = objectIdCollection[j].GetObject(OpenMode.ForRead) as Line;
			Point3d point3d = GlobalFunction.Point3dCenter(line2);
			for (int k = j + 1; k < objectIdCollection.Count; k++)
			{
				Line line3 = objectIdCollection[k].GetObject(OpenMode.ForRead) as Line;
				Point3d endPoint = GlobalFunction.Point3dCenter(line3);
				double num = point3d.DistanceTo(line3.GetClosestPointTo(point3d, extend: false)) / 2.0;
				Line line4 = new Line(point3d, endPoint);
				Line line5 = line2;
				if (line2.Length < line3.Length)
				{
					line5 = line3;
				}
				DBObjectCollection offsetCurves = line5.GetOffsetCurves(num);
				Line line6 = offsetCurves[0] as Line;
				Point3dCollection point3dCollection3 = new Point3dCollection();
				line6.IntersectWith(line4, Intersect.OnBothOperands, point3dCollection3, IntPtr.Zero, IntPtr.Zero);
				line4.Dispose();
				line4 = null;
				if (point3dCollection3.Count == 0)
				{
					offsetCurves = line5.GetOffsetCurves(-1.0 * num);
					line6 = offsetCurves[0] as Line;
				}
				double angle = line6.Angle;
				Point3d point3d2 = GlobalFunction.Point3dCenter(line6);
				if (split.CheckPointInsiheRegion(reBound, point3d2) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d2))
				{
					point3dCollection3.Clear();
					line6.IntersectWith(circle, Intersect.OnBothOperands, point3dCollection3, IntPtr.Zero, IntPtr.Zero);
					if (point3dCollection3.Count != 0)
					{
						foreach (Point3d item in point3dCollection3)
						{
							if (point3dCollection2.Contains(item))
							{
								continue;
							}
							point3dCollection2.Add(item);
							if (nDirect == 1)
							{
								if (point3d2.X > center.X)
								{
									continue;
								}
								double x = center.X - Math.Cos(angle) * radius;
								double y = center.Y + Math.Sin(angle) * radius;
								Line ent = new Line(endPoint: new Point3d(x, y, center.Z), stPoint: center);
								GlobalFunction.AppendBlockNearSquare(doc.Database, ent);
							}
							if (nDirect == 2 && !(point3d2.X < center.X))
							{
								double x2 = center.X + Math.Cos(angle) * 200.0;
								double y2 = center.Y + Math.Sin(angle) * 200.0;
								Line ent2 = new Line(endPoint: new Point3d(x2, y2, center.Z), stPoint: center);
								GlobalFunction.AppendBlockNearSquare(doc.Database, ent2);
							}
						}
					}
				}
				line6.Dispose();
				line6 = null;
			}
		}
		circle.Dispose();
		circle = null;
		foreach (Point3d item2 in point3dCollection2)
		{
			Line ent3 = new Line(center, item2);
			GlobalFunction.AppendEntityWall(doc.Database, ent3);
		}
	}

	private void newLine(Line lineOrg, Point3d center, double dRad)
	{
		double length = lineOrg.Length;
		if (!(length <= 0.0) && !(dRad <= 0.0))
		{
			Point3d startPoint = lineOrg.StartPoint;
			Point3d endPoint = lineOrg.EndPoint;
			Vector3d vector3d = ((!(center.DistanceTo(startPoint) > center.DistanceTo(endPoint))) ? (endPoint - startPoint) : (startPoint - endPoint));
			vector3d = vector3d * dRad / length;
			new Line(center, center + vector3d);
		}
	}

	public bool CheckLineHasCenter(Line LineOffset, List<Region> lstRegionNotMax, Region reBound)
	{
		Point3dCollection point3dCollection = new Point3dCollection();
		LineOffset.IntersectWith(reBound, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
		if (point3dCollection.Count != 0)
		{
			return false;
		}
		Point3d startPoint = LineOffset.StartPoint;
		Point3d endPoint = LineOffset.EndPoint;
		Point3d ptSel = GlobalFunction.Point3dCenter(LineOffset);
		if (!split.CheckPointInsiheRegion(reBound, ptSel) || !split.CheckPointInsiheRegion(reBound, startPoint) || !split.CheckPointInsiheRegion(reBound, endPoint))
		{
			return false;
		}
		foreach (Region item in lstRegionNotMax)
		{
			point3dCollection.Clear();
			LineOffset.IntersectWith(item, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
			if (point3dCollection.Count != 0)
			{
				return false;
			}
			if (split.CheckPointInsiheRegion(item, ptSel) || split.CheckPointInsiheRegion(item, startPoint) || split.CheckPointInsiheRegion(item, endPoint))
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckLineOffsetHasCenterWall(Line LineOffset, List<Region> lstRegionInterSect)
	{
		if (lstRegionInterSect.Count == 0)
		{
			return false;
		}
		Point3d startPoint = LineOffset.StartPoint;
		Point3d endPoint = LineOffset.EndPoint;
		Point3d ptSel = GlobalFunction.Point3dCenter(LineOffset);
		foreach (Region item in lstRegionInterSect)
		{
			Point3dCollection point3dCollection = new Point3dCollection();
			LineOffset.IntersectWith(item, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
			if (point3dCollection.Count != 0)
			{
				return false;
			}
			if (split.CheckPointInsiheRegion(item, ptSel) || split.CheckPointInsiheRegion(item, startPoint) || split.CheckPointInsiheRegion(item, endPoint))
			{
				return false;
			}
		}
		return true;
	}

	public void GetRegionHasLine(Line LineOffset, List<Region> lstRegionInterSect, ref List<Region> lstRegionHasLine)
	{
		List<Region> list = new List<Region>();
		Point3d startPoint = LineOffset.StartPoint;
		Point3d endPoint = LineOffset.EndPoint;
		foreach (Region item in lstRegionInterSect)
		{
			if ((split.CheckPointInsiheRegion(item, startPoint) || split.CheckPointInsiheRegion(item, endPoint)) && !list.Contains(item))
			{
				if (list.Count == 0)
				{
					list.Add(item);
				}
				else if (item.Area > list[0].Area)
				{
					list[0] = item;
				}
			}
		}
		if (list.Count != 0 && !lstRegionHasLine.Contains(list[0]))
		{
			lstRegionHasLine.Add(list[0]);
		}
	}

	public void GetAllRegionInterSectCir(Circle cir, List<Region> lstRegionNotMax, ref List<Region> lstRegionInterSect)
	{
		foreach (Region item in lstRegionNotMax)
		{
			Point3dCollection point3dCollection = new Point3dCollection();
			cir.IntersectWith(item, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
			if (point3dCollection.Count != 0)
			{
				lstRegionInterSect.Add(item);
			}
		}
	}

	private bool CheckGroupLineWall(Line li1, Line li2, ObjectIdCollection idsLine)
	{
		Point3d stPoint = GlobalFunction.Point3dCenter(li1);
		Point3d endPoint = GlobalFunction.Point3dCenter(li2);
		Line line = new Line(stPoint, endPoint);
		for (int i = 0; i < idsLine.Count; i++)
		{
			ObjectId objectId = idsLine[i];
			if (objectId == li1.Id || objectId == li2.Id)
			{
				continue;
			}
			Line line2 = objectId.GetObject(OpenMode.ForRead) as Line;
			if (!(line2 == null))
			{
				Point3dCollection point3dCollection = new Point3dCollection();
				line.IntersectWith(line2, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
				line2.Dispose();
				line2 = null;
				if (point3dCollection.Count != 0)
				{
					line.Dispose();
					line = null;
					return false;
				}
			}
		}
		line.Dispose();
		line = null;
		return true;
	}

	public bool GetWall(Line li1, ref ObjectIdCollection idsLine, ref CWall wall)
	{
		bool result = false;
		for (int i = 0; i < idsLine.Count; i++)
		{
			ObjectId objectId = idsLine[i];
			if (!(objectId == li1.Id))
			{
				Line line = objectId.GetObject(OpenMode.ForRead) as Line;
				if (!(line == null) && CheckGroupLineWall(li1, line, idsLine))
				{
					wall.m_idLine1 = li1.Id;
					wall.m_idLine2 = line.Id;
					idsLine.RemoveAt(i);
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public bool Check2LineSameWall2(DBObjectCollection dbColsObj)
	{
		DBObjectCollection dBObjectCollection = Region.CreateFromCurves(dbColsObj);
		for (int i = 0; i < dbColsObj.Count; i++)
		{
			DBObject dBObject = dbColsObj[i];
			dBObject.Dispose();
			dBObject = null;
		}
		if (dBObjectCollection.Count == 0)
		{
			return false;
		}
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		int num = 0;
		foreach (DBObject item in dBObjectCollection)
		{
			if (!(item is Region))
			{
				item.Dispose();
				continue;
			}
			Region region = item as Region;
			if (region.Area != 0.0)
			{
				num++;
			}
			ObjectId value = GlobalFunction.AddWallToModelSpace(mdiActiveDocument.Database, region);
			objectIdCollection.Add(value);
		}
		split.DeleteEntityS(mdiActiveDocument.Database, objectIdCollection);
		if (num != 1)
		{
			return false;
		}
		return true;
	}

	public bool Check2LineSameWall(Line l1, Line l2)
	{
		Point3d point3d = GlobalFunction.Point3dCenter(l1);
		double length = l1.Length;
		double length2 = l2.Length;
		Line line = l1.Clone() as Line;
		Line line2 = l2.Clone() as Line;
		if (length2 < length)
		{
			line.Dispose();
			line = null;
			line = l2.Clone() as Line;
			line2.Dispose();
			line2 = null;
			line2 = l1.Clone() as Line;
			point3d = GlobalFunction.Point3dCenter(l2);
		}
		Point3d startPoint = line.StartPoint;
		Point3d endPoint = line.EndPoint;
		Point3d startPoint2 = line2.StartPoint;
		Point3d endPoint2 = line2.EndPoint;
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		dBObjectCollection.Add(line);
		dBObjectCollection.Add(line2);
		Point3d closestPointTo = line2.GetClosestPointTo(startPoint, extend: true);
		dBObjectCollection.Add(new Line(startPoint, closestPointTo));
		Point3d closestPointTo2 = line2.GetClosestPointTo(endPoint, extend: true);
		dBObjectCollection.Add(new Line(endPoint, closestPointTo2));
		if (Check2LineSameWall2(dBObjectCollection))
		{
			return true;
		}
		dBObjectCollection.Clear();
		dBObjectCollection.Add(new Line(startPoint, endPoint));
		line2 = new Line(startPoint2, endPoint2);
		dBObjectCollection.Add(line2);
		dBObjectCollection.Add(new Line(startPoint, closestPointTo));
		Point3d closestPointTo3 = line2.GetClosestPointTo(point3d, extend: true);
		dBObjectCollection.Add(new Line(point3d, closestPointTo3));
		if (Check2LineSameWall2(dBObjectCollection))
		{
			return true;
		}
		dBObjectCollection.Clear();
		dBObjectCollection.Add(new Line(startPoint, endPoint));
		dBObjectCollection.Add(new Line(startPoint2, endPoint2));
		dBObjectCollection.Add(new Line(point3d, closestPointTo3));
		dBObjectCollection.Add(new Line(endPoint, closestPointTo2));
		return Check2LineSameWall2(dBObjectCollection);
	}

	public Line GetClosestParallel(Line baseLine, List<Line> lines, ref double distance)
	{
		Point3d startPoint = baseLine.StartPoint;
		Vector3d vector3d = baseLine.EndPoint - startPoint;
		Line result = null;
		distance = double.MaxValue;
		foreach (Line line in lines)
		{
			if (line != baseLine && vector3d.IsParallelTo(line.EndPoint - line.StartPoint) && Check2LineSameWall(baseLine, line))
			{
				double num = line.GetClosestPointTo(startPoint, extend: true).DistanceTo(startPoint);
				if (num < distance)
				{
					distance = num;
					result = line;
				}
			}
		}
		return result;
	}

	public bool Insert1SquaseCmd(Document doc, CSquares quaSel, List<Region> lstRegionNotMax, Region reBound, double dDistance, int nDirect, string sPathFileSym, ref Point3d pt)
	{
		Point3d maxPoint = quaSel.m_Region.GeometricExtents.MaxPoint;
		Point3d minPoint = quaSel.m_Region.GeometricExtents.MinPoint;
		Point3d ptCenter = quaSel.m_ptCenter;
		double num = dDistance;
		Circle circle = new Circle();
		circle.SetDatabaseDefaults(doc.Database);
		circle.Center = ptCenter;
		if (num < GlobalFunction.g_dRadiusSquare)
		{
			num = GlobalFunction.g_dRadiusSquare;
		}
		circle.Radius = num;
		new List<Region>();
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			circle.Dispose();
			circle = null;
			return false;
		}
		List<CWall> list = new List<CWall>();
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId objectId = objectIds[i];
			Line line = (Line)objectId.GetObject(OpenMode.ForRead);
			if (line == null)
			{
				continue;
			}
			int colorIndex = line.ColorIndex;
			if (colorIndex == GlobalFunction.nColorWall)
			{
				Point3dCollection point3dCollection = new Point3dCollection();
				line.IntersectWith(circle, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
				if (point3dCollection.Count != 0)
				{
					CWall cWall = new CWall();
					cWall.m_idLine1 = objectId;
					cWall.m_bCheck = false;
					cWall.m_dLength1 = line.Length;
					cWall.m_dLength2 = 0.0;
					list.Add(cWall);
					objectIdCollection.Add(objectId);
				}
			}
			line.Dispose();
			line = null;
		}
		circle.Dispose();
		List<CWall> list2 = new List<CWall>();
		for (int j = 0; j < list.Count; j++)
		{
			CWall cWall2 = list[j];
			if (cWall2.m_bCheck)
			{
				continue;
			}
			ObjectId idLine = cWall2.m_idLine1;
			Line line2 = idLine.GetObject(OpenMode.ForRead) as Line;
			List<Line> list3 = new List<Line>();
			for (int k = 0; k < objectIdCollection.Count; k++)
			{
				ObjectId objectId2 = objectIdCollection[k];
				if (!(objectId2 == idLine))
				{
					Line item = objectId2.GetObject(OpenMode.ForRead) as Line;
					list3.Add(item);
				}
			}
			double distance = 0.0;
			Line closestParallel = GetClosestParallel(line2, list3, ref distance);
			if (closestParallel != null)
			{
				list[j].m_idLine2 = closestParallel.Id;
				list[j].m_bCheck = true;
				CWall cWall3 = new CWall();
				cWall3.m_idLine1 = list[j].m_idLine1;
				cWall3.m_dLength1 = list[j].m_dLength1;
				cWall3.m_idLine2 = closestParallel.Id;
				cWall3.m_dLength2 = closestParallel.Length;
				bool flag = false;
				for (int l = 0; l < list2.Count; l++)
				{
					CWall cWall4 = list2[l];
					if ((cWall4.m_idLine1 == cWall3.m_idLine1 || cWall4.m_idLine1 == cWall3.m_idLine2) && (cWall4.m_idLine2 == cWall3.m_idLine1 || cWall4.m_idLine2 == cWall3.m_idLine2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list2.Add(cWall3);
				}
			}
			for (int m = 0; m < list3.Count; m++)
			{
				Line line3 = list3[m];
				line3.Dispose();
				line3 = null;
			}
			line2.Dispose();
			line2 = null;
		}
		Point3dCollection point3dCollection2 = new Point3dCollection();
		for (int n = 0; n < list2.Count; n++)
		{
			CWall cWall5 = list2[n];
			ObjectId objectId3 = cWall5.m_idLine1;
			if (cWall5.m_dLength2 < cWall5.m_dLength1)
			{
				objectId3 = cWall5.m_idLine2;
			}
			Line line4 = objectId3.GetObject(OpenMode.ForRead) as Line;
			new Point3dCollection();
			Point3d closestPointTo = line4.GetClosestPointTo(quaSel.m_ptCenter, extend: true);
			Line entityPointer = new Line(quaSel.m_ptCenter, closestPointTo);
			Point3dCollection point3dCollection3 = new Point3dCollection();
			line4.IntersectWith(entityPointer, Intersect.OnBothOperands, point3dCollection3, IntPtr.Zero, IntPtr.Zero);
			List<Line> list4 = new List<Line>();
			if (point3dCollection3.Count != 0)
			{
				list4.Add(new Line(closestPointTo, line4.StartPoint));
				list4.Add(new Line(closestPointTo, line4.EndPoint));
			}
			else
			{
				list4.Add(line4);
			}
			for (int num2 = 0; num2 < list4.Count; num2++)
			{
				Line line5 = list4[num2];
				Point3d startPoint = line5.StartPoint;
				Point3d endPoint = line5.EndPoint;
				double length = startPoint.GetVectorTo(endPoint).Length;
				if (!(length <= 0.0) && !(dDistance <= 0.0))
				{
					Point3d point3d = startPoint;
					Point3d point3d2 = endPoint;
					Vector3d vector3d = ((!(ptCenter.DistanceTo(point3d) > ptCenter.DistanceTo(point3d2))) ? (point3d2 - point3d) : (point3d - point3d2));
					vector3d = vector3d * dDistance / length;
					Line line6 = new Line(ptCenter, ptCenter + vector3d);
					Point3d endPoint2 = line6.EndPoint;
					bool flag2 = false;
					switch (nDirect)
					{
					case 1:
						if (endPoint2.X < minPoint.X)
						{
							flag2 = true;
						}
						break;
					case 2:
						if (endPoint2.X > maxPoint.X)
						{
							flag2 = true;
						}
						break;
					case 3:
						if (endPoint2.Y > maxPoint.Y)
						{
							flag2 = true;
						}
						break;
					case 4:
						if (endPoint2.Y < minPoint.Y)
						{
							flag2 = true;
						}
						break;
					}
					line6.Dispose();
					line6 = null;
					if (flag2 && split.CheckPointInsiheRegion(reBound, endPoint2) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, endPoint2) && !point3dCollection2.Contains(endPoint2))
					{
						point3dCollection2.Add(endPoint2);
					}
				}
				line5.Dispose();
				line5 = null;
			}
			if (line4 != null)
			{
				line4.Dispose();
				line4 = null;
			}
		}
		if (point3dCollection2.Count == 0)
		{
			return false;
		}
		pt = GlobalFunction.GetMinPointX(point3dCollection2);
		switch (nDirect)
		{
		case 2:
			pt = GlobalFunction.GetMaxPointX(point3dCollection2);
			break;
		case 3:
			pt = GlobalFunction.GetMaxPointY(point3dCollection2);
			break;
		case 4:
			pt = GlobalFunction.GetMinPointY(point3dCollection2);
			break;
		}
		return true;
	}

	public void InsertSymGroupNot105(Database db, List<Region> lstRegionNotMax, Region reBound, double dLeftRight, double dTopBottom, string sPathFileSym, List<CSquares> lstSquares, List<CSquares> lstSquaresNo105)
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		_ = mdiActiveDocument.Editor;
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		for (int i = 0; i < lstSquaresNo105.Count; i++)
		{
			Point3d pt = default(Point3d);
			CSquares cSquares = lstSquaresNo105[i];
			double num = dLeftRight;
			int num2 = 1;
			if (dLeftRight < 0.0)
			{
				num2 = 2;
			}
			while (true)
			{
				_ = cSquares.m_ptCenter;
				int num3 = num2;
				if (!Insert1SquaseCmd(mdiActiveDocument, cSquares, lstRegionNotMax, reBound, Math.Abs(num), num3, sPathFileSym, ref pt))
				{
					num3 = ((num3 != 1) ? 1 : 2);
					if (!Insert1SquaseCmd(mdiActiveDocument, cSquares, lstRegionNotMax, reBound, Math.Abs(num), num3, sPathFileSym, ref pt))
					{
						if (num == 80.0)
						{
							break;
						}
						num = 80.0;
						continue;
					}
				}
				string sHandleRes = "";
				if (!symbol.InsertBlockFromFile(db, sPathFileSym, pt, ref sHandleRes))
				{
					break;
				}
				try
				{
					ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes, 16)), 0);
					if (objectId.IsNull)
					{
						break;
					}
					BlockReference blockReference = objectId.GetObject(OpenMode.ForRead) as BlockReference;
					bool flag = false;
					foreach (CSquares lstSquare in lstSquares)
					{
						if (!(cSquares.m_ptCenter.DistanceTo(lstSquare.m_ptCenter) <= 0.1) && CheckInterSect(blockReference, lstSquare.m_Region))
						{
							flag = true;
							break;
						}
					}
					blockReference.Dispose();
					blockReference = null;
					if (flag)
					{
						DeleteObject(db, sHandleRes);
						if (num != 80.0)
						{
							num = 80.0;
							continue;
						}
					}
					else if (CheckInterSect(objectId, objectIdCollection))
					{
						DeleteObject(db, sHandleRes);
						if (num != 80.0)
						{
							num = 80.0;
							continue;
						}
					}
					else if (!objectIdCollection.Contains(objectId))
					{
						objectIdCollection.Add(objectId);
					}
				}
				catch (Exception)
				{
				}
				break;
			}
		}
		objectIdCollection.Clear();
		for (int j = 0; j < lstSquaresNo105.Count; j++)
		{
			Point3d pt2 = default(Point3d);
			CSquares cSquares2 = lstSquaresNo105[j];
			double num4 = dTopBottom;
			int num5 = 3;
			if (num4 < 0.0)
			{
				num5 = 4;
			}
			while (true)
			{
				_ = cSquares2.m_ptCenter;
				int num6 = num5;
				if (!Insert1SquaseCmd(mdiActiveDocument, cSquares2, lstRegionNotMax, reBound, Math.Abs(num4), num6, sPathFileSym, ref pt2))
				{
					num6 = ((num6 != 3) ? 3 : 4);
					if (!Insert1SquaseCmd(mdiActiveDocument, cSquares2, lstRegionNotMax, reBound, Math.Abs(num4), num6, sPathFileSym, ref pt2))
					{
						if (num4 == 80.0)
						{
							break;
						}
						num4 = 80.0;
						continue;
					}
				}
				string sHandleRes2 = "";
				if (!symbol.InsertBlockFromFile(db, sPathFileSym, pt2, ref sHandleRes2))
				{
					break;
				}
				try
				{
					ObjectId objectId2 = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(sHandleRes2, 16)), 0);
					if (objectId2.IsNull)
					{
						break;
					}
					BlockReference blockReference2 = objectId2.GetObject(OpenMode.ForRead) as BlockReference;
					bool flag2 = false;
					foreach (CSquares lstSquare2 in lstSquares)
					{
						if (!(cSquares2.m_ptCenter.DistanceTo(lstSquare2.m_ptCenter) <= 0.1) && CheckInterSect(blockReference2, lstSquare2.m_Region))
						{
							flag2 = true;
							break;
						}
					}
					blockReference2.Dispose();
					blockReference2 = null;
					if (flag2)
					{
						DeleteObject(db, sHandleRes2);
						if (num4 != 80.0)
						{
							num4 = 80.0;
							continue;
						}
					}
					else if (CheckInterSect(objectId2, objectIdCollection))
					{
						DeleteObject(db, sHandleRes2);
						if (num4 != 80.0)
						{
							num4 = 80.0;
							continue;
						}
					}
					else if (!objectIdCollection.Contains(objectId2))
					{
						objectIdCollection.Add(objectId2);
					}
				}
				catch (Exception)
				{
				}
				break;
			}
		}
	}

	public void Insert1Squase(Document doc, CSquares quaSel, List<Region> lstRegionNotMax, Region reBound, double dDistance, int nDirect, string sPathFileSym)
	{
		Point3d maxPoint = quaSel.m_Region.GeometricExtents.MaxPoint;
		Point3d minPoint = quaSel.m_Region.GeometricExtents.MinPoint;
		Point3d ptCenter = quaSel.m_ptCenter;
		double num = dDistance;
		Circle circle = new Circle();
		circle.SetDatabaseDefaults(doc.Database);
		circle.Center = ptCenter;
		if (num < GlobalFunction.g_dRadiusSquare)
		{
			num = GlobalFunction.g_dRadiusSquare;
		}
		circle.Radius = num;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerWall)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			circle.Dispose();
			circle = null;
			return;
		}
		ObjectIdCollection objectIdCollection = new ObjectIdCollection();
		new DBObjectCollection();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value = objectIds[i];
			Line line = (Line)value.GetObject(OpenMode.ForRead);
			if (line == null)
			{
				continue;
			}
			int colorIndex = line.ColorIndex;
			if (colorIndex == GlobalFunction.nColorWall)
			{
				Point3dCollection point3dCollection = new Point3dCollection();
				line.IntersectWith(circle, Intersect.OnBothOperands, point3dCollection, IntPtr.Zero, IntPtr.Zero);
				if (point3dCollection.Count != 0)
				{
					objectIdCollection.Add(value);
				}
			}
			line.Dispose();
			line = null;
		}
		Point3dCollection point3dCollection2 = new Point3dCollection();
		Point3dCollection point3dCollection3 = new Point3dCollection();
		for (int j = 0; j < objectIdCollection.Count; j++)
		{
			Line line2 = objectIdCollection[j].GetObject(OpenMode.ForRead) as Line;
			Point3d point3d = GlobalFunction.Point3dCenter(line2);
			for (int k = j + 1; k < objectIdCollection.Count; k++)
			{
				Line line3 = objectIdCollection[k].GetObject(OpenMode.ForRead) as Line;
				Point3d endPoint = GlobalFunction.Point3dCenter(line3);
				double num2 = point3d.DistanceTo(line3.GetClosestPointTo(point3d, extend: true)) / 2.0;
				Line line4 = line2;
				if (line2.Length < line3.Length)
				{
					line4 = line3;
				}
				Line line5 = new Line(point3d, endPoint);
				DBObjectCollection offsetCurves = line4.GetOffsetCurves(num2);
				Line line6 = offsetCurves[0] as Line;
				Point3dCollection point3dCollection4 = new Point3dCollection();
				line6.IntersectWith(line5, Intersect.OnBothOperands, point3dCollection4, IntPtr.Zero, IntPtr.Zero);
				line5.Dispose();
				line5 = null;
				if (point3dCollection4.Count == 0)
				{
					offsetCurves = line4.GetOffsetCurves(-1.0 * num2);
					line6 = offsetCurves[0] as Line;
				}
				_ = line6.Angle;
				Point3d point3d2 = GlobalFunction.Point3dCenter(line6);
				if (split.CheckPointInsiheRegion(reBound, point3d2) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d2))
				{
					point3dCollection2.Add(point3d2);
					Point3d origin = Point3d.Origin;
					point3dCollection4.Clear();
					line6.IntersectWith(circle, Intersect.OnBothOperands, point3dCollection4, IntPtr.Zero, IntPtr.Zero);
					if (point3dCollection4.Count != 0)
					{
						bool flag = false;
						foreach (Point3d item in point3dCollection4)
						{
							switch (nDirect)
							{
							case 1:
								if (item.X < minPoint.X)
								{
									flag = true;
								}
								break;
							case 2:
								if (item.X > maxPoint.X)
								{
									flag = true;
								}
								break;
							case 3:
								if (item.Y > maxPoint.Y)
								{
									flag = true;
								}
								break;
							case 4:
								if (item.Y < minPoint.Y)
								{
									flag = true;
								}
								break;
							}
							if (flag)
							{
								break;
							}
						}
						if (flag)
						{
							Point3d startPoint = line6.StartPoint;
							Point3d endPoint2 = line6.EndPoint;
							double length = startPoint.GetVectorTo(endPoint2).Length;
							if (length <= 0.0 || dDistance <= 0.0)
							{
								return;
							}
							Point3d point3d4 = startPoint;
							Point3d point3d5 = endPoint2;
							Vector3d vector3d = ((!(ptCenter.DistanceTo(point3d4) > ptCenter.DistanceTo(point3d5))) ? (point3d5 - point3d4) : (point3d4 - point3d5));
							vector3d = vector3d * dDistance / length;
							Line line7 = new Line(ptCenter, ptCenter + vector3d);
							origin = line7.EndPoint;
							flag = false;
							switch (nDirect)
							{
							case 1:
								if (origin.X < minPoint.X)
								{
									flag = true;
								}
								break;
							case 2:
								if (origin.X > maxPoint.X)
								{
									flag = true;
								}
								break;
							case 3:
								if (origin.Y > maxPoint.Y)
								{
									flag = true;
								}
								break;
							case 4:
								if (origin.Y < minPoint.Y)
								{
									flag = true;
								}
								break;
							}
							if (!flag)
							{
								line7 = new Line(ptCenter, ptCenter - vector3d);
								origin = line7.EndPoint;
							}
							line7.Dispose();
							line7 = null;
							if (!point3dCollection3.Contains(origin) && split.CheckPointInsiheRegion(reBound, origin) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, origin))
							{
								point3dCollection3.Add(origin);
							}
						}
					}
				}
				line6.Dispose();
				line6 = null;
			}
		}
		circle.Dispose();
		circle = null;
		if (point3dCollection3.Count != 0)
		{
			Point3d point3d6 = GlobalFunction.GetMinPointX(point3dCollection3);
			switch (nDirect)
			{
			case 2:
				point3d6 = GlobalFunction.GetMaxPointX(point3dCollection3);
				break;
			case 3:
				point3d6 = GlobalFunction.GetMaxPointY(point3dCollection3);
				break;
			case 4:
				point3d6 = GlobalFunction.GetMinPointY(point3dCollection3);
				break;
			}
			Point3dCollection point3dCollection5 = new Point3dCollection();
			point3dCollection5.Add(point3d6);
			ObjectIdCollection idsLine = new ObjectIdCollection();
			symbol.DeleteBlockRefSame(doc, "symNearSquare", point3dCollection5, ref idsLine);
			string sHandleRes = "";
			if (symbol.InsertBlockFromFile(doc.Database, sPathFileSym, point3d6, ref sHandleRes))
			{
				GlobalFunction.DeleteEntityS(doc.Database, idsLine);
			}
		}
	}

	public bool cmdInsSymNearSquares2_old(string NameFileSym)
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return false;
		}
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				if (!split.CheckExitRoom(mdiActiveDocument))
				{
					return false;
				}
				subInsSymNearSquares2(mdiActiveDocument, NameFileSym);
				symbol.DeleteBlockSame(mdiActiveDocument, NameFileSym);
			}
		}
		catch (Exception)
		{
		}
		return true;
	}

	public void subInsSymNearSquares200(Document doc, string sNameSym)
	{
		_ = doc.Database;
		Editor editor = doc.Editor;
		if (!split.CheckExitRoom(doc))
		{
			return;
		}
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string text = pathFolderSymbol + "\\SymBackGround\\" + sNameSym + ".dwg";
		if (!File.Exists(text))
		{
			string message = text + "do not found";
			editor.WriteMessage("\n");
			editor.WriteMessage(message);
			return;
		}
		string fileName = Path.Combine(pathFolderSymbol + "\\SurfaceCut", "TemplateOut.dwg");
		Database database = new Database(buildDefaultDrawing: false, noDocument: true);
		database.ReadDwgFile(fileName, FileOpenMode.OpenForReadAndReadShare, allowCPConversion: true, "");
		database.CloseInput(closeFile: true);
		string sHandleRes = "";
		if (!symbol.InsertBlockFromFile(database, text, new Point3d(200.0, 0.0, 0.0), ref sHandleRes))
		{
			return;
		}
		ObjectId blockTableRecord = doc.Database.Insert("*U", database, preserveSourceDatabase: true);
		database.Dispose();
		database = null;
		BlockReference blockReference;
		while (true)
		{
			blockReference = new BlockReference(Point3d.Origin, blockTableRecord);
			blockReference.SetDatabaseDefaults(doc.Database);
			blockReference.Normal = editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
			blockReference.ColorIndex = 1;
			if (!BlockMoveRotateJig.JigOnly(blockReference, "\nSelect base point"))
			{
				break;
			}
			string value = GlobalFunction.AppendBlockIntersect(doc.Database, blockReference);
			ObjectId objectId = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
			if (objectId.IsNull)
			{
				return;
			}
			blockReference = objectId.GetObject(OpenMode.ForWrite) as BlockReference;
			DBObjectCollection dBObjectCollection = new DBObjectCollection();
			blockReference.Explode(dBObjectCollection);
			blockReference.Erase();
			blockReference.Dispose();
			blockReference = null;
			List<Region> lstRegionNotMax = new List<Region>();
			Region reBound = null;
			split.GetListRegion(doc, ref lstRegionNotMax, ref reBound);
			foreach (DBObject item in dBObjectCollection)
			{
				if (item is BlockReference)
				{
					blockReference = item as BlockReference;
					Point3d position = blockReference.Position;
					if (split.CheckPointInsiheRegion(reBound, position) && !split.CheckPointIsOnRegionAll(lstRegionNotMax, position))
					{
						GlobalFunction.AppendBlockIntersect(doc.Database, blockReference);
					}
				}
				try
				{
					item.Dispose();
				}
				catch (Exception)
				{
				}
			}
			split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
			editor.Regen();
			editor.UpdateScreen();
		}
		blockReference.Dispose();
		blockReference = null;
	}

	public bool cmdInsSymNearSquares2(string NameFileSym)
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return false;
		}
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				if (!split.CheckExitRoom(mdiActiveDocument))
				{
					return false;
				}
				if (string.Compare(NameFileSym, "symNearSquare2", ignoreCase: true) == 0)
				{
					subInsSymNearSquares2(mdiActiveDocument, NameFileSym);
				}
				else
				{
					subInsSymNearSquares200(mdiActiveDocument, NameFileSym);
				}
				symbol.DeleteBlockSame(mdiActiveDocument, NameFileSym);
			}
		}
		catch (Exception)
		{
		}
		return true;
	}

	public void subInsSymNearSquares2(Document doc, string sNameSym)
	{
		_ = doc.Database;
		Editor editor = doc.Editor;
		if (!split.CheckExitRoom(doc))
		{
			return;
		}
		TypedValue[] typedVals = new TypedValue[3]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerLineSquare),
			new TypedValue(62, GlobalFunction.nColorLineSquare)
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
			ObjectId value = objectIds[i];
			using Line line = value.GetObject(OpenMode.ForRead) as Line;
			DBObject value2 = line.Clone() as DBObject;
			dBObjectCollection.Add(value2);
			objectIdCollection.Add(value);
		}
		List<Region> lstRegion = new List<Region>();
		CreateRegionSquares(doc.Database, dBObjectCollection, ref lstRegion);
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
		List<CSquares> list = new List<CSquares>();
		Point3dCollection point3dCollection = new Point3dCollection();
		foreach (Region item2 in lstRegion)
		{
			if (!CheckObjectIsSquare(item2))
			{
				item2.UpgradeOpen();
				item2.Visible = false;
				continue;
			}
			CSquares cSquares = new CSquares();
			cSquares.m_Region = item2;
			cSquares.m_ptCenter = GlobalFunction.PointCenter(item2);
			if (split.CheckPointInsiheRegion(reBound, cSquares.m_ptCenter))
			{
				list.Add(cSquares);
				point3dCollection.Add(GlobalFunction.PointCenter(item2));
			}
		}
		VisibaleEntity(objectIdCollection, bVisibale: false);
		editor.Regen();
		editor.UpdateScreen();
		double num = 0.0;
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
				split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
				split.DeleteRegionS(doc.Database, lstRegion, null);
				VisibaleEntity(objectIdCollection);
				editor.Regen();
				editor.UpdateScreen();
				return;
			}
		}
		while (promptDoubleResult.Status != PromptStatus.OK || promptDoubleResult.Value <= 0.0);
		num = promptDoubleResult.Value;
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string text = pathFolderSymbol + "\\SymBackGround\\" + sNameSym + ".dwg";
		if (!File.Exists(text))
		{
			string message = text + "do not found";
			editor.WriteMessage("\n");
			editor.WriteMessage(message);
			return;
		}
		while (true)
		{
			CSquares quaSel = new CSquares();
			bool flag;
			do
			{
				PromptEntityOptions promptEntityOptions = new PromptEntityOptions("\nSelect Squares:");
				promptEntityOptions.SetRejectMessage("\nSelect Squares");
				promptEntityOptions.AddAllowedClass(typeof(Region), match: true);
				promptEntityOptions.AllowNone = true;
				PromptEntityResult entity = editor.GetEntity(promptEntityOptions);
				if (entity.Status != PromptStatus.OK)
				{
					split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
					split.DeleteRegionS(doc.Database, lstRegion, null);
					VisibaleEntity(objectIdCollection);
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
				ObjectId objectId = entity.ObjectId;
				flag = false;
				foreach (CSquares item3 in list)
				{
					if (item3.m_Region.ObjectId == objectId)
					{
						flag = true;
						quaSel = item3;
						break;
					}
				}
			}
			while (!flag);
            string inputText = "HD15KN";
            PromptStringOptions promptTextOptions = new PromptStringOptions("\nInput value [15/20/26/30] <15>: ")
            {
                AllowSpaces = false,
                DefaultValue = "15"
            };

            PromptResult textPrompt = editor.GetString(promptTextOptions);

            if (textPrompt.Status == PromptStatus.Cancel)
            {
                split.DeleteRegionS(doc.Database, lstRegion, null);
                split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
                VisibaleEntity(objectIdCollection);
                editor.Regen();
                editor.UpdateScreen();
                return;
            }

            string inputValue = textPrompt.StringResult;

            // Use default if empty
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                inputValue = "15";
            }

            // Validate against allowed values and retry if invalid
            int[] allowedValues = { 15, 20, 26, 30 };
            int parsedValue = 15; // default value
            bool isValid = false;

            while (!isValid)
            {
                if (int.TryParse(inputValue, out parsedValue))
                {
                    // Check if value is in allowed list
                    foreach (int allowed in allowedValues)
                    {
                        if (parsedValue == allowed)
                        {
                            isValid = true;
                            break;
                        }
                    }
                }

                if (!isValid)
                {
                    editor.WriteMessage("\nInvalid value. Please enter 15, 20, 26, or 30.");
                    textPrompt = editor.GetString(promptTextOptions);

                    if (textPrompt.Status == PromptStatus.Cancel)
                    {
                        split.DeleteRegionS(doc.Database, lstRegion, null);
                        split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
                        VisibaleEntity(objectIdCollection);
                        editor.Regen();
                        editor.UpdateScreen();
                        return;
                    }

                    inputValue = string.IsNullOrWhiteSpace(textPrompt.StringResult) ? "15" : textPrompt.StringResult;
                }
            }

            // ? FINAL TEXT
            inputText = $"HD{parsedValue}KN";
            // Store it for later use by cmdDrawLeader
            GlobalFunction.g_CurrentTextInput = inputText;
            int nDirect = 1;
			string text2;
			do
			{
				text2 = "";
				PromptStringOptions promptStringOptions = new PromptStringOptions("\nInput direction horizontal with Squares [Left(L)/Right(R)/Top(T)/Bottom(B)] <Left(L)>:");
				promptStringOptions.AllowSpaces = true;
				PromptResult promptResult = editor.GetString(promptStringOptions);
				if (promptResult.Status == PromptStatus.Cancel)
				{
					split.DeleteRegionS(doc.Database, lstRegion, null);
					split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
					VisibaleEntity(objectIdCollection);
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
				text2 = promptResult.StringResult;
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "L";
				}
				text2 = text2.Substring(0, 1);
			}
			while (string.Compare(text2, "L", ignoreCase: true) != 0 && string.Compare(text2, "R", ignoreCase: true) != 0 && string.Compare(text2, "T", ignoreCase: true) != 0 && string.Compare(text2, "B", ignoreCase: true) != 0);
			if (string.Compare(text2, "R", ignoreCase: true) == 0)
			{
				nDirect = 2;
			}
			if (string.Compare(text2, "T", ignoreCase: true) == 0)
			{
				nDirect = 3;
			}
			if (string.Compare(text2, "B", ignoreCase: true) == 0)
			{
				nDirect = 4;
			}
			Point3d pt = Point3d.Origin;
			if (!Insert1SquaseCmd(doc, quaSel, lstRegionNotMax, reBound, Math.Abs(num), nDirect, text, ref pt))
			{
				editor.WriteMessage("Point Insert do not correct\n");
				continue;
			}
			Point3dCollection point3dCollection2 = new Point3dCollection();
			point3dCollection2.Add(pt);
			ObjectIdCollection idsLine = new ObjectIdCollection();
			symbol.DeleteBlockRefSame(doc, sNameSym, point3dCollection2, ref idsLine);
			string sHandleRes = "";
			if (!symbol.InsertBlockFromFile(doc.Database, text, pt, ref sHandleRes))
			{
				break;
			}
			GlobalFunction.DeleteEntityS(doc.Database, idsLine);
			editor.Regen();
			editor.UpdateScreen();
		}
		split.DeleteRegionS(doc.Database, lstRegion, null);
		split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
		VisibaleEntity(objectIdCollection);
		editor.WriteMessage("The symbols have insert false\n");
		editor.Regen();
		editor.UpdateScreen();
	}

	public void subInsSymNearSquares(Document doc)
	{
		Database database = doc.Database;
		Editor editor = doc.Editor;
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
			string sHandle = "";
			DrawPolySelect(database, point3dCollection, ref sHandle);
			double num = 0.0;
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
					DeleteObject(database, sHandle);
					return;
				}
			}
			while (promptDoubleResult.Status != PromptStatus.OK || promptDoubleResult.Value <= 0.0);
			num = promptDoubleResult.Value;
			double num2 = num;
			string text;
			do
			{
				text = "";
				PromptStringOptions promptStringOptions = new PromptStringOptions("\nInput direction horizontal with Squares[ Left(L)/Right(R) ] <Left(L)>:");
				promptStringOptions.AllowSpaces = true;
				PromptResult promptResult = editor.GetString(promptStringOptions);
				if (promptResult.Status == PromptStatus.Cancel)
				{
					DeleteObject(database, sHandle);
					return;
				}
				text = promptResult.StringResult;
				if (string.IsNullOrEmpty(text))
				{
					text = "L";
				}
				text = text.Substring(0, 1);
			}
			while (string.Compare(text, "L", ignoreCase: true) != 0 && string.Compare(text, "R", ignoreCase: true) != 0);
			if (string.Compare(text, "R", ignoreCase: true) == 0)
			{
				num2 = -1.0 * num2;
			}
			double num3 = num;
			string text2;
			do
			{
				text2 = "";
				PromptStringOptions promptStringOptions2 = new PromptStringOptions("\nInput direction vertical with Squares[ Top(T)/Bottom(B) ] <Top(T)>:");
				promptStringOptions2.AllowSpaces = true;
				PromptResult promptResult2 = editor.GetString(promptStringOptions2);
				if (promptResult2.Status == PromptStatus.Cancel)
				{
					DeleteObject(database, sHandle);
					return;
				}
				text2 = promptResult2.StringResult;
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "T";
				}
				text2 = text2.Substring(0, 1);
			}
			while (string.Compare(text2, "T", ignoreCase: true) != 0 && string.Compare(text2, "B", ignoreCase: true) != 0);
			if (string.Compare(text2, "B", ignoreCase: true) == 0)
			{
				num3 = -1.0 * num3;
			}
			DeleteObject(database, sHandle);
			bool flag = true;
			DBObjectCollection dBObjectCollection;
			while (true)
			{
				dBObjectCollection = new DBObjectCollection();
				try
				{
					PromptSelectionResult promptSelectionResult = editor.SelectCrossingWindow(value, value2);
					if (promptSelectionResult.Status != PromptStatus.OK)
					{
						continue;
					}
					ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
					foreach (ObjectId objectId in objectIds)
					{
						Line line = objectId.GetObject(OpenMode.ForRead) as Line;
						if (!(line == null))
						{
							string layer = line.Layer;
							int colorIndex = line.ColorIndex;
							if (layer.Equals(GlobalFunction.S_layerLineSquare, StringComparison.CurrentCultureIgnoreCase) && colorIndex == GlobalFunction.nColorLineSquare)
							{
								dBObjectCollection.Add(line.Clone() as DBObject);
							}
							line.Dispose();
							line = null;
						}
					}
					break;
				}
				catch (Exception)
				{
					break;
				}
			}
			if (dBObjectCollection.Count == 0)
			{
				Application.ShowAlertDialog("The Object Image Squares has not found in select Window.\nYou can not setting correct. Please setting it.");
			}
			else
			{
				string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
				string text3 = pathFolderSymbol + "\\SymBackGround\\symNearSquare.dwg";
				if (!File.Exists(text3))
				{
					string message = text3 + "do not found";
					editor.WriteMessage("\n");
					editor.WriteMessage(message);
					flag = false;
					foreach (DBObject item in dBObjectCollection)
					{
						item.Dispose();
					}
				}
				else
				{
					ObjectIdCollection idsLine = new ObjectIdCollection();
					symbol.DeleteBlockInPolyline(doc, "symNearSquare", point3dCollection, ref idsLine);
					GlobalFunction.DeleteEntityS(database, idsLine);
					InsertWithCenterSquare(doc, dBObjectCollection, num2, num3, text3);
					foreach (DBObject item2 in dBObjectCollection)
					{
						item2.Dispose();
					}
				}
			}
			if (!flag)
			{
				break;
			}
			editor.Regen();
			editor.UpdateScreen();
		}
		editor.Regen();
		editor.UpdateScreen();
	}

	public void cmdInsSymNearSquares()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				if (split.CheckExitRoom(mdiActiveDocument))
				{
					subInsSymNearSquares(mdiActiveDocument);
					symbol.DeleteBlockSame(mdiActiveDocument, "symNearSquare");
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public int DeleteLineBy75(Document doc)
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

	public void subDrawLineby75(Document doc)
	{
		_ = doc.Database;
		Editor editor = doc.Editor;
		TypedValue[] typedVals = new TypedValue[2]
		{
			new TypedValue(0, "line"),
			new TypedValue(8, GlobalFunction.S_layerLineSquare)
		};
		SelectionFilter filter = new SelectionFilter(typedVals);
		PromptSelectionResult promptSelectionResult = doc.Editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			Application.ShowAlertDialog("The Drawing has not found Square.\nYou can not setting correct. Please setting it.");
			return;
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
		CreateRegionSquares(doc.Database, dBObjectCollection, ref lstRegion);
		foreach (DBObject item in dBObjectCollection)
		{
			item.Dispose();
		}
		if (lstRegion.Count == 0)
		{
			Application.ShowAlertDialog("The Drawing has not found one Square.");
			return;
		}
		List<CSquares> list = new List<CSquares>();
		foreach (Region item2 in lstRegion)
		{
			if (!CheckObjectIsSquare(item2))
			{
				item2.UpgradeOpen();
				item2.Visible = false;
				continue;
			}
			CSquares cSquares = new CSquares();
			cSquares.m_Region = item2;
			cSquares.m_ptCenter = GlobalFunction.PointCenter(item2);
			list.Add(cSquares);
		}
		VisibaleEntity(objectIdCollection, bVisibale: false);
		editor.Regen();
		editor.UpdateScreen();
		double num = 75.0;
		while (true)
		{
			CSquares cSquares2 = new CSquares();
			bool flag;
			do
			{
				PromptEntityOptions promptEntityOptions = new PromptEntityOptions("\nSelect Squares:");
				promptEntityOptions.SetRejectMessage("\nSelect Squares");
				promptEntityOptions.AddAllowedClass(typeof(Region), match: true);
				promptEntityOptions.AllowNone = true;
				PromptEntityResult entity = editor.GetEntity(promptEntityOptions);
				if (entity.Status != PromptStatus.OK)
				{
					split.DeleteRegionS(doc.Database, lstRegion, null);
					VisibaleEntity(objectIdCollection);
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
				ObjectId objectId = entity.ObjectId;
				flag = false;
				foreach (CSquares item3 in list)
				{
					if (item3.m_Region.ObjectId == objectId)
					{
						flag = true;
						cSquares2 = item3;
						break;
					}
				}
			}
			while (!flag);
			int num2 = 1;
			string text;
			do
			{
				text = "";
				PromptStringOptions promptStringOptions = new PromptStringOptions("\nInput direction horizontal with Squares[ Left(L)/Right(R)/Top(T)/Bottom(B) ] <Left(L)>:");
				promptStringOptions.AllowSpaces = true;
				PromptResult promptResult = editor.GetString(promptStringOptions);
				if (promptResult.Status == PromptStatus.Cancel)
				{
					split.DeleteRegionS(doc.Database, lstRegion, null);
					VisibaleEntity(objectIdCollection);
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
				text = promptResult.StringResult;
				if (string.IsNullOrEmpty(text))
				{
					text = "L";
				}
				text = text.Substring(0, 1);
			}
			while (string.Compare(text, "L", ignoreCase: true) != 0 && string.Compare(text, "R", ignoreCase: true) != 0 && string.Compare(text, "T", ignoreCase: true) != 0 && string.Compare(text, "B", ignoreCase: true) != 0);
			if (string.Compare(text, "R", ignoreCase: true) == 0)
			{
				num2 = 2;
			}
			if (string.Compare(text, "T", ignoreCase: true) == 0)
			{
				num2 = 3;
			}
			if (string.Compare(text, "B", ignoreCase: true) == 0)
			{
				num2 = 4;
			}
			Point3d maxPoint = cSquares2.m_Region.GeometricExtents.MaxPoint;
			Point3d minPoint = cSquares2.m_Region.GeometricExtents.MinPoint;
			_ = cSquares2.m_ptCenter;
			Line line2 = new Line();
			line2.SetDatabaseDefaults(doc.Database);
			Point3d startPoint = new Point3d(cSquares2.m_ptCenter.X - num, minPoint.Y, minPoint.Z);
			Point3d endPoint = new Point3d(cSquares2.m_ptCenter.X - num, maxPoint.Y, maxPoint.Z);
			switch (num2)
			{
			case 2:
				startPoint = new Point3d(cSquares2.m_ptCenter.X + num, minPoint.Y, minPoint.Z);
				endPoint = new Point3d(cSquares2.m_ptCenter.X + num, maxPoint.Y, maxPoint.Z);
				break;
			case 3:
				startPoint = new Point3d(minPoint.X, cSquares2.m_ptCenter.Y + num, minPoint.Z);
				endPoint = new Point3d(maxPoint.X, cSquares2.m_ptCenter.Y + num, maxPoint.Z);
				break;
			case 4:
				startPoint = new Point3d(minPoint.X, cSquares2.m_ptCenter.Y - num, minPoint.Z);
				endPoint = new Point3d(maxPoint.X, cSquares2.m_ptCenter.Y - num, maxPoint.Z);
				break;
			}
			line2.StartPoint = startPoint;
			line2.EndPoint = endPoint;
			GlobalFunction.AppendEntity(doc.Database, line2, GlobalFunction.S_layerLineSquare, 256);
			editor.Regen();
			editor.UpdateScreen();
		}
	}

	public void cmdInsSymNearSquares4(string NameFileSym)
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				if (split.CheckExitRoom(mdiActiveDocument))
				{
					subInsSymNearSquares4(mdiActiveDocument, NameFileSym);
					symbol.DeleteBlockSame(mdiActiveDocument, NameFileSym);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public void subInsSymNearSquares4(Document doc, string sNameSym)
	{
		Database database = doc.Database;
		Editor editor = doc.Editor;
		if (!split.CheckExitRoom(doc))
		{
			return;
		}
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string text = pathFolderSymbol + "\\SymBackGround\\" + sNameSym + ".dwg";
		if (!File.Exists(text))
		{
			string message = text + "do not found";
			editor.WriteMessage("\n");
			editor.WriteMessage(message);
			return;
		}
		string blockTableId = symbol.GetBlockTableId(database, text);
		if (string.IsNullOrEmpty(blockTableId))
		{
			return;
		}
		ObjectId objectId = database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(blockTableId, 16)), 0);
		if (objectId.IsNull)
		{
			return;
		}
		List<Region> lstRegionNotMax = new List<Region>();
		Region reBound = null;
		split.GetListRegion(doc, ref lstRegionNotMax, ref reBound);
		while (true)
		{
			BlockReference blockReference = new BlockReference(Point3d.Origin, objectId);
			blockReference.SetDatabaseDefaults(database);
			if (!BlockMovingScaling.JigOnly(blockReference))
			{
				blockReference.Dispose();
				blockReference = null;
				split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
				editor.Regen();
				editor.UpdateScreen();
				return;
			}
			string value = GlobalFunction.AppendBlockNearSquare(database, blockReference);
			ObjectId objectId2 = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
			if (objectId2.IsNull)
			{
				break;
			}
			blockReference = objectId2.GetObject(OpenMode.ForRead) as BlockReference;
			Point3d position = blockReference.Position;
			if (!split.CheckPointInsiheRegion(reBound, position) || split.CheckPointIsOnRegionAll(lstRegionNotMax, position))
			{
				blockReference.UpgradeOpen();
				blockReference.Erase();
				blockReference.Dispose();
				blockReference = null;
				editor.WriteMessage("\nPlease insert symbol in Wall");
				continue;
			}
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
					blockReference.UpgradeOpen();
					blockReference.Erase();
					blockReference.Dispose();
					blockReference = null;
					split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
				if (promptDoubleResult.Status == PromptStatus.None)
				{
					break;
				}
				if (promptDoubleResult.Status == PromptStatus.OK)
				{
					num = promptDoubleResult.Value;
					break;
				}
			}
			if (num == 0.0)
			{
				blockReference.Dispose();
				blockReference = null;
				continue;
			}
			int num2 = 1;
			string text2;
			do
			{
				text2 = "";
				PromptStringOptions promptStringOptions = new PromptStringOptions("\nInput direction horizontal with Squares[ Left(L)/Right(R)/Top(T)/Bottom(B) ] <Left(L)>:");
				promptStringOptions.AllowSpaces = true;
				PromptResult promptResult = editor.GetString(promptStringOptions);
				if (promptResult.Status == PromptStatus.Cancel)
				{
					blockReference.UpgradeOpen();
					blockReference.Erase();
					blockReference.Dispose();
					blockReference = null;
					split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
					editor.Regen();
					editor.UpdateScreen();
					return;
				}
				text2 = promptResult.StringResult;
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "L";
				}
				text2 = text2.Substring(0, 1);
			}
			while (string.Compare(text2, "L", ignoreCase: true) != 0 && string.Compare(text2, "R", ignoreCase: true) != 0 && string.Compare(text2, "T", ignoreCase: true) != 0 && string.Compare(text2, "B", ignoreCase: true) != 0);
			if (string.Compare(text2, "R", ignoreCase: true) == 0)
			{
				num2 = 2;
			}
			if (string.Compare(text2, "T", ignoreCase: true) == 0)
			{
				num2 = 3;
			}
			if (string.Compare(text2, "B", ignoreCase: true) == 0)
			{
				num2 = 4;
			}
			Point3d point3d = new Point3d(position.X - num, position.Y, position.Z);
			switch (num2)
			{
			case 2:
				point3d = new Point3d(position.X + num, position.Y, position.Z);
				break;
			case 3:
				point3d = new Point3d(position.X, position.Y + num, position.Z);
				break;
			case 4:
				point3d = new Point3d(position.X, position.Y - num, position.Z);
				break;
			}
			if (!split.CheckPointInsiheRegion(reBound, point3d) || split.CheckPointIsOnRegionAll(lstRegionNotMax, point3d))
			{
				blockReference.UpgradeOpen();
				blockReference.Erase();
				blockReference.Dispose();
				blockReference = null;
				editor.WriteMessage("\nSymbol has with out Wall");
				continue;
			}
			blockReference.UpgradeOpen();
			blockReference.Position = point3d;
			blockReference.Dispose();
			blockReference = null;
			Point3dCollection point3dCollection = new Point3dCollection();
			point3dCollection.Add(point3d);
			ObjectIdCollection idsLine = new ObjectIdCollection();
			symbol.DeleteBlockRefSame(doc, sNameSym, point3dCollection, ref idsLine);
			idsLine.Remove(objectId2);
			GlobalFunction.DeleteEntityS(doc.Database, idsLine);
			editor.Regen();
			editor.UpdateScreen();
		}
		split.DeleteRegionS(doc.Database, lstRegionNotMax, reBound);
		editor.Regen();
		editor.UpdateScreen();
	}
}
