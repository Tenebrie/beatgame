using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

/// Notes:
/// - 5 skill trees: Tank, Heal, Physical Damage, Magical Damage, Summons

/// - Tank:
/// -- Basic damage mitigation (upgrades into having multiple charges)
/// -- Thorns
/// -- CC reduction
/// -- CC ignore, full invuln
/// -- Save position, press again to return
/// -- Dying heals you back to full hp instead
/// -- Mana shield - toggle to now tank all damage into mana

/// - Physical:
/// -- Quick attacks, little casting (warrior + rogue together)

/// - Magical:
/// -- Longer casts, but more powerful
/// -- Ability to cast while moving for the next N beats

/// - Heal:
/// -- Left branch is about proper healing
/// -- Right branch is about damage through healing (whenever you restore HP, deal damage to targeted enemy)

/// - Summons:
/// -- Friends that have simple AI and cast their spells on a loop
/// -- Rescue to pull them to safety
/// -- Take damage from target to yourself (Alternatively, move damage from yourself to target) 
public class SkillTree
{
	public SkillGroup Group;
	public List<BaseSkill> Skills;
	public List<SkillConnection> Links = new();

	public SkillTree(SkillGroup group, List<BaseSkill> roots, List<IApiSkillConnection<BaseSkill, BaseSkill>> links)
	{
		Group = group;
		Skills = new();
		Skills.AddRange(roots);

		foreach (var root in roots)
		{
			root.Group = Group;
		}

		foreach (var link in links)
		{
			var t0 = link.GetType().GetGenericArguments()[0];
			var t1 = link.GetType().GetGenericArguments()[1];
			var source = Skills.Find(skill => skill.GetType() == t0);
			var target = Skills.Find(skill => skill.GetType() == t1);
			if (source == null)
			{
				source = (BaseSkill)Activator.CreateInstance(t0);
				source.Group = group;
				Skills.Add(source);
			}
			if (target == null)
			{
				target = (BaseSkill)Activator.CreateInstance(t1);
				target.Group = group;
				Skills.Add(target);
			}

			var internalLink = new SkillConnection()
			{
				Source = source,
				Target = target,
				PointsRequired = link.PointsRequired,
				PassivePerPoint = link.PassivePerPoint,
			};
			Links.Add(internalLink);

			target.ParentLink = internalLink;
			source.ChildrenLinks.Add(internalLink);
		}

		SetupTreePositions(roots);
	}

	void SetupTreePositions(List<BaseSkill> roots)
	{
		foreach (var root in roots)
		{
			EvaluateChildrenDepth(root, 0, 0);
		}

		int EvaluateDescendantCount(BaseSkill skill)
		{
			var childrenCount = skill.ChildrenLinks.Count;
			var subchildrenCount = childrenCount > 0 ? skill.ChildrenLinks.Select(link => EvaluateDescendantCount(link.Target)).Sum() : 1;
			return Math.Max(childrenCount, subchildrenCount);
		}

		void EvaluateChildrenDepth(BaseSkill skill, int depth, float posX)
		{
			skill.Depth = depth;
			float descendantCount = EvaluateDescendantCount(skill);
			skill.PosX = posX;

			float currentPos = posX - descendantCount / 2;

			for (var i = 0; i < skill.ChildrenLinks.Count; i++)
			{
				var child = skill.ChildrenLinks[i];

				float widthTaken = EvaluateDescendantCount(child.Target);

				EvaluateChildrenDepth(child.Target, depth + 1, currentPos + widthTaken / 2);
				currentPos += widthTaken;
			}
		}

		//=========================================
		// Compact the tree (Optional step)
		//=========================================
		foreach (var root in roots)
		{
			CompactSubTree(root);
		}

		bool CanCompact(BaseSkill skill, float target, float dist)
		{
			var newPosX = skill.PosX - dist * Math.Sign(skill.PosX);
			bool isCloser = Math.Abs(newPosX - target) < Math.Abs(skill.PosX - target);
			bool willNotOverlap = Skills.All(s => s == skill || s.Depth != skill.Depth || Math.Abs(s.PosX - newPosX) >= 1.0f);
			return isCloser && willNotOverlap && skill.ChildrenLinks.All(child => CanCompact(child.Target, newPosX, dist));
		}

		void DoCompact(BaseSkill skill, float target, float dist)
		{
			if (CanCompact(skill, target, dist))
			{
				var newPosX = skill.PosX - dist * Math.Sign(skill.PosX);
				skill.PosX = newPosX;
			}
		}

		void DoCompactChildren(BaseSkill skill, float target, float dist)
		{
			foreach (var child in skill.ChildrenLinks.Select(child => child.Target))
			{
				DoCompact(child, skill.PosX, dist);
			}
		}

		void CompactSubTree(BaseSkill skill)
		{
			int iterations = 0;
			bool hasCompacted;
			do
			{
				iterations += 1;
				hasCompacted = false;
				foreach (var child in skill.ChildrenLinks.Select(child => child.Target))
				{
					if (CanCompact(child, skill.PosX, 0.25f))
					{
						hasCompacted = true;
						DoCompact(child, skill.PosX, 0.25f);
					}
					DoCompactChildren(child, skill.PosX, 0.25f);
				}
			} while (hasCompacted && iterations < 20);
		}
	}
}