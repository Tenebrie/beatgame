using System;
using Godot;

namespace Project;
public partial class CastSummonWisp : BaseCast
{
	double spawnAngleRadians = 0;

	readonly UnitValue WispDamage = UnitValue.WispPower(5);
	readonly UnitValue WispLifetime = UnitValue.WispLifetime(3);

	public CastSummonWisp(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Summon Wisp",
			DynamicDesc = () => MakeDescription(
				$"Summon an allied Wisp that circles the target, dealing periodic damage.",
				$"Deals {{{WispDamage}}} damage every other beat. The wisp stays active for {{{WispLifetime}}} shots."
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
		wisp.Damage = Unwrap(WispDamage);
		wisp.lifeDuration = Unwrap(WispLifetime) * 2;
		GetTree().CurrentScene.AddChild(wisp);
		wisp.GlobalPosition = Parent.GlobalCastAimPosition;

		spawnAngleRadians += Math.PI * 2 / Unwrap(WispLifetime);
	}
}