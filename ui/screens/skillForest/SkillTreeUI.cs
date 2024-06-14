using Godot;
using System;

namespace Project;

public partial class SkillTreeUI : Control
{
	[Export]
	public SkillGroup SkillGroup = SkillGroup.Tank;

	bool TreeReady = false;

	public override void _Ready()
	{
		GenerateTree();
	}

	void GenerateTree()
	{
		if (this.Size.X == 0)
			return;

		TreeReady = true;
		Vector2 GridSize = new(80, 150);
		Vector2 DrawOffset = new(this.Size.X / 2 - 38, 10);

		var tree = SkillTreeManager.Singleton.SkillTrees.Find(tree => tree.Group == SkillGroup);
		if (tree == null)
			return;

		foreach (var link in tree.Links)
		{
			var linkUI = Lib.LoadScene(Lib.UI.SkillLink).Instantiate<SkillLinkVisual>();
			linkUI.SkillLink = link;
			linkUI.Position = new Vector2(link.Source.PosX, link.Source.Depth) * GridSize + DrawOffset + new Vector2(38, 85);
			linkUI.Target = new Vector2(link.Target.PosX - link.Source.PosX, link.Target.Depth - link.Source.Depth) * GridSize + new Vector2(0, -60);
			AddChild(linkUI);
		}

		foreach (var skill in tree.Skills)
		{
			var buttonType = skill.SkillType == BaseSkill.Type.Active ? Lib.UI.ActiveSkillButton : Lib.UI.PassiveSkillButton;
			var button = Lib.LoadScene(buttonType).Instantiate<SkillButton>();
			button.Position = new Vector2(skill.PosX, skill.Depth) * GridSize + DrawOffset;
			button.AssignSkill(skill);
			AddChild(button);
		}
	}

	public override void _Process(double delta)
	{
		if (TreeReady || !Visible)
			return;

		GenerateTree();
	}
}
