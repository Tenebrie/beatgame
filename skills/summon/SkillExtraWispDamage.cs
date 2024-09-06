namespace Project;

public partial class SkillExtraWispDamage : BaseSkill
{
	public SkillExtraWispDamage()
	{
		Settings = new()
		{
			FriendlyName = "Extra Summon Power",
			IconPath = "res://assets/icons/SpellBook06_58.png",
			PassiveBuff = BuffFactory.Of<BuffExtraWispDamage>(),
		};
	}
}

public partial class BuffExtraWispDamage : BaseBuff
{
	const int PowerIncrease = 20;

	public BuffExtraWispDamage()
	{
		Settings = new()
		{
			Description = MakeDescription($"Increases your Summon Power by {{{PowerIncrease}}}."),
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.Stats[UnitStat.SummonPower] += PowerIncrease;
	}
}