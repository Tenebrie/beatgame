using Godot;

namespace Project;
public partial class BossAuto : BaseCast
{
	public float Damage = 10;

	public BossAuto(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Zap",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			HoldTime = 0,
			RecastTime = 0,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		this.CreateZapEffect(Parent.GlobalCastAimPosition, target.HostileUnit.GlobalCastAimPosition);
		target.HostileUnit.Health.Damage(Damage, this);
	}
}