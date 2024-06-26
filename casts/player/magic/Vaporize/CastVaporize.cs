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
				$"{{{BuffMinorIgnite.DamagePerBeat * IgniteStacks}}} damage per beat for {BuffMinorIgnite.BurnDuration} beats."
			),
			IconPath = "res://assets/icons/SpellBook06_23.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole,
			ChannelingTickTimings = BeatTime.EveryFullBeat,
			HoldTime = 8,
			RecastTime = 16,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 50;
	}

	protected override void OnCastStarted(CastTargetData targetData)
	{
		Parent.Buffs.RefreshDuration<BuffManaFrenzy>();
		var circle = this.CreateGroundCircularArea(targetData.HostileUnit.GetGroundedPosition());
		circle.Radius = 1;
		circle.Alliance = Parent.Alliance;
		circle.GrowTime = 8;
		circle.OnFinishedPerTargetCallback = (unit) =>
		{
			unit.Health.Damage(BaseDamage, this);
			for (var i = 0; i < IgniteStacks; i++)
				unit.Buffs.Add(new BuffMinorIgnite()
				{
					SourceCast = this,
				});
		};
	}

	protected override void OnCastTicked(CastTargetData _, BeatTime time)
	{
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