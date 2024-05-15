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
}