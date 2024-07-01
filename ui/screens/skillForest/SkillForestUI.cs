using Godot;

namespace Project;

public partial class SkillForestUI : Control
{
	[Export] HBoxContainer container;
	[Export] Label skillPointsLabel;

	int skillTreeCount = 0;

	public override void _EnterTree()
	{
		instance = this;
		Visible = false;

		SkillTreeManager.Singleton.SkillPointsChanged += OnSkillPointsChanged;
		SkillTreeManager.Singleton.SkillTreeUnlocked += OnSkillTreeUnlocked;
		OnSkillPointsChanged();
	}

	void OnSkillPointsChanged()
	{
		skillPointsLabel.Text = $"Skill points: {SkillTreeManager.Singleton.SkillPoints}";
	}

	void OnSkillTreeUnlocked(SkillGroup group)
	{
		var tree = Lib.LoadScene(Lib.UI.SkillTree).Instantiate<SkillTreeUI>();
		tree.SkillGroup = group;
		container.AddChild(tree);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleSkillForest".ToStringName()))
		{
			Visible = !Visible;
		}
		if (@event.IsActionPressed("Escape".ToStringName()) && Visible)
		{
			Visible = false;
			GetViewport().SetInputAsHandled();
		}
	}

	private static SkillForestUI instance = null;
	public static SkillForestUI Singleton
	{
		get => instance;
	}
}
