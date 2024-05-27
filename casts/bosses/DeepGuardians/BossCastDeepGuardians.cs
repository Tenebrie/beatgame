using System;
using System.Collections.Generic;
using Godot;

namespace Project;
public partial class BossCastDeepGuardians : BaseCast
{
	private bool SpawningGuardians;
	private readonly Queue<DeepGuardian> GuardianQueue = new();
	private PackedScene PackedGuardian = GD.Load<PackedScene>("res://casts/bosses/DeepGuardians/tokens/DeepGuardian.tscn");

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
		var instance = PackedGuardian.Instantiate<DeepGuardian>();
		GetTree().CurrentScene.AddChild(instance);
		instance.RotationDegrees = new Vector3(0, 90, 0);
		var effectiveIndex = index - 1.5f;
		instance.Position = new Vector3(8 * effectiveIndex, 0, 16);
		GuardianQueue.Enqueue(instance);

		// TODO: Separate rects for center grow and directional grow
		var rect = this.CreateGroundRectangularArea(instance.Position);
		rect.Rotate(Vector3.Up, (float)Math.PI / 2);
		rect.Width = 8;
		rect.Length = 32;
		rect.GrowTime = 4;
		rect.LengthOrigin = GroundAreaRect.Origin.Start;

		if (GuardianQueue.Count >= 4)
			SpawningGuardians = false;
	}

	private void ReleaseGuardian()
	{
		var guardian = GuardianQueue.Dequeue();
		if (guardian == null)
			return;

		guardian.Activate();
	}

	protected override void CastOnNone() { }
}