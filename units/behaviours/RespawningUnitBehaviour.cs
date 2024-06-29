using Godot;

namespace Project;

public partial class RespawningUnitBehaviour : Node
{
	[Export]
	public string sceneToSpawn;

	DelayedUnitSpawner spawner;

	public override void _EnterTree()
	{
		this.CallDeferred(() =>
		{
			var parent = (BaseUnit)GetParent();
			spawner = new()
			{
				alliance = parent.Alliance,
				transform = parent.GlobalTransform,
				sceneToSpawn = sceneToSpawn
			};
			GetTree().CurrentScene.AddChild(spawner);
		});
	}

	public override void _ExitTree()
	{
		spawner.SpawnWithDelay();
	}

	partial class DelayedUnitSpawner : Node
	{
		public UnitAlliance alliance;
		public Transform3D transform;
		public string sceneToSpawn;

		public async void SpawnWithDelay()
		{
			if (!IsInsideTree())
				return;

			await ToSignal(GetTree().CreateTimer(2), "timeout");
			var newUnit = Lib.LoadScene(sceneToSpawn).Instantiate<BaseUnit>();
			newUnit.Alliance = alliance;
			newUnit.Transform = transform;
			GetTree().CurrentScene.AddChild(newUnit);
			QueueFree();
		}
	}
}