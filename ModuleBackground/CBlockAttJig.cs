using System.Collections.Generic;
using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace ModuleBackground;

public class CBlockAttJig : EntityJig
{
	private Matrix3d _ucs;

	private Point3d _pos;

	private Dictionary<ObjectId, ObjectId> _atts;

	private Transaction _tr;

	public CBlockAttJig(Matrix3d ucs, Transaction tr, BlockReference br, Dictionary<ObjectId, ObjectId> atts)
		: base(br)
	{
		_ucs = ucs;
		_pos = br.Position;
		_atts = atts;
		_tr = tr;
	}

	protected override bool Update()
	{
		BlockReference blockReference = (BlockReference)base.Entity;
		blockReference.Position = _pos.TransformBy(_ucs);
		if (blockReference.AttributeCollection.Count > 0)
		{
			foreach (ObjectId item in blockReference.AttributeCollection)
			{
				DBObject dBObject = _tr.GetObject(item, OpenMode.ForRead);
				AttributeReference attributeReference = dBObject as AttributeReference;
				if (attributeReference != null)
				{
					attributeReference.UpgradeOpen();
					ObjectId id2 = _atts[attributeReference.ObjectId];
					DBObject dBObject2 = _tr.GetObject(id2, OpenMode.ForRead);
					AttributeDefinition definition = (AttributeDefinition)dBObject2;
					attributeReference.SetAttributeFromBlock(definition, blockReference.BlockTransform);
					attributeReference.AdjustAlignment(blockReference.Database);
				}
			}
		}
		return true;
	}

	protected override SamplerStatus Sampler(JigPrompts prompts)
	{
		JigPromptPointOptions jigPromptPointOptions = new JigPromptPointOptions("\nSelect insertion point:");
		jigPromptPointOptions.BasePoint = Point3d.Origin;
		jigPromptPointOptions.UserInputControls = UserInputControls.NoZeroResponseAccepted;
		PromptPointResult promptPointResult = prompts.AcquirePoint(jigPromptPointOptions);
		Point3d point3d = promptPointResult.Value.TransformBy(_ucs.Inverse());
		if (_pos == point3d)
		{
			return SamplerStatus.NoChange;
		}
		_pos = point3d;
		return SamplerStatus.OK;
	}

	public PromptStatus Run()
	{
		Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
		if (mdiActiveDocument == null)
		{
			return PromptStatus.Error;
		}
		return mdiActiveDocument.Editor.Drag(this).Status;
	}
}
