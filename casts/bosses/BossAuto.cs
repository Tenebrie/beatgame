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
			HoldTime = 1,
			RecastTime = 0,
		};
	}

	protected override void CastOnUnit(BaseUnit target)
	{
		target.Health.Damage(5, Parent);
	}
}