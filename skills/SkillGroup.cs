namespace Project;

public enum SkillGroup : int
{
	TankDamage,
	TankSurvival,
	MagicalDamage,
	Healing,
	Summoning,
	Utility,
	Secret,
}

static class SkillGroupExtensions
{
	public static int ToVariant(this SkillGroup type)
	{
		return (int)type;
	}
}
