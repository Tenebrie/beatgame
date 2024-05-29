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

	public void Add(double beatIndex, BaseCast cast)
	{
		var element = new TimelineElement
		{
			BeatIndex = beatIndex - 1,
			Cast = cast
		};
		Elements.Add(element);
	}

	public void Add(double beatIndex, Action action)
	{
		var element = new TimelineElement
		{
			BeatIndex = beatIndex - 1,
			Action = action
		};
		Elements.Add(element);
	}

	public void Target(double beatIndex, Vector3 point)
	{
		var element = new TimelineElement
		{
			BeatIndex = beatIndex - 1,
			Action = () => targetData.Point = point,
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
