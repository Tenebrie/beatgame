namespace Project;

public partial class TestBoss : BasicEnemyController
{
	public BossAuto AutoAttack;
	public BossGroundAttack GroundAttack;
	public BossLargeCornerAttack LargeCornerAttack;
	public BossCastDeepGuardians DeepGuardians;
	public BossCastDeepGuardians DeepGuardiansTwo;
	public BossCastMidGuardians MidGuardians;
	public BossCastTridents AnimatedTridents;
	public BossCastTorrentialRain TorrentialRain;
	public TestBoss()
	{
		AutoAttack = new(this);
		AddChild(AutoAttack);
		GroundAttack = new(this);
		AddChild(GroundAttack);
		LargeCornerAttack = new(this);
		AddChild(LargeCornerAttack);
		DeepGuardians = new(this);
		DeepGuardiansTwo = new(this);
		AddChild(DeepGuardians);
		AddChild(DeepGuardiansTwo);
		MidGuardians = new(this);
		AddChild(MidGuardians);
		AnimatedTridents = new(this);
		AddChild(AnimatedTridents);
		TorrentialRain = new(this);
		AddChild(TorrentialRain);

		FriendlyName = "THE BOSS";
		Health.SetMax(10000);
		Alliance = UnitAlliance.Hostile;
	}
}
