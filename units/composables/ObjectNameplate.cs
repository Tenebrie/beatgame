using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectNameplate : BaseBehaviour
{
	public override void _Ready()
	{
		if (Parent is PlayerController)
			return;

		var text = new Label3D
		{
			Text = Parent.FriendlyName,
			Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
			Position = Raycast.GetFirstHitPositionRelative(Parent, Vector3.Up * 5, Vector3.Zero, Raycast.Layer.Base) + Vector3.Up * 0.15f,
			FixedSize = true,
			FontSize = 24,
			PixelSize = 0.0008f,
			// OutlineSize = 0,
			Modulate = CastUtils.GetAltAllianceColor(Parent.Alliance),
		};
		Parent.AddChild(text);
	}
}
