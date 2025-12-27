using System;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class TextPlacementJig : EntityJig
{
	private Point3d _position;

	private double _angle;

	private double _txtSize;

	public TextPlacementJig(Entity ent, double dHieght = 125.0)
		: base(ent)
	{
		_angle = 0.0;
		_txtSize = dHieght;
	}

	protected override SamplerStatus Sampler(JigPrompts jp)
	{
		JigPromptPointOptions jigPromptPointOptions = new JigPromptPointOptions("\nPosition of text");
		jigPromptPointOptions.UserInputControls = (UserInputControls)195;
		jigPromptPointOptions.SetMessageAndKeywords("\nSpecify position of text or [Rotate90(R)]: ", "R");
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

	protected override bool Update()
	{
		DBText dBText = (DBText)base.Entity;
		dBText.Position = _position;
		dBText.Height = _txtSize;
		dBText.Rotation = _angle;
		return true;
	}
}
