namespace Project;

public partial class BasicEnemyController : BaseUnit
{
	public BasicEnemyController()
	{
		Alliance = UnitAlliance.Hostile;
		Targetable.selectionRadius = 0.6f;
	}
}
