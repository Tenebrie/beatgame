using System;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastEliteGuardian : BaseCast
{
	public ArenaFacing Orientation;
	public bool Mirrored;

	public BossCastEliteGuardian(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Skyreaching Guardian",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			HoldTime = 30f,
			RecastTime = 0,
		};
	}

	public void RandomizeOrientation()
	{
		Orientation = (ArenaFacing)(Math.Abs((int)GD.Randi()) % 4);
		Mirrored = GD.Randf() < 0.5f;
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		SpawnGuardian();
	}

	private void SpawnGuardian()
	{
		var guardian = Lib.LoadScene(Lib.Token.DeepGuardian).Instantiate<DeepGuardian>();
		guardian.Position = this.GetArenaEdgePosition(new Vector3(0, 0, 0), Orientation);
		GetTree().CurrentScene.AddChild(guardian);

		var rect = this.CreateRectangularTelegraph(guardian.Position);

		guardian.Rotate(Vector3.Up, this.GetArenaFacingAngle(Orientation));
		rect.Rotate(Vector3.Up, this.GetArenaFacingAngle(Orientation));

		var arenaSize = this.GetArenaSize();
		rect.Settings.Width = arenaSize * 2;
		rect.Settings.Length = arenaSize * 2;
		rect.Settings.Height = 40;
		rect.Settings.GrowTime = Settings.HoldTime;
		rect.Settings.LengthOrigin = RectangularTelegraph.Origin.Start;

		var forward = guardian.ForwardVector;
		rect.Settings.TargetValidator = (target) => target.HostileTo(Parent);
		rect.Settings.OnFinishedPerTargetCallback = (BaseUnit target) =>
		{
			target.Health.Damage(10, this);
			target.ForcefulMovement.Push(64, forward, 2f);
		};
		rect.Settings.OnFinishedCallback = () => guardian.QueueFree();
	}
}