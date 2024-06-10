using Godot;

namespace Project;

public partial class CastSentinel : BaseCast
{
	float DamageMitigation = 0.5f;
	float Duration = 4;
	public CastSentinel(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Sentinel",
			Description = MakeDescription(
				$"Prepare your defenses for an incoming attack.",
				$"For the next {Colors.Tag(Duration)} beats, you gain {Colors.Tag(DamageMitigation * 100)}% damage reduction."
			),
			IconPath = "res://assets/icons/SpellBook06_78.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.Free,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 20;
	}
}