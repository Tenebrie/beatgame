using System;
using System.Collections.Generic;
using Godot;

namespace Project;

public partial class SkillTreeManager : Node
{
	[Signal]
	public delegate void SkillUpEventHandler(BaseSkill skill);
	[Signal]
	public delegate void SkillDownEventHandler(BaseSkill skill);

	public List<SkillTree> SkillTrees = new();

	public override void _EnterTree()
	{
		instance = this;

		// =================================
		// Tank tree
		// =================================
		var tankTree = new SkillTree
		(
			group: SkillGroup.Tank,
			roots: new() { new SkillGuardUp() },

			links: new()
			{
				new SkillLink<SkillGuardUp, SkillImmovableObject>(),
				new SkillLink<SkillGuardUp, SkillImprovedGuardUp>(),
				new SkillLink<SkillImmovableObject, SkillManaShield>(),
				new SkillLink<SkillImmovableObject, SkillThorns>(),
				new SkillLink<SkillImmovableObject, SkillCelestialShield>(),
				new SkillLink<SkillCelestialShield, SkillSecondWind>(),
			}
		);

		SkillTrees.Add(tankTree);
		this.Log("Done prepping");
	}

	public override void _Ready()
	{

	}

	public void IncreaseLevel(BaseSkill skill)
	{
		//
	}

	public void DecreaseLevel(BaseSkill skill)
	{
	}

	private static SkillTreeManager instance = null;
	public static SkillTreeManager Singleton
	{
		get => instance;
	}
}
