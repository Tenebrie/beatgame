using Godot;

namespace Project;

public partial class UnitCard : Control
{
	[Export] Control MouseSurface;
	[Export] Label NameLabel;
	[Export] ResourceBar HealthBar;
	[Export] Panel BackgroundPanel;
	[Export] BuffContainer BuffCard;

	bool isHovered = false;
	bool isPressed = false;
	bool isTargeted = false;
	BaseUnit trackedUnit;

	Color hoveredAlliedColor = new(0.1f, 0.4f, 0.4f);
	Color hoveredHostileColor = new(0.4f, 0.1f, 0.1f);
	Color targetedAlliedColor = new(0.1f, 0.7f, 0.7f);
	Color targetedHostileColor = new(0.7f, 0.1f, 0.1f);
	Color normalColor = new(0.5f, 0.5f, 0.5f);

	public override void _EnterTree()
	{
		MouseSurface.MouseEntered += OnMouseEntered;
		MouseSurface.MouseExited += OnMouseExited;
		SignalBus.Singleton.ObjectTargeted += OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted += OnObjectUntargeted;
		UpdateHoverValue();
	}

	public override void _ExitTree()
	{
		SignalBus.Singleton.ObjectTargeted -= OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted -= OnObjectUntargeted;
	}

	void OnObjectTargeted(BaseUnit unit, TargetedUnitAlliance alliance)
	{
		if (isTargeted && !trackedUnit.Alliance.EqualsTo(alliance))
			return;

		isTargeted = unit == trackedUnit;
		UpdateHoverValue();
	}

	void OnObjectUntargeted(TargetedUnitAlliance alliance)
	{
		if (alliance.EqualsTo(trackedUnit.Alliance))
			isTargeted = false;
		UpdateHoverValue();
	}

	public override void _Input(InputEvent @event)
	{
		if (isHovered && @event.IsActionPressed("MouseInteract"))
		{
			isPressed = true;
			UpdateHoverValue();
		}
		else if (@event.IsActionReleased("MouseInteract"))
		{
			isPressed = false;
			if (isHovered)
				trackedUnit.Targetable.MakeTargeted();
			UpdateHoverValue();
		}
	}

	void OnMouseEntered()
	{
		isHovered = true;
		UpdateHoverValue();
	}

	void OnMouseExited()
	{
		isHovered = false;
		UpdateHoverValue();
	}

	void UpdateHoverValue()
	{
		var stylebox = (StyleBoxFlat)BackgroundPanel.GetThemeStylebox("panel");
		var color = normalColor;
		if (isTargeted && trackedUnit.Alliance != UnitAlliance.Hostile)
			color = targetedAlliedColor;
		else if (isTargeted && trackedUnit.Alliance == UnitAlliance.Hostile)
			color = targetedHostileColor;
		else if (isHovered && trackedUnit.Alliance != UnitAlliance.Hostile)
			color = hoveredAlliedColor;
		else if (isHovered && trackedUnit.Alliance == UnitAlliance.Hostile)
			color = hoveredHostileColor;

		color = new Color(color);
		if (isPressed)
			color *= 0.75f;

		stylebox.BorderColor = color;
		BackgroundPanel.AddThemeStyleboxOverride("panel", stylebox);
	}

	public void TrackUnit(BaseUnit unit)
	{
		trackedUnit = unit;

		NameLabel.Text = trackedUnit.FriendlyName;
		HealthBar.TrackUnit(trackedUnit, ObjectResourceType.Health);
		BuffCard.TrackUnit(trackedUnit);
	}

	public void UntrackUnit()
	{
		HealthBar.UntrackUnit();
		BuffCard.UntrackUnit();
	}
}
