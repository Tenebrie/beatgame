using Godot;

namespace Project;
public partial class BossAuto : BaseCast
{
	public BossAuto(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			TargetAlliances = new() { UnitAlliance.Hostile },
			HoldTime = 0.5f,
			RecastTime = 0,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		target.HostileUnit.Health.Damage(1, Parent);
	}
}