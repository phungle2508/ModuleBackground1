using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class RotateJig : EntityJig
{
	private double m_baseAngle;

	private double m_deltaAngle;

	private Point3d m_rotationPoint;

	private Matrix3d m_ucs;

	public RotateJig(Entity ent, Point3d rotationPoint, double baseAngle, Matrix3d ucs)
		: base(ent.Clone() as Entity)
	{
		m_rotationPoint = rotationPoint;
		m_baseAngle = baseAngle;
		m_ucs = ucs;
	}

	protected override SamplerStatus Sampler(JigPrompts jp)
	{
		JigPromptAngleOptions jigPromptAngleOptions = new JigPromptAngleOptions("\nAngle of rotation: ");
		jigPromptAngleOptions.DefaultValue = 0.0;
		jigPromptAngleOptions.UseBasePoint = true;
		jigPromptAngleOptions.BasePoint = m_rotationPoint;
		jigPromptAngleOptions.Cursor = CursorType.RubberBand;
		jigPromptAngleOptions.UserInputControls = (UserInputControls)130;
		PromptDoubleResult promptDoubleResult = jp.AcquireAngle(jigPromptAngleOptions);
		if (promptDoubleResult.Status == PromptStatus.OK)
		{
			if (m_baseAngle == promptDoubleResult.Value)
			{
				return SamplerStatus.NoChange;
			}
			m_deltaAngle = promptDoubleResult.Value;
			return SamplerStatus.OK;
		}
		return SamplerStatus.Cancel;
	}

	protected override bool Update()
	{
		if (m_deltaAngle > Tolerance.Global.EqualPoint)
		{
			Matrix3d transform = Matrix3d.Rotation(m_deltaAngle - m_baseAngle, m_ucs.CoordinateSystem3d.Zaxis, m_rotationPoint);
			base.Entity.TransformBy(transform);
			m_baseAngle = m_deltaAngle;
			m_deltaAngle = 0.0;
		}
		return true;
	}

	public Entity GetEntity()
	{
		return base.Entity;
	}

	public double GetRotation()
	{
		if (m_deltaAngle == 0.0)
		{
			return m_deltaAngle;
		}
		return m_baseAngle + m_deltaAngle;
	}
}
