using Godot;
using Project;
using System;

public partial class CastBar : Control
{
	ProgressBar progressBar;
	Label label;

	BaseCast trackedCast;
	float castStartedAt;
	float castFinishesAt;
	float prepFinishesAt;

	public override void _Ready()
	{
		SignalBus.Singleton.CastStarted += OnCastStarted;
		SignalBus.Singleton.CastPerformed += OnCastPerformed;

		progressBar = GetNode<ProgressBar>("ProgressBar");
		label = GetNode<Label>("Label");
	}

	public override void _ExitTree()
	{
		SignalBus.Singleton.CastStarted -= OnCastStarted;
		SignalBus.Singleton.CastPerformed -= OnCastPerformed;
	}

	public override void _Process(double delta)
	{
		if (trackedCast == null || !trackedCast.IsCasting)
			return;

		UpdateBar();
	}

	void OnCastStarted(BaseCast cast)
	{
		if (cast != trackedCast)
			return;

		castStartedAt = Time.GetTicksMsec();
		prepFinishesAt = castStartedAt + cast.Settings.PrepareTime * Music.Singleton.SecondsPerBeat * 1000;
		castFinishesAt = prepFinishesAt + cast.Settings.HoldTime * Music.Singleton.SecondsPerBeat * 1000;
		UpdateBar();
	}

	void UpdateBar()
	{
		var time = (float)Time.GetTicksMsec();
		var value = (time - prepFinishesAt) / (castFinishesAt - prepFinishesAt);
		if (time < prepFinishesAt)
			value = (time - castStartedAt) / (prepFinishesAt - castStartedAt);

		if (trackedCast.Settings.PrepareTime > 0 && time > prepFinishesAt)
			value = 1 - value;

		if (trackedCast.Settings.ReversedCastBar)
			value = 1 - value;

		progressBar.Value = value * 100;
	}

	void OnCastPerformed(BaseCast cast)
	{
		if (cast != trackedCast)
			return;
	}

	public void TrackCast(BaseCast cast)
	{
		trackedCast = cast;
		label.Text = trackedCast.Settings.FriendlyName;
		if (cast.IsCasting)
		{
			OnCastStarted(cast);
		}
	}
}
