using System;
using Godot;

namespace Project;
public partial class CastSummonWisp : BaseCast
{
	double spawnAngleRadians = 0;

	public CastSummonWisp(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Summon Wisp",
			Description = MakeDescription(
				$"Summon an allied Wisp that circles the target, dealing periodic damage.",
				$"Deals {{{10}}} damage per beat over {{{4}}} beats."
			),
			LoreDescription = MakeDescription(
				$"Highly effective against demons."
			),
			IconPath = "res://assets/icons/SpellBook06_58.png",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			HoldTime = 1,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var wisp = Lib.LoadScene(Lib.Entity.SummonWisp).Instantiate<EntitySummonWisp>();
		wisp.TargetUnit = target.HostileUnit;
		wisp.PositionRadians = spawnAngleRadians;
		wisp.SourceCast = this;
		wisp.lifeDuration = 6;
		GetTree().CurrentScene.AddChild(wisp);
		wisp.GlobalPosition = Parent.GlobalCastAimPosition;

		// degToRad((360 / 36) * 11 + 3)
		// Random angle that will wrap back to 0 after 359 iterations
		spawnAngleRadians += 1.97222;
	}
}