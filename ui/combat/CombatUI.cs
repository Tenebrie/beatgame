using System.Reflection;
using Godot;

namespace Project;
public partial class CombatUI : Control
{
	private ResourceBar HealthBar;
	private UnitCardList AlliedUnitList;
	private UnitCardList HostileUnitList;
	public override void _Ready()
	{
		HealthBar = GetNode<ResourceBar>("HealthBar");
		AlliedUnitList = GetNode<UnitCardList>("AllyTargeting");
		HostileUnitList = GetNode<UnitCardList>("HostileTargeting");
		SignalBus.GetInstance(this).UnitCreated += OnUnitCreated;
		SignalBus.GetInstance(this).UnitDestroyed += OnUnitDestroyed;
	}

	private void OnUnitCreated(BaseUnit unit)
	{
		if (unit is PlayerController)
		{
			HealthBar.TrackUnit(unit, ObjectResourceType.Health);
		}
		else if (unit.Alliance == UnitAlliance.Player)
		{
			AlliedUnitList.TrackUnit(unit);
		}
		else if (unit.Alliance == UnitAlliance.Hostile)
		{
			HostileUnitList.TrackUnit(unit);
		}
	}

	private void OnUnitDestroyed(BaseUnit unit)
	{
		if (unit is PlayerController)
		{
			HealthBar.UntrackUnit();
		}
		else if (unit.Alliance == UnitAlliance.Player)
		{
			AlliedUnitList.UntrackUnit(unit);
		}
		else if (unit.Alliance == UnitAlliance.Hostile)
		{
			HostileUnitList.UntrackUnit(unit);
		}
	}

	public void SetHealth(float value)
	{

	}

	public void SetMaxHealth(float value)
	{

	}

	public override void _Process(double delta)
	{
	}
}
