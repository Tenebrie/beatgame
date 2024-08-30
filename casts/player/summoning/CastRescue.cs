using System;
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
			HoldTime = 0,
			RecastTime = 8,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var targetUnit = target.AlliedUnit;
		var vector = (Parent.Position - targetUnit.Position).Flatten(Math.Max(Parent.Position.Y, targetUnit.Position.Y));
		targetUnit.ForcefulMovement.Push(vector.Length(), vector, 1);
		targetUnit.Buffs.Add(new BuffPushImmunity()
		{
			Duration = 1,
		});
	}
}