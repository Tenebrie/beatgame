using Godot;
using System;

namespace Project;

public partial class SkillTreeUI : Control
{
	[Export]
	public SkillGroup SkillGroup = SkillGroup.Tank;

	public override void _Ready()
	{
		this.Log("Started generating UI");
		var tree = SkillTreeManager.Singleton.SkillTrees.Find(tree => tree.Group == SkillGroup);
		foreach (var skill in tree.Skills)
		{
			var button = Lib.Scene(Lib.UI.SkillButton).Instantiate<SkillButton>();
			button.Position = new Vector2(this.Size.X / 2 + skill.PosX * 100 - 50, skill.Depth * 100);
			button.AssignSkill(skill);
			AddChild(button);
		}
	}

	public override void _Process(double delta)
	{
	}
}
