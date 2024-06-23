namespace Project;

public partial class BuffHealthRegen : BaseBuff
{
	public const float Regen = 1.0f;

	public BuffHealthRegen()
	{
		Settings = new()
		{
			FriendlyName = "Regeneration",
			Description = MakeDescription($"Increase your Health regeneration by {{{Regen * 100}%}}."),
			IconPath = "res://assets/icons/SpellBook06_07.png",
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageResourceRegen[ObjectResourceType.Health] += Regen;
	}
}