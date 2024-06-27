using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastLightningStorm : BaseCast
{
	int CurrentCircleIndex = 0;
	double ImpactIndex = 0;

	public BossCastLightningStorm(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Lightning Storm",
			InputType = CastInputType.AutoRelease,
			HoldTime = 6,
			RecastTime = 0,
			HiddenCastBar = true,
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
		if (!IsCasting)
			return;

		if (CurrentCircleIndex <= 5 && time == BeatTime.Sixteenth)
			return;

		if (CurrentCircleIndex >= 12)
			return;

		Spawn();
	}

	List<Vector3> GetSpawnPositions()
	{
		var list = new List<Vector3>();
		var targets = BaseUnit.AllUnits.Where(unit => unit.HostileTo(Parent)).ToList();
		foreach (var target in targets)
		{
			list.Add(new Vector3(target.Position.X, 0, target.Position.Z));
		}
		return list;
	}

	void Spawn()
	{
		var positions = GetSpawnPositions();
		foreach (var pos in positions)
		{
			var area = this.CreateGroundCircularArea(pos);
			area.GrowTime = (float)(ImpactIndex - Music.Singleton.BeatIndex);
			area.Radius = 1.5f;
			area.OnFinishedPerTargetCallback = (BaseUnit unit) =>
			{
				unit.Health.Damage(30, this);
				this.CreateZapEffect(pos + new Vector3(0.5f, 5, 0.5f), pos);
			};
		}

		CurrentCircleIndex += 1;
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		CurrentCircleIndex = 0;
		ImpactIndex = Music.Singleton.BeatIndex + Settings.HoldTime;
		OnBeatTick(BeatTime.Quarter);
	}

	protected override void OnCastCompleted(CastTargetData _) { }
}