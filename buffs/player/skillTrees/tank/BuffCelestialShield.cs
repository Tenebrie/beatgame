namespace Project;

public partial class BuffCelestialShield : BaseBuff
{
	public const float EffectDuration = 8;

	public BuffCelestialShield()
	{
		Settings = new()
		{
			FriendlyName = "Celestial Shield",
			Description = MakeDescription(
				$"You are completely invulnerable."
			),
			IconPath = "res://assets/icons/SpellBook06_12.png",
		};
		Duration = EffectDuration;
	}

	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.PercentageDamageTaken[ObjectResourceType.Health] = 0;
		visitor.PercentageDamageTaken[ObjectResourceType.Mana] = 0;
		visitor.PercentageCCReduction = 1;
	}
}