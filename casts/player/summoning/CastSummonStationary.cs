using Godot;

namespace Project;
public partial class CastSummonStationary : BaseCast
{
	public const float BaseMaxHealth = 50;

	public CastSummonStationary(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Summon Totem",
			Description = MakeDescription(
				$"Fighting the powerful entities with your bare spells is a task too taxing. To compensate for being alone, you decide to summon some help.",
				$"\nSummon a stationary ally that casts spells for you.",
				$"The ally has {Colors.Tag(BaseMaxHealth, Colors.Health)} Health and casts a spell every {Colors.Tag(8)} beats.",
				"If it doesn't have other spells available, the ally will use the basic attack.",
				$"\n\n{Colors.Tag("Basic Attack:", Colors.Active)} Deal {Colors.Tag(SummonCastZap.Damage)} Damage to an enemy."
			),
			IconPath = "res://assets/icons/SpellBook06_44.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.Point,
			CastTimings = BeatTime.Free,
			HoldTime = 8,
			RecastTime = 64,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 120;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var summon = Lib.LoadScene(Lib.Unit.StationarySummon).Instantiate<UnitStationarySummon>();
		summon.Position = target.Point + Parent.ForwardVector.Flatten(target.Point.Y).Normalized() * 0.5f;
		summon.Alliance = Parent.Alliance;
		GetTree().CurrentScene.AddChild(summon);

		var health = BaseMaxHealth + Parent.Buffs.State.SummonHealth;
		summon.Health.SetMaxValue(health);
		summon.Health.SetBaseMaxValue(health);

		summon.CastLibrary.Register(new SummonCastZap(summon));
		if (this.HasSkill<SkillSummonFireball>())
		{
			summon.hostility = UnitHostility.Hostile;
			summon.CastLibrary.Register(new Fireball(summon));
		}
	}
}