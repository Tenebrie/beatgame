using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class SkillTreeManager : Node
{
	[Signal]
	public delegate void SkillPointsChangedEventHandler();
	[Signal]
	public delegate void SkillUpEventHandler(BaseSkill skill);
	[Signal]
	public delegate void SkillDownEventHandler(BaseSkill skill);
	[Signal]
	public delegate void SkillLinkUpEventHandler(SkillConnection connection);
	[Signal]
	public delegate void SkillLinkDownEventHandler(SkillConnection connection);

	const int BaseSkillPoints = 9;
	public int SkillPoints;
	public List<BaseSkill> Skills = new();
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
			roots: new() { new SkillSentinel() },

			links: new()
			{
				Link<SkillSentinel,         SkillSentinelCharges>   (),
				Link<SkillSentinel,         SkillImmovableObject>   (1, BuffFactory.Of<BuffPlus25Health>()),
				Link<SkillSentinel,         SkillSentinelMana>      (),
				Link<SkillImmovableObject,  SkillManaShield>        (1, BuffFactory.Of<BuffPlus25Health>()),
				Link<SkillImmovableObject,  SkillCelestialShield>   (1, BuffFactory.Of<BuffPlus25Health>()),
				Link<SkillImmovableObject,  SkillThorns>            (1, BuffFactory.Of<BuffPlus25Health>()),
				Link<SkillCelestialShield,  SkillSecondWind>        (3, BuffFactory.Of<BuffPlus25Health>()),
			}
		);

		var magicTree = new SkillTree
		(
			group: SkillGroup.MagicalDamage,
			roots: new() { new SkillFireball() },

			links: new()
			{
				Link<SkillFireball, SkillSpiritwalkersGrace>(3, BuffFactory.Of<BuffPlus10Mana>()),
				Link<SkillSpiritwalkersGrace, SkillSpiritrunnersGrace>(2, BuffFactory.Of<BuffPlus10Mana>()),
			}
		);

		var healingTree = new SkillTree
		(
			group: SkillGroup.Healing,
			roots: new() { new SkillSelfHeal() },

			links: new()
			{

			}
		);

		var utilityTree = new SkillTree
		(
			group: SkillGroup.Utility,
			roots: new() { new SkillQuickDash() },

			links: new()
			{

			}
		);

		SkillTrees.Add(tankTree);
		SkillTrees.Add(magicTree);
		SkillTrees.Add(healingTree);
		SkillTrees.Add(utilityTree);

		Skills = SkillTrees.Aggregate(new List<BaseSkill>(), (total, tree) =>
		{
			total.AddRange(tree.Skills);
			return total;
		});
	}

	public override void _Ready()
	{
		SignalBus.Singleton.UnitCreated += OnUnitCreated;
	}

	void OnUnitCreated(BaseUnit unit)
	{
		if (unit is PlayerController)
			Recalculate();
	}

	public void LearnSkill(BaseSkill skill)
	{
		if (SkillPoints == 0)
			return;

		BaseSkill traversedNode = skill;
		List<TreePathNode> path = new(){
			new TreePathNode() { Skill = skill }
		};

		int iterationCount = 0;
		do
		{
			iterationCount += 1;
			if (traversedNode.ParentLink == null)
				break;

			if (traversedNode.ParentLink.PointsRequired > 0)
			{
				var linkPointsNeeded = traversedNode.ParentLink.PointsRequired - traversedNode.ParentLink.PointsInvested;
				if (linkPointsNeeded == 0)
					break;

				var linkNode = new TreePathNode()
				{
					Link = traversedNode.ParentLink,
				};
				path.Add(linkNode);
			}

			var parentSkill = traversedNode.ParentLink.Source;
			if (parentSkill.IsLearned)
				break;

			var skillNode = new TreePathNode()
			{
				Skill = parentSkill,
			};
			path.Add(skillNode);

			traversedNode = parentSkill;

		} while (iterationCount < 30);

		path.Reverse();

		foreach (var node in path)
		{
			if (node.Skill != null)
			{
				SkillPoints -= 1;
				node.Skill.Learn();
			}

			if (node.Link != null)
			{
				var pointsMissing = node.Link.PointsRequired - node.Link.PointsInvested;
				node.Link.PointsInvested = Math.Min(node.Link.PointsRequired, node.Link.PointsInvested + SkillPoints);
				SkillPoints = Math.Max(0, SkillPoints - pointsMissing);
				EmitSignal(SignalName.SkillLinkUp, node.Link);
			}

			if (SkillPoints == 0)
				break;
		}

		Recalculate();
	}

	public void UnlearnSkill(BaseSkill skill)
	{
		UnlearnSkillRecursively(skill);
		Recalculate();
	}

	public void UnlearnSkillRecursively(BaseSkill skill)
	{
		skill.Unlearn();
		foreach (var link in skill.ChildrenLinks)
		{
			link.PointsInvested = 0;
			EmitSignal(SignalName.SkillLinkDown, link);
			UnlearnSkill(link.Target);
		}
	}

	public void Recalculate()
	{
		if (PlayerController.AllPlayers.Count == 0)
			return;

		var affectedUnit = PlayerController.AllPlayers[0];
		affectedUnit.Buffs.RemoveWithFlag(BaseBuff.Flag.SkillCreated);
		var skillPoints = BaseSkillPoints;
		foreach (var skillTree in SkillTrees)
		{
			foreach (var skill in skillTree.Skills)
			{
				if (!skill.IsLearned)
					continue;

				skillPoints -= 1;
				if (skill.Settings.PassiveBuff == null)
					continue;

				var newBuff = skill.Settings.PassiveBuff.Create();
				newBuff.Flags |= BaseBuff.Flag.SkillCreated;
				affectedUnit.Buffs.Add(newBuff);
			}
			foreach (var link in skillTree.Links)
			{
				skillPoints -= link.PointsInvested;
				if (link.PassivePerPoint != null)
				{
					for (var i = 0; i < link.PointsInvested; i++)
					{
						var newBuff = link.PassivePerPoint.Create();
						newBuff.Flags |= BaseBuff.Flag.SkillCreated;
						affectedUnit.Buffs.Add(newBuff);
					}
				}
			}
		}
		SkillPoints = skillPoints;
		EmitSignal(SignalName.SkillPointsChanged);
	}

	static ApiSkillConnection<T1, T2> Link<T1, T2>() where T1 : BaseSkill, new() where T2 : BaseSkill, new()
	{
		return new ApiSkillConnection<T1, T2>();
	}
	static ApiSkillConnection<T1, T2> Link<T1, T2>(int pointsRequired, BuffFactory factory) where T1 : BaseSkill, new() where T2 : BaseSkill, new()
	{
		return new ApiSkillConnection<T1, T2>(pointsRequired, factory);
	}

	private static SkillTreeManager instance = null;
	public static SkillTreeManager Singleton
	{
		get => instance;
	}

	class TreePathNode
	{
		public BaseSkill Skill;
		public SkillConnection Link;
	}
}
