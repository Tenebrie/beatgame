using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
namespace Project;

public class BaseCast : ComposableScript
{
	public Timer RecastTimerHandle;

	public CastTargetType TargetType = CastTargetType.None;
	public List<UnitAlliance> TargetAlliances = new() { UnitAlliance.Player, UnitAlliance.Neutral, UnitAlliance.Hostile };
	public float RecastTime = .25f;

	public BaseCast(BaseUnit parent) : base(parent) { }

	public override void _ExitTree()
	{
		base._ExitTree();
		Parent.GetTree().Root.RemoveChild(RecastTimerHandle);
	}

	private void EnsureTimerExists()
	{
		if (RecastTimerHandle != null)
			return;

		RecastTimerHandle = new Timer
		{
			OneShot = true
		};
		Parent.GetTree().Root.AddChild(RecastTimerHandle);
	}

	public bool ValidateTarget(BaseUnit target, out string errorMessage)
	{
		errorMessage = null;

		EnsureTimerExists();
		if (!RecastTimerHandle.IsStopped())
		{
			errorMessage = "Cooling down";
			return false;
		}

		if (TargetType == CastTargetType.Unit && target == null)
		{
			errorMessage = "Needs a target";
			return false;
		}
		else if (TargetType == CastTargetType.Unit && !TargetAlliances.Contains(target.Alliance))
		{
			errorMessage = "Can't target this unit";
			return false;
		}

		return true;
	}

	public void Cast(BaseUnit targetUnit)
	{
		RecastTimerHandle.Start(RecastTime);

		if (TargetType == CastTargetType.None)
			CastOnNone();

		if (TargetType == CastTargetType.Unit)
			CastOnUnit(targetUnit);
	}

	protected virtual void CastOnNone() { }
	protected virtual void CastOnUnit(BaseUnit target) { }
}