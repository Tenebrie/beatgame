// $"\n\n((This cast does not interrupt other abilities.))"

using Godot;

namespace Project;

public partial class CastFlagellation : BaseCast
{
	public const float SelfDamage = 20;
	public const float EnemyDamage = 50;

	public CastFlagellation(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Flagellation",
			Description = MakeDescription(
				$"Deal {{{SelfDamage}}} damage to yourself to deal {{{EnemyDamage}}} Spirit damage to your target."
			),
			IconPath = "res://assets/icons/SpellBook06_03.PNG",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole | BeatTime.Half | BeatTime.Quarter,
		};
		Settings.ResourceCost[ObjectResourceType.Health] = SelfDamage;
	}

	protected override void OnCastCompleted(CastTargetData targetData)
	{
		targetData.HostileUnit.Health.Damage(EnemyDamage, this);
		this.CreateZapEffect(Parent.GlobalCastAimPosition, targetData.HostileUnit.GlobalCastAimPosition);

		var effect = Lib.LoadScene(Lib.Effect.ShieldBashImpact).Instantiate<SimpleParticleEffect>();
		GetTree().CurrentScene.AddChild(effect);
		effect.SetLifetime(0.01f);
		effect.Position = Parent.GlobalCastAimPosition;
	}
}