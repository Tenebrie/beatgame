namespace Project;
public enum UnitAlliance
{
	Player,
	Neutral,
	Hostile,
}

static class UnitAllianceExtensions
{
	public static bool HostileTo(this UnitAlliance alliance, UnitAlliance another)
	{
		return alliance != another && alliance != UnitAlliance.Neutral && another != UnitAlliance.Neutral;
	}

	public static bool HostileTo(this BaseUnit unit, BaseUnit another)
	{
		return unit.Alliance.HostileTo(another.Alliance);
	}

	public static bool HostileTo(this BaseUnit unit, UnitAlliance another)
	{
		return unit.Alliance.HostileTo(another);
	}
}