using Godot;

namespace Project;
public partial class BerserkersCatharsis : BaseCast
{
	public const float DamagePerStack = 30;
	public const float HealthPerStack = 10;

	public BerserkersCatharsis(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Berserker's Catharsis",
			Description = MakeDescription(
				$"Lose all your stacks of {{Berserker's Rage}}, deal {{{DamagePerStack}}} Spirit damage to your target per stack lost",
				$"and restore {{{HealthPerStack}}} Health per stack lost."
			),
			LoreDescription = "Release your fury.",
			IconPath = "res://assets/icons/SpellBook06_65.png",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole | BeatTime.Half,
			RecastTime = 32,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 50;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var stacks = Parent.Buffs.Stacks<SkillBerserkersRage.RageBuff>();
		Parent.Buffs.RemoveAll<SkillBerserkersRage.RageBuff>();
		target.HostileUnit.Health.Damage(stacks * DamagePerStack, this);
		Parent.Health.Restore(stacks * HealthPerStack, this);

		this.CreateZapEffect(Parent.GlobalCastAimPosition, target.HostileUnit.GlobalCastAimPosition);
		this.CreateZapEffect(Parent.GlobalCastAimPosition, target.HostileUnit.GlobalCastAimPosition);
		this.CreateZapEffect(Parent.GlobalCastAimPosition, target.HostileUnit.GlobalCastAimPosition);
	}
}