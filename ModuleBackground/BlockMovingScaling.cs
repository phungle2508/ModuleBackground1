using System;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class BlockMovingScaling : EntityJig
{
	public int mCurJigFactorNumber = 1;

	private Point3d mPosition;

	private double mScaleFactor;

	private double mRotation;

	private double mAngleOffset;

	public BlockMovingScaling(Entity ent)
		: base(ent)
	{
		ent.TransformBy(Application.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem);
		mAngleOffset = (ent as BlockReference).Rotation;
		mScaleFactor = (ent as BlockReference).ScaleFactors.X;
		double num = (double)(int)Math.Round(mScaleFactor, 0) * 1.0;
		mScaleFactor = num;
	}

	protected override bool Update()
	{
		switch (mCurJigFactorNumber)
		{
		case 1:
			(base.Entity as BlockReference).Position = mPosition;
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
			jigPromptPointOptions.UseBasePoint = true;
			jigPromptPointOptions.UserInputControls = (UserInputControls)4224;
			PromptPointResult promptPointResult = prompts.AcquirePoint(jigPromptPointOptions);
			if (promptPointResult.Status == PromptStatus.Cancel)
			{
				return SamplerStatus.Cancel;
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
			double value = promptDoubleResult.Value;
			double num = value / 100.0;
			mScaleFactor = num;
			return SamplerStatus.OK;
		}
		default:
			return SamplerStatus.OK;
		}
	}

	public static bool Jig(BlockReference ent)
	{
		try
		{
			Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
			BlockMovingScaling blockMovingScaling = new BlockMovingScaling(ent);
			PromptResult promptResult;
			do
			{
				promptResult = editor.Drag(blockMovingScaling);
			}
			while (promptResult.Status != PromptStatus.Cancel && promptResult.Status != PromptStatus.Error && blockMovingScaling.mCurJigFactorNumber++ < 2);
			double num = (double)(int)Math.Round(blockMovingScaling.mScaleFactor, 0) * 1.0;
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
			BlockMovingScaling blockMovingScaling = new BlockMovingScaling(ent);
			PromptResult promptResult;
			do
			{
				promptResult = editor.Drag(blockMovingScaling);
			}
			while (promptResult.Status != PromptStatus.Cancel && promptResult.Status != PromptStatus.Error && blockMovingScaling.mCurJigFactorNumber++ < 1);
			return promptResult.Status == PromptStatus.OK;
		}
		catch
		{
			return false;
		}
	}
}
