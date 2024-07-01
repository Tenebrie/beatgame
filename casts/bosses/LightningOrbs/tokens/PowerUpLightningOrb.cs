using Godot;

namespace Project;

public partial class PowerUpLightningOrb : Node3D
{
	PlayerController PlayerInRange;

	Projectile EnergyOrb;
	CircularTelegraph GroundAreaCircle;
	Area3D TriggerArea;

	public override void _Ready()
	{
		EnergyOrb = GetNode<Projectile>("EnergyOrb");
		TriggerArea = GetNode<Area3D>("TriggerArea");
		TriggerArea.BodyEntered += OnBodyEntered;
		TriggerArea.BodyExited += OnBodyExited;
		GroundAreaCircle = GetNode<CircularTelegraph>("GroundAreaCircle");
		GroundAreaCircle.Settings.Periodic = true;
		GroundAreaCircle.Settings.Alliance = UnitAlliance.Neutral;
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
		if (PlayerInRange == null || IsQueuedForDeletion())
			return;

		PlayerInRange.Buffs.Add(new BuffPowerUpLightningOrb());
		PlayerInRange = null;

		var impact = Lib.LoadScene(Lib.Effect.EnergyOrbPickupImpact).Instantiate<ProjectileImpact>();
		impact.Position = EnergyOrb.GlobalPosition;
		GetParent().AddChild(impact);

		var position = GroundAreaCircle.GlobalPosition;
		RemoveChild(GroundAreaCircle);
		GetTree().CurrentScene.AddChild(GroundAreaCircle);
		GroundAreaCircle.Position = position;
		GroundAreaCircle.CleanUp();

		RemoveChild(EnergyOrb);
		GetParent().AddChild(EnergyOrb);
		EnergyOrb.CleanUp();

		QueueFree();
	}
}