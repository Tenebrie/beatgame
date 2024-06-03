using Godot;

namespace Project;

public partial class PowerUpLightningOrb : Node3D
{
	PlayerController PlayerInRange;

	Projectile EnergyOrb;
	GroundAreaCircle GroundAreaCircle;
	Area3D TriggerArea;
	public override void _Ready()
	{
		EnergyOrb = GetNode<Projectile>("EnergyOrb");
		TriggerArea = GetNode<Area3D>("TriggerArea");
		TriggerArea.BodyEntered += OnBodyEntered;
		TriggerArea.BodyExited += OnBodyExited;
		GroundAreaCircle = GetNode<GroundAreaCircle>("GroundAreaCircle");
		GroundAreaCircle.Periodic = true;
		GroundAreaCircle.Alliance = UnitAlliance.Neutral;
	}

	void OnBodyEntered(Node3D body)
	{
		if (body is not PlayerController player)
			return;

		PlayerInRange = player;
	}

	void OnBodyExited(Node3D body)
	{
		if (body is not PlayerController player)
			return;

		if (player == PlayerInRange)
			PlayerInRange = null;
	}

	public override void _Process(double delta)
	{
		if (PlayerInRange == null)
			return;

		if (PlayerInRange.Buffs.Stacks<BuffEnergyOrbPower>() >= 5)
			return;

		PlayerInRange.Buffs.Add(new BuffEnergyOrbPower());
		PlayerInRange = null;

		var impact = Lib.Scene(Lib.Effect.EnergyOrbPickupImpact).Instantiate<ProjectileImpact>();
		impact.Position = EnergyOrb.GlobalPosition;
		GetParent().AddChild(impact);

		RemoveChild(GroundAreaCircle);
		var position = GroundAreaCircle.GlobalPosition;
		GetParent().AddChild(GroundAreaCircle);
		GroundAreaCircle.GlobalPosition = position;
		GroundAreaCircle.CleanUp();

		RemoveChild(EnergyOrb);
		GetParent().AddChild(EnergyOrb);
		EnergyOrb.CleanUp();

		QueueFree();
	}
}