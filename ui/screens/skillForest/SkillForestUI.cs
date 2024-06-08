using Godot;

namespace Project;

public partial class SkillForestUI : Control
{
	Label SkillPointsLabel;

	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		SkillPointsLabel = GetNode<Label>("SkillPointsLabel");
		SkillTreeManager.Singleton.SkillPointsChanged += OnSkillPointsChanged;
		OnSkillPointsChanged();
	}

	public void OnSkillPointsChanged()
	{
		SkillPointsLabel.Text = $"Skill points: {SkillTreeManager.Singleton.SkillPoints}";
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleSkillForest"))
		{
			Visible = !Visible;
		}
	}

	private static SkillForestUI instance = null;
	public static SkillForestUI Singleton
	{
		get => instance;
	}
}
