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
	public BossCastTinyStorm TinyStorm;
	public BossCastLightningStorm LightningStorm;
	public BossCastTorrentialRain TorrentialRain;
	public BossCastTorrentialRain OpeningTorrentialRain;
	public BossCastRavagingWinds RavagingWinds;
	public BossCastQuickRavagingWinds QuickRavagingWinds;
	public BossCastConsumingWinds ConsumingWinds;
	public BossCastConsumingWinds TwiceConsumingWinds;
	public BossCastConsumingWinds ThriceConsumingWinds;
	public BossCastGeysers Geysers;
	public BossCastLightningOrbs LightningOrbs;
	public BossCastHardEnrage HardEnrage;

	public override void _Ready()
	{
		IsBoss = true;

		FriendlyName = "Aeriel, Eye of the Storm";
		Alliance = UnitAlliance.Hostile;

		base._Ready();

		Health.SetBaseMaxValue(10000);

		AutoAttack = new(this);
		CastLibrary.Register(AutoAttack);

		Buster = new(this);
		CastLibrary.Register(Buster);

		MiniBuster = new(this);
		CastLibrary.Register(MiniBuster);
		MiniBuster.Settings.FriendlyName = "Mini Buster";
		MiniBuster.Damage = 120;

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

		TinyStorm = new(this);
		CastLibrary.Register(TinyStorm);

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

		QuickRavagingWinds = new(this);
		CastLibrary.Register(QuickRavagingWinds);

		ConsumingWinds = new(this);
		CastLibrary.Register(ConsumingWinds);

		TwiceConsumingWinds = new(this);
		CastLibrary.Register(TwiceConsumingWinds);
		TwiceConsumingWinds.Settings.FriendlyName = "Twice Consuming Winds";

		ThriceConsumingWinds = new(this);
		CastLibrary.Register(ThriceConsumingWinds);
		ThriceConsumingWinds.Settings.FriendlyName = "All Consuming Winds";
		ThriceConsumingWinds.PushDistance = 32;
		ThriceConsumingWinds.AreaRadius += 12;
		ThriceConsumingWinds.Damage = 0;

		Geysers = new(this);
		CastLibrary.Register(Geysers);

		LightningOrbs = new(this);
		CastLibrary.Register(LightningOrbs);

		HardEnrage = new(this);
		CastLibrary.Register(HardEnrage);
	}

	public void ReleaseDarkness()
	{
		var darkness = GetNode<GpuParticles3D>("DarknessParticles");
		darkness.Emitting = false;
		var impact = Lib.LoadScene(Lib.Effect.AerielDarknessRelease).Instantiate<ProjectileImpact>();
		impact.Position = GetNode<GpuParticles3D>("CoreParticles").GlobalPosition;
		GetTree().CurrentScene.AddChild(impact);
	}

	public void Reset()
	{
		var darkness = GetNode<GpuParticles3D>("DarknessParticles");
		darkness.Emitting = true;
	}
}
