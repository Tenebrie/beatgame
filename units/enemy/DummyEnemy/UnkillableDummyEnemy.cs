using Godot;

namespace Project;

public partial class UnkillableDummyEnemy : BasicEnemyController
{
	public override void _Ready()
	{
		FriendlyName = "Unkillable Dummy";
		base._Ready();
		Health.SetBaseMaxValue(300);
	}

	protected override void HandleDeath()
	{
		// Nope.
	}
}
