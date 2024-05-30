namespace Project;

public partial class TestBoss : BasicEnemyController
{
	public BossAuto AutoAttack;
	public BossCastAreaAttack AreaAttack;
	public BossCastDeepGuardians DeepGuardians;
	public BossCastDeepGuardians DeepGuardiansTwo;
	public BossCastMidGuardians MidGuardians;
	public BossCastTridents AnimatedTridents;
	public BossCastLightningStorm LightningStorm;
	public BossCastTorrentialRain TorrentialRain;
	public BossCastRavagingWinds RavagingWinds;
	public BossCastConsumingWinds ConsumingWinds;
	public BossCastHardEnrage HardEnrage;
	public TestBoss()
	{
		AutoAttack = new(this);
		AddChild(AutoAttack);
		AreaAttack = new(this);
		AddChild(AreaAttack);
		DeepGuardians = new(this);
		DeepGuardiansTwo = new(this);
		AddChild(DeepGuardians);
		AddChild(DeepGuardiansTwo);
		MidGuardians = new(this);
		AddChild(MidGuardians);
		AnimatedTridents = new(this);
		AddChild(AnimatedTridents);
		LightningStorm = new(this);
		AddChild(LightningStorm);
		TorrentialRain = new(this);
		AddChild(TorrentialRain);
		RavagingWinds = new(this);
		AddChild(RavagingWinds);
		ConsumingWinds = new(this);
		AddChild(ConsumingWinds);
		HardEnrage = new(this);
		AddChild(HardEnrage);

		FriendlyName = "THE BOSS";
		Health.SetMax(10000);
		Alliance = UnitAlliance.Hostile;
	}
}
