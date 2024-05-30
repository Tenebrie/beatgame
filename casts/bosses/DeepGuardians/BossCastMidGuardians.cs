using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastMidGuardians : BaseCast
{
	private bool SpawningGuardians;
	private readonly Queue<DeepGuardian> GuardianQueue = new();

	public ArenaFacing Orientation;
	public bool Mirrored;

	public BossCastMidGuardians(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			TargetAlliances = new() { UnitAlliance.Hostile },
			HoldTime = 4,
			RecastTime = 0,
		};
	}

	public void RandomizeOrientation()
	{
		Orientation = (ArenaFacing)(Math.Abs((int)GD.Randi()) % 4);
		Mirrored = GD.Randf() < 0.5f;
	}

	public void AdvanceOrientation()
	{
		if (Mirrored)
			Orientation += 1;
		else
			Orientation -= 1;

		if ((int)Orientation > 3)
			Orientation = 0;
		else if ((int)Orientation < 0)
			Orientation = (ArenaFacing)3;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Music.Singleton.BeatTick += OnBeatTick;
		GuardianQueue.EnsureCapacity(4);
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		SpawningGuardians = true;
		SpawnGuardian();
	}

	private void OnBeatTick(BeatTime time)
	{
		if (time != BeatTime.One || !IsCasting)
			return;

		if (SpawningGuardians)
			SpawnGuardian();
		else
			ReleaseGuardian();
	}

	private void SpawnGuardian()
	{
		var index = GuardianQueue.Count;
		var guardian = Lib.Scene(Lib.Token.DeepGuardian).Instantiate<DeepGuardian>();
		GetTree().CurrentScene.AddChild(guardian);

		guardian.Position = this.GetArenaEdgePosition(new Vector3(0, 0, 0), Orientation);

		GuardianQueue.Enqueue(guardian);

		var rect = this.CreateGroundRectangularArea(guardian.Position);

		guardian.Rotate(Vector3.Up, this.GetArenaFacingAngle(Orientation));
		rect.Rotate(Vector3.Up, this.GetArenaFacingAngle(Orientation));

		rect.Width = 16;
		rect.Length = 32;
		rect.GrowTime = 4;
		rect.LengthOrigin = GroundAreaRect.Origin.Start;
		guardian.AttachRect(rect);

		AdvanceOrientation();

		if (GuardianQueue.Count >= 2)
			SpawningGuardians = false;
	}

	private void ReleaseGuardian()
	{
		if (GuardianQueue.Count == 0)
			return;

		var guardian = GuardianQueue.Dequeue();
		guardian.Activate();
	}

	protected override void OnCastCompleted(CastTargetData _) { }
}