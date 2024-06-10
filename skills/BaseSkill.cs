using System.Collections.Generic;
using System.Text;
using Godot;

namespace Project;

public partial class BaseSkill : Node
{
	public class SkillSettings
	{
		public string FriendlyName;
		public string IconPath = null;
		public CastFactory ActiveCast;
		public BuffFactory PassiveBuff;
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
			if (Settings.ActiveCast != null)
			{
				var settings = Settings.ActiveCast.Settings;
				// Description
				builder.Append($"[color={Colors.Active}]Active:[/color] ").Append(settings.Description);

				// Input type
				builder.Append($"\n[color={Colors.Passive}]Input:[/color] ");
				if (settings.InputType == CastInputType.Instant)
					builder.Append("Instant");
				else if (settings.InputType == CastInputType.AutoRelease)
					builder.Append($"Cast ({settings.HoldTime} beat{(settings.HoldTime == 1 ? "" : "s")})");
				else if (settings.InputType == CastInputType.HoldRelease)
					builder.Append($"Hold ({settings.HoldTime} beat{(settings.HoldTime == 1 ? "" : "s")})");

				// Resource cost
				if (settings.ResourceCost[ObjectResourceType.Mana] > 0)
					builder.Append($"\n[color={Colors.Mana}]Mana cost:[/color] ").Append(settings.ResourceCost[ObjectResourceType.Mana]);
			}
			if (Settings.ActiveCast != null && Settings.PassiveBuff != null)
				builder.Append("\n\n");
			if (Settings.PassiveBuff != null)
			{
				builder.Append($"[color={Colors.Passive}]Passive:[/color] ").Append(Settings.PassiveBuff.Settings.Description);
			}
			return builder.ToString();
		}
	}

	public enum Type
	{
		Active,
		Passive,
	}
}
