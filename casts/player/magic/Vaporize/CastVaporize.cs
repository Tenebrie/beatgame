using Godot;

namespace Project;
public partial class CastVaporize : BaseCast
{
	float Damage = 50;
	public CastVaporize(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Vaporize",
			Description = MakeDescription(
				$"Momentarily opens a rift to the plane of flame underneath the target, dealing {Colors.Tag(Damage)} Fire damage."
			),
			IconPath = "res://assets/icons/SpellBook06_23.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole,
			HoldTime = 8,
			RecastTime = 2,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 20;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		// Damage
	}
}