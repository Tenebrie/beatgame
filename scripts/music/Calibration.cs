using System.Diagnostics;
using Godot;

namespace Project;

public partial class Calibration : Node
{
	public long LastBeatTime;
	public long LastPlayerInputTime = 0;
	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		// Music.Singleton.QuarterNoteTimer.Timeout += OnBeat;
		// Music.Singleton.EigthNoteTimer.Timeout += OnBeat;
	}

	public void OnBeat(BeatTime time)
	{
		if (LastPlayerInputTime > 0)
		{
			ProcessInput();
		}
		LastBeatTime = (long)Time.Singleton.GetTicksMsec();
	}

	public void ProcessInput()
	{
		var beatTime = (long)Time.Singleton.GetTicksMsec();
		if (beatTime - LastPlayerInputTime > LastPlayerInputTime - LastBeatTime)
		{
			Debug.WriteLine("Player input is " + (LastPlayerInputTime - LastBeatTime) + "ms late");
		}
		else
		{
			Debug.WriteLine("Player input is " + (beatTime - LastPlayerInputTime) + "ms early");
		}
		LastPlayerInputTime = 0;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("QuickCalibration".ToStringName()))
		{
			LastPlayerInputTime = (long)Time.Singleton.GetTicksMsec();
		}
	}

	private static Calibration instance = null;
	public static Calibration Singleton
	{
		get
		{
			return instance;
		}
	}
}