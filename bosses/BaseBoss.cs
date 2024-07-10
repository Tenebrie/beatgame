namespace Project;

public partial class BaseBoss : BasicEnemyController
{
	public BaseTimeline timeline;

	public override void _Ready()
	{
		IsBoss = true;
		Alliance = UnitAlliance.Hostile;

		base._Ready();
	}
}