namespace Project;

public partial class BasicEnemyController : BaseUnit
{
	public bool IsBoss = false;

	public override void _Ready()
	{
		base._Ready();
		Targetable.SelectionRadius = 0.3f;
	}
}
