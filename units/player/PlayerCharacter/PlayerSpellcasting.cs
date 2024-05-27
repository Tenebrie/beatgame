using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public class PlayerSpellcasting : ComposableScript
{
	public readonly Dictionary<string, BaseCast> CastBindings = new(); // Dictionary<InputName, BaseCast>

	new readonly PlayerController Parent;

	public PlayerSpellcasting(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public BaseCast GetCurrentCastingSpell()
	{
		var castingSpells = CastBindings.Values.Where(cast => cast.IsCasting).ToList();
		if (castingSpells.Count == 0)
			return null;
		return castingSpells[0];
	}

	public void Bind(string input, BaseCast cast)
	{
		CastBindings.Add(input, cast);
		Parent.AddChild(cast);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastAssigned, cast, input);
	}

	public BaseCast GetBinding(string input)
	{
		return CastBindings.GetValueOrDefault(input, null);
	}

	public override void _Input(InputEvent @input)
	{
		var beatIndex = Music.Singleton.GetNearestBeatIndex();
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
				var isValidTiming = cast.ValidateCastTiming(out errorMessage);
				if (!isValidTiming)
				{
					Debug.WriteLine(errorMessage);
					return;
				}

				var currentCastingSpell = GetCurrentCastingSpell();
				if (currentCastingSpell != null)
				{
					CastRelease(currentCastingSpell, beatIndex);
				}

				var targetData = new CastTargetData()
				{
					HostileUnit = Parent.Targeting.targetedUnit,
					Point = Parent.Position, // TODO: Implement ground targeting
				};

				cast.CastBegin(targetData);
			}
			else if (@Input.IsActionJustReleased(key))
			{
				if (cast.Settings.InputType == CastInputType.HoldRelease && cast.IsCasting)
				{
					CastRelease(cast, beatIndex);
				}
			}
		}
	}

	private static void CastRelease(BaseCast cast, double beatIndex)
	{
		if (cast.ValidateReleaseTiming())
			cast.CastPerform();
		else
			cast.CastFail();
	}
}