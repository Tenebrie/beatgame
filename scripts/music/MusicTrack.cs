using Godot;

namespace Project;

public partial class MusicTrack : Node
{
	public int BeatsPerMinute;
	public string ResourcePath;

	private AudioStreamOggVorbis AudioStream;
	private AudioStreamPlayer AudioPlayer = new();

	public async void PlayAfterDelay(float delay, float fromPosition)
	{
		AudioStream = Lib.Vorbis(ResourcePath);
		AudioPlayer.Stream = AudioStream;
		AddChild(AudioPlayer);

		await ToSignal(GetTree().CreateTimer(delay), "timeout");

		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.TrackStarted, this);
		Volume = Preferences.Singleton.MainVolume;
		AudioPlayer.Play();
		this.Log(fromPosition);
		AudioPlayer.Seek(fromPosition);
	}

	public void Stop()
	{
		AudioPlayer.Stop();
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