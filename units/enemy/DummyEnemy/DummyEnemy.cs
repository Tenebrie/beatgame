using Godot;

namespace Project;

public partial class DummyEnemy : BasicEnemyController
{
	public override void _Ready()
	{
		base._Ready();
		FriendlyName = Alliance == UnitAlliance.Hostile ? "Dummy Enemy" : "Dummy Fren";
		Health.SetBaseMaxValue(300);
	}
}
