using System;
using Godot;

namespace Project;
public partial class ShieldBash : BaseCast
{
	public const float InstantDamage = 15;
	public const float ExtraSkillDamage = 20;
	public const float IncreasedRange = 9.0f;

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
				$"Throw an ephemeral shield at your enemy, dealing {{{damage}}} Spirit damage on impact.",
				$"You gain {{Thorned Shield}}, returning {{{Math.Round(Buff.RetaliateDamageFraction * 100)}%}} of all damage taken back to the enemy.",
				$"Stacks up to {{{Buff.MaxStacks}}} times.",
				$"\n\n((Retaliation damage is a reaction, and will not trigger other reactions.))"
			),
			IconPath = "res://assets/icons/SpellBook06_05.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.EveryFullBeat,
			MaximumRange = 3.0f,
		};

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
		Parent.Buffs.Add(new Buff());
	}

	void SpawnShield(CastTargetData target, float rotation, bool applyDamage)
	{
		var weapon = Lib.LoadScene(Lib.Effect.ShieldBashWeapon).Instantiate() as ShieldBashEffect;
		target.HostileUnit.AddChild(weapon);
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

	public partial class Buff : BaseBuff
	{
		public const float RetaliateDamageFraction = 0.15f;
		public const float EffectDuration = 8;
		public const int MaxStacks = 5;

		public Buff()
		{
			Settings = new()
			{
				FriendlyName = "Thorned Shield",
				DynamicDesc = () => MakeDescription(
					$"Whenever you take damage, deal {{{Math.Round(RetaliateDamageFraction * Stacks * 100) + "%"}}} of it back to the attacker."
				),
				IconPath = "res://assets/icons/SpellBook06_05.png",
				RefreshOthersWhenAdded = true,
				MaximumStacks = MaxStacks,
			};
			Duration = EffectDuration;
		}

		public override void ModifyUnit(BuffUnitStatsVisitor unit)
		{
			unit.RetaliateDamageFraction += RetaliateDamageFraction;
		}
	}
}