using Godot;

public partial class Message : PanelContainer
{
	public string Body
	{
		get => BodyLabel.Text;
		set => BodyLabel.Text = value;
	}

	public Label BodyLabel;

	public override void _EnterTree()
	{
		BodyLabel = GetNode<Label>("BodyLabel");
		BodyLabel.Text = Body;
	}
}
