using System.Linq;
using Godot;

namespace Project;

public partial class TimelineManager : Node
{
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
		Music.Singleton.Start();
		EnvironmentController.Singleton.SetEnabled("bg_audio", false);

		var bosses = BaseUnit.AllUnits.Where(unit => unit is BossAeriel).Cast<BossAeriel>().ToList();
		if (bosses.Count > 0)
			AddChild(new BossAerielTimeline(bosses[0]));
	}

	public void ResetFight()
	{
		fightStarted = false;
		var transitionToScene = Lib.LoadScene(GetTree().CurrentScene.SceneFilePath);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionStarted, transitionToScene);
	}

	private static TimelineManager instance = null;
	public static TimelineManager Singleton
	{
		get => instance;
	}
}