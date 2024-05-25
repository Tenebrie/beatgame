namespace Project;

public partial class TestBoss : BasicEnemyController
{
	public BossGroundAttack AutoAttack;
	public TestBoss()
	{
		AutoAttack = new(this);
		AddChild(AutoAttack);
		FriendlyName = "THE BOSS";
		Health.SetMax(10000);
	}
}
