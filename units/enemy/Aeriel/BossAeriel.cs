using Godot;

namespace Project;

public partial class BossAeriel : BasicEnemyController
{
	public BossAuto AutoAttack;
	public BossCastBuster Buster;
	public BossCastBuster MiniBuster;
	public BossCastRaidwide Raidwide;
	public BossCastAreaAttack AreaAttack;
	public BossCastDeepGuardians DeepGuardians;
	public BossCastDeepGuardians DeepGuardiansTwo;
	public BossCastEliteGuardian EliteGuardian;
	public BossCastTridents AnimatedTridents;
	public BossCastLightningStorm LightningStorm;
	public BossCastTorrentialRain TorrentialRain;
	public BossCastTorrentialRain OpeningTorrentialRain;
	public BossCastRavagingWinds RavagingWinds;
	public BossCastConsumingWinds ConsumingWinds;
	public BossCastConsumingWinds TwiceConsumingWinds;
	public BossCastConsumingWinds ThriceConsumingWinds;
	public BossCastGeysers Geysers;
	public BossCastLightningOrbs LightningOrbs;
	public BossCastHardEnrage HardEnrage;
	public BossAeriel()
	{
		AutoAttack = new(this);
		CastLibrary.Register(AutoAttack);

		Buster = new(this);
		CastLibrary.Register(Buster);

		MiniBuster = new(this);
		CastLibrary.Register(MiniBuster);
		MiniBuster.Settings.FriendlyName = "Mini Buster";
		MiniBuster.Damage = 100;

		Raidwide = new(this);
		CastLibrary.Register(Raidwide);

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

		OpeningTorrentialRain = new(this);
		CastLibrary.Register(OpeningTorrentialRain);
		OpeningTorrentialRain.Settings.HoldTime = 12;
		OpeningTorrentialRain.Settings.PrepareTime = 4;

		RavagingWinds = new(this);
		CastLibrary.Register(RavagingWinds);

		ConsumingWinds = new(this);
		CastLibrary.Register(ConsumingWinds);

		TwiceConsumingWinds = new(this);
		CastLibrary.Register(TwiceConsumingWinds);
		TwiceConsumingWinds.Settings.FriendlyName = "Twice Consuming Winds";
		TwiceConsumingWinds.AreaRadius += 2;
		TwiceConsumingWinds.ExtraPullStrength += 0.25f;

		ThriceConsumingWinds = new(this);
		CastLibrary.Register(ThriceConsumingWinds);
		ThriceConsumingWinds.Settings.FriendlyName = "Thrice Consuming Winds";
		ThriceConsumingWinds.AreaRadius += 4;
		ThriceConsumingWinds.ExtraPullStrength += 0.25f;
		ThriceConsumingWinds.Damage = 20;

		Geysers = new(this);
		CastLibrary.Register(Geysers);

		LightningOrbs = new(this);
		CastLibrary.Register(LightningOrbs);

		HardEnrage = new(this);
		CastLibrary.Register(HardEnrage);

		FriendlyName = "Aeriel, Eye of the Storm";
		Health.SetMax(10000);
		Alliance = UnitAlliance.Hostile;
	}

	public void ReleaseDarkness()
	{
		var darkness = GetNode<GpuParticles3D>("DarknessParticles");
		darkness.Emitting = false;
		var impact = Lib.Scene(Lib.Effect.AerielDarknessRelease).Instantiate<ProjectileImpact>();
		impact.Position = GetNode<GpuParticles3D>("CoreParticles").GlobalPosition;
		GetTree().CurrentScene.AddChild(impact);
	}

	public void Reset()
	{
		var darkness = GetNode<GpuParticles3D>("DarknessParticles");
		darkness.Emitting = true;
	}
}
