using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

public class PlayerSpellcasting : ComposableScript
{
	private readonly Dictionary<string, BaseCast> CastBindings = new();

	new readonly PlayerController Parent;

	public PlayerSpellcasting(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public void Bind(string input, BaseCast cast)
	{
		CastBindings.Add(input, cast);
		Composables.Add(cast);
	}

	public override void _Input(InputEvent @input)
	{
		foreach (var key in CastBindings.Keys)
		{
			if (!@input.IsAction(key, true))
				continue;

			if (!@Input.IsActionJustPressed(key))
				continue;

			var binding = CastBindings.TryGetValue(key, out var cast);
			if (!binding)
				return;

			var isValid = cast.ValidateTarget(Parent.Targeting.targetedUnit, out var errorMessage);
			if (!isValid)
			{
				Debug.WriteLine(errorMessage);
				return;
			}

			cast.Cast(Parent.Targeting.targetedUnit);
		}
	}
}