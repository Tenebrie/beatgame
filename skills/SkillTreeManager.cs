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
	[Signal]
	public delegate void SkillTreeUnlockedEventHandler(SkillGroup group);

	const int BaseSkillPoints = 30;
	public int SkillPoints = BaseSkillPoints;
	public List<BaseSkill> Skills = new();
	public List<SkillTree> SkillTrees = new();

	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		RegisterSkillTree(new SkillTree
		(
			group: SkillGroup.TankDamage,
			roots: new() { new SkillShieldBash() },

			links: new()
			{
				// Shield bash
				Link<SkillShieldBashRange,  SkillShieldsUp>             (2, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillShieldBashRange,  SkillParry>                 (2, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillShieldBash,       SkillShieldBashRange>       (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillShieldsUp,        SkillShieldBashMulticast>   (4, BuffFactory.Of<BuffTankTreeHealth>()),
			}
		));

		RegisterSkillTree(new SkillTree
		(
			group: SkillGroup.TankSurvival,
			roots: new() { new SkillSentinel() },

			links: new()
			{
				// L1
				Link<SkillSentinel,         SkillSentinelCharges>       (),
				Link<SkillSentinel,         SkillImmovableObject>       (1, BuffFactory.Of<BuffTankTreeHealth>(), length: 2),
				Link<SkillSentinel,         SkillSentinelMana>          (),

				// L2
				Link<SkillImmovableObject,  SkillHealthRegen1>          (1, BuffFactory.Of<BuffTankTreeHealth>()),

				// L3
				// Link<SkillHealthRegen1,  SkillManaShield>            (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillHealthRegen1,     SkillJuggernaut>            (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillHealthRegen1,     SkillCelestialShield>       (1, BuffFactory.Of<BuffTankTreeHealth>()),
				Link<SkillHealthRegen1,     SkillThorns>                (1, BuffFactory.Of<BuffTankTreeHealth>()),

				// L4
				Link<SkillThorns,           SkillBerserkersRage>        (1, BuffFactory.Of<BuffTankTreeHealth>()),
			}
		));

		RegisterSkillTree(new SkillTree
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
				Link<SkillManaEfficiency1, SkillVampiricEssence>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),
				Link<SkillManaEfficiency1, SkillVaporize>(1, BuffFactory.Of<BuffMagicTreeManaRegen>()),
				Link<SkillManaEfficiency1, SkillSpiritwalkersGrace>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L4 Left
				Link<SkillVampiricEssence, SkillAllConsumingFlame>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L4 Main
				Link<SkillVaporize, SkillFireballMastery>(1, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L4 Right
				Link<SkillSpiritwalkersGrace, SkillSpiritrunnersGrace>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),

				// L5
				Link<SkillFireballMastery, SkillManaFrenzy>(2, BuffFactory.Of<BuffMagicTreeManaRegen>()),
			}
		));

		RegisterSkillTree(new SkillTree
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
		));

		RegisterSkillTree(new SkillTree
		(
			group: SkillGroup.Summoning,
			roots: new() { new SkillSummonStationary() },

			links: new()
			{
				Link<SkillSummonStationary, SkillSummonFireball>(),
				Link<SkillSummonStationary, SkillRescue>(1, BuffFactory.Of<BuffSummonTreeSummonHealth>()),
				Link<SkillSummonStationary, SkillFriendsWithShields>(3, BuffFactory.Of<BuffSummonTreeSummonHealth>()),
			}
		));

		RegisterSkillTree(new SkillTree
		(
			group: SkillGroup.Utility,
			roots: new() { new SkillQuickDash() },

			links: new()
			{

			}
		));

		SignalBus.Singleton.UnitCreated += OnUnitCreated;
	}

	public void RegisterSkillTree(SkillTree tree)
	{
		SkillTrees.Add(tree);
		foreach (var skill in tree.Skills)
		{
			Skills.Add(skill);
			AddChild(skill);
		}
		EmitSignal(SignalName.SkillTreeUnlocked, tree.Group.ToVariant());
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

	public static ApiSkillConnection<T1, T2> Link<T1, T2>() where T1 : BaseSkill, new() where T2 : BaseSkill, new()
	{
		return new ApiSkillConnection<T1, T2>();
	}
	public static ApiSkillConnection<T1, T2> Link<T1, T2>(int pointsRequired, BuffFactory factory, float offset = 0, int length = 1) where T1 : BaseSkill, new() where T2 : BaseSkill, new()
	{
		return new ApiSkillConnection<T1, T2>(pointsRequired, factory, offset, length);
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
