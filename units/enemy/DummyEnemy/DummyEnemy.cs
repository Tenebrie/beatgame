using Godot;

namespace Project;

public partial class DummyEnemy : BasicEnemyController
{
	public override void _Ready()
	{
		FriendlyName = Alliance == UnitAlliance.Hostile ? "Dummy Enemy" : "Dummy Fren";
		base._Ready();
		Health.SetBaseMaxValue(300);
	}
}
