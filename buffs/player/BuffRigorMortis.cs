namespace Project;

public partial class BuffRigorMortis : BaseBuff
{
	public BuffRigorMortis()
	{
		Settings = new()
		{
			FriendlyName = "Rigor Mortis",
			Description = MakeDescription("Unfortunately, you are afflicted by death. But don't worry, we can get you back up for just {{$0.99}}!"),
			IconPath = "res://assets/icons/SpellBook06_41.PNG",
			MaximumStacks = 1
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageDamageTaken[ObjectResourceType.Health] = 0;
		unit.PercentageResourceRegen[ObjectResourceType.Health] = 0;
		unit.PercentageResourceRegen[ObjectResourceType.Mana] = 0;
		unit.MoveSpeedPercentage = 0;
		unit.PercentageCCReduction = 1;
	}
}