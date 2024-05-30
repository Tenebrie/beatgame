using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BaseTimeline<ParentT> : Node where ParentT : BaseUnit
{
	public ParentT Parent;

	public List<TimelineElement> Elements = new();
	public int CurrentElementIndex = 0;
	public double EditorPointer = 0;

	public CastTargetData targetData;

	public BaseTimeline(ParentT parent)
	{
		Parent = parent;
		Music.Singleton.BeatTick += OnBeatTick;
	}

	public void Start()
	{
		targetData = new CastTargetData() { HostileUnit = PlayerController.AllPlayers[0], Point = PlayerController.AllPlayers[0].Position };
	}

	public void OnBeatTick(BeatTime time)
	{
		if (FightEditorUI.Singleton.EditingMode)
			return;

		var beatIndex = Music.Singleton.BeatIndex;

		if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
			return;

		while (Elements[CurrentElementIndex].BeatIndex <= beatIndex)
		{
			Elements[CurrentElementIndex].Cast?.CastBegin(targetData);
			Elements[CurrentElementIndex].Action?.Invoke();

			CurrentElementIndex += 1;

			if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
				break;
		}
	}

	public void AdvanceTime(double beatIndex)
	{
		EditorPointer += beatIndex;
	}

	public void Cast(BaseCast cast, bool advance = true)
	{
		Cast(1, cast, advance);
	}

	public void Cast(double beatIndex, BaseCast cast, bool advance = true)
	{
		var element = new TimelineElement
		{
			BeatIndex = beatIndex - 1 + EditorPointer,
			Cast = cast
		};
		Elements.Add(element);

		if (advance)
			EditorPointer += beatIndex - 1 + cast.Settings.HoldTime;
	}

	public void Act(Action action)
	{
		Act(1, action);
	}

	public void Act(double beatIndex, Action action)
	{
		var element = new TimelineElement
		{
			BeatIndex = beatIndex - 1 + EditorPointer,
			Action = action
		};
		Elements.Add(element);
	}

	public void Target(Vector3 point, bool allowMultitarget = false)
	{
		Target(1, point, allowMultitarget);
	}

	public void Target(double beatIndex, Vector3 point, bool allowMultitarget = false)
	{
		var element = new TimelineElement
		{
			BeatIndex = beatIndex - 1 + EditorPointer,
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
	public double BeatIndex;
	public BaseCast Cast;
	public Action Action;
}

public class CastQueueElement
{
	public double ReleaseAt;
	public BaseCast Cast;
}
