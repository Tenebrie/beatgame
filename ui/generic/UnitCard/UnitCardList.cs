using Godot;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project;
public partial class UnitCardList : VBoxContainer
{
	private readonly static PackedScene UnitCardResource = GD.Load<PackedScene>("uid://c2m17h2hubigh");

	public Dictionary<BaseUnit, UnitCard> UnitCards = new();

	public override void _Ready()
	{
		foreach (var child in GetChildren())
			RemoveChild(child);
	}

	public void TrackUnit(BaseUnit unit)
	{
		var unitCard = UnitCardResource.Instantiate<UnitCard>();
		unitCard.TrackUnit(unit);
		AddChild(unitCard);
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
		unitCard.QueueFree();
	}
}
