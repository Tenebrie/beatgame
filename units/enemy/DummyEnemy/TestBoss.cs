namespace Project;

public partial class TestBoss : BasicEnemyController
{
	public BossAuto AutoAttack;
	public BossCastAreaAttack AreaAttack;
	public BossCastDeepGuardians DeepGuardians;
	public BossCastDeepGuardians DeepGuardiansTwo;
	public BossCastEliteGuardian EliteGuardian;
	public BossCastTridents AnimatedTridents;
	public BossCastLightningStorm LightningStorm;
	public BossCastTorrentialRain TorrentialRain;
	public BossCastRavagingWinds RavagingWinds;
	public BossCastConsumingWinds ConsumingWinds;
	public BossCastGeysers Geysers;
	public BossCastHardEnrage HardEnrage;
	public TestBoss()
	{
		AutoAttack = new(this);
		CastLibrary.Register(AutoAttack);

		AreaAttack = new(this);
		CastLibrary.Register(AreaAttack);

		DeepGuardians = new(this);
		DeepGuardiansTwo = new(this);
		CastLibrary.Register(DeepGuardians);
		CastLibrary.Register(DeepGuardiansTwo);

		EliteGuardian = new(this);
		CastLibrary.Register(EliteGuardian);

		AnimatedTridents = new(this);
		CastLibrary.Register(AnimatedTridents);

		LightningStorm = new(this);
		CastLibrary.Register(LightningStorm);

		TorrentialRain = new(this);
		CastLibrary.Register(TorrentialRain);

		RavagingWinds = new(this);
		CastLibrary.Register(RavagingWinds);

		ConsumingWinds = new(this);
		CastLibrary.Register(ConsumingWinds);

		Geysers = new(this);
		CastLibrary.Register(Geysers);

		HardEnrage = new(this);
		CastLibrary.Register(HardEnrage);

		FriendlyName = "THE BOSS";
		Health.SetMax(10000);
		Alliance = UnitAlliance.Hostile;
	}
}
