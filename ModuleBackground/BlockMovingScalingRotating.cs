using System;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class BlockMovingScalingRotating : EntityJig
{
	public int mCurJigFactorNumber = 1;

	private Point3d mPosition;

	private double mRotation;

	private double mScaleFactor;

	private double mAngleOffset;

	public BlockMovingScalingRotating(Entity ent)
		: base(ent)
	{
		ent.TransformBy(Application.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem);
		mAngleOffset = (ent as BlockReference).Rotation;
		mScaleFactor = (ent as BlockReference).ScaleFactors.X;
	}

	protected override bool Update()
	{
		switch (mCurJigFactorNumber)
		{
		case 1:
			(base.Entity as BlockReference).Position = mPosition;
			break;
		case 3:
			(base.Entity as BlockReference).Rotation = mAngleOffset + mRotation;
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
		case 3:
		{
			JigPromptAngleOptions jigPromptAngleOptions = new JigPromptAngleOptions("\nBlock rotation angle:");
			jigPromptAngleOptions.BasePoint = mPosition;
			jigPromptAngleOptions.UseBasePoint = true;
			PromptDoubleResult promptDoubleResult2 = prompts.AcquireAngle(jigPromptAngleOptions);
			if (promptDoubleResult2.Status == PromptStatus.Cancel)
			{
				return SamplerStatus.Cancel;
			}
			if (promptDoubleResult2.Value.Equals(mRotation))
			{
				return SamplerStatus.NoChange;
			}
			mRotation = promptDoubleResult2.Value;
			return SamplerStatus.OK;
		}
		case 2:
		{
			JigPromptDistanceOptions jigPromptDistanceOptions = new JigPromptDistanceOptions("\nBlock scale factor in percentage(1/100):");
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

	public static bool Jig(BlockReference ent)
	{
		try
		{
			Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
			BlockMovingScalingRotating blockMovingScalingRotating = new BlockMovingScalingRotating(ent);
			PromptResult promptResult;
			do
			{
				promptResult = editor.Drag(blockMovingScalingRotating);
			}
			while (promptResult.Status != PromptStatus.Cancel && promptResult.Status != PromptStatus.Error && blockMovingScalingRotating.mCurJigFactorNumber++ < 3);
			double num = (double)(int)Math.Round(blockMovingScalingRotating.mScaleFactor, 0) * 1.0;
			ent.ScaleFactors = new Scale3d(num, num, num);
			return promptResult.Status == PromptStatus.OK;
		}
		catch
		{
			return false;
		}
	}
}
