using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastTridents : BaseCast
{
	readonly CastAutomata<State> stateMachine;
	readonly List<AnimatedTrident> AllTridents = new();

	public BossCastTridents(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Animated Tridents",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			HoldTime = 32,
			PrepareTime = 8,
			RecastTime = 0,
		};

		stateMachine = new(State.Spawning, OnStateTransition, new() {
			(9, State.Charging),
			(17, State.Moving),
			(33, State.Despawning),
			(41, State.Finished),
		});
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Music.Singleton.BeatTick += stateMachine.Tick;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Music.Singleton.BeatTick -= stateMachine.Tick;
	}

	List<(Vector3 position, float rotation)> GetSpawnPositions()
	{
		var list = new List<(Vector3, float)>();
		foreach (var facing in CastUtils.AllArenaFacings())
		{
			var angle = this.GetArenaFacingAngle(facing);
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.1f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.2f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.3f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.4f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.5f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.6f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
			// list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.7f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			// list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.8f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
		}
		return list;
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		stateMachine.Start();
		foreach (var (position, _) in GetSpawnPositions())
		{
			var rect = this.CreateGroundCircularArea(position);
			rect.Radius = 2;
			rect.GrowTime = 8;
			rect.Alliance = UnitAlliance.Hostile;
			rect.OnFinishedPerTargetCallback = (unit) =>
			{
				unit.Health.Damage(30, this);
				unit.ForcefulMovement.Push(2, unit.Position - rect.Position, 1);
			};
		}
	}

	void OnStateTransition(State state)
	{
		var tridents = AllTridents.Where(trident => IsInstanceValid(trident)).ToList();
		if (state == State.Charging)
		{
			var trident = Lib.LoadScene(Lib.Token.AnimatedTrident);
			foreach (var (position, rotation) in GetSpawnPositions())
			{
				var instance = trident.Instantiate<AnimatedTrident>();
				instance.Position = position;
				instance.Rotate(Vector3.Up, rotation);
				GetTree().CurrentScene.AddChild(instance);
				AllTridents.Add(instance);
			}
		}
		if (state == State.Moving)
		{
			foreach (var trident in tridents)
			{
				trident.Activate(this.GetArenaSize() * 2, 16 * Music.Singleton.SecondsPerBeat);
			}
		}
		if (state == State.Despawning)
		{
			foreach (var trident in tridents)
			{
				trident.SetActive(false);
				var rect = this.CreateGroundCircularArea(trident.Position);
				rect.Radius = 4;
				rect.GrowTime = 8;
				rect.Alliance = UnitAlliance.Hostile;
				rect.OnFinishedPerTargetCallback = (unit) =>
				{
					unit.Health.Damage(30, this);
					unit.ForcefulMovement.Push(2, unit.Position - rect.Position, 1f);
				};
				rect.OnFinishedCallback = () => trident.QueueFree();
			}
		}
	}

	protected override void OnCastCompleted(CastTargetData _) { }

	private enum State
	{
		Spawning,
		Charging,
		Moving,
		Despawning,
		Finished,
	}
}