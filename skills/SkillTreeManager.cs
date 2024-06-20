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

	const int BaseSkillPoints = 30;
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
				Link<SkillSentinel,         SkillImmovableObject>   (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillSentinel,         SkillSentinelMana>      (),
				Link<SkillImmovableObject,  SkillManaShield>        (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillImmovableObject,  SkillCelestialShield>   (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillImmovableObject,  SkillThorns>            (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillCelestialShield,  SkillSecondWind>        (3, BuffFactory.Of<BuffTankTreeHealth>()),
			}
		);

		// TODO: Add lifesteal to damage tree
		var magicTree = new SkillTree
		(
			group: SkillGroup.MagicalDamage,
			roots: new() { new SkillFireball() },

			links: new()
			{
				// L1
				Link<SkillFireball, SkillIgnitingFireball>(3, BuffFactory.Of<BuffMagicTreeManaRegen>()),
				Link<SkillFireball, SkillIgnite>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),
				Link<SkillFireball, SkillFlamethrower>(4, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L2 Left
				Link<SkillIgnitingFireball, SkillTwinFireball>(3, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L2 Main
				Link<SkillIgnite, SkillManaEfficiency1>(1, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L3
				// Link<SkillManaEfficiency1, SkillEtherealFocus>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),
				Link<SkillManaEfficiency1, SkillVaporize>(1, BuffFactory.Of<BuffMagicTreeManaRegen>()),
				Link<SkillManaEfficiency1, SkillSpiritwalkersGrace>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L4 Left
				// Link<SkillEtherealFocus, SkillEtherealDarkness>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L4 Main
				Link<SkillVaporize, SkillFireballMastery>(1, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L4 Right
				Link<SkillSpiritwalkersGrace, SkillSpiritrunnersGrace>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L5
				Link<SkillFireballMastery, SkillManaFrenzy>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),
			}
		);

		var healingTree = new SkillTree
		(
			group: SkillGroup.Healing,
			roots: new() { new SkillSelfHeal() },

			links: new()
			{
				Link<SkillSelfHeal, SkillKindness>(1, BuffFactory.Of<BuffHealTreeExtraMana>()),
				Link<SkillSelfHeal, SkillManaEfficiency2>(1, BuffFactory.Of<BuffHealTreeExtraMana>()),
				Link<SkillManaEfficiency2, SkillEtherealFocus>(2, BuffFactory.Of<BuffHealTreeExtraMana>()),
				Link<SkillEtherealFocus, SkillEtherealDarkness>(2, BuffFactory.Of<BuffHealTreeExtraMana>()),
			}
		);

		var summoningTree = new SkillTree
		(
			group: SkillGroup.Summoning,
			roots: new() { new SkillSummonStationary() },

			links: new()
			{
				Link<SkillSummonStationary, SkillSummonFireball>(),
				Link<SkillSummonStationary, SkillRescue>(1, BuffFactory.Of<BuffSummonTreeSummonHealth>()),
				Link<SkillSummonStationary, SkillFriendsWithShields>(3, BuffFactory.Of<BuffSummonTreeSummonHealth>()),
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

		var secretTree = new SkillTree(
			group: SkillGroup.Secret,
			roots: new() { new SecretSkillUnlimitedPower0() },

			links: new()
			{
				Link<SecretSkillUnlimitedPower0, SecretSkillUnlimitedPower1>(),
				Link<SecretSkillUnlimitedPower1, SecretSkillUnlimitedPower2>(),
				Link<SecretSkillUnlimitedPower2, SecretSkillUnlimitedPower3>(),
			}
		);

		SkillTrees.Add(tankTree);
		SkillTrees.Add(magicTree);
		SkillTrees.Add(healingTree);
		SkillTrees.Add(summoningTree);
		SkillTrees.Add(utilityTree);
		SkillTrees.Add(secretTree);

		Skills = SkillTrees.Aggregate(new List<BaseSkill>(), (total, tree) =>
		{
			total.AddRange(tree.Skills);
			return total;
		});

		foreach (var skill in Skills)
			AddChild(skill);
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
		List<BaseSkill> skillsToLearn = new();

		foreach (var node in path)
		{
			if (node.Skill != null)
			{
				SkillPoints -= 1;
				node.Skill.IsLearned = true;
				skillsToLearn.Add(node.Skill);
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
		foreach (var s in skillsToLearn)
			EmitSignal(SignalName.SkillUp, s);
	}

	public void UnlearnSkill(BaseSkill skill)
	{
		List<BaseSkill> skillsToUnlearn = new();
		UnlearnSkillRecursively(skill, skillsToUnlearn);
		Recalculate();
		foreach (var s in skillsToUnlearn)
			EmitSignal(SignalName.SkillDown, s);
	}

	public void UnlearnSkillRecursively(BaseSkill skill, List<BaseSkill> unlearnedSkills)
	{
		skill.IsLearned = false;
		unlearnedSkills.Add(skill);
		foreach (var link in skill.ChildrenLinks)
		{
			link.PointsInvested = 0;
			EmitSignal(SignalName.SkillLinkDown, link);
			UnlearnSkillRecursively(link.Target, unlearnedSkills);
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
