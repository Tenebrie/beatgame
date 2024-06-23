using System.Diagnostics;
using System.Reflection;
using Godot;

namespace Project;
public partial class CombatUI : Control
{
	[Export] ResourceBar HealthBar;
	[Export] ResourceBar ManaBar;
	[Export] UnitCardList AlliedUnitList;
	[Export] UnitCardList HostileUnitList;
	[Export] UnitCardList HostileBossList;
	[Export] CastBarGroup BossCastBarGroup;
	[Export] BuffContainer PlayerBuffContainer;

	public override void _Ready()
	{
		GetTree().Root.ContentScaleFactor = DisplayServer.ScreenGetScale();
		SignalBus.Singleton.UnitCreated += OnUnitCreated;
		SignalBus.Singleton.UnitDestroyed += OnUnitDestroyed;
	}

	private void OnUnitCreated(BaseUnit unit)
	{
		if (unit is PlayerController)
		{
			HealthBar.TrackUnit(unit, ObjectResourceType.Health);
			ManaBar.TrackUnit(unit, ObjectResourceType.Mana);
			PlayerBuffContainer.TrackUnit(unit);
		}
		else if (unit.Alliance == UnitAlliance.Player)
		{
			AlliedUnitList.TrackUnit(unit);
		}
		else if (unit.Alliance == UnitAlliance.Hostile && unit is BossAeriel)
		{
			HostileBossList.TrackUnit(unit);
			BossCastBarGroup.TrackUnit(unit);
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
		else if (unit.Alliance == UnitAlliance.Hostile && unit is BossAeriel)
		{
			HostileBossList.UntrackUnit(unit);
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
