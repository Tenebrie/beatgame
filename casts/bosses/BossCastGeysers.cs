using System.Collections.Generic;
using Godot;

namespace Project;

public partial class BossCastGeysers : BaseCast
{
	public BossCastGeysers(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Geysers",
			InputType = CastInputType.AutoRelease,
			HoldTime = 8,
			RecastTime = 0,
		};
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		var arenaSize = this.GetArenaSize();
		List<Vector3> spawns = new()
		{
			new Vector3(+arenaSize * 0.75f, 0, +arenaSize * 0.75f),
			new Vector3(+arenaSize * 0.75f, 0, -arenaSize * 0.75f),
			new Vector3(-arenaSize * 0.75f, 0, +arenaSize * 0.75f),
			new Vector3(-arenaSize * 0.75f, 0, -arenaSize * 0.75f),
		};

		List<ObjectForcefulMovement.ContinuousMovement> movements = new();
		foreach (var spawn in spawns)
		{
			var circle = this.CreateGroundCircularArea(spawn);
			circle.GrowTime = Settings.HoldTime;
			circle.Alliance = UnitAlliance.Neutral;
			circle.TargetValidator = (unit) => unit.Alliance == UnitAlliance.Player;
			circle.OnFinishedPerTargetCallback = (unit) =>
			{
				unit.ForcefulMovement.PushContinuously(() => -unit.Velocity.Y, () => Vector3.Up);
			};
		}
	}
}