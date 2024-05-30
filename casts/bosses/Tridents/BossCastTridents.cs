using System;
using System.Collections.Generic;
using Godot;

namespace Project;
public partial class BossCastTridents : BaseCast
{
	// On each side of the arena, 3 (5?) tridents appear, alternating direction, combing through the entire side of the arena, but not the corners
	// Then, corners explode with a large aoe
	// In the middle of the arena, a puddle of water should exist

	readonly CastAutomata<State> stateMachine;
	readonly List<AnimatedTrident> tridents = new();

	public BossCastTridents(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			TargetAlliances = new() { UnitAlliance.Hostile },
			HoldTime = 4,
			RecastTime = 0,
		};

		stateMachine = new(State.Spawning, OnStateTransition, new() {
			(3, State.Charging),
			(5, State.Moving),
			(13, State.Despawning),
			(15, State.Finished),
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
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.7f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.8f + 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
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
			rect.GrowTime = 2;
			rect.Alliance = UnitAlliance.Hostile;
			rect.OnFinishedCallback = () =>
			{
				foreach (var unit in rect.GetUnitsInside())
				{
					unit.Health.Damage(30);
					unit.ForcefulMovement.Push(2, unit.Position - rect.Position, 0.5f);
				}
			};
		}
	}

	void OnStateTransition(State state)
	{
		if (state == State.Charging)
		{
			var trident = Lib.Scene(Lib.Token.AnimatedTrident);
			foreach (var (position, rotation) in GetSpawnPositions())
			{
				var instance = trident.Instantiate<AnimatedTrident>();
				GetTree().CurrentScene.AddChild(instance);
				instance.Position = position;
				instance.Rotate(Vector3.Up, rotation);
				tridents.Add(instance);
			}
		}
		if (state == State.Moving)
		{
			foreach (var trident in tridents)
			{
				trident.Activate(this.GetArenaSize() * 2, 8 * Music.Singleton.SecondsPerBeat);
			}
		}
		if (state == State.Despawning)
		{
			foreach (var trident in tridents)
			{
				trident.SetActive(false);
				var rect = this.CreateGroundCircularArea(trident.Position);
				rect.Radius = 2;
				rect.GrowTime = 2;
				rect.Alliance = UnitAlliance.Hostile;
				rect.OnFinishedCallback = () =>
				{
					foreach (var unit in rect.GetUnitsInside())
					{
						unit.Health.Damage(30);
						unit.ForcefulMovement.Push(2, unit.Position - rect.Position, 0.5f);
					}
				};
			}
		}
		if (state == State.Finished)
		{
			foreach (var trident in tridents)
			{
				trident.QueueFree();
			}
			tridents.Clear();
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