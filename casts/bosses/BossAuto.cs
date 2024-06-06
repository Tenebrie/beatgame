using Godot;

namespace Project;
public partial class BossAuto : BaseCast
{
	public BossAuto(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Zap",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			HoldTime = 0,
			RecastTime = 0,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		this.CreateZapEffect(Parent.CastAimPosition, target.HostileUnit.CastAimPosition);
		target.HostileUnit.Health.Damage(1, this);
	}
}