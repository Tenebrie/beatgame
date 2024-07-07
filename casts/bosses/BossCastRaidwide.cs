using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastRaidwide : BaseCast
{
	float damageDealt = 0;
	float damageExpected = 0;
	protected float DamageTotal = 100;

	public BossCastRaidwide(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Raidwide",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			HoldTime = 2,
			PrepareTime = 6,
			RecastTime = 0,
		};
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (IsCasting && !IsPreparing)
			damageExpected += DamageTotal / Settings.HoldTime * (float)delta / Music.Singleton.SecondsPerBeat;
		if (damageExpected - damageDealt > 1)
			ApplyDamage();
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
	}

	protected override void OnCastCompleted(CastTargetData _) { }
}