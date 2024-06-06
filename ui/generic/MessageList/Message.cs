using Godot;

namespace Project;

public partial class Message : PanelContainer
{
	float alpha = 2.0f;
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

	public override void _Process(double delta)
	{
		alpha -= (float)delta;
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, alpha);
		if (alpha <= 0.00f)
			QueueFree();
	}
}
