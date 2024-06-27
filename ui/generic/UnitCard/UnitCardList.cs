using Godot;
using System.Collections.Generic;

namespace Project;
public partial class UnitCardList : Control
{
	[Export]
	public AnimatedVBoxContainer Container;

	readonly Dictionary<BaseUnit, UnitCard> UnitCards = new();

	public void TrackUnit(BaseUnit unit)
	{
		var unitCard = Lib.LoadScene(Lib.UI.UnitCard).Instantiate<UnitCard>();
		Container.AddChild(unitCard);
		unitCard.TrackUnit(unit);
		UnitCards.Add(unit, unitCard);
	}

	public void UntrackUnit(BaseUnit unit)
	{
		var cardFound = UnitCards.TryGetValue(unit, out UnitCard unitCard);
		if (!cardFound)
			return;

		unitCard.UntrackUnit();
		Container.FreeChildWithAnimation(unitCard);
	}
}
