using System;
using System.Drawing;
using Bricscad.ApplicationServices;
using Teigha.Runtime;

namespace ModuleBackground;

public class CMyCommand : IExtensionApplication
{
	public void Initialize()
	{
		initForm();
	}

	public void Terminate()
	{
	}

	public void initForm()
	{
		try
		{
			if (GlobalFunction.dlg1 == null)
			{
				GlobalFunction.dlg1 = new FormMenu();
				Application.ShowModelessDialog(Application.MainWindow, GlobalFunction.dlg1, persist: false);
				int x = Application.MainWindow.Location.X + 50;
				int y = Application.MainWindow.Location.Y + 125;
				GlobalFunction.dlg1.Location = new Point(x, y);
			}
			else
			{
				GlobalFunction.dlg1.Activate();
			}
		}
		catch (System.Exception)
		{
		}
	}

	public void HideFormMenu()
	{
		try
		{
			if (GlobalFunction.dlg1 != null)
			{
				GlobalFunction.dlg1.Hide();
			}
		}
		catch (System.Exception)
		{
		}
	}

	public void ShowFormMenu()
	{
		try
		{
			if (GlobalFunction.dlg1 != null)
			{
				GlobalFunction.dlg1.Show();
				GlobalFunction.dlg1.Activate();
			}
		}
		catch (System.Exception)
		{
		}
	}

	[CommandMethod("Nctri_Module", "ST_SplitRoom", "ST_SplitRoom", CommandFlags.Modal)]
	public void ST_SplitRoom()
	{
		HideFormMenu();
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		DocumentLock documentLock = mdiActiveDocument.LockDocument();
		using (documentLock)
		{
			GlobalFunction.InitIni();
			CSplitRoom cSplitRoom = new CSplitRoom();
			cSplitRoom.SplitRoom(mdiActiveDocument);
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_DrawGridsIntersect", "ST_DrawGridsIntersect", CommandFlags.Modal)]
	public void ST_DrawGridsIntersect()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInsSymIntersect cInsSymIntersect = new CInsSymIntersect();
				cInsSymIntersect.cmdDrawGridsIntersect(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InsSymIntersect1", "ST_InsSymIntersect1", CommandFlags.Modal)]
	public void ST_InsSymIntersect1()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInsSymIntersect cInsSymIntersect = new CInsSymIntersect();
				cInsSymIntersect.cmdInsSymIntersect();
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InsSymIntersect2", "ST_InsSymIntersect2", CommandFlags.Modal)]
	public void ST_InsSymIntersect2()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInsSymIntersect cInsSymIntersect = new CInsSymIntersect();
				cInsSymIntersect.cmdInsSymIntersect2();
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InsSymNearSquares1", "ST_InsSymNearSquares1", CommandFlags.Modal)]
	public void ST_InsSymNearSquares()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (!(mdiActiveDocument == null))
		{
			HideFormMenu();
			GlobalFunction.InitIni();
			object curOsnap = 0;
			GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
			CInsSymNearSquares cInsSymNearSquares = new CInsSymNearSquares();
			cInsSymNearSquares.cmdInsSymNearSquares();
			GlobalFunction.revertOSNAP(curOsnap);
			ShowFormMenu();
		}
	}

	[CommandMethod("Nctri_Module", "ST_InsSymNearSquares2", "ST_InsSymNearSquares2", CommandFlags.Modal)]
	public void ST_InsSymNearSquares2()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (!(mdiActiveDocument == null))
		{
			HideFormMenu();
			GlobalFunction.InitIni();
			object curOsnap = 0;
			GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
			CInsSymNearSquares cInsSymNearSquares = new CInsSymNearSquares();
			cInsSymNearSquares.cmdInsSymNearSquares2("symNearSquare");
			GlobalFunction.revertOSNAP(curOsnap);
			ShowFormMenu();
		}
	}

	[CommandMethod("Nctri_Module", "ST_InsSymNearSquares3", "ST_InsSymNearSquares3", CommandFlags.Modal)]
	public void ST_InsSymNearSquares3()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		GlobalFunction.InitIni();
		object curOsnap = 0;
		GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
		CInsSymNearSquares cInsSymNearSquares = new CInsSymNearSquares();
		if (cInsSymNearSquares.cmdInsSymNearSquares2("symNearSquare2"))
		{
			try
			{
				using (mdiActiveDocument.LockDocument())
				{
					CDrawLeader cDrawLeader = new CDrawLeader();
					cDrawLeader.cmdDrawLeader("symNearSquare2");
				}
			}
			catch (System.Exception)
			{
			}
		}
		GlobalFunction.revertOSNAP(curOsnap);
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InsSymNearSquares4", "ST_InsSymNearSquares4", CommandFlags.Modal)]
	public void ST_InsSymNearSquares4()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (!(mdiActiveDocument == null))
		{
			HideFormMenu();
			GlobalFunction.InitIni();
			object curOsnap = 0;
			GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
			CInsSymNearSquares cInsSymNearSquares = new CInsSymNearSquares();
			cInsSymNearSquares.cmdInsSymNearSquares4("symNearSquare");
			GlobalFunction.revertOSNAP(curOsnap);
			ShowFormMenu();
		}
	}

	[CommandMethod("Nctri_Module", "ST_DrawLine75", "ST_DrawLine75", CommandFlags.Modal)]
	public void ST_DrawLine75()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInsSymNearSquares cInsSymNearSquares = new CInsSymNearSquares();
				cInsSymNearSquares.subDrawLineby75(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_DrawLeader", "ST_DrawLeader", CommandFlags.Modal)]
	public void ST_DrawLeader()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CDrawLeader cDrawLeader = new CDrawLeader();
				cDrawLeader.cmdDrawLeader("symNearSquare2");
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_SettingBackground", "ST_SettingBackground", CommandFlags.Modal)]
	public void ST_SettingBackground()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CSettingObject cSettingObject = new CSettingObject();
				cSettingObject.CmdSettingObject();
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InsSymJintsuko", "ST_InsSymJintsuko", CommandFlags.Modal)]
	public void ST_InsSymJintsuko()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInsSymJintsuko cInsSymJintsuko = new CInsSymJintsuko();
				cInsSymJintsuko.cmdInsSymJintsuko(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_DrawLine100", "ST_DrawLine100", CommandFlags.Modal)]
	public void ST_DrawLine100()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CDrawLine cDrawLine = new CDrawLine();
				cDrawLine.cmdDrawline100(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InputSurfaceCut", "ST_InputSurfaceCut", CommandFlags.Modal)]
	public void ST_InputSurfaceCut()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInfoSurfaceCut cInfoSurfaceCut = new CInfoSurfaceCut();
				cInfoSurfaceCut.cmdInputSurfaceCut(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_OutputSurfaceCut", "ST_OutputSurfaceCut", CommandFlags.Modal)]
	public void ST_OutputSurfaceCut()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInfoSurfaceCut cInfoSurfaceCut = new CInfoSurfaceCut();
				cInfoSurfaceCut.cmdOutputSurfaceCut(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InputOther", "ST_InputOther", CommandFlags.Modal)]
	public void ST_InputOther()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInfoSurfaceCut cInfoSurfaceCut = new CInfoSurfaceCut();
				cInfoSurfaceCut.cmdInputOther(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_TextNote", "ST_TextNote", CommandFlags.Modal)]
	public void ST_TextNote()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CTextNote cTextNote = new CTextNote();
				cTextNote.cmdTextNote(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_DrawPattern", "ST_DrawPattern", CommandFlags.Modal)]
	public void ST_DrawPattern()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CDrawPattern cDrawPattern = new CDrawPattern();
				cDrawPattern.cmdDrawPattern(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}

	[CommandMethod("Nctri_Module", "ST_InputUB", "ST_InputUB", CommandFlags.Modal)]
	public void ST_InputUB()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return;
		}
		HideFormMenu();
		try
		{
			using (mdiActiveDocument.LockDocument())
			{
				GlobalFunction.InitIni();
				object curOsnap = 0;
				GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
				CInputRoomUB cInputRoomUB = new CInputRoomUB();
				cInputRoomUB.cmdInputRoomUB(mdiActiveDocument);
				GlobalFunction.revertOSNAP(curOsnap);
			}
		}
		catch (System.Exception)
		{
		}
		ShowFormMenu();
	}
}
