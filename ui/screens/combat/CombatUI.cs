using System.Diagnostics;
using System.Reflection;
using Godot;

namespace Project;
public partial class CombatUI : Control
{
	private ResourceBar HealthBar;
	private ResourceBar ManaBar;
	private UnitCardList AlliedUnitList;
	private UnitCardList HostileUnitList;
	private UnitCardList HostileBossList;
	private CastBarGroup BossCastBarGroup;

	public override void _Ready()
	{
		GetTree().Root.ContentScaleFactor = DisplayServer.ScreenGetScale();
		HealthBar = GetNode<ResourceBar>("HealthBar");
		ManaBar = GetNode<ResourceBar>("ManaBar");
		AlliedUnitList = GetNode<UnitCardList>("AllyTargeting");
		HostileUnitList = GetNode<UnitCardList>("HostileTargeting");
		HostileBossList = GetNode<UnitCardList>("BossTargeting");
		BossCastBarGroup = GetNode<CastBarGroup>("BossCastBarGroup");
		SignalBus.Singleton.UnitCreated += OnUnitCreated;
		SignalBus.Singleton.UnitDestroyed += OnUnitDestroyed;
	}

	private void OnUnitCreated(BaseUnit unit)
	{
		if (unit is PlayerController)
		{
			HealthBar.TrackUnit(unit, ObjectResourceType.Health);
			ManaBar.TrackUnit(unit, ObjectResourceType.Mana);
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
