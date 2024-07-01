using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastTinyStorm : BaseCast
{
	int CurrentCircleIndex = 0;
	double ImpactIndex = 0;

	public BossCastTinyStorm(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Tiny Storm",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			HoldTime = 2,
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

		if (CurrentCircleIndex >= 3)
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
			var area = this.CreateCircularTelegraph(pos);
			area.Settings.GrowTime = (float)(ImpactIndex - Music.Singleton.BeatIndex);
			area.Settings.Radius = 1;
			area.Settings.OnFinishedPerTargetCallback = (BaseUnit unit) =>
			{
				unit.Health.Damage(20, this);
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