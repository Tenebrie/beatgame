using Godot;

namespace Project;
public partial class Fireblast : BaseCast
{
	public Fireblast(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Fireblast",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Quarter | BeatTime.Eighth,
			RecastTime = 0.1f,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var impact = Lib.Scene(Lib.Effect.FireballProjectileImpact).Instantiate() as ProjectileImpact;
		GetTree().Root.AddChild(impact);
		impact.GlobalPosition = Parent.GlobalPosition;

		var damage = 5f;
		target.HostileUnit.Health.Damage(damage, this);
	}
}