using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

internal class BlockJig : EntityJig
{
	private Point3d ptBase;

	private Point3d ptOrg;

	private Matrix3d mtMirror;

	private Matrix3d mtUcs;

	private bool bUpper;

	private double dAngle;

	public BlockJig(Matrix3d Mirror, Matrix3d ucs, Point3d org, double angle, BlockReference br)
		: base(br)
	{
		ptOrg = org;
		ptBase = br.Position;
		mtUcs = ucs;
		dAngle = angle;
		mtMirror = Mirror;
		bUpper = true;
	}

	protected override bool Update()
	{
		BlockReference blockReference = (BlockReference)base.Entity;
		bool flag = GlobalFunction.ptAboveLine(ptOrg, dAngle, ptBase);
		if (flag ^ bUpper)
		{
			bUpper = flag;
			blockReference.TransformBy(mtMirror);
		}
		return true;
	}

	protected override SamplerStatus Sampler(JigPrompts prompts)
	{
		JigPromptPointOptions jigPromptPointOptions = new JigPromptPointOptions("\nSelect Mirror:");
		jigPromptPointOptions.BasePoint = Point3d.Origin;
		jigPromptPointOptions.UserInputControls = UserInputControls.NoZeroResponseAccepted;
		PromptPointResult promptPointResult = prompts.AcquirePoint(jigPromptPointOptions);
		Point3d value = promptPointResult.Value;
		if (ptBase == value)
		{
			return SamplerStatus.NoChange;
		}
		ptBase = value;
		return SamplerStatus.OK;
	}

	public Point3d selectedPoint()
	{
		return ptBase;
	}
}
