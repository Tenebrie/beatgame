namespace Project;

public partial class BasicEnemyController : BaseUnit
{
	public BasicEnemyController()
	{
		alliance = Alliance.Enemy;
		targeting.selectionRadius = 0.6f;
	}
}
