namespace Project;

public partial class BuffPlus10ManaEfficiency : BaseBuff
{
	public BuffPlus10ManaEfficiency()
	{
		Settings = new()
		{
			Description = $"Reduces all Mana costs by {Colors.Tag("10%")}."
		};
	}
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.CastManaEfficiency += 0.1f;
	}
}