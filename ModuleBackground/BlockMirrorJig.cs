using System;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class BlockMirrorJig : EntityJig
{
	private Point3d _position;

	private double _angle;

	public int mCurJigFactorNumber = 1;

	private Vector2d vec = new Vector2d(1.0, 0.0);

	private Matrix3d mLastMat = Matrix3d.Identity;

	public BlockMirrorJig(Entity ent)
		: base(ent)
	{
		_angle = 0.0;
		ent.TransformBy(Application.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem);
	}

	protected override SamplerStatus Sampler(JigPrompts jp)
	{
		switch (mCurJigFactorNumber)
		{
		case 1:
		{
			JigPromptPointOptions jigPromptPointOptions = new JigPromptPointOptions("\nPosition of Block");
			jigPromptPointOptions.UserInputControls = (UserInputControls)195;
			jigPromptPointOptions.SetMessageAndKeywords("\nSpecify position of Block or [Rotate90(R)]: ", "R");
			PromptPointResult promptPointResult = jp.AcquirePoint(jigPromptPointOptions);
			if (promptPointResult.Status == PromptStatus.Keyword)
			{
				switch (promptPointResult.StringResult)
				{
				case "R":
				case "r":
					_angle += Math.PI / 2.0;
					while (_angle > Math.PI * 2.0)
					{
						_angle -= Math.PI * 2.0;
					}
					break;
				}
				return SamplerStatus.OK;
			}
			if (promptPointResult.Status == PromptStatus.OK)
			{
				if (_position.DistanceTo(promptPointResult.Value) < Tolerance.Global.EqualPoint)
				{
					return SamplerStatus.NoChange;
				}
				_position = promptPointResult.Value;
				return SamplerStatus.OK;
			}
			return SamplerStatus.Cancel;
		}
		case 2:
		{
			JigPromptStringOptions jigPromptStringOptions = new JigPromptStringOptions();
			jigPromptStringOptions.UserInputControls = (UserInputControls)5251;
			PromptResult promptResult = jp.AcquireString(jigPromptStringOptions);
			if (promptResult.Status == PromptStatus.Cancel)
			{
				return SamplerStatus.Cancel;
			}
			if (_angle == 0.0 || _angle == Math.PI)
			{
				vec = new Vector2d(0.0, 1.0);
			}
			return SamplerStatus.OK;
		}
		default:
			return SamplerStatus.OK;
		}
	}

	protected override bool Update()
	{
		BlockReference blockReference = (BlockReference)base.Entity;
		switch (mCurJigFactorNumber)
		{
		case 1:
			blockReference.Position = _position;
			blockReference.Rotation = _angle;
			break;
		case 2:
		{
			Matrix3d transform = Matrix3d.Mirroring(new Line3d(_position, _position + new Vector3d(vec.X, vec.Y, 0.0)));
			base.Entity.TransformBy(transform);
			break;
		}
		default:
			return false;
		}
		return true;
	}

	public static bool Jig(BlockReference ent)
	{
		try
		{
			Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
			BlockMirrorJig blockMirrorJig = new BlockMirrorJig(ent);
			PromptResult promptResult;
			do
			{
				promptResult = editor.Drag(blockMirrorJig);
				if (blockMirrorJig.mCurJigFactorNumber == 1)
				{
					PromptStatus status = promptResult.Status;
					if (status == PromptStatus.Keyword)
					{
						blockMirrorJig.mCurJigFactorNumber = 0;
					}
				}
			}
			while (promptResult.Status != PromptStatus.Cancel && promptResult.Status != PromptStatus.Error && blockMirrorJig.mCurJigFactorNumber++ < 1);
			return promptResult.Status == PromptStatus.OK;
		}
		catch
		{
			return false;
		}
	}
}
