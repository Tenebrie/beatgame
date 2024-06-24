using Project;

namespace Project;

public partial class BuffPowerUpLightningOrb : BaseBuff
{
	public const float ExtraMoveSpeed = 0.1f;
	public BuffPowerUpLightningOrb()
	{
		Settings = new()
		{
			FriendlyName = "Power of the Storm",
			DynamicDesc = () => MakeDescription(
				$"Allows you to damage Power Pylon and increases your movement speed by {{{ExtraMoveSpeed * 100 * Stacks}%}}."
			),
			IconPath = "res://assets/icons/SpellBook06_119.PNG",
		};
	}
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MoveSpeedPercentage += ExtraMoveSpeed;
	}
}