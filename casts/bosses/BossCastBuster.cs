using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastBuster : BaseCast
{
	public float Damage = 220;
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
	}

	protected override void OnCastCompleted(CastTargetData targetData)
	{
		targetData.HostileUnit.Buffs.RemoveAll<BuffBusterTarget>();
		targetData.HostileUnit.Health.Damage(220);
	}
}