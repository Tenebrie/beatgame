using Godot;

namespace Project;
public partial class Fireball : BaseCast
{
	public const float Damage = 30;
	BaseUnit TwinFireballTarget;

	public Fireball(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Fireball",
			Description = $"Shoots out a fiery projectile that will always hit the target, dealing {Colors.Tag(Damage)} Fire damage.",
			IconPath = "res://assets/icons/SpellBook06_15.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			TickDuration = 0.5f,
			TickMode = CastTickMode.AlwaysResetOnCast,
			HoldTime = 1,
			RecastTime = 1,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 0;
		if (this.HasSkill<SkillIgnitingFireball>())
			Settings.ResourceCost[ObjectResourceType.Mana] += SkillIgnitingFireball.ExtraManaCost;
		if (this.HasSkill<SkillTwinFireball>())
			Settings.ResourceCost[ObjectResourceType.Mana] += SkillTwinFireball.ExtraManaCost;
		if (this.HasSkill<SkillFireballMastery>())
		{
			Settings.GlobalCooldown = GlobalCooldownMode.Ignore;
			Settings.HoldTime = 1;
			Settings.RecastTime = 1;
			Settings.InputType = CastInputType.Instant;
		}
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		if (this.HasSkill<SkillManaFrenzy>())
			Parent.Buffs.RefreshDuration<BuffManaFrenzy>();
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		SendFireballAt(target.HostileUnit);

		if (this.HasSkill<SkillTwinFireball>())
			TwinFireballTarget = target.HostileUnit;
		if (this.HasSkill<SkillManaFrenzy>())
			Parent.Buffs.Add(new BuffManaFrenzy());
	}

	void SendFireballAt(BaseUnit target)
	{
		var fireball = Lib.LoadScene(Lib.Effect.FireballProjectile).Instantiate() as Projectile;
		GetTree().Root.AddChild(fireball);
		fireball.ImpactEffect = Lib.Effect.FireballProjectileImpact;
		fireball.GlobalPosition = Parent.GlobalCastAimPosition;
		fireball.Source = this;
		fireball.TargetUnit = target;
		var damage = Flags.CastSuccessful ? Damage : 5;
		fireball.ImpactDamage = damage;
	}

	protected override void OnCastTicked(CastTargetData _)
	{
		if (TwinFireballTarget == null)
			return;

		if (!IsInstanceValid(TwinFireballTarget))
		{
			TwinFireballTarget = null;
			return;
		}

		SendFireballAt(TwinFireballTarget);
		TwinFireballTarget = null;
	}
}