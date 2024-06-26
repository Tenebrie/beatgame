using Godot;
using System;
using System.Collections.Generic;

namespace Project;

public partial class CastBarGroup : VBoxContainer
{
	BaseUnit trackedUnit;
	readonly List<CastBarEntry> activeBars = new();

	public override void _Ready()
	{
		SignalBus.Singleton.CastStarted += OnCastStarted;

		foreach (var child in GetChildren())
			RemoveChild(child);
	}

	public override void _ExitTree()
	{
		SignalBus.Singleton.CastStarted -= OnCastStarted;
	}

	void OnCastStarted(BaseCast cast)
	{
		if (cast.Parent != trackedUnit || cast.Settings.InputType == CastInputType.Instant || cast.Settings.HiddenCastBar)
			return;

		var oldBar = activeBars.Find(entry => entry.cast == cast);
		if (oldBar != null)
			return;

		var newBar = Lib.LoadScene(Lib.UI.CastBar).Instantiate<CastBar>();
		AddChild(newBar);
		newBar.TrackCast(cast);
		newBar.Finished += () => OnCastBarFinished(newBar);
		activeBars.Add(new CastBarEntry()
		{
			cast = cast,
			bar = newBar,
		});
	}

	void OnCastBarFinished(CastBar castBar)
	{
		var entry = activeBars.Find(entry => entry.bar == castBar);
		if (entry == null)
			return;

		activeBars.Remove(entry);
	}

	public void TrackUnit(BaseUnit unit)
	{
		trackedUnit = unit;
		unit.UnitDestroyed += UntrackUnit;
	}

	public void UntrackUnit()
	{
		trackedUnit = null;

		foreach (var entry in activeBars)
			entry.bar.QueueFree();
		activeBars.Clear();
	}

	class CastBarEntry
	{
		public BaseCast cast;
		public CastBar bar;
	}
}
