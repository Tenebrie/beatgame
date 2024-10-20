using Godot;

namespace Project;
public partial class CastSpiritwalkersGrace : BaseCast
{
	float duration = 32;
	public CastSpiritwalkersGrace(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Spiritwalker's Grace",
			Description = MakeDescription(
				$"Summon a burst of concentration allowing you to move while preparing your next spell.",
				$"For the next {duration} beats, you can cast while moving at {(this.HasSkill<SkillSpiritrunnersGrace>() ? "full" : "half")} speed.",
				$"\n\n((This cast does not interrupt other abilities.))"
			),
			IconPath = "res://assets/icons/SpellBook06_13.png",
			InputType = CastInputType.Instant,
			HoldTime = 0,
			RecastTime = 48,
			GlobalCooldown = GlobalCooldownMode.Ignore,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		Parent.Buffs.Add(new BuffCastWhileMoving()
		{
			Duration = duration,
		});
		if (!this.HasSkill<SkillSpiritrunnersGrace>())
		{
			Parent.Buffs.Add(new BuffHalfMoveSpeed()
			{
				Duration = duration
			});
		}
	}
}