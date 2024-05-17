using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public partial class Music : Node
{
	public readonly long SongDelay = 3000; // ms

	public float Bpm;
	public bool IsStarted = false;

	public AccurateTimer BeatTimer;
	public AccurateTimer VisualBeatTimer;
	public AudioStreamOggVorbis AudioStream = AudioStreamOggVorbis.LoadFromFile("res://assets/music/t14d-spaceship.ogg");
	public AudioStreamPlayer AudioPlayer;

	private BeatTime BeatTimeState = BeatTime.Free;

	public override void _EnterTree()
	{
		instance = this;

		BeatTimer = new AccurateTimer();
		AddChild(BeatTimer);
		BeatTimer.Calibration = 0;
		VisualBeatTimer = new AccurateTimer();
		AddChild(VisualBeatTimer);
		VisualBeatTimer.Calibration = SongDelay;
		AudioPlayer = new();
		AddChild(AudioPlayer);

		// Ensure no timer starts in the future
		List<AccurateTimer> timers = new() { BeatTimer, VisualBeatTimer };
		long longestCalibration = timers.OrderByDescending(timer => timer.Calibration).ToList()[0].Calibration;
		foreach (var timer in timers)
		{
			timer.Calibration -= longestCalibration;
		}

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
		AudioPlayer.Stream = AudioStream;
		PlayMusicAfterDelay();

		Bpm = 120;
		IsStarted = true;
		BeatTimer.Start(Bpm);
		VisualBeatTimer.Start(Bpm);
	}

	public async void PlayMusicAfterDelay()
	{
		var delay = (float)SongDelay / 1000;
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
		AudioPlayer.Play();
	}

	public long GetBeatIndex()
	{
		return BeatTimer.GetTickIndexAt((long)Time.Singleton.GetTicksMsec());
	}

	// Returns the time (in ms) to the nearest beat
	public long GetCurrentBeatOffset(BeatTime beatTime)
	{
		if (beatTime != BeatTime.One)
			throw new NotImplementedException();

		var beatDuration = (long)(1f / Bpm * 60 * 1000);
		var currentTime = (long)Time.Singleton.GetTicksMsec();
		var lastTickedAt = BeatTimer.LastTickedAt;
		if (currentTime - lastTickedAt < lastTickedAt + beatDuration - currentTime)
			return currentTime - lastTickedAt;
		else
			return -(lastTickedAt + beatDuration - currentTime);
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