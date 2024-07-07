using Godot;
using Project;
using System;
using System.Linq;

namespace Project;

public partial class SpinningDummy : BasicEnemyController
{
	[Export] AnimationTree animationTree;
	AnimationNodeStateMachinePlayback stateMachine;

	CastDummyWhirlwind Whirlwind;

	public override void _Ready()
	{
		FriendlyName = "Spinning Dummy";
		base._Ready();
		Whirlwind = CastLibrary.Register(new CastDummyWhirlwind(this));

		SignalBus.Singleton.CastStarted += OnCastStateChanged;
		SignalBus.Singleton.CastPrepared += OnCastStateChanged;
		SignalBus.Singleton.CastCompleted += OnCastStateChanged;

		stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		UpdateAnimationState();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		SignalBus.Singleton.CastStarted -= OnCastStateChanged;
		SignalBus.Singleton.CastPrepared -= OnCastStateChanged;
		SignalBus.Singleton.CastPrepared -= OnCastStateChanged;
	}

	void OnCastStateChanged(BaseCast cast)
	{
		if (cast.Parent != this)
			return;

		UpdateAnimationState();
	}

	void UpdateAnimationState()
	{
		var activeCasts = CastLibrary.GetActiveCasts();
		animationTree.Set("parameters/conditions/idle", activeCasts.Count == 0);
		animationTree.Set("parameters/conditions/whirlwind", Whirlwind.IsCasting);
	}

	protected override void ProcessBeatTick(BeatTime time)
	{
		if (time.HasNot(BeatTime.EveryFullBeat) || Whirlwind.IsCasting || Whirlwind.GetCooldownTimeLeft() > 0)
			return;

		var targets = new CastTargetData()
		{
			HostileUnit = PlayerController.AllPlayers.FirstOrDefault(),
		};
		Whirlwind.CastBegin(targets);
	}
}
