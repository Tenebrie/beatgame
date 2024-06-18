using Godot;
using System;
using System.Collections.Generic;

namespace Project;

public partial class SkillLinkVisual : Control
{
	public SkillConnection SkillLink;
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
		var filledColor = new Color(1.0f, 0.5f, 0.0f);
		var unfilledColor = new Color(0.7f, 0.7f, 0.7f);

		List<Vector2> circlePositions = new();
		var segmentStartsAt = new Vector2(0, 0);

		var direction = Target.Normalized();

		var totalSpaceWithCircles = (SkillLink.PointsRequired - 1) * 15f;
		var segmentStep = SkillLink.PointsRequired > 1 ? totalSpaceWithCircles / (SkillLink.PointsRequired - 1) : 0;
		var segmentsToDraw = SkillLink.PointsRequired + 1;
		var middleOfTheLine = Target / 2f;
		for (var i = 0; i < segmentsToDraw; i++)
		{
			var segmentsEndsAt = middleOfTheLine - direction * totalSpaceWithCircles / 2f + direction * segmentStep * i;
			if (i == segmentsToDraw - 1)
				segmentsEndsAt = Target;

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
			DrawCircle(pos, 3, color);
			DrawArc(pos, 4, 0, (float)Math.PI * 2, 10, color, 1, true);
		}
	}

	public override void _Process(double delta)
	{
	}
}
