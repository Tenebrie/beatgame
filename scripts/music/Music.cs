using Godot;

namespace Project;

public partial class Music : Node
{
	public float Bpm;
	public bool IsStarted = false;

	public AccurateTimer BeatTimer;

	private BeatTime BeatTimeState = BeatTime.Free;

	public override void _EnterTree()
	{
		instance = this;

		BeatTimer = new AccurateTimer();
		AddChild(BeatTimer);

		BeatTimer.BeatWindowUnlock += () => BeatTimeState |= BeatTime.One;
		BeatTimer.BeatWindowLock += () => BeatTimeState &= ~BeatTime.One;
	}

	public bool IsTimeOpen(BeatTime time)
	{
		return (BeatTimeState & time) > 0;
	}

	public override void _Ready()
	{
		Start();
	}

	public void Start()
	{
		Bpm = 120;
		IsStarted = true;
		BeatTimer.Start(Bpm);
	}

	private static Music instance = null;
	public static Music Singleton
	{
		get
		{
			return instance;
		}
	}
}