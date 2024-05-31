using Godot;

namespace Project;
public partial class CastRescue : BaseCast
{
	public CastRescue(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Rescue",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.AlliedUnit,
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