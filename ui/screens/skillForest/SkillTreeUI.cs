using Godot;
using System;

namespace Project;

public partial class SkillTreeUI : Control
{
	[Export]
	public SkillGroup SkillGroup = SkillGroup.Tank;

	public override void _Ready()
	{
		Vector2 GridSize = new(80, 150);
		Vector2 DrawOffset = new(this.Size.X / 2 - 38, 10);

		var tree = SkillTreeManager.Singleton.SkillTrees.Find(tree => tree.Group == SkillGroup);

		foreach (var link in tree.Links)
		{
			var linkUI = Lib.Scene(Lib.UI.SkillLink).Instantiate<SkillLinkUI>();
			linkUI.SkillLink = link;
			linkUI.Position = new Vector2(link.Source.PosX, link.Source.Depth) * GridSize + DrawOffset + new Vector2(38, 85);
			linkUI.Target = new Vector2(link.Target.PosX - link.Source.PosX, link.Target.Depth - link.Source.Depth) * GridSize + new Vector2(0, -60);
			AddChild(linkUI);
		}

		foreach (var skill in tree.Skills)
		{
			var button = Lib.Scene(Lib.UI.SkillButton).Instantiate<SkillButton>();
			button.Position = new Vector2(skill.PosX, skill.Depth) * GridSize + DrawOffset;
			button.AssignSkill(skill);
			AddChild(button);
		}
	}

	public override void _Process(double delta)
	{
	}
}
