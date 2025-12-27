using System;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class BlockMovingChangeBaseScaling : EntityJig
{
	public int mCurJigFactorNumber = 1;

	private Point3d mPosition;

	private double mScaleFactor;

	private double mRotation;

	private double mAngleOffset;

	private int m_nChangebase;

	private double _angle;

	private bool _bBasePoint;

	public BlockMovingChangeBaseScaling(Entity ent, bool bBasePoint = false)
		: base(ent)
	{
		ent.TransformBy(Application.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem);
		mAngleOffset = (ent as BlockReference).Rotation;
		mScaleFactor = (ent as BlockReference).ScaleFactors.X;
		m_nChangebase = 0;
		_angle = 0.0;
		_bBasePoint = bBasePoint;
	}

	protected override bool Update()
	{
		switch (mCurJigFactorNumber)
		{
		case 1:
			(base.Entity as BlockReference).Position = mPosition;
			(base.Entity as BlockReference).Rotation = _angle;
			break;
		case 2:
			(base.Entity as BlockReference).ScaleFactors = new Scale3d(mScaleFactor, mScaleFactor, mScaleFactor);
			break;
		default:
			return false;
		}
		return true;
	}

	protected override SamplerStatus Sampler(JigPrompts prompts)
	{
		switch (mCurJigFactorNumber)
		{
		case 1:
		{
			JigPromptPointOptions jigPromptPointOptions = new JigPromptPointOptions("\nBlock position:");
			jigPromptPointOptions.UserInputControls = (UserInputControls)195;
			if (_bBasePoint)
			{
				jigPromptPointOptions.SetMessageAndKeywords("\nBlock position or change Base Point or  Rotate [C/R]:", "C R");
			}
			else
			{
				jigPromptPointOptions.SetMessageAndKeywords("\nBlock position or Rotate[R]:", "R");
			}
			PromptPointResult promptPointResult = prompts.AcquirePoint(jigPromptPointOptions);
			if (promptPointResult.Status == PromptStatus.Cancel)
			{
				return SamplerStatus.Cancel;
			}
			if (promptPointResult.Status == PromptStatus.Keyword)
			{
				switch (promptPointResult.StringResult)
				{
				case "C":
				case "c":
				{
					BlockTableRecord blockTableRecord = (base.Entity as BlockReference).BlockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord;
					int num = m_nChangebase % 2;
					Matrix3d transform = Matrix3d.Displacement(new Point3d(-15.0, -8.0, 0.0) - Point3d.Origin);
					if (num != 0)
					{
						transform = Matrix3d.Displacement(Point3d.Origin - new Point3d(-15.0, -8.0, 0.0));
					}
					m_nChangebase++;
					foreach (ObjectId item in blockTableRecord)
					{
						Entity entity = item.GetObject(OpenMode.ForWrite) as Entity;
						entity.TransformBy(transform);
						entity.Dispose();
						entity = null;
					}
					blockTableRecord.Dispose();
					blockTableRecord = null;
					break;
				}
				case "R":
				case "r":
					_angle -= Math.PI / 2.0;
					while (_angle < Math.PI * 2.0)
					{
						_angle += Math.PI * 2.0;
					}
					break;
				}
				return SamplerStatus.OK;
			}
			if (promptPointResult.Value.Equals(mPosition))
			{
				return SamplerStatus.NoChange;
			}
			mPosition = promptPointResult.Value;
			return SamplerStatus.OK;
		}
		case 2:
		{
			JigPromptDistanceOptions jigPromptDistanceOptions = new JigPromptDistanceOptions("\nBlock scale factor in percentage(1/100):");
			jigPromptDistanceOptions.UserInputControls = (UserInputControls)4224;
			jigPromptDistanceOptions.BasePoint = mPosition;
			jigPromptDistanceOptions.UseBasePoint = true;
			PromptDoubleResult promptDoubleResult = prompts.AcquireDistance(jigPromptDistanceOptions);
			if (promptDoubleResult.Status == PromptStatus.Cancel)
			{
				return SamplerStatus.Cancel;
			}
			if (promptDoubleResult.Value.Equals(mRotation))
			{
				return SamplerStatus.NoChange;
			}
			mScaleFactor = promptDoubleResult.Value / 100.0;
			return SamplerStatus.OK;
		}
		default:
			return SamplerStatus.OK;
		}
	}

	public static bool Jig(BlockReference ent, bool bBasePoint = false)
	{
		try
		{
			Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
			BlockMovingChangeBaseScaling blockMovingChangeBaseScaling = new BlockMovingChangeBaseScaling(ent, bBasePoint);
			PromptResult promptResult;
			do
			{
				promptResult = editor.Drag(blockMovingChangeBaseScaling);
				if (blockMovingChangeBaseScaling.mCurJigFactorNumber == 1)
				{
					PromptStatus status = promptResult.Status;
					if (status == PromptStatus.Keyword)
					{
						blockMovingChangeBaseScaling.mCurJigFactorNumber = 0;
					}
				}
			}
			while (promptResult.Status != PromptStatus.Cancel && promptResult.Status != PromptStatus.Error && blockMovingChangeBaseScaling.mCurJigFactorNumber++ < 2);
			double num = (double)(int)Math.Round(blockMovingChangeBaseScaling.mScaleFactor, 0) * 1.0;
			ent.ScaleFactors = new Scale3d(num, num, num);
			return promptResult.Status == PromptStatus.OK;
		}
		catch
		{
			return false;
		}
	}

	public static bool JigOnly(BlockReference ent)
	{
		try
		{
			Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
			BlockMovingChangeBaseScaling blockMovingChangeBaseScaling = new BlockMovingChangeBaseScaling(ent);
			PromptResult promptResult;
			do
			{
				promptResult = editor.Drag(blockMovingChangeBaseScaling);
			}
			while (promptResult.Status != PromptStatus.Cancel && promptResult.Status != PromptStatus.Error && blockMovingChangeBaseScaling.mCurJigFactorNumber++ < 1);
			return promptResult.Status == PromptStatus.OK;
		}
		catch
		{
			return false;
		}
	}
}
