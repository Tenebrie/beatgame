using System;
using Godot;

namespace Project;

public partial class CastImmovableObject : BaseCast
{
	public CastImmovableObject(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Immovable Object",
			Description = MakeDescription(
				$"Brace yourself firmly on the ground, ignoring any forceful movement or crowd control effects for the next",
				$"{{{BuffImmovableObject.EffectDuration}}} beats.",
				"However, you also lose the ability to move yourself for the duration."
			),
			IconPath = "res://assets/icons/SpellBook06_78.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.Free,
			RecastTime = 16,
			Charges = 1,
			GlobalCooldown = false,
		};
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		Parent.Buffs.Add(new BuffImmovableObject()
		{
			SourceCast = this,
		});
	}
}