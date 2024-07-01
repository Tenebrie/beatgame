using Godot;
using Project;
using System;
using System.Linq;

namespace Project;

public partial class SpinningDummy : BasicEnemyController
{
	CastDummyWhirlwind Whirlwind;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Whirlwind = CastLibrary.Register(new CastDummyWhirlwind(this));
	}

	protected override void ProcessBeatTick(BeatTime time)
	{
		if (time.HasNot(BeatTime.EveryFullBeat) || Whirlwind.IsCasting)
			return;

		var targets = new CastTargetData()
		{
			HostileUnit = PlayerController.AllPlayers.FirstOrDefault(),
		};
		Whirlwind.CastBegin(targets);
	}
}
