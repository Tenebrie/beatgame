using Godot;
using System;

namespace Project;

public partial class LampBridgeGlowing : StaticBody3D
{
	[Export] public RigidBody3D LampRigidBody;

	public override void _Ready()
	{
		var lights = this.GetComponentsUncached<Light3D>();
		foreach (var light in lights)
		{
			var oldPos = light.GlobalPosition;
			RemoveChild(light);
			LampRigidBody.AddChild(light);
			light.GlobalPosition = oldPos;
		}
	}
}
