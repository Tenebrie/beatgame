using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastTorrentialRain : BaseCast
{
	int dropletsSpawned = 0;
	float dropletsExpected = 0;
	float damageDealt = 0;
	float damageExpected = 0;
	float damageTotal = 300;

	public BossCastTorrentialRain(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Torrential Rain",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			HoldTime = 6,
			PrepareTime = 2,
			RecastTime = 0,
		};
	}

	List<Vector3> GetSpawnPositions()
	{
		var list = new List<Vector3>();
		var targets = BaseUnit.AllUnits.Where(unit => unit.HostileTo(Parent)).ToList();
		foreach (var facing in CastUtils.AllArenaFacings())
		{
			list.Add(this.GetArenaEdgePosition(new Vector3(GD.Randf() * 2 - 1, 0, GD.Randf()), facing));
		}
		var target = targets[(int)(GD.Randi() % targets.Count)];
		list.Add(new Vector3(target.Position.X + GD.Randf() * 1 - 0.5f, 0, target.Position.Z + GD.Randf() * 1 - 0.5f));
		return list;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		ApplyDamage();
		if (!IsCasting || IsPreparing)
			return;

		dropletsExpected += 200 * (float)delta;
		var dropletsToSpawn = Math.Floor(dropletsExpected - dropletsSpawned);
		for (var i = 0; i < dropletsToSpawn; i += 5)
		{
			var positions = GetSpawnPositions();
			foreach (var pos in positions)
			{
				dropletsSpawned += 1;
				var area = this.CreateCircularTelegraph(pos);
				area.Settings.GrowTime = 0.5f;
				area.Settings.Radius = 0.4f;
			}
		}

		damageExpected += damageTotal / Settings.HoldTime * (float)delta / Music.Singleton.SecondsPerBeat;
		if (damageExpected - damageDealt > 1)
		{
			ApplyDamage();
		}
	}

	void ApplyDamage()
	{
		if (damageDealt == damageExpected)
			return;

		var targets = BaseUnit.AllUnits.Where(unit => unit.Alliance.HostileTo(Parent.Alliance));
		foreach (var target in targets)
		{
			target.Health.Damage(damageExpected - damageDealt, this);
		}
		damageDealt = damageExpected;
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		damageDealt = 0;
		damageExpected = 0;
		dropletsSpawned = 0;
		dropletsExpected = 0;
	}

	protected override void OnCastCompleted(CastTargetData _) { }
}