using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Project;

public partial class BaseTimeline<ParentT> : Node where ParentT : BaseUnit
{
	public ParentT Parent;

	public List<TimelineElement> Elements = new();
	public int CurrentElementIndex = 0;

	public BaseTimeline(ParentT parent)
	{
		Parent = parent;
		Music.Singleton.BeatTick += OnBeatTick;
	}

	public void OnBeatTick(BeatTime time)
	{
		var beatIndex = Music.Singleton.BeatIndex;

		if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
			return;

		while (Elements[CurrentElementIndex].BeatIndex <= beatIndex)
		{
			Elements[CurrentElementIndex].Cast.CastBegin(new CastTargetData() { HostileUnit = PlayerController.AllPlayers[0], TargetPoint = PlayerController.AllPlayers[0].Position });
			Elements[CurrentElementIndex].Cast.CastPerform();
			CurrentElementIndex += 1;

			if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
				break;
		}
	}

	public void Add(long beatIndex, BaseCast cast)
	{
		var element = new TimelineElement
		{
			BeatIndex = beatIndex,
			Cast = cast
		};
		Elements.Add(element);
	}
}

public class TimelineElement
{
	public long BeatIndex;
	public BaseCast Cast;
}
