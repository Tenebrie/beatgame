using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public partial class PlayerSpellcasting : ComposableScript
{
	public readonly Dictionary<StringName, BaseCast> CastBindings = new(); // Dictionary<InputName, BaseCast>

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

	public override void _ExitTree()
	{
		base._ExitTree();
		SkillTreeManager.Singleton.SkillUp -= OnSkillUp;
		SkillTreeManager.Singleton.SkillDown -= OnSkillDown;
	}

	public BaseCast GetCurrentCastingSpell()
	{
		return CastBindings.FirstOrDefault((entry) => entry.Value.IsCasting).Value;
	}

	// Automatically bind a new cast to a first available slot
	void OnSkillUp(BaseSkill skill)
	{
		RebindAffectedCasts(skill);
		if (skill.Settings.ActiveCast == null)
			return;

		if (CastBindings.Values.Any(value => value.GetType() == skill.Settings.ActiveCast.CastType))
			return;

		foreach (var binding in CastBindingSlots)
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

		foreach (var binding in CastBindingSlots)
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

		if (!skill.Settings.RebindsAllCasts)
			return;

		var entries = CastBindings.ToList();
		foreach (var cast in entries)
			Bind(cast.Key, cast.Value.GetType());
	}

	public void RebindAll()
	{
		var array = CastBindings.ToArray();
		foreach (var cast in array)
			Bind(cast.Key, cast.Value.GetType());
	}

	public void Bind(StringName input, Type castType)
	{
		var factory = CastFactory.Of(castType);
		Bind(input, factory.Create(Parent));
	}

	public void Bind(StringName input, BaseCast cast)
	{
		Unbind(input);
		CastBindings.Add(input, cast);
		Parent.CastLibrary.Register(cast);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastAssigned, cast, input);
	}

	public void Unbind(StringName input)
	{
		var hasPreviousBinding = CastBindings.TryGetValue(input, out var existingCast);
		if (hasPreviousBinding)
		{
			if (existingCast.IsCasting)
				existingCast.CastFail();
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

	public void LoadBindings(Dictionary<StringName, BaseCast> dict)
	{
		foreach (var key in dict.Keys)
		{
			var hasValue = dict.TryGetValue(key, out var existingCast);
			if (!hasValue)
				continue;

			Bind(key, existingCast.GetType());
		}
	}

	public override void _Input(InputEvent @input)
	{
		if (SkillForestUI.Singleton.Visible || Parent.Buffs.Has<BuffRigorMortis>())
			return;

		var keys = CastBindings.Keys;
		for (var i = 0; i < keys.Count; i++)
		{
			var key = keys.ElementAt(i);
			if (!@input.IsAction(key, true))
				continue;

			var binding = CastBindings.TryGetValue(key, out var cast);
			if (!binding)
				return;

			if (@Input.IsActionJustPressed(key))
				CastInputPressed(cast);
			else if (@Input.IsActionJustReleased(key))
				CastInputReleased(cast);
		}
	}

	CastTargetData GetTargetData()
	{
		var alliedTarget = Parent.Targeting.targetedAlliedUnit ?? (PlayerController.AllPlayers.Count > 0 ? PlayerController.AllPlayers[0] : null);
		var hostileTarget = Parent.Targeting.targetedHostileUnit ?? BaseUnit.AllUnits.Where(unit => unit is BasicEnemyController enemy && enemy.IsBoss).FirstOrDefault();

		return new CastTargetData()
		{
			AlliedUnit = alliedTarget,
			HostileUnit = hostileTarget,
			Point = Parent.Position, // TODO: Implement ground targeting
		};
	}

	public void CastInputPressed(BaseCast cast)
	{
		if (cast.IsCasting)
			return;

		var targetData = GetTargetData();

		Parent.Movement.StopAutorun();
		if (cast.Settings.InputType != CastInputType.Instant || cast.Settings.GlobalCooldown)
			ReleaseCurrentCastingSpell();

		var canCast = cast.ValidateIfCastIsPossible(targetData, out var errorMessage);
		if (!canCast)
		{
			SignalBus.SendMessage(errorMessage);
			return;
		}

		cast.CastBegin(targetData);
	}

	public void CastInputReleased(BaseCast cast)
	{
		if (!cast.IsCasting)
			return;

		var targetData = GetTargetData();

		if (cast.Settings.InputType == CastInputType.HoldRelease)
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

	public void ReleaseCurrentCastingSpell()
	{
		var currentCastingSpell = GetCurrentCastingSpell();
		if (currentCastingSpell != null)
			CastRelease(currentCastingSpell);
	}

	public void TriggerGlobalCooldown()
	{
		foreach (var cast in CastBindings.Values.Where(cast => cast.Settings.GlobalCooldown))
		{
			cast.StartGlobalCooldown();
		}
	}

	private static void CastRelease(BaseCast cast)
	{
		if (cast.ValidateReleaseTiming())
			cast.CastComplete();
		else
			cast.CastFail();
	}

	static readonly List<StringName> bindings = new()
	{
		"Cast1".ToStringName(),
		"Cast2".ToStringName(),
		"Cast3".ToStringName(),
		"Cast4".ToStringName(),
		"ShiftCast1".ToStringName(),
		"ShiftCast2".ToStringName(),
		"ShiftCast3".ToStringName(),
		"ShiftCast4".ToStringName(),
	};
	public static List<StringName> CastBindingSlots => bindings;
}