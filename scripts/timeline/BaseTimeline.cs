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
	}

	public override void _Process(double delta)
	{
		var beatIndex = Music.Singleton.GetNearestBeatIndex();

		if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
			return;

		while (Elements[CurrentElementIndex].BeatIndex <= beatIndex)
		{
			Elements[CurrentElementIndex].Cast.CastBegin(new CastTargetData() { HostileUnit = PlayerController.All[0] });
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
