using System;
using System.IO;
using Bricscad.ApplicationServices;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CInputRoomUB
{
	public void cmdInputRoomUB(Document doc)
	{
		FormSettingRoomUB formSettingRoomUB = new FormSettingRoomUB();
		Application.ShowModalDialog(Application.MainWindow, formSettingRoomUB, persist: false);
		_ = 1;
		_ = formSettingRoomUB.DialogResult;
	}

	public static void InsertUb(Document doc, string sPathFile)
	{
		string value = CInfoSurfaceCut.CreateNewBlockDynamic(doc.Database, sPathFile);
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		ObjectId objectId = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
		if (objectId.IsNull)
		{
			return;
		}
		BlockReference blockReference = new BlockReference(Point3d.Origin, objectId);
		blockReference.SetDatabaseDefaults(doc.Database);
		Path.GetFileNameWithoutExtension(sPathFile);
		if (BlockMovingScaling.JigOnly(blockReference))
		{
			string value2 = GlobalFunction.AppendEntity(doc.Database, blockReference, "0", 6);
			ObjectId objectId2 = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value2, 16)), 0);
			if (objectId2.IsNull)
			{
				blockReference.Dispose();
				blockReference = null;
				return;
			}
			blockReference = objectId2.GetObject(OpenMode.ForWrite) as BlockReference;
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
