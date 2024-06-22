using System;
using Godot;

namespace Project;
public partial class ShieldBash : BaseCast
{
	public const float InstantDamage = 5;
	public const float ExtraSkillDamage = 5;
	public const float IncreasedRange = 4.5f;

	float damage;
	bool mirrorNextCast = false;

	public ShieldBash(BaseUnit parent) : base(parent)
	{
		damage = InstantDamage;
		if (this.HasSkill<SkillShieldBashMulticast>())
			damage += ExtraSkillDamage;

		Settings = new()
		{
			FriendlyName = "Shield Bash",
			Description = MakeDescription(
				$"Throw an ephemeral shield at your enemy, dealing {{{damage}}} Spirit damage on impact."
			),
			IconPath = "res://assets/icons/SpellBook06_05.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.EveryFullBeat,
			MaximumRange = 1.5f,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 10;

		if (this.HasSkill<SkillShieldBashRange>())
		{
			Settings.MaximumRange = IncreasedRange;
		}
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		int shieldsToSpawn = this.HasSkill<SkillShieldBashMulticast>() ? 3 : 1;
		float rotation = 0;
		for (var i = 0; i < shieldsToSpawn; i++)
		{
			SpawnShield(target, rotation, applyDamage: i == 0);
			rotation += (float)Math.PI * 2f / shieldsToSpawn;
		}
	}

	void SpawnShield(CastTargetData target, float rotation, bool applyDamage)
	{
		var weapon = Lib.LoadScene(Lib.Effect.ShieldBashWeapon).Instantiate() as ShieldBashEffect;
		GetTree().CurrentScene.AddChild(weapon);
		Vector3 spawnPosition;
		if (this.HasSkill<SkillShieldBashRange>())
		{
			var dirVector = target.HostileUnit.ForwardVector;
			spawnPosition = target.HostileUnit.GlobalCastAimPosition + dirVector.Rotated(Vector3.Up, rotation) * 1.0f + new Vector3(0, 0.5f, 0);
			mirrorNextCast = !mirrorNextCast;
		}
		else
			spawnPosition = Parent.GlobalCastAimPosition + (target.HostileUnit.GlobalCastAimPosition - Parent.GlobalCastAimPosition).Normalized() * 0.25f;
		weapon.GlobalPosition = spawnPosition;
		weapon.LookAt(target.HostileUnit.GlobalCastAimPosition);
		weapon.DistanceToTarget = (weapon.GlobalPosition - target.HostileUnit.GlobalCastAimPosition).Length();
		if (!applyDamage)
			return;

		weapon.Impact += () =>
		{
			var enemy = target.HostileUnit;
			if (enemy == null || !IsInstanceValid(enemy))
				return;

			enemy.Health.Damage(damage, this);
			this.CreateSimpleParticleEffect(Lib.Effect.ShieldBashImpact, enemy.GlobalCastAimPosition).SetLifetime(0.1f);
		};
	}
}