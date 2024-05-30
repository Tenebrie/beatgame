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

	protected override void OnCastCompleted(CastTargetData target)
	{
		target.HostileUnit.ForcefulMovement.Push(target.HostileUnit.Position.FlatDistanceTo(Parent.Position), Parent.Position - target.HostileUnit.Position, 1);

	}
}