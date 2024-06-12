using Godot;

namespace Project;
public partial class CastSpiritwalkersGrace : BaseCast
{
	float duration = 16;
	public CastSpiritwalkersGrace(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Spiritwalker's Grace",
			Description = MakeDescription(
				$"Summon a burst of concentration allowing you to move while preparing your next spell.",
				$"For the next {duration} beats, you can cast while moving at {(this.HasSkill<SkillSpiritrunnersGrace>() ? "full" : "half")} speed."
			),
			IconPath = "res://assets/icons/SpellBook06_13.png",
			InputType = CastInputType.Instant,
			CastTimings = BeatTime.Free,
			HoldTime = 0,
			RecastTime = 64,
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