namespace Project;

public partial class BasicEnemyController : BaseUnit
{
	public BasicEnemyController()
	{
		Alliance = UnitAlliance.Enemy;
		Targetable.selectionRadius = 0.6f;
	}
}
