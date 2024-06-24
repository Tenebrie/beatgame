using System;
using System.Linq;

namespace Project;

public partial class NukeOfTheGreatTree : BaseCast
{
	public NukeOfTheGreatTree(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Nuke of the Great Tree",
			Description = MakeDescription($"Deal {{1000000}} damage to all enemies."),
			LoreDescription = "You know what a nuke is.",
			IconPath = "res://assets/icons/SpellBook06_57.png",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Free,
			HoldTime = 8,
		};
	}

	protected override void OnCastCompleted(CastTargetData targetData)
	{
		var targets = BaseUnit.AllUnits.Where(unit => unit.HostileTo(Parent));
		foreach (var target in targets)
			target.Health.Damage(1000000, this);

		var effect = Lib.LoadScene(Lib.Effect.NukeOfTheGreatTree).Instantiate<SimpleParticleEffect>();
		GetTree().CurrentScene.AddChild(effect);
		effect.Position = targetData.HostileUnit.GlobalPosition;
		effect.SetLifetime(5f);
	}
}
