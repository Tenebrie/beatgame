using Godot;

namespace Project;

public partial class MusicTrack : Node
{
	public int BeatsPerMinute;
	public string ResourcePath;
	public bool Loop;

	private AudioStreamOggVorbis AudioStream;
	private AudioStreamPlayer AudioPlayer = new();

	public override void _EnterTree()
	{
		AudioStream = Lib.LoadVorbis(ResourcePath);
		AudioPlayer.Stream = AudioStream;
		AddChild(AudioPlayer);
		AudioPlayer.Finished += OnFinished;
	}

	public async void PlayAfterDelay(float delay, float fromPosition)
	{
		await ToSignal(GetTree().CreateTimer(delay), "timeout");

		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.TrackStarted, this);
		Volume = Preferences.Singleton.MusicVolume;
		AudioPlayer.Play();
		AudioPlayer.Seek(fromPosition);
	}

	public void Stop()
	{
		AudioPlayer.Stop();
	}

	async void OnFinished()
	{
		Music.Singleton.Stop();
		if (Loop)
		{
			await ToSignal(GetTree().CreateTimer(2), "timeout");
			Music.Singleton.Start();
		}
	}

	public float Volume
	{
		get => Mathf.DbToLinear(AudioPlayer.VolumeDb);
		set => AudioPlayer.VolumeDb = Mathf.LinearToDb(value);
	}

	public float BeatDuration
	{
		get => 1f / BeatsPerMinute * 60;
	}
}