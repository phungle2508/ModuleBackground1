using System;
using System.Collections.Generic;
using System.IO;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CSymbol
{
	public string CreateNewBlock(Database db, string strDwgFile)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(strDwgFile);
		if (File.Exists(strDwgFile))
		{
			using (Database database = new Database(buildDefaultDrawing: false, noDocument: true))
			{
				database.ReadDwgFile(strDwgFile, FileOpenMode.OpenForReadAndReadShare, allowCPConversion: true, "");
				database.CloseInput(closeFile: true);
				ObjectId objectId = db.Insert(fileNameWithoutExtension, database, preserveSourceDatabase: true);
				database.Dispose();
				return objectId.Handle.ToString();
			}
		}
		return "";
	}

	public string GetBlockTableId(Database db, string strDwgFile)
	{
		string result = "";
		try
		{
			using BlockTable blockTable = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(strDwgFile);
			if (!blockTable.Has(fileNameWithoutExtension))
			{
				result = CreateNewBlock(db, strDwgFile);
			}
			else
			{
				result = blockTable[fileNameWithoutExtension].Handle.ToString();
				using BlockTableRecord blockTableRecord = blockTable[fileNameWithoutExtension].GetObject(OpenMode.ForRead) as BlockTableRecord;
				ObjectIdCollection blockReferenceIds = blockTableRecord.GetBlockReferenceIds(directOnly: false, forceValidity: false);
				if (blockReferenceIds.Count == 0 && !blockTableRecord.IsLayout)
				{
					blockTableRecord.UpgradeOpen();
					blockTableRecord.Erase();
					result = CreateNewBlock(db, strDwgFile);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public bool InsertBlockFromFile(Database db, string strDwgFile, Point3d ptIns)
	{
		string blockTableId = GetBlockTableId(db, strDwgFile);
		if (string.IsNullOrEmpty(blockTableId))
		{
			return false;
		}
		ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(blockTableId, 16)), 0);
		if (objectId.IsNull)
		{
			return false;
		}
		BlockReference blockReference = new BlockReference(ptIns, objectId);
		blockReference.SetDatabaseDefaults(db);
		GlobalFunction.AppendBlockIntersect(db, blockReference);
		return true;
	}

	public bool InsertBlockFromFile(Database db, string strDwgFile, Point3d ptIns, ref string sHandleRes)
	{
		string blockTableId = GetBlockTableId(db, strDwgFile);
		if (string.IsNullOrEmpty(blockTableId))
		{
			return false;
		}
		ObjectId objectId = db.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(blockTableId, 16)), 0);
		if (objectId.IsNull)
		{
			return false;
		}
		BlockReference blockReference = new BlockReference(ptIns, objectId);
		blockReference.SetDatabaseDefaults(db);
		sHandleRes = GlobalFunction.AppendBlockIntersect(db, blockReference);
		return true;
	}

	public void DeleteBlockRefSame(Document doc, string sBlockName, Point3dCollection ptsInterSect, ref ObjectIdCollection idsLine)
	{
		Editor editor = doc.Editor;
		SelectionFilter filter = new SelectionFilter(new TypedValue[2]
		{
			new TypedValue(0, "INSERT"),
			new TypedValue(2, sBlockName)
		});
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return;
		}
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value = objectIds[i];
			BlockReference blockReference = value.GetObject(OpenMode.ForRead) as BlockReference;
			if (!(blockReference == null))
			{
				Point3d position = blockReference.Position;
				blockReference.Dispose();
				blockReference = null;
				if (ptsInterSect.Contains(position))
				{
					idsLine.Add(value);
				}
			}
		}
	}

	public void DeleteBlockInPolyline(Document doc, string sBlockName, Point3dCollection arrPointPolyLine, ref ObjectIdCollection idsLine)
	{
		Editor editor = doc.Editor;
		SelectionFilter filter = new SelectionFilter(new TypedValue[2]
		{
			new TypedValue(0, "INSERT"),
			new TypedValue(2, sBlockName)
		});
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return;
		}
		CSplitRoom cSplitRoom = new CSplitRoom();
		ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
		for (int i = 0; i < objectIds.Length; i++)
		{
			ObjectId value = objectIds[i];
			BlockReference blockReference = value.GetObject(OpenMode.ForRead) as BlockReference;
			if (!(blockReference == null))
			{
				Point3d position = blockReference.Position;
				blockReference.Dispose();
				blockReference = null;
				if (cSplitRoom.IsInPolygon(position, arrPointPolyLine))
				{
					idsLine.Add(value);
				}
			}
		}
	}

	public void DeleteBlockSame(Document doc, string sBlockName)
	{
		Editor editor = doc.Editor;
		SelectionFilter filter = new SelectionFilter(new TypedValue[2]
		{
			new TypedValue(0, "INSERT"),
			new TypedValue(2, sBlockName)
		});
		PromptSelectionResult promptSelectionResult = editor.SelectAll(filter);
		if (promptSelectionResult.Status != PromptStatus.OK)
		{
			return;
		}
		List<ObjectId> list = new List<ObjectId>();
		list.AddRange(promptSelectionResult.Value.GetObjectIds());
		new CSplitRoom();
		for (int i = 0; i < list.Count - 1; i++)
		{
			BlockReference blockReference = list[i].GetObject(OpenMode.ForRead) as BlockReference;
			if (blockReference == null)
			{
				continue;
			}
			bool flag = false;
			Point3d position = blockReference.Position;
			for (int j = i + 1; j < list.Count; j++)
			{
				BlockReference blockReference2 = list[j].GetObject(OpenMode.ForRead) as BlockReference;
				if (!(blockReference2 == null))
				{
					Point3d position2 = blockReference2.Position;
					if (position.DistanceTo(position2) <= 0.001)
					{
						flag = true;
						blockReference2.Dispose();
						blockReference2 = null;
						break;
					}
					blockReference2.Dispose();
					blockReference2 = null;
				}
			}
			if (flag)
			{
				blockReference.UpgradeOpen();
				blockReference.Erase();
			}
			blockReference.Dispose();
			blockReference = null;
		}
	}
}
