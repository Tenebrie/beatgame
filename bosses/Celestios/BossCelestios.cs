using Godot;
using System;

namespace Project;

public partial class BossCelestios : BaseBoss
{
	public BossAuto AutoAttack;

	public override void _Ready()
	{
		timeline = new BossCelestiosTimeline(this);
		FriendlyName = "Celestios, the Flaming Moon";

		base._Ready();

		Health.SetMaxValue(25000);
		Targetable.selectionRadius = 3;

		AutoAttack = new(this);
		CastLibrary.Register(AutoAttack);

		AddChild(new BossCelestiosTimeline(this));
	}
}
