using Godot;

namespace Project;

public partial class DummyEnemy : BasicEnemyController
{
	public DummyEnemy()
	{
		FriendlyName = Alliance == UnitAlliance.Hostile ? "Dummy Enemy" : "Dummy Fren";
		Health.SetBaseMaxValue(300);
	}
}
