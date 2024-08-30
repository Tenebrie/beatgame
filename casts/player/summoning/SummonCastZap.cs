namespace Project;

public partial class SummonCastZap : BaseCast
{
	public const float Damage = 5;
	public SummonCastZap(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Zap",
			IconPath = "res://assets/icons/SpellBook06_50.PNG",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
		};
	}

	void SpawnZap(CastTargetData target)
	{
		this.CreateZapEffect(Parent.GlobalCastAimPosition, target.HostileUnit.GlobalCastAimPosition);
		target.HostileUnit.Health.Damage(Damage, this);
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		SpawnZap(target);
	}
}
