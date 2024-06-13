using Godot;

namespace Project;

public partial class CastSentinel : BaseCast
{
	float DamageMitigation = 0.5f;
	float Duration = 4;

	public const float ManaCost = 85;

	public CastSentinel(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Sentinel",
			Description = MakeDescription(
				$"Prepare your defenses for an incoming attack.",
				$"For the next {Colors.Tag(Duration)} beats, you gain {Colors.Tag(DamageMitigation * 100 + "%")} damage reduction."
			),
			IconPath = "res://assets/icons/SpellBook06_78.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.Free,
			RecastTime = 16,
			Charges = 1,
		};

		if (this.HasSkill<SkillSentinelCharges>())
		{
			Settings.Charges = 2;
		}
		if (this.HasSkill<SkillSentinelMana>())
		{
			Settings.RecastTime = 1;
			Settings.ResourceCost[ObjectResourceType.Mana] = ManaCost;
		}
	}
}