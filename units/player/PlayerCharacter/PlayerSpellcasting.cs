using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

public class PlayerSpellcasting : ComposableScript
{
	public readonly Dictionary<string, BaseCast> CastBindings = new();

	new readonly PlayerController Parent;

	public PlayerSpellcasting(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public void Bind(string input, BaseCast cast)
	{
		CastBindings.Add(input, cast);
		Parent.AddChild(cast);
	}

	public override void _Input(InputEvent @input)
	{
		foreach (var key in CastBindings.Keys)
		{
			if (!@input.IsAction(key, true))
				continue;

			var binding = CastBindings.TryGetValue(key, out var cast);
			if (!binding)
				return;

			var isValidTarget = cast.ValidateTarget(Parent.Targeting.targetedUnit, out var errorMessage);
			if (!isValidTarget)
			{
				Debug.WriteLine(errorMessage);
				return;
			}

			if (@Input.IsActionJustPressed(key))
			{
				var isValidTiming = cast.ValidateTiming(out errorMessage);
				if (!isValidTiming)
				{
					Debug.WriteLine(errorMessage);
					return;
				}

				if (cast.InputType == CastInputType.Instant)
					cast.CastPerform(Parent.Targeting.targetedUnit, true);
				else if (cast.InputType == CastInputType.HoldRelease)
					cast.CastBegin();
			}
			else if (@Input.IsActionJustReleased(key))
			{
				var isValidTiming = cast.ValidateTiming(out _);

				if (cast.InputType == CastInputType.HoldRelease && cast.IsCasting)
					cast.CastPerform(Parent.Targeting.targetedUnit, isValidTiming);
			}
		}
	}
}