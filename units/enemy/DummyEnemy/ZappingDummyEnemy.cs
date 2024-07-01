using System;
using System.Linq;
using Godot;

namespace Project;

public partial class ZappingDummyEnemy : BasicEnemyController
{
	public float ZappingRadius = 2;

	CircularTelegraph circle;

	public override void _Ready()
	{
		FriendlyName = "Zapping Enemy";

		base._Ready();

		Health.SetBaseMaxValue(300);
		CastLibrary.Register(new BossAuto(this));
		this.CallDeferred(() =>
		{
			var area = this.CreateCircularTelegraph(this.SnapToGround(Position));
			area.Settings.GrowTime = 1.0f;
			area.Settings.Radius = ZappingRadius;
			area.Settings.Alliance = Alliance;
			area.Settings.Periodic = true;
			circle = area;
		});
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		circle.CleanUp();
	}

	protected override void ProcessBeatTick(BeatTime time)
	{
		if (time.HasNot(BeatTime.EveryFullBeat))
			return;

		var closestEnemy = AllUnits
			.Where(unit => unit.HostileTo(this))
			.Where(unit => unit.Position.DistanceSquaredTo(Position) <= ZappingRadius * ZappingRadius)
			.OrderBy(unit => unit.Position.DistanceSquaredTo(Position))
			.FirstOrDefault();

		if (closestEnemy == null)
			return;

		CastLibrary.Get<BossAuto>().CastBegin(new CastTargetData()
		{
			HostileUnit = closestEnemy,
		});
	}
}
