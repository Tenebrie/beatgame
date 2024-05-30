using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastLightningStorm : BaseCast
{
	public BossCastLightningStorm(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			HoldTime = 4,
			RecastTime = 0,
		};
	}

	public override void _Ready()
	{
		Music.Singleton.BeatTick += OnBeatTick;
	}

	public override void _ExitTree()
	{
		Music.Singleton.BeatTick -= OnBeatTick;
	}

	void OnBeatTick(BeatTime time)
	{
		if ((time != BeatTime.One && time != BeatTime.Half) || !IsCasting)
			return;

		var positions = GetSpawnPositions();
		foreach (var pos in positions)
		{
			var area = this.CreateGroundCircularArea(pos);
			area.GrowTime = 1;
			area.Radius = 2;
			area.OnFinishedPerTargetCallback = (BaseUnit unit) =>
			{
				unit.Health.Damage(50);
			};
		}
	}

	List<Vector3> GetSpawnPositions()
	{
		var list = new List<Vector3>();
		var targets = BaseUnit.AllUnits.Where(unit => unit.Alliance.HostileTo(Parent.Alliance)).ToList();
		foreach (var facing in CastUtils.AllArenaFacings())
		{
			list.Add(this.GetArenaEdgePosition(new Vector3(GD.Randf() * 2 - 1, 0, GD.Randf()), facing));
		}
		foreach (var target in targets)
		{
			list.Add(new Vector3(target.Position.X, 0, target.Position.Z));
		}
		return list;
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		OnBeatTick(BeatTime.One);
	}

	protected override void OnCastCompleted(CastTargetData _) { }
}