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

		var pos = Raycast.GetFirstHitPositionRelative(Parent, Vector3.Up * 5, Vector3.Zero, Raycast.Layer.Hoverable) + Vector3.Up * 0.15f;
		var text = new Label3D
		{
			Text = Parent.FriendlyName,
			Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
			Position = pos / Math.Min(Parent.Scale.X, Math.Min(Parent.Scale.Y, Parent.Scale.Z)),
			FixedSize = true,
			FontSize = 24,
			PixelSize = 0.0008f,
			// OutlineSize = 0,
			Modulate = CastUtils.GetAltAllianceColor(Parent.Alliance),
		};
		Parent.AddChild(text);
		text.SetDisableScale(true);
	}
}
