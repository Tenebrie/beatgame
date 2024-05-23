using Godot;

namespace Project;

public partial class MusicTrack : Node
{
	public int BeatsPerMinute;
	public string ResourcePath;

	private AudioStreamOggVorbis AudioStream;
	private AudioStreamPlayer AudioPlayer;

	public override void _EnterTree()
	{
		AudioStream = AudioStreamOggVorbis.LoadFromFile(ResourcePath);
		AudioPlayer = new()
		{
			Stream = AudioStream,
		};
		AddChild(AudioPlayer);
	}

	public async void PlayAfterDelay(float delay)
	{
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.TrackStarted, this);
		AudioPlayer.VolumeDb = Mathf.LinearToDb(0.03f);
		AudioPlayer.Play();
	}

	public float BeatDuration
	{
		get => 1f / BeatsPerMinute * 60;
	}
}