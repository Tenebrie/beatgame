namespace Project;

public partial class UnitValue
{
	public static UnitValue WispPower(int baseValue)
	{
		return new UnitValue(baseValue,
			(UnitStat.Power, 1),
			(UnitStat.SummonPower, 0.5f)
		);
	}

	public static UnitValue WispLifetime(int baseValue)
	{
		return new UnitValue(baseValue,
			(UnitStat.SummonEfficiency, 1)
		);
	}
}