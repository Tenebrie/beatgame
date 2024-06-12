using Godot;

namespace Project;
public partial class CastQuickDash : BaseCast
{
	float Force = 8;

	public CastQuickDash(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Quick Dash",
			Description = $"Evade danger or dive in close. With a burst of speed, you perform a quick dash forward.",
			IconPath = "res://assets/icons/SpellBook06_21.PNG",
			InputType = CastInputType.Instant,
			CastTimings = BeatTime.Free,
			RecastTime = 8,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 30;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		Parent.ForcefulMovement.AddInertia(Force, Parent.ForwardVector);
	}
}