using System.Collections.Generic;
using Godot;

namespace Project;

public partial class Audio : Node
{
	const int BASE_AUDIO_PLAYERS = 16;
	const int MAX_AUDIO_PLAYERS = 128;
	const float AUDIO_COOLDOWN = 0.1f;
	readonly List<AudioStreamPlayer3D> audioPlayers = new();
	readonly Dictionary<string, float> audioCooldowns = new();

	public override void _Ready()
	{
		instance = this;
		for (int i = 0; i < BASE_AUDIO_PLAYERS; i++)
			CreatePooledAudioStream();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	static AudioStreamPlayer3D CreatePooledAudioStream()
	{
		var player = new AudioStreamPlayer3D();
		instance.AddChild(player);
		instance.audioPlayers.Add(player);
		return player;
	}

	static bool GetFreePlayer(out AudioStreamPlayer3D player)
	{
		foreach (var p in instance.audioPlayers)
		{
			if (!p.Playing)
			{
				player = p;
				return true;
			}
		}
		if (instance.audioPlayers.Count < MAX_AUDIO_PLAYERS)
		{
			player = CreatePooledAudioStream();
			return true;
		}
		player = null;
		return false;
	}

	private void PlayInternal(string audioPath, Vector3 position, float volume = 1)
	{
		var engineTime = CastUtils.GetEngineTime();
		if (audioCooldowns.TryGetValue(audioPath, out var lastPlayed) && engineTime - lastPlayed < AUDIO_COOLDOWN)
			return;

		if (!GetFreePlayer(out var player))
			return;

		player.Stream = Lib.LoadVorbis(audioPath);
		player.GlobalPosition = position;
		player.VolumeDb = Mathf.LinearToDb(volume / 2);
		player.Bus = "Effects".ToStringName();
		player.AttenuationFilterDb = -12;
		player.AttenuationModel = AudioStreamPlayer3D.AttenuationModelEnum.Logarithmic;
		player.Play();

		audioCooldowns[audioPath] = engineTime;
	}

	public static void Play(string audioPath, Vector3 position, float volume = 1)
	{
		instance.PlayInternal(audioPath, position, volume);
	}

	static Audio instance;
}