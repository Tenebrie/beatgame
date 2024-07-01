using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;

namespace Project;

public static class Lib
{
	public readonly static UnitLibrary Unit = new();
	public readonly static TokenLibrary Token = new();
	public readonly static EffectLibrary Effect = new();
	public readonly static UILibrary UI = new();
	public readonly static AudioLibrary Audio = new();
	public readonly static SceneLibrary Scene = new();

	public static PackedScene LoadScene(string path)
	{
		AssertResourceStatus(path);
		return GD.Load<PackedScene>(path);
	}

	public static AudioStreamOggVorbis LoadVorbis(string path)
	{
		AssertResourceStatus(path);
		return GD.Load<AudioStreamOggVorbis>(path);
	}

	static void AssertResourceStatus(string path)
	{
		if (OS.IsDebugBuild())
		{
			var status = ResourceLoader.LoadThreadedGetStatus(path);
			if (status == ResourceLoader.ThreadLoadStatus.InvalidResource)
				GD.PushWarning($"Attempting to load resource {path} which has not been preloaded.");
			else if (status != ResourceLoader.ThreadLoadStatus.Loaded)
				GD.PushWarning($"Attempting to load resource {path} which has not finished loading yet.");
		}
	}
}

public partial class AssetManager : Node
{
	private bool Loaded = false;
	private int TotalResourceCount = 0;

	public override void _EnterTree()
	{
		foreach (var field in typeof(UnitLibrary).GetFields())
			PreloadResource(field.GetValue(Lib.Unit));
		foreach (var field in typeof(TokenLibrary).GetFields())
			PreloadResource(field.GetValue(Lib.Token));
		foreach (var field in typeof(EffectLibrary).GetFields())
			PreloadResource(field.GetValue(Lib.Effect));
		foreach (var field in typeof(UILibrary).GetFields())
			PreloadResource(field.GetValue(Lib.UI));
		foreach (var field in typeof(AudioLibrary).GetFields())
			PreloadResource(field.GetValue(Lib.Audio));
		foreach (var field in typeof(SceneLibrary).GetFields())
			PreloadResource(field.GetValue(Lib.Scene));
	}

	private void PreloadResource(object value)
	{
		if (value is string v)
		{
			TotalResourceCount += 1;
			ResourceLoader.LoadThreadedRequest(v, useSubThreads: true);
		}
		else if (value is Dictionary<PlayableScene, string> dictionary)
		{
			foreach (var name in dictionary.Values)
				PreloadResource(name);
		}
	}

	private static float GetResourceProgress(object value)
	{
		var array = new Godot.Collections.Array();

		if (value is string v)
		{
			ResourceLoader.LoadThreadedGetStatus(v, array);
			return (float)array[0];
		}
		else if (value is Dictionary<PlayableScene, string> dictionary)
		{
			return dictionary.Values.Aggregate(0f, (total, val) => total + GetResourceProgress(val));
		}
		return (float)array[0];
	}

	public override void _Process(double delta)
	{
		if (Loaded)
			return;

		CheckLoadingStatus();
	}

	private void CheckLoadingStatus()
	{
		float currentlyLoaded = 0;
		foreach (var field in typeof(UnitLibrary).GetFields())
			currentlyLoaded += GetResourceProgress(field.GetValue(Lib.Unit));
		foreach (var field in typeof(TokenLibrary).GetFields())
			currentlyLoaded += GetResourceProgress(field.GetValue(Lib.Token));
		foreach (var field in typeof(EffectLibrary).GetFields())
			currentlyLoaded += GetResourceProgress(field.GetValue(Lib.Effect));
		foreach (var field in typeof(UILibrary).GetFields())
			currentlyLoaded += GetResourceProgress(field.GetValue(Lib.UI));
		foreach (var field in typeof(AudioLibrary).GetFields())
			currentlyLoaded += GetResourceProgress(field.GetValue(Lib.Audio));
		foreach (var field in typeof(SceneLibrary).GetFields())
			currentlyLoaded += GetResourceProgress(field.GetValue(Lib.Scene));

		if (currentlyLoaded >= TotalResourceCount)
		{
			Loaded = true;
			Debug.WriteLine($"Successfully preloaded {TotalResourceCount} resources");
		}
		else
		{
			var percentage = Math.Round(currentlyLoaded / TotalResourceCount * 100);
			Debug.WriteLine($"Loading assets: {percentage}%");
		}
	}

	public static List<string> GetAllSceneResources()
	{
		List<string> resources = new();
		foreach (var field in typeof(UnitLibrary).GetFields())
			resources.Add((string)field.GetValue(Lib.Unit));
		foreach (var field in typeof(TokenLibrary).GetFields())
			resources.Add((string)field.GetValue(Lib.Token));
		foreach (var field in typeof(EffectLibrary).GetFields())
			resources.Add((string)field.GetValue(Lib.Effect));

		return resources;
	}
}