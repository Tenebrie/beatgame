using Godot;

namespace Project;
public partial class CastVaporize : BaseCast
{
	const float BaseDamage = 120;
	const int IgniteStacks = 3;

	public CastVaporize(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Vaporize",
			Description = MakeDescription(
				$"Momentarily opens a rift to the plane of flame underneath the target, dealing {{{BaseDamage}}} Fire damage to every enemy in proximity",
				$"and applying {{{IgniteStacks}}} stacks of {{Minor Ignite}}, dealing",
				$"{{{BuffMinorIgnite.DamagePerBeat * IgniteStacks}}} damage per beat for {{{BuffMinorIgnite.BurnDuration}}} beats."
			),
			IconPath = "res://assets/icons/SpellBook06_23.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			HoldTime = 4,
			RecastTime = 16,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 50;
	}

	protected override void OnCastStarted(CastTargetData targetData)
	{
		if (this.HasSkill<SkillManaFrenzy>())
			Parent.Buffs.RefreshDuration<BuffManaFrenzy>();
		var circle = this.CreateCircularTelegraph(targetData.HostileUnit.GetGroundedPosition());
		circle.Settings.Radius = 1;
		circle.Settings.Alliance = Parent.Alliance;
		circle.Settings.GrowTime = Settings.HoldTime;
		circle.Settings.OnFinishedPerTargetCallback = (unit) =>
		{
			unit.Health.Damage(BaseDamage, this);
			for (var i = 0; i < IgniteStacks; i++)
				unit.Buffs.Add(new BuffMinorIgnite()
				{
					SourceCast = this,
				});
		};
	}

	protected override void OnCastTicked(CastTargetData _)
	{
		if (this.HasSkill<SkillManaFrenzy>())
			Parent.Buffs.RefreshDuration<BuffManaFrenzy>();
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var effect = Lib.LoadScene(Lib.Effect.Vaporize).Instantiate<VaporizeEffect>();
		effect.Position = CastUtils.GetGroundedPosition(target.HostileUnit);
		GetTree().CurrentScene.AddChild(effect);

		if (this.HasSkill<SkillManaFrenzy>())
			Parent.Buffs.Add(new BuffManaFrenzy());
	}
}