namespace Project;

public partial class BuffGeyserLevitation : BaseBuff
{
	public BuffGeyserLevitation()
	{
		Settings = new()
		{
			FriendlyName = "Levitation",
			Description = "There is no gravity",
			IconPath = "res://assets/icons/SpellBook06_119.PNG",
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.GravityModifier = 0;
	}
}