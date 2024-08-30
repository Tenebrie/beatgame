using System;
using Godot;

namespace Project;
public partial class BossCastDeepGuardians : BaseCast
{
	private bool SpawningGuardians;
	private int GuardianCount;

	public ArenaFacing Orientation;
	public bool Mirrored;

	public BossCastDeepGuardians(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Deep Guardians",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			TickDuration = 2,
			TickWhilePreparing = true,
			HoldTime = 8,
			RecastTime = 0,
			PrepareTime = 16,
		};
	}

	public void Reset()
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
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		GuardianCount = 0;
		SpawningGuardians = true;
		SpawnGuardian();
	}

	protected override void OnCastTicked(CastTargetData targetData)
	{
		if (SpawningGuardians)
			SpawnGuardian();
	}

	private void SpawnGuardian()
	{
		var guardian = Lib.LoadScene(Lib.Token.DeepGuardian).Instantiate<DeepGuardian>();
		var effectiveIndex = (GuardianCount - 1.5f) * (Mirrored ? -1 : 1);
		guardian.Position = this.GetArenaEdgePosition(new Vector3(0.5f * effectiveIndex, 0, 0), Orientation);
		GetTree().CurrentScene.AddChild(guardian);

		GuardianCount += 1;

		var rect = this.CreateRectangularTelegraph(guardian.Position);

		var angle = this.GetArenaFacingAngle(Orientation);
		guardian.Rotate(Vector3.Up, angle);
		rect.Rotate(Vector3.Up, angle);

		rect.Settings.Width = 8;
		rect.Settings.Length = 32;
		rect.Settings.GrowTime = Settings.PrepareTime;
		rect.Settings.LengthOrigin = RectangularTelegraph.Origin.Start;

		var forward = guardian.ForwardVector;
		rect.Settings.TargetValidator = (target) => target.HostileTo(Parent);
		rect.Settings.OnTargetEntered = (BaseUnit target) =>
		{
			target.Health.Damage(3, this);
		};
		rect.Settings.OnFinishedPerTargetCallback = (BaseUnit target) =>
		{
			target.Health.Damage(50, this);
			target.ForcefulMovement.Push(8, forward, 1);
		};
		rect.Settings.OnFinishedCallback = () =>
		{
			guardian.QueueFree();
		};

		if (GuardianCount >= 4)
			SpawningGuardians = false;
	}
}