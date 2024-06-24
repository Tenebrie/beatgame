using System;

namespace Project;

public partial class CastUnlimitedPower : BaseCast
{
	BaseUnit Target;
	float ZapsToSpawn = 0;

	public CastUnlimitedPower(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Unlimited Power",
			Description = MakeDescription($"Deal ~{{{GetZapsPerSecond() * 0.5f}}} damage per beat to the target enemy."),
			LoreDescription = "For those cases where just a zap is not enough.",
			IconPath = "res://assets/icons/SpellBook06_119.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Free,
			HoldTime = 8,
		};
	}

	float GetZapsPerSecond()
	{
		float zapsPerSecond = 50 * Music.Singleton.BeatsPerSecond;
		if (this.HasSkill<SecretSkillUnlimitedPower1>())
			zapsPerSecond *= 2;
		if (this.HasSkill<SecretSkillUnlimitedPower2>())
			zapsPerSecond *= 2;
		if (this.HasSkill<SecretSkillUnlimitedPower3>())
			zapsPerSecond *= 4;
		return zapsPerSecond;
	}

	void SpawnZap()
	{
		var zap = Lib.LoadScene(Lib.Effect.LightningZap).Instantiate<LightningZapEffect>();
		zap.Position = Parent.GlobalCastAimPosition;
		GetTree().CurrentScene.AddChild(zap);
		zap.SetTarget(Target.GlobalCastAimPosition);
		zap.FadeDuration = 0.50f;

		Target.Health.Damage(0.5f, this);
	}

	protected override void OnCastStarted(CastTargetData target)
	{
		Target = target.HostileUnit;
		ZapsToSpawn = 0;

		SpawnZap();
	}

	public override void _Process(double delta)
	{
		if (!IsCasting)
			return;
		if (Target == null || !IsInstanceValid(Target))
		{
			CastComplete();
			return;
		}

		float zapsPerSecond = GetZapsPerSecond();

		ZapsToSpawn += zapsPerSecond * (float)delta;

		var zapsToSpawnNow = (int)Math.Floor(ZapsToSpawn);

		for (var i = 0; i < zapsToSpawnNow; i++)
		{
			SpawnZap();
		}

		ZapsToSpawn -= zapsToSpawnNow;
	}
}
