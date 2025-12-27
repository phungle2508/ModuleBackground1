using System;
using System.IO;
using System.Windows.Forms;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CInsSymJintsuko
{
	public const string S_NameFolderSym = "\\SymBackGround";

	public CSymbol symbol = new CSymbol();

	public void UpdateTextBlock(ObjectId idBlock, string sText1, string sText2)
	{
		BlockTableRecord blockTableRecord = idBlock.GetObject(OpenMode.ForWrite) as BlockTableRecord;
		if (blockTableRecord.HasAttributeDefinitions)
		{
			foreach (ObjectId item in blockTableRecord)
			{
				DBObject dBObject = item.GetObject(OpenMode.ForRead);
				AttributeDefinition attributeDefinition = dBObject as AttributeDefinition;
				if (attributeDefinition != null && !attributeDefinition.Constant)
				{
					DBText dBText = new DBText();
					dBText.SetPropertiesFrom(attributeDefinition);
					dBText.Position = attributeDefinition.Position;
					dBText.Height = attributeDefinition.Height;
					dBText.Layer = attributeDefinition.Layer;
					dBText.ColorIndex = attributeDefinition.ColorIndex;
					if (string.Compare(attributeDefinition.Tag, "Number1", ignoreCase: true) == 0)
					{
						dBText.TextString = sText1;
					}
					if (string.Compare(attributeDefinition.Tag, "Number2", ignoreCase: true) == 0)
					{
						dBText.TextString = sText2;
					}
					blockTableRecord.AppendEntity(dBText);
					dBText.Dispose();
					dBText = null;
					attributeDefinition.UpgradeOpen();
					attributeDefinition.Erase();
				}
				dBObject.Dispose();
				dBObject = null;
			}
		}
		blockTableRecord.Dispose();
		blockTableRecord = null;
	}

	public void UpdateAttributeBlock(ObjectId idBref, string sText1, string sText2)
	{
		BlockReference blockReference = idBref.GetObject(OpenMode.ForWrite) as BlockReference;
		DBObjectCollection dBObjectCollection = new DBObjectCollection();
		blockReference.Explode(dBObjectCollection);
		Point3dCollection point3dCollection = new Point3dCollection();
		point3dCollection.Add(Point3d.Origin);
		point3dCollection.Add(Point3d.Origin);
		foreach (DBObject item in dBObjectCollection)
		{
			if (item is DBText)
			{
				DBText dBText = item as DBText;
				if (dBText != null)
				{
					string textString = dBText.TextString;
					if (string.Compare(textString, sText1, ignoreCase: true) != 0 && string.Compare(textString, sText2, ignoreCase: true) != 0)
					{
						item.Dispose();
						continue;
					}
					Point3d minPoint = dBText.GeometricExtents.MinPoint;
					if (string.Compare(textString, sText1, ignoreCase: true) == 0)
					{
						point3dCollection[0] = minPoint;
					}
					if (string.Compare(textString, sText2, ignoreCase: true) == 0)
					{
						point3dCollection[1] = minPoint;
					}
				}
			}
			item.Dispose();
		}
		BlockTableRecord blockTableRecord = blockReference.BlockTableRecord.GetObject(OpenMode.ForWrite) as BlockTableRecord;
		if (blockTableRecord.HasAttributeDefinitions)
		{
			foreach (ObjectId item2 in blockTableRecord)
			{
				DBObject dBObject2 = item2.GetObject(OpenMode.ForRead);
				DBText dBText2 = dBObject2 as DBText;
				if (dBText2 != null)
				{
					string textString2 = dBText2.TextString;
					if (string.Compare(textString2, sText1, ignoreCase: true) != 0 && string.Compare(textString2, sText2, ignoreCase: true) != 0)
					{
						dBObject2.Dispose();
						dBObject2 = null;
						continue;
					}
					Point3d point3d = dBText2.GeometricExtents.MinPoint.TransformBy(blockReference.BlockTransform);
					AttributeReference attributeReference = new AttributeReference();
					if (string.Compare(textString2, sText1, ignoreCase: true) == 0)
					{
						attributeReference.Tag = "Number1";
						point3d = point3dCollection[0];
					}
					if (string.Compare(textString2, sText2, ignoreCase: true) == 0)
					{
						attributeReference.Tag = "Number2";
						point3d = point3dCollection[1];
					}
					attributeReference.TextString = textString2;
					attributeReference.Position = dBText2.Position.TransformBy(blockReference.BlockTransform);
					attributeReference.Height = dBText2.Height;
					attributeReference.Layer = dBText2.Layer;
					attributeReference.ColorIndex = dBText2.ColorIndex;
					double num;
					for (num = blockReference.Rotation; num >= Math.PI; num -= Math.PI)
					{
					}
					attributeReference.Rotation = num;
					attributeReference.WidthFactor = dBText2.WidthFactor;
					attributeReference.AlignmentPoint = dBText2.AlignmentPoint;
					attributeReference.HorizontalMode = dBText2.HorizontalMode;
					attributeReference.VerticalMode = dBText2.VerticalMode;
					attributeReference.TransformBy(Matrix3d.Displacement(point3d - attributeReference.GeometricExtents.MinPoint));
					blockReference.AttributeCollection.AppendAttribute(attributeReference);
					attributeReference.Dispose();
					attributeReference = null;
					dBText2.UpgradeOpen();
					dBText2.Erase();
				}
				dBObject2.Dispose();
				dBObject2 = null;
			}
		}
		blockTableRecord.Dispose();
		blockTableRecord = null;
		blockReference.Dispose();
		blockReference = null;
	}

	public void cmdInsSymJintsuko(Document doc)
	{
		Editor editor = doc.Editor;
		Matrix3d currentUserCoordinateSystem = editor.CurrentUserCoordinateSystem;
		string pathFolderSymbol = GlobalFunction.GetPathFolderSymbol();
		string arg = pathFolderSymbol + "\\SymBackGround";
		int nSel = 0;
		while (true)
		{
			editor.WriteMessage("\n");
			FormSettingSymJintsuko formSettingSymJintsuko = new FormSettingSymJintsuko();
			formSettingSymJintsuko.m_nSel = nSel;
			Bricscad.ApplicationServices.Application.ShowModalDialog(Bricscad.ApplicationServices.Application.MainWindow, formSettingSymJintsuko, persist: true);
			if (DialogResult.OK != formSettingSymJintsuko.DialogResult)
			{
				break;
			}
			string sText = formSettingSymJintsuko.m_sText;
			string[] array = sText.Split('/');
			string text = array[0];
			string text2 = array[1];
			string text3 = $"{arg}\\symJint_{text}{text2}.dwg";
			if (!File.Exists(text3))
			{
				string message = text3 + "do not found";
				editor.WriteMessage("\n");
				editor.WriteMessage(message);
				break;
			}
			nSel = formSettingSymJintsuko.m_nSel;
			string value = CInfoSurfaceCut.CreateNewBlockDynamic(doc.Database, text3);
			if (string.IsNullOrEmpty(value))
			{
				break;
			}
			ObjectId objectId = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
			if (objectId.IsNull)
			{
				break;
			}
			if (string.Compare(text, "1075", ignoreCase: true) == 0)
			{
				text = "1,075";
			}
			UpdateTextBlock(objectId, text, text2);
			BlockReference blockReference = new BlockReference(Point3d.Origin, objectId);
			blockReference.Normal = editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis;
			blockReference.ColorIndex = 1;
			if (BlockMirrorJig.Jig(blockReference))
			{
				Vector2d vector2d = new Vector2d(1.0, 0.0);
				double num = blockReference.Rotation;
				if (num >= Math.PI * 2.0)
				{
					num -= Math.PI * 2.0;
				}
				if (num == 0.0 || num == Math.PI)
				{
					vector2d = new Vector2d(0.0, 1.0);
				}
				Point3d position = blockReference.Position;
				Matrix3d mirror = Matrix3d.Mirroring(new Line3d(position, position + new Vector3d(vector2d.X, vector2d.Y, 0.0)));
				BlockJig jig = new BlockJig(mirror, currentUserCoordinateSystem, position, num, blockReference);
				if (editor.Drag(jig).Status != PromptStatus.OK)
				{
					blockReference.Dispose();
					blockReference = null;
					continue;
				}
				value = GlobalFunction.AppendEntity(doc.Database, blockReference, GlobalFunction.S_layerWall, 256);
				if (!string.IsNullOrEmpty(value))
				{
					ObjectId objectId2 = doc.Database.GetObjectId(createIfNotFound: false, new Handle(Convert.ToInt64(value, 16)), 0);
					if (!objectId.IsNull)
					{
						UpdateAttributeBlock(objectId2, text, text2);
					}
				}
			}
			else
			{
				blockReference.Dispose();
				blockReference = null;
			}
		}
	}
}
