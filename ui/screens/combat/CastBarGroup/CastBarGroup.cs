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
		SignalBus.Singleton.CastPerformed += OnCastPerformed;

		foreach (var child in GetChildren())
			RemoveChild(child);
	}

	public override void _ExitTree()
	{
		SignalBus.Singleton.CastStarted -= OnCastStarted;
		SignalBus.Singleton.CastPerformed -= OnCastPerformed;
	}

	void OnCastStarted(BaseCast cast)
	{
		if (cast.Parent != trackedUnit || cast.Settings.InputType == CastInputType.Instant)
			return;

		var newBar = Lib.Scene(Lib.UI.CastBar).Instantiate<CastBar>();
		AddChild(newBar);
		newBar.TrackCast(cast);
		activeBars.Add(new CastBarEntry()
		{
			cast = cast,
			bar = newBar,
		});
	}

	void OnCastPerformed(BaseCast cast)
	{
		if (cast.Parent != trackedUnit)
			return;

		var entry = activeBars.Find(entry => entry.cast == cast);
		if (entry == null)
			return;

		entry.bar.QueueFree();
		activeBars.Remove(entry);
	}

	public void TrackUnit(BaseUnit unit)
	{
		trackedUnit = unit;
	}

	class CastBarEntry
	{
		public BaseCast cast;
		public CastBar bar;
	}
}
