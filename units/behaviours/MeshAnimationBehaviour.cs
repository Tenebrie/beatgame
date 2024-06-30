using Godot;
using Project;
using System;

namespace Project;

/// <summary>
/// This behaviour will respond to animation requests by BaseCast and other classes.
/// It's suitable only for objects with an AnimationPlayer.
/// </summary>
public partial class MeshAnimationBehaviour : BaseBehaviour<BaseUnit>
{
	[Export] AnimationPlayer animationPlayer;

	public override void _Ready()
	{
	}
}
