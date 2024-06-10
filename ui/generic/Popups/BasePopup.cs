using Godot;

namespace Project;

public partial class BasePopup : Control
{
	Anchor horizontalAnchor = Anchor.Begin;
	Anchor verticalAnchor = Anchor.End;

	RichTextLabel label;
	MarginContainer marginContainer;
	Vector2 virtualPosition = new();
	Vector2 userOffset = new();
	Vector2 anchorOffset = new();
	bool isFollowingMouse = false;

	public override void _Ready()
	{
		label = GetNode<RichTextLabel>("Label");
		marginContainer = GetNode<MarginContainer>("Label/MarginContainer");
		RecalculateOffsets();
	}

	public void MoveTo(Vector2 pos)
	{
		virtualPosition = pos + GetScreenSizeOffset(pos);
	}

	public void SetOffset(Vector2 offset)
	{
		userOffset = offset;
	}

	public virtual void SetBody(string text)
	{
		label.Text = text;
		RecalculateOffsets();
	}

	public void SetHorizontalAnchor(Anchor value)
	{
		horizontalAnchor = value;
		RecalculateOffsets();
	}

	public void SetVerticalAnchor(Anchor value)
	{
		verticalAnchor = value;
		RecalculateOffsets();
	}

	public void FollowMouse()
	{
		isFollowingMouse = true;
	}

	protected void RecalculateOffsets()
	{
		Vector2 offset = new();
		if (horizontalAnchor == Anchor.End)
			offset.X = -marginContainer.Size.X;
		if (verticalAnchor == Anchor.Begin)
			offset.Y = marginContainer.Size.Y / 2;
		if (verticalAnchor == Anchor.End)
			offset.Y = -marginContainer.Size.Y / 2;

		anchorOffset = offset;
	}

	public async void MakeVisible()
	{
		await ToSignal(GetTree().CreateTimer(0), "timeout");
		Visible = true;
		RecalculateOffsets();
		UpdatePosition();
	}

	public void MakeHidden()
	{
		Visible = false;
	}

	Vector2 GetScreenSizeOffset(Vector2 pos)
	{
		Vector2 offset = new();
		var popupSize = marginContainer.Size;
		var screenSize = GetViewport().GetVisibleRect().Size;

		var leftEdge = pos.X;
		var rightEdge = pos.X + popupSize.X;
		if (rightEdge > screenSize.X)
			offset.X = -(rightEdge - screenSize.X);
		else if (leftEdge < 0)
			offset.X = -leftEdge;

		var topEdge = pos.Y - popupSize.Y / 2;
		var bottomEdge = pos.Y + popupSize.Y / 2;
		if (bottomEdge > screenSize.Y)
			offset.Y = -(bottomEdge - screenSize.Y);
		else if (topEdge < 0)
			offset.Y = -topEdge;

		return offset;
	}

	public override void _Process(double delta)
	{
		if (!Visible)
			return;

		UpdatePosition();
	}

	void UpdatePosition()
	{
		if (isFollowingMouse)
			virtualPosition = GetViewport().GetMousePosition();

		var pos = virtualPosition + userOffset + anchorOffset;

		if (isFollowingMouse)
			pos += GetScreenSizeOffset(pos);

		Position = pos;
	}
}
