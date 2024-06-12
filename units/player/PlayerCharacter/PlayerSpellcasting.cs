using System;
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

	public override void _Ready()
	{
		SkillTreeManager.Singleton.SkillUp += OnSkillUp;
		SkillTreeManager.Singleton.SkillDown += OnSkillDown;
	}

	public BaseCast GetCurrentCastingSpell()
	{
		var castingSpells = CastBindings.Values.Where(cast => cast.IsCasting).ToList();
		if (castingSpells.Count == 0)
			return null;
		return castingSpells[0];
	}

	// Automatically bind a new cast to a first available slot
	void OnSkillUp(BaseSkill skill)
	{
		RebindAffectedCasts(skill);
		if (skill.Settings.ActiveCast == null)
			return;

		var bindingSlots = GetPossibleBindings();
		if (CastBindings.Values.Any(value => value.GetType() == skill.Settings.ActiveCast.CastType))
			return;

		foreach (var binding in bindingSlots)
		{
			var hasValue = CastBindings.TryGetValue(binding, out var _);
			if (hasValue)
				continue;

			Bind(binding, skill.Settings.ActiveCast.Create(Parent));
			break;
		}
	}

	// Unbind cast if skill point has been removed
	void OnSkillDown(BaseSkill skill)
	{
		RebindAffectedCasts(skill);
		if (skill.Settings.ActiveCast == null)
			return;

		var bindingSlots = GetPossibleBindings();
		foreach (var binding in bindingSlots)
		{
			var hasValue = CastBindings.TryGetValue(binding, out var value);
			if (!hasValue || value.GetType() != skill.Settings.ActiveCast.CastType)
				continue;

			Unbind(binding);
			break;
		}
	}

	void RebindAffectedCasts(BaseSkill skill)
	{
		foreach (var cast in skill.Settings.AffectedCasts)
		{
			var boundCasts = CastBindings.Where(v => cast.CastType == v.Value.GetType()).ToList();
			if (boundCasts.Count == 0)
				continue;

			var boundCast = boundCasts[0];
			Bind(boundCast.Key, boundCast.Value.GetType());
		}
	}

	public void RebindAll()
	{
		var array = CastBindings.ToArray();
		foreach (var cast in array)
			Bind(cast.Key, cast.Value.GetType());
	}

	public void Bind(string input, Type castType)
	{
		var factory = CastFactory.Of(castType);
		Bind(input, factory.Create(Parent));
	}

	public void Bind(string input, BaseCast cast)
	{
		Unbind(input);
		CastBindings.Add(input, cast);
		Parent.CastLibrary.Register(cast);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastAssigned, cast, input);
	}

	public void Unbind(string input)
	{
		var hasPreviousBinding = CastBindings.TryGetValue(input, out var existingCast);
		if (hasPreviousBinding)
		{
			CastBindings.Remove(input);
			Parent.CastLibrary.Unregister(existingCast);
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastUnassigned, existingCast, input);
		}
	}

	public void UnbindAll(Type castType)
	{
		foreach (var key in CastBindings.Keys)
		{
			var hasValue = CastBindings.TryGetValue(key, out var existingCast);
			if (hasValue && existingCast.GetType() == castType)
				Unbind(key);
		}
	}

	public void LoadBindings(Dictionary<string, BaseCast> dict)
	{
		foreach (var key in dict.Keys)
		{
			var hasValue = dict.TryGetValue(key, out var existingCast);
			if (!hasValue)
				continue;

			Bind(key, existingCast.GetType());
		}
	}

	public BaseCast GetBinding(string input)
	{
		return CastBindings.GetValueOrDefault(input, null);
	}

	public override void _Input(InputEvent @input)
	{
		if (SkillForestUI.Singleton.Visible)
			return;

		foreach (var key in CastBindings.Keys)
		{
			if (!@input.IsAction(key, true))
				continue;

			var binding = CastBindings.TryGetValue(key, out var cast);
			if (!binding)
				return;

			var targetData = new CastTargetData()
			{
				AlliedUnit = Parent.Targeting.targetedUnit,
				HostileUnit = Parent.Targeting.targetedUnit,
				Point = Parent.Position, // TODO: Implement ground targeting
			};

			if (@Input.IsActionJustPressed(key))
			{
				Parent.Movement.StopAutorun();
				if (cast.Settings.InputType != CastInputType.Instant)
					ReleaseCurrentCastingSpell();

				var canCast = cast.ValidateIfCastIsPossible(targetData, out var errorMessage);
				if (!canCast)
				{
					SignalBus.SendMessage(errorMessage);
					return;
				}

				cast.CastBegin(targetData);
			}
			else if (@Input.IsActionJustReleased(key))
			{
				if (cast.Settings.InputType == CastInputType.HoldRelease && cast.IsCasting)
				{
					var isValidTarget = cast.ValidateTarget(targetData, out var errorMessage);
					if (!isValidTarget)
					{
						SignalBus.SendMessage(errorMessage);
						return;
					}

					CastRelease(cast);
				}
			}
		}
	}

	public void ReleaseCurrentCastingSpell()
	{
		var currentCastingSpell = GetCurrentCastingSpell();
		if (currentCastingSpell != null)
			CastRelease(currentCastingSpell);
	}

	private static void CastRelease(BaseCast cast)
	{
		if (cast.ValidateReleaseTiming())
			cast.CastPerform();
		else
			cast.CastFail();
	}

	public static List<string> GetPossibleBindings()
	{
		return new()
		{
			"Cast1",
			"Cast2",
			"Cast3",
			"Cast4",
			"ShiftCast1",
			"ShiftCast2",
			"ShiftCast3",
			"ShiftCast4",
		};
	}
}