using Godot;

namespace Project;

/// <summary>
/// When destroyed, this unit will respawn after a short delay.
/// Expected to be used for training dummies in the hub room.
/// </summary>
public partial class RespawningUnitBehaviour : BaseBehaviour
{
	[Export]
	public string sceneToSpawn;

	DelayedUnitSpawner spawner;

	public override void _EnterTree()
	{
		this.CallDeferred(() =>
		{
			spawner = new()
			{
				alliance = Parent.Alliance,
				transform = Parent.GlobalTransform,
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

			await ToSignal(GetTree().CreateTimer(2), "timeout".ToStringName());
			var newUnit = Lib.LoadScene(sceneToSpawn).Instantiate<BaseUnit>();
			newUnit.Alliance = alliance;
			newUnit.Transform = transform;
			GetTree().CurrentScene.AddChild(newUnit);
			if (newUnit.GetComponentOrDefault<RespawningUnitBehaviour>() == null)
			{
				var newRespawningBehaviour = Lib.LoadScene("res://units/behaviours/RespawningUnitBehaviour.tscn").Instantiate<RespawningUnitBehaviour>();
				newRespawningBehaviour.sceneToSpawn = sceneToSpawn;
				newUnit.AddChild(newRespawningBehaviour);
			}
			QueueFree();
		}
	}
}