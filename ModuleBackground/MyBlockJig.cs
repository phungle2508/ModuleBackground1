using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.GraphicsInterface;

namespace ModuleBackground;

public class MyBlockJig : DrawJig
{
	public Point3d _point;

	private ObjectId _blockId = ObjectId.Null;

	public PromptResult DragMe(ObjectId i_blockId, out Point3d o_pnt)
	{
		_blockId = i_blockId;
		Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
		PromptResult result = editor.Drag(this);
		o_pnt = _point;
		return result;
	}

	protected override SamplerStatus Sampler(JigPrompts prompts)
	{
		JigPromptPointOptions jigPromptPointOptions = new JigPromptPointOptions();
		jigPromptPointOptions.UserInputControls = (UserInputControls)130;
		jigPromptPointOptions.Message = "Select a point:";
		PromptPointResult promptPointResult = prompts.AcquirePoint(jigPromptPointOptions);
		Point3d value = promptPointResult.Value;
		if (value == _point)
		{
			return SamplerStatus.NoChange;
		}
		_point = value;
		if (promptPointResult.Status == PromptStatus.OK)
		{
			return SamplerStatus.OK;
		}
		return SamplerStatus.Cancel;
	}

	protected override bool WorldDraw(WorldDraw draw)
	{
		BlockReference blockReference = new BlockReference(_point, _blockId);
		draw.Geometry.Draw(blockReference);
		blockReference.Dispose();
		return true;
	}
}
