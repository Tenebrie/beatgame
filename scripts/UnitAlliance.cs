namespace Project;

public enum UnitAlliance : int
{
	Player,
	Neutral,
	Hostile,
}

public enum TargetedUnitAlliance : int
{
	Player = UnitAlliance.Player,
	Hostile = UnitAlliance.Hostile,
}

public enum UnitHostility : int
{
	SameFaction,
	Neutral,
	Hostile,
}

static class UnitAllianceExtensions
{
	public static int ToVariant(this TargetedUnitAlliance type)
	{
		return (int)type;
	}

	public static bool EqualsTo(this TargetedUnitAlliance a1, UnitAlliance a2)
	{
		return (a1 == TargetedUnitAlliance.Player && a2 == UnitAlliance.Player) || (a1 == TargetedUnitAlliance.Hostile && a2 != UnitAlliance.Player);
	}

	public static bool EqualsTo(this UnitAlliance a1, TargetedUnitAlliance a2)
	{
		return a2.EqualsTo(a1);
	}

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