using System;
using System.Diagnostics;
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

	public static PackedScene Scene(string path)
	{
		AssertResourceStatus(path);
		return GD.Load<PackedScene>(path);
	}

	public static AudioStreamOggVorbis Vorbis(string path)
	{
		AssertResourceStatus(path);
		return GD.Load<AudioStreamOggVorbis>(path);
	}

	static void AssertResourceStatus(string path)
	{
		if (OS.IsDebugBuild())
		{
			var status = ResourceLoader.LoadThreadedGetStatus(path);
			if (status != ResourceLoader.ThreadLoadStatus.Loaded)
				GD.PushWarning($"Attempting to load resource {path} which has not finished loading yet.");
		}
	}
}

public partial class AssetManager : Node
{
	private bool Loaded = false;
	private int TotalResourceCount = 0;

	public override void _Ready()
	{
		foreach (var field in typeof(UnitLibrary).GetFields())
			PreloadResource((string)field.GetValue(Lib.Unit));
		foreach (var field in typeof(TokenLibrary).GetFields())
			PreloadResource((string)field.GetValue(Lib.Token));
		foreach (var field in typeof(EffectLibrary).GetFields())
			PreloadResource((string)field.GetValue(Lib.Effect));
		foreach (var field in typeof(UILibrary).GetFields())
			PreloadResource((string)field.GetValue(Lib.UI));
		foreach (var field in typeof(AudioLibrary).GetFields())
			PreloadResource((string)field.GetValue(Lib.Audio));

		CheckLoadingStatus();
	}

	private void PreloadResource(string name)
	{
		TotalResourceCount += 1;
		ResourceLoader.LoadThreadedRequest(name);
	}

	private static float GetResourceProgress(string name)
	{
		var array = new Godot.Collections.Array();
		ResourceLoader.LoadThreadedGetStatus(name, array);
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
			currentlyLoaded += GetResourceProgress((string)field.GetValue(Lib.Unit));
		foreach (var field in typeof(TokenLibrary).GetFields())
			currentlyLoaded += GetResourceProgress((string)field.GetValue(Lib.Token));
		foreach (var field in typeof(EffectLibrary).GetFields())
			currentlyLoaded += GetResourceProgress((string)field.GetValue(Lib.Effect));
		foreach (var field in typeof(UILibrary).GetFields())
			currentlyLoaded += GetResourceProgress((string)field.GetValue(Lib.UI));
		foreach (var field in typeof(AudioLibrary).GetFields())
			currentlyLoaded += GetResourceProgress((string)field.GetValue(Lib.Audio));

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
}