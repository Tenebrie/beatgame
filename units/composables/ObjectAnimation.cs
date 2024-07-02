using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
namespace Project;

public partial class ObjectAnimation : ComposableScript
{
	[Signal] public delegate void AnimationRequestedEventHandler(StringName name);

	public ObjectAnimation(BaseUnit parent) : base(parent) { }

	public void Play(StringName animationName)
	{
		if (animationName == null)
			return;

		EmitSignal(SignalName.AnimationRequested, animationName);
	}
}
