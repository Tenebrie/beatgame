using Godot;

namespace Project;
public partial class CastRescue : BaseCast
{
	public CastRescue(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Rescue",
			Description = MakeDescription(
				$"You won't leave an ally in danger if you can help it!",
				$"Pull a targeted ally to your position over the duration of the next {Colors.Tag(1)} beat.",
				$"\nTarget is also immune to other push effects for that duration."
			),
			IconPath = "res://assets/icons/SpellBook06_101.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.AlliedUnit,
			CastTimings = BeatTime.Free,
			HoldTime = 0,
			RecastTime = 8,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var targetUnit = target.AlliedUnit;
		targetUnit.ForcefulMovement.Push(targetUnit.Position.FlatDistanceTo(Parent.Position), Parent.Position - targetUnit.Position, 1);
		this.Log(targetUnit);
		this.Log((Parent.Position - targetUnit.Position).Length());
		this.Log(targetUnit.Position.FlatDistanceTo(Parent.Position));
		targetUnit.Buffs.Add(new BuffPushImmunity()
		{
			Duration = 1,
		});
	}
}