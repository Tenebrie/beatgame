namespace Project;

public partial class TestBoss : BasicEnemyController
{
	public Fireball AutoAttack;
	public TestBoss()
	{
		AutoAttack = new(this);
		AddChild(AutoAttack);
		FriendlyName = "THE BOSS";
		Health = new ObjectResource(this, ObjectResourceType.Health, max: 10000);
	}

	public override void _Ready()
	{
		base._Ready();
	}
}
