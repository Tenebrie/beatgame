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
	public float RootOffset;

	public SkillTree(SkillGroup group, List<BaseSkill> roots, List<IApiSkillConnection<BaseSkill, BaseSkill>> links, float rootOffset = 0)
	{
		Group = group;
		Skills = new();
		Skills.AddRange(roots);
		RootOffset = rootOffset;

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
				LinkLength = link.LinkLength,
				LinkOffset = link.LinkOffset,
			};
			Links.Add(internalLink);

			target.ParentLink = internalLink;
			source.ChildrenLinks.Add(internalLink);
		}

		SetupTreePositions(roots);
	}

	void SetupTreePositions(List<BaseSkill> roots)
	{
		float total = 0;
		foreach (var root in roots)
		{
			var count = (float)EvaluateDescendantCount(root);
			EvaluateChildrenDepth(root, 0, total + count / 2);
			total += count;
		}
		foreach (var skill in Skills)
		{
			skill.PosX -= total / 2;
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

				EvaluateChildrenDepth(child.Target, depth + child.LinkLength, currentPos + widthTaken / 2);
				currentPos += widthTaken;
			}
		}

		//=========================================
		// Compact the tree
		//=========================================
		CompactTree();

		foreach (var root in roots)
		{
			root.PosX += RootOffset;
			foreach (var child in root.ChildrenLinks)
				ApplyOffset(child, RootOffset);
		}

		void ApplyOffset(SkillConnection link, float cumulativeOffset)
		{
			link.Target.PosX += link.LinkOffset + cumulativeOffset;
			foreach (var child in link.Target.ChildrenLinks)
				ApplyOffset(child, cumulativeOffset + link.LinkOffset);
		}


		bool CanCompact(BaseSkill skill, float target, float dist, List<BaseSkill> ignored)
		{
			var delta = -dist * Math.Sign(skill.PosX - target);
			var newPosX = skill.PosX + delta;
			bool isCloser = Math.Abs(newPosX - target) < Math.Abs(skill.PosX - target);
			bool willNotOverlap = Skills.All(s => ignored.Contains(s) || s.Depth != skill.Depth || Math.Abs(s.PosX - newPosX) >= 1.0f);
			return isCloser && willNotOverlap && skill.ChildrenLinks.All(child => CanMove(child.Target, delta, skill.ChildrenLinks.Select(c => c.Target).ToList()));
		}

		bool CanMove(BaseSkill skill, float delta, List<BaseSkill> ignored)
		{
			bool willNotOverlap = Skills.All(s => ignored.Contains(s) || s.Depth != skill.Depth || Math.Abs(s.PosX - skill.PosX - delta) >= 1.0f);
			return willNotOverlap && skill.ChildrenLinks.All(child => CanMove(child.Target, delta, skill.ChildrenLinks.Select(c => c.Target).ToList()));
		}

		void DoCompact(BaseSkill skill, float target, float dist)
		{
			var delta = -dist * Math.Sign(skill.PosX - target);
			var newPosX = skill.PosX + delta;
			skill.PosX = newPosX;
			DoMoveChildren(skill, delta);
		}

		void DoMoveChildren(BaseSkill skill, float offset)
		{
			foreach (var child in skill.ChildrenLinks.Select(l => l.Target))
			{
				child.PosX += offset;
				DoMoveChildren(child, offset);
			}
		}

		bool DoCompactChildren(BaseSkill skill, float target, float dist)
		{
			bool hasCompacted = false;
			if (skill.ChildrenLinks.All(c => c.LinkLength > 1 || c.Target.PosX == skill.PosX || CanCompact(c.Target, skill.PosX, dist, new() { c.Target })))
			{
				hasCompacted = true;
				foreach (var child in skill.ChildrenLinks.Select(child => child.Target))
					DoCompact(child, skill.PosX, dist);
			}
			foreach (var child in skill.ChildrenLinks.Select(child => child.Target))
				hasCompacted = DoCompactChildren(child, child.PosX, dist) || hasCompacted;
			return hasCompacted;
		}

		void CompactTree()
		{
			BaseSkill virtualSkill = new()
			{
				PosX = 0,
				Depth = -1,
				ChildrenLinks = roots.Select(s => new SkillConnection() { Target = s }).ToList(),
			};
			CompactSubTree(virtualSkill);
		}

		void CompactSubTree(BaseSkill skill)
		{
			int iterations = 0;
			bool hasCompacted;
			do
			{
				iterations += 1;
				hasCompacted = false;
				hasCompacted = DoCompactChildren(skill, skill.PosX, 0.25f);
			} while (hasCompacted && iterations < 40);
		}

		//=========================================
		// Center the trees
		//=========================================
		float leftMostSkill = Mathf.Inf;
		float rightMostSkill = -Mathf.Inf;
		foreach (var skill in Skills)
		{
			if (skill.PosX > rightMostSkill)
				rightMostSkill = skill.PosX;
			if (skill.PosX < leftMostSkill)
				leftMostSkill = skill.PosX;
		}
		float centerOffset = (leftMostSkill + rightMostSkill) / 2;
		foreach (var skill in Skills)
		{
			skill.PosX -= centerOffset;
		}
	}
}