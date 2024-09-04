using System.Collections.Generic;
using Godot;

namespace Project;

public partial class Audio : Node
{
	const int BASE_AUDIO_PLAYERS = 16;
	const int MAX_AUDIO_PLAYERS = 128;
	readonly List<AudioStreamPlayer3D> audioPlayers = new();

	public override void _Ready()
	{
		instance = this;
		for (int i = 0; i < BASE_AUDIO_PLAYERS; i++)
			CreatePooledAudioStream();
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

	public static void Play(string audioPath, Vector3 position, float volume = 1)
	{
		if (!GetFreePlayer(out var player))
			return;

		player.Stream = Lib.LoadVorbis(audioPath);
		player.GlobalPosition = position;
		player.VolumeDb = Mathf.LinearToDb(volume);
		player.Play();
	}

	static Audio instance;
}