using System.Diagnostics;
using Godot;

namespace Project;
public partial class CombatUI : Control
{
	private ResourceBar HealthBar;
	public override void _Ready()
	{
		HealthBar = GetNode<ResourceBar>("HealthBar");
		SignalBus.GetInstance(this).ResourceChanged += OnResourceChanged;
		SignalBus.GetInstance(this).MaxResourceChanged += OnMaxResourceChanged;
	}

	private void OnResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit is not PlayerController)
			return;

		if (type == ObjectResourceType.Health)
		{
			HealthBar.SetCurrent(value);
		}
	}

	private void OnMaxResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit is not PlayerController)
			return;

		if (type == ObjectResourceType.Health)
		{
			HealthBar.SetMaximum(value);
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
