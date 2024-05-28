using Godot;

namespace Project;
public partial class CastRescue : BaseCast
{
	public CastRescue(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.AlliedUnit,
			TargetAlliances = new() { UnitAlliance.Player },
			CastTimings = BeatTime.Free,
			HoldTime = 0,
			RecastTime = 1,
		};
	}

	protected override void CastOnUnit(BaseUnit target)
	{
		// target.ForcefulMovement.Push(target.Position.FlatDistanceTo(Parent.Position), Parent.Position - target.Position, 1);
		// target.ForcefulMovement.Push(target.Position.FlatDistanceTo(Parent.Position), Parent.Position - target.Position, 1);
		target.ForcefulMovement.Push(target.Position.FlatDistanceTo(Parent.Position), Parent.Position - target.Position, 1);

	}
}