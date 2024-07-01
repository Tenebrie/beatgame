using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
namespace Project;

public partial class ObjectAnimation : ComposableScript
{
	[Signal] public delegate void AnimationRequestedEventHandler(string name);

	public ObjectAnimation(BaseUnit parent) : base(parent) { }

	public void Play(string animationName)
	{
		EmitSignal(SignalName.AnimationRequested, animationName);
	}
}
