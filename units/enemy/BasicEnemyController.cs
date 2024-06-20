namespace Project;

public partial class BasicEnemyController : BaseUnit
{
	public bool IsBoss = false;

	public BasicEnemyController()
	{
		Targetable.selectionRadius = 0.3f;
	}
}
