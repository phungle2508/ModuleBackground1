using System.IO;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;

namespace ModuleBackground;

public class CSettingObject
{
	public bool settingObject(string sMsg, string sKey, CIniFile ini)
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return false;
		}
		_ = mdiActiveDocument.Database;
		Editor editor = mdiActiveDocument.Editor;
		PromptEntityOptions promptEntityOptions = new PromptEntityOptions(sMsg);
		promptEntityOptions.SetRejectMessage(sMsg);
		promptEntityOptions.AddAllowedClass(typeof(Line), match: true);
		promptEntityOptions.AllowNone = true;
		PromptEntityResult entity = editor.GetEntity(promptEntityOptions);
		if (entity.Status != PromptStatus.OK)
		{
			return false;
		}
		Line line = entity.ObjectId.GetObject(OpenMode.ForRead) as Line;
		if (line != null)
		{
			ini.SetValue("SystemBackground", sKey, $"{line.Layer},{line.ColorIndex}");
		}
		line.Dispose();
		line = null;
		return true;
	}

	public void CmdSettingObject()
	{
		string pathFolderIni = GlobalFunction.GetPathFolderIni();
		string text = pathFolderIni + "\\Background.ini";
		if (!File.Exists(text))
		{
			Application.ShowAlertDialog("The file system Background.ini has not found");
			return;
		}
		CIniFile ini = new CIniFile(text);
		settingObject("\nSelect Line Wall:", "LineWall", ini);
		settingObject("\nSelect Line Hatch:", "LineHatch", ini);
		settingObject("\nSelect Line Square:", "LineSquare", ini);
	}
}
