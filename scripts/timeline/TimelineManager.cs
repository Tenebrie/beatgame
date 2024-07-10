using System.Linq;
using Godot;

namespace Project;

public partial class TimelineManager : Node
{
	[Signal] public delegate void FightStartingEventHandler();

	bool fightStarted = false;

	public override void _EnterTree()
	{
		instance = this;
	}

	public bool IsFightingBoss()
	{
		return fightStarted;
	}

	public void StartFight()
	{
		if (fightStarted)
			return;

		fightStarted = true;
		EmitSignal(SignalName.FightStarting);
		Music.Singleton.Start();
		EnvironmentController.Singleton.SetEnabled("bg_audio", false);
	}

	public void ResetFight()
	{
		fightStarted = false;
		LoadingManager.Singleton.TransitionToCurrentScene();
	}

	public void StopFight()
	{
		fightStarted = false;
	}

	private static TimelineManager instance = null;
	public static TimelineManager Singleton
	{
		get => instance;
	}
}