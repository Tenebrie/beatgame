using Godot;
using Project;
using System;

namespace Project;

public partial class CastBar : Control
{
	private bool IsActive;

	private ProgressBar Bar;

	public override void _Ready()
	{
		Bar = GetNode<ProgressBar>("ProgressBar");
		SignalBus.GetInstance(this).CastStarted += OnCastStarted;
	}

	public void OnCastStarted(BaseCast cast)
	{
		if (cast.Parent is not PlayerController)
			return;

		IsActive = true;
		Bar.Value = 0;
		Bar.MaxValue = cast.HoldTime * (1f / Music.Singleton.Bpm * 60);
	}

	public override void _Process(double delta)
	{
		if (!IsActive)
			return;

		Bar.Value += delta;
	}
}
