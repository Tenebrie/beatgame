namespace Project;

public partial class TestBoss : BasicEnemyController
{
	public BossAuto AutoAttack;
	public BossGroundAttack GroundAttack;
	public BossCastDeepGuardians DeepGuardians;
	public TestBoss()
	{
		AutoAttack = new(this);
		AddChild(AutoAttack);
		GroundAttack = new(this);
		AddChild(GroundAttack);
		DeepGuardians = new(this);
		AddChild(DeepGuardians);
		FriendlyName = "THE BOSS";
		Health.SetMax(10000);
	}
}
