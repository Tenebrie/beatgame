using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BeatGame.scripts.music;
using Godot;

namespace Project;
public partial class BaseTimeline : Node
{
	public BaseBoss Parent;

	public List<TimelineElement> Elements = new();
	public Dictionary<string, double> Marks = new();
	public int CurrentElementIndex = 0;
	public double EditorPointer = 0;

	public MusicTrack MusicTrack;

	bool isStarted = false;
	public CastTargetData targetData = new();

	public BaseTimeline(BaseBoss parent)
	{
		Parent = parent;
		GotoAbleton(1);
		Mark("Start");
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Music.Singleton.BeatTick += OnBeatTick;
		SignalBus.Singleton.UnitCreated += OnUnitCreated;
		TimelineManager.Singleton.FightStarting += OnFightStarting;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Music.Singleton.BeatTick -= OnBeatTick;
		SignalBus.Singleton.UnitCreated -= OnUnitCreated;
		TimelineManager.Singleton.FightStarting -= OnFightStarting;
	}

	void OnUnitCreated(BaseUnit unit)
	{
		if (unit is PlayerController)
			targetData.HostileUnit = unit;
	}

	public double GetMarkBeatIndex(string name)
	{
		var valueFound = Marks.TryGetValue(name, out var beatIndex);
		if (!valueFound)
			GD.PushWarning($"Unable to find a mark with name {name}, starting from beginning");
		return beatIndex;
	}

	void OnFightStarting()
	{
		var targetIndex = GetMarkBeatIndex("Start");
		Music.Singleton.SeekTo(targetIndex);
		Start(targetIndex);
	}

	public void Start(double targetIndex)
	{
		isStarted = true;
		targetData.HostileUnit = PlayerController.AllPlayers[0];
		Elements = Elements.OrderBy(el => el.BeatIndex).ThenBy(el => el.Priority).ToList();
		FastForwardTo(targetIndex);
		Music.Singleton.Start();
	}

	public void OnBeatTick(BeatTime time)
	{
		if (FightEditorUI.Singleton.EditingMode || !isStarted)
			return;

		var beatIndex = Music.Singleton.BeatIndex;

		if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
			return;

		while (Elements[CurrentElementIndex].BeatIndex <= beatIndex)
		{
			try
			{
				Elements[CurrentElementIndex].Cast?.CastBegin(targetData);
				Elements[CurrentElementIndex].Action?.Invoke();
			}
			catch (Exception ex)
			{
				GD.PrintErr(ex.ToString());
			}

			CurrentElementIndex += 1;

			if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
				break;
		}
		HandleBeatTick(time);
	}

	public void FastForwardTo(double beatIndex)
	{
		if (Elements.Count == 0)
			return;

		var number = 0;

		while (Elements[number].BeatIndex < beatIndex)
		{
			Elements[number].Action?.Invoke();
			number += 1;

			if (Elements.ElementAtOrDefault(number) == null)
			{
				number = 0;
				break;
			}
		}

		CurrentElementIndex = number;
	}

	protected virtual void HandleBeatTick(BeatTime time) { }

	public void Wait(double beatIndex)
	{
		EditorPointer += beatIndex;
	}

	public void Cast(BaseCast cast, bool advance = true)
	{
		Cast(0, cast, advance);
	}

	public void Cast(double beatIndex, BaseCast cast, bool advance = true)
	{
		var targetIndex = beatIndex + EditorPointer;

		var element = new TimelineElement
		{
			Priority = 1,
			BeatIndex = targetIndex,
			Cast = cast
		};
		Elements.Add(element);

		if (advance)
			EditorPointer += beatIndex + cast.Settings.HoldTime + cast.Settings.PrepareTime;
	}

	public void Act(Action action) => Act(0, action);
	public void Act(double beatIndex, Action action)
	{
		var element = new TimelineElement
		{
			Priority = 0,
			BeatIndex = beatIndex + EditorPointer,
			Action = action
		};
		Elements.Add(element);
	}

	public void Goto(double beatIndex)
	{
		EditorPointer = beatIndex;
	}

	public void GotoAbleton(double abletonIndex)
	{
		string str = abletonIndex.ToString("0.0", CultureInfo.InvariantCulture);
		int wholePart = int.Parse(str.Split(".")[0]) - 1;
		int subPart = int.Parse(str.Split(".")[1]) - 1;
		if (subPart == -1)
			subPart = 0;
		else if (subPart > 4)
			GD.PushError("GotoAbleton index decimal should not exceed 4");

		EditorPointer = wholePart * 4 + subPart;
	}

	public void Mark(string name) => Mark(0, name);

	public void Mark(double beatIndex, string name)
	{
		Marks[name] = beatIndex + EditorPointer;
		EditorPointer += beatIndex;
	}

	public void Target(Vector3 point, bool allowMultitarget = false)
	{
		Target(0, point, allowMultitarget);
	}

	public void Target(double beatIndex, Vector3 point, bool allowMultitarget = false)
	{
		var element = new TimelineElement
		{
			Priority = 0,
			BeatIndex = beatIndex + EditorPointer,
			Action = () =>
			{
				targetData.Point = point;
				if (!allowMultitarget)
					targetData.MultitargetPoints.Clear();
				targetData.MultitargetPoints.Add(point);
			},
		};
		Elements.Add(element);
	}
}

public class TimelineElement
{
	public int Priority; // Lower is better
	public double BeatIndex;
	public BaseCast Cast;
	public Action Action;
}

public class CastQueueElement
{
	public double ReleaseAt;
	public BaseCast Cast;
}
