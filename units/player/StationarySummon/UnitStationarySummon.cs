using Godot;
using System;
using System.Linq;

namespace Project;

public partial class UnitStationarySummon : BaseUnit
{
	int currentCastIndex = 0;
	public UnitHostility hostility;
	BaseUnit lastTarget;
	Node3D spinningGem;

	public UnitStationarySummon()
	{
		FriendlyName = "Stationary Ally";
	}

	public override void _Ready()
	{
		base._Ready();

		spinningGem = GetNode<Node3D>("SpinningGem");

		Music.Singleton.BeatTick += OnBeatTick;
		Targetable.selectionRadius = 0.3f;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Music.Singleton.BeatTick -= OnBeatTick;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		spinningGem.Rotate(Vector3.Up, (float)(Math.PI * delta));

		if (lastTarget == null || !IsInstanceValid(lastTarget))
			return;

		LookAt(lastTarget.GlobalCastAimPosition);
		Rotation = new Vector3(0, Rotation.Y, 0);
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

		if (casts.Count == 1)
			currentCastIndex = 0;
		else if (currentCastIndex >= casts.Count)
			currentCastIndex = 1;

		var targetCast = casts[currentCastIndex++];

		var isValidTarget = targetCast.ValidateTarget(data, out var _);
		if (!isValidTarget)
			return;

		targetCast.CastBegin(data);
		if (targetCast.Settings.TargetType == CastTargetType.HostileUnit)
			lastTarget = data.HostileUnit;
		else if (targetCast.Settings.TargetType == CastTargetType.AlliedUnit)
			lastTarget = data.AlliedUnit;
		else if (targetCast.Settings.TargetType == CastTargetType.None)
			lastTarget = null;
	}
}
