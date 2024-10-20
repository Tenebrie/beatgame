using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastBuster : BaseCast
{
	public float Damage = 210;
	public BaseEffect effect;

	public BossCastBuster(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Buster",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			HoldTime = 8,
			PrepareTime = 0,
			RecastTime = 0,
		};
	}

	protected override void OnCastStarted(CastTargetData targetData)
	{
		targetData.HostileUnit.Buffs.Add(new BuffBusterTarget(this));
		effect = Lib.LoadScene(Lib.Effect.BusterTarget).Instantiate<SimpleParticleEffect>();
		effect.Attach(targetData.HostileUnit, targetData.HostileUnit.CastAimPosition);
	}

	protected override void OnCastCompleted(CastTargetData targetData)
	{
		targetData.HostileUnit.Buffs.RemoveAll<BuffBusterTarget>();
		targetData.HostileUnit.Health.Damage(Damage, this);
		effect.SetLifetime(0.01f);
	}
}