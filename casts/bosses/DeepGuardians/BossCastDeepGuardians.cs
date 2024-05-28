using System;
using System.Collections.Generic;
using Godot;

namespace Project;
public partial class BossCastDeepGuardians : BaseCast
{
	private bool SpawningGuardians;
	private readonly Queue<DeepGuardian> GuardianQueue = new();

	public ArenaFacing Orientation;
	public bool Mirrored;

	public BossCastDeepGuardians(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			TargetAlliances = new() { UnitAlliance.Hostile },
			HoldTime = 8,
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
		Orientation += 1 * Math.Sign(GD.Randf() - 0.5f);
		if ((int)Orientation > 3)
			Orientation = 0;
		else if ((int)Orientation < 0)
			Orientation = (ArenaFacing)3;
		Mirrored = GD.Randf() < 0.5f;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Music.Singleton.BeatTick += OnBeatTick;
		GuardianQueue.EnsureCapacity(4);
	}

	protected override void CastStarted(CastTargetData _)
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
		var instance = Lib.Scene(Lib.Token.DeepGuardian).Instantiate<DeepGuardian>();
		GetTree().CurrentScene.AddChild(instance);

		var effectiveIndex = (index - 1.5f) * (Mirrored ? -1 : 1);
		instance.Position = this.RotatePositionToArenaEdge(new Vector3(8 * effectiveIndex, 0, 0), Orientation);

		GuardianQueue.Enqueue(instance);

		var rect = this.CreateGroundRectangularArea(instance.Position);

		instance.Rotate(Vector3.Up, this.GetArenaFacingAngle(Orientation));
		rect.Rotate(Vector3.Up, this.GetArenaFacingAngle(Orientation));

		rect.Width = 8;
		rect.Length = 32;
		rect.GrowTime = 4;
		rect.LengthOrigin = GroundAreaRect.Origin.Start;

		if (GuardianQueue.Count >= 4)
			SpawningGuardians = false;
	}

	private void ReleaseGuardian()
	{
		if (GuardianQueue.Count == 0)
			return;
		var guardian = GuardianQueue.Dequeue();
		guardian.Activate();
	}

	protected override void CastOnNone() { }
}