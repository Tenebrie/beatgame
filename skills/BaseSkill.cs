using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace Project;

public partial class BaseSkill : Node
{
	public class SkillSettings
	{
		public string FriendlyName;
		public string Description;
		public string IconPath = null;
		public CastFactory ActiveCast;
		public List<CastFactory> AffectedCasts = new();
		public BuffFactory PassiveBuff;
		public List<SkillWrapper> IncompatibleSkills = new();
	}

	public SkillSettings Settings;

	public int Depth;
	public float PosX;

	public bool IsLearned;
	public SkillGroup Group;
	public SkillConnection ParentLink;
	public List<SkillConnection> ChildrenLinks = new();

	public Type SkillType
	{
		get => Settings.ActiveCast != null ? Type.Active : Type.Passive;
	}

	public string Description
	{
		get
		{
			StringBuilder builder = new();
			// Internal description
			if (Settings.Description != null)
				builder.Append(Settings.Description);

			// Incompatible with
			if (Settings.IncompatibleSkills.Count > 0)
			{
				if (builder.ToString().Length > 0)
					builder.Append("\n\n");
				var skills = Settings.IncompatibleSkills.Select(wrapper => wrapper.GetName()).ToArray();
				builder.Append($"{Colors.Tag("Incompatible with:", Colors.Forbidden)} {skills.Join(", ")}");
			}

			if (Settings.ActiveCast != null)
			{
				if (builder.ToString().Length > 0)
					builder.Append("\n\n");
				var castSettings = Settings.ActiveCast.Settings;
				// Description
				builder.Append($"[color={Colors.Active}]Active:[/color] ").Append(castSettings.Description);

				// Input type
				builder.Append($"\n[color={Colors.Passive}]Input:[/color] ");
				if (castSettings.InputType == CastInputType.Instant)
					builder.Append("Instant");
				else if (castSettings.InputType == CastInputType.AutoRelease)
					builder.Append($"Cast ({castSettings.HoldTime} beat{(castSettings.HoldTime == 1 ? "" : "s")})");
				else if (castSettings.InputType == CastInputType.HoldRelease)
					builder.Append($"Hold ({castSettings.HoldTime} beat{(castSettings.HoldTime == 1 ? "" : "s")})");

				// Charges
				if (castSettings.Charges > 1)
					builder.Append($"\n[color={Colors.Passive}]Charges:[/color] ").Append($"{castSettings.Charges}");

				// Recast time
				if (castSettings.RecastTime >= 0.25f)
					builder.Append($"\n[color={Colors.Passive}]Cooldown:[/color] ").Append($"{castSettings.RecastTime} beat{(castSettings.RecastTime == 1 ? "" : "s")}");

				// Resource cost
				if (castSettings.ResourceCost[ObjectResourceType.Mana] > 0)
					builder.Append($"\n[color={Colors.Mana}]Mana cost:[/color] ").Append(castSettings.ResourceCost[ObjectResourceType.Mana]);

				// Lore
				if (castSettings.LoreDescription != null)
					builder.Append("\n\n").Append(Colors.Lore(castSettings.LoreDescription));
			}
			if (Settings.PassiveBuff != null)
			{
				if (builder.ToString().Length > 0)
					builder.Append("\n\n");
				builder.Append($"[color={Colors.Passive}]Passive:[/color] ").Append(Settings.PassiveBuff.Settings.Description);
			}
			return builder.ToString();
		}
	}

	public void Learn()
	{
		IsLearned = true;
		SkillTreeManager.Singleton.EmitSignal(SkillTreeManager.SignalName.SkillUp, this);
	}

	public void Unlearn()
	{
		IsLearned = false;
		SkillTreeManager.Singleton.EmitSignal(SkillTreeManager.SignalName.SkillDown, this);
	}

	public static string MakeDescription(params string[] strings) => CastUtils.MakeDescription(strings);

	public enum Type
	{
		Active,
		Passive,
	}
}

public class SkillWrapper
{
	public readonly Type skillType;
	public BaseSkill.SkillSettings settings;

	public SkillWrapper(Type type)
	{
		skillType = type;
	}

	public void EnsureSettingsAvailable()
	{
		if (settings == null)
		{
			var instance = (BaseSkill)Activator.CreateInstance(skillType);
			settings = instance.Settings;
		}
	}

	public string GetName()
	{
		EnsureSettingsAvailable();
		return settings.FriendlyName;
	}

	public bool Is(BaseSkill skill)
	{
		return skill.GetType() == skillType;
	}

	public static SkillWrapper Of<T>() where T : BaseSkill
	{
		return new SkillWrapper(typeof(T));
	}
}
