using Godot;
using System.Collections.Generic;

namespace Project;
public partial class UnitCardList : Control
{
	[Export]
	public AnimatedVBoxContainer.Animation AnimationType = AnimatedVBoxContainer.Animation.SlideLeft;
	public Dictionary<BaseUnit, UnitCard> UnitCards = new();
	private AnimatedVBoxContainer Container;

	public override void _Ready()
	{
		Container = GetNode<AnimatedVBoxContainer>("AnimatedVBoxContainer");
		Container.AnimationType = AnimationType;
	}

	public void TrackUnit(BaseUnit unit)
	{
		var unitCard = Lib.Scene(Lib.UI.UnitCard).Instantiate<UnitCard>();
		unitCard.TrackUnit(unit);
		Container.AddChild(unitCard);
		UnitCards.Add(unit, unitCard);
	}

	public void UntrackUnit(BaseUnit unit)
	{
		var cardFound = UnitCards.TryGetValue(unit, out UnitCard unitCard);
		if (!cardFound)
		{
			return;
		}

		unitCard.UntrackUnit();
		Container.FreeChildWithAnimation(unitCard);
	}
}
