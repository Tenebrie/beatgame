using Godot;
using System;
using System.Linq;

namespace Project;

public partial class UnitStationarySummon : BaseUnit
{
	int currentCastIndex = 0;
	UnitHostility hostility;

	public UnitStationarySummon()
	{
		FriendlyName = "Stationary Ally";
	}

	public override void _Ready()
	{
		base._Ready();
		Music.Singleton.BeatTick += OnBeatTick;
		Targetable.selectionRadius = 0.3f;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Music.Singleton.BeatTick -= OnBeatTick;
	}

	void OnBeatTick(BeatTime time)
	{
		if (time != BeatTime.Whole)
			return;

		var hostileTargets = AllUnits.Where(unit => unit.HostileTo(this)).ToList();

		CastTargetData data = new()
		{
			HostileUnit = hostileTargets.Count > 0 ? hostileTargets[0] : null,
		};

		var casts = CastLibrary.Casts;

		if (casts.Count == 0)
			return;

		if (currentCastIndex >= casts.Count)
			currentCastIndex = 0;

		var targetCast = casts[currentCastIndex++];

		var isValidTarget = targetCast.ValidateTarget(data, out var _);
		if (!isValidTarget)
			return;

		targetCast.CastBegin(data);
	}
}
