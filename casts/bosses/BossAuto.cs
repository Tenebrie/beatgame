using Godot;

namespace Project;
public partial class BossAuto : BaseCast
{
	public BossAuto(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Zap",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			HoldTime = 0,
			RecastTime = 0,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		this.CreateZapEffect(Parent.CastAimPosition, target.HostileUnit.CastAimPosition);
		target.HostileUnit.Health.Damage(1, this);
		AddChild(GD.Load<PackedScene>("res://assets/PolygonDungeon/Prefabs/Characters/Character_Goblin_Male.tscn").Instantiate());
		AddChild(GD.Load<PackedScene>("res://assets/PolygonDungeon/Prefabs/Characters/Character_Goblin_WarChief.tscn").Instantiate());
		AddChild(GD.Load<PackedScene>("res://assets/PolygonDungeon/Prefabs/Characters/Character_Goblin_Warrior_Male.tscn").Instantiate());
	}
}