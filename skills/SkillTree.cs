using System;
using System.Collections.Generic;
using System.Linq;

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
	public List<BaseSkill> Skills = new();
	public List<ISkillConnectionData<BaseSkill, BaseSkill>> Links;

	public SkillTree(SkillGroup group, List<BaseSkill> roots, List<ISkillConnectionData<BaseSkill, BaseSkill>> links)
	{
		Group = group;
		Skills = new();
		Links = links;

		Skills.AddRange(roots);

		foreach (var link in links)
		{
			var t0 = link.GetType().GetGenericArguments()[0];
			var t1 = link.GetType().GetGenericArguments()[1];
			var source = Skills.Find(skill => skill.GetType() == t0);
			var target = Skills.Find(skill => skill.GetType() == t1);
			if (source == null)
			{
				source = (BaseSkill)Activator.CreateInstance(t0);
				Skills.Add(source);
			}
			if (target == null)
			{
				target = (BaseSkill)Activator.CreateInstance(t1);
				Skills.Add(target);
			}

			var internalLink = new InternalSkillLink()
			{
				Source = source,
				Target = target,
				PointsRequired = link.PointsRequired,
				PassivePerPoint = link.PassivePerPoint,
			};

			source.Children.Add(internalLink);
			target.Parents.Add(internalLink);
		}

		int EvaluateDescendantCount(BaseSkill skill)
		{
			var childrenCount = skill.Children.Count;
			var subchildrenCount = childrenCount > 0 ? skill.Children.Select(link => EvaluateDescendantCount(link.Target)).Sum() : 1;
			return Math.Max(childrenCount, subchildrenCount);
		}

		void EvaluateChildrenDepth(BaseSkill skill, int depth, float posX)
		{
			skill.Depth = depth;
			float descendantCount = EvaluateDescendantCount(skill);
			skill.PosX = posX;

			float currentPos = posX - descendantCount / 2;

			for (var i = 0; i < skill.Children.Count; i++)
			{
				var child = skill.Children[i];

				float widthTaken = EvaluateDescendantCount(child.Target);

				EvaluateChildrenDepth(child.Target, depth + 1, currentPos + widthTaken / 2);
				currentPos += widthTaken;
			}
		}

		// TODO: Add compacting logic. If the entire subtree can move closer to the middle without overlap, it should
		foreach (var root in roots)
		{
			EvaluateChildrenDepth(root, 0, 0);
		}

		foreach (var skill in Skills)
		{
			this.Log($"{skill.FriendlyName} at {skill.Depth} / {skill.PosX}");
			this.Log($"{skill.FriendlyName} has {EvaluateDescendantCount(skill)}");

		}
	}
}