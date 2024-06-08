using Godot;
using System;
using System.Collections.Generic;

namespace Project;

public partial class SkillLinkUI : Control
{
	public SkillConnection SkillLink;
	public int Segments;
	public int SegmentsFilled;
	public Vector2 Target;

	public override void _Ready()
	{
		SkillTreeManager.Singleton.SkillUp += OnSkillChanged;
		SkillTreeManager.Singleton.SkillDown += OnSkillChanged;
		SkillTreeManager.Singleton.SkillLinkUp += OnLinkChanged;
		SkillTreeManager.Singleton.SkillLinkDown += OnLinkChanged;
	}

	void OnSkillChanged(BaseSkill skill)
	{
		if (skill == SkillLink.Target)
			QueueRedraw();
	}

	void OnLinkChanged(SkillConnection link)
	{
		if (link == SkillLink)
			QueueRedraw();
	}

	public override void _Draw()
	{
		Segments = SkillLink.PointsRequired;

		var filledColor = new Color(1.0f, 0.5f, 0.0f);
		var unfilledColor = new Color(0.7f, 0.7f, 0.7f);

		List<Vector2> circlePositions = new();
		var segmentStartsAt = new Vector2(0, 0);
		var segmentsToDraw = Segments + 1;
		for (var i = 0; i < segmentsToDraw; i++)
		{
			var segmentsEndsAt = segmentStartsAt + Target / segmentsToDraw;
			var isFilled = SkillLink.PointsInvested >= i + 1 || SkillLink.Target.IsLearned;

			var color = isFilled ? filledColor : unfilledColor;
			DrawLine(segmentStartsAt, segmentsEndsAt, color, 1, true);
			segmentStartsAt = segmentsEndsAt;
			circlePositions.Add(segmentsEndsAt);
		}
		circlePositions.RemoveAt(circlePositions.Count - 1);

		for (var i = 0; i < segmentsToDraw - 1; i++)
		{
			var pos = circlePositions[i];
			var isFilled = SkillLink.PointsInvested >= i + 1 || SkillLink.Target.IsLearned;
			var color = isFilled ? filledColor : unfilledColor;
			DrawCircle(pos, 5, color);
		}
	}

	public override void _Process(double delta)
	{
	}
}
