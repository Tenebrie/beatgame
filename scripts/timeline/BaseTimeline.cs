using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
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
		if (FightEditorUI.Singleton.EditingMode)
			return;

		var beatIndex = Music.Singleton.BeatIndex;

		if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
			return;

		while (Elements[CurrentElementIndex].BeatIndex <= beatIndex)
		{
			var targetData = new CastTargetData() { HostileUnit = PlayerController.AllPlayers[0], Point = PlayerController.AllPlayers[0].Position };

			Elements[CurrentElementIndex].Cast.CastBegin(targetData);
			CurrentElementIndex += 1;

			if (Elements.ElementAtOrDefault(CurrentElementIndex) == null)
				break;
		}
	}

	public void Add(double beatIndex, BaseCast cast)
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
	public double BeatIndex;
	public BaseCast Cast;
}

public class CastQueueElement
{
	public double ReleaseAt;
	public BaseCast Cast;
}
