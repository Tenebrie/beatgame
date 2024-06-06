using Godot;
using System;
using System.Collections.Generic;

namespace Project;

// [Tool] 
public partial class LightningZapEffect : Node3D
{
	[Export]
	StandardMaterial3D LightningMaterial;
	MeshInstance3D MeshInstance;
	Vector3 Target = new(10.0f, 10.0f, 10.0f);
	public float FadeDuration = 0.75f;

	public override void _Ready()
	{
		MeshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
	}

	void Draw()
	{
		var segmentSize = 0.2f;
		var randomness = 0.12f;
		var branchRandomness = 1f;

		var color = LightningMaterial.AlbedoColor;
		LightningMaterial.AlbedoColor = new Color(color.R, color.G, color.B, 1.0f);
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Lines);
		st.SetMaterial(LightningMaterial);

		void GenerateBranch(Vector3 from, Vector3 targetPosition, int depth)
		{
			if (depth >= 4)
				return;

			st.AddVertex(from);

			var stepsSinceLastBranch = 0;
			var currentPosition = from;
			var nodesSpawned = 0;
			var distanceToTarget = targetPosition.Length();
			while (nodesSpawned < 350)
			{
				if (nodesSpawned >= 300)
				{
					st.AddVertex(targetPosition);
					break;
				}

				var straightVector = currentPosition + (targetPosition - currentPosition).Normalized() * Math.Min(segmentSize, distanceToTarget);
				if (straightVector.DistanceTo(targetPosition) <= 0.10f || straightVector.X >= targetPosition.X)
				{
					st.AddVertex(straightVector);
					st.AddVertex(straightVector);
					st.AddVertex(targetPosition);
					break;
				}

				var randomVector = new Vector3(0, GD.Randf() - 0.5f, GD.Randf() - 0.5f).Normalized() * Math.Max(0.5f, GD.Randf()) * randomness;

				var newVertex = straightVector + randomVector;
				distanceToTarget = newVertex.DistanceTo(targetPosition);
				nodesSpawned += 1;
				currentPosition = newVertex;

				st.AddVertex(newVertex);

				stepsSinceLastBranch += 1;
				if (stepsSinceLastBranch >= 1 && GD.Randf() <= ((0.01f + Math.Min(100, nodesSpawned) / 100f) / 8) * stepsSinceLastBranch)
				{
					stepsSinceLastBranch = 0;
					float getRandom() => (GD.Randf() - 0.5f) * 2;
					var r2 = new Vector3(Math.Max(0.5f, GD.Randf()), getRandom(), getRandom()).Normalized() * Math.Max(0.9f, GD.Randf()) * branchRandomness;
					var t = currentPosition + r2;
					GenerateBranch(currentPosition, t, depth + 1);
				}

				st.AddVertex(newVertex);
			}
			return;
		}

		var targetPosition = new Vector3((Target - GlobalPosition).Length(), 0, 0);
		GenerateBranch(new Vector3(0, 0, 0), targetPosition, 0);

		var mesh = st.Commit();

		MeshInstance.Mesh = mesh;

		MeshInstance.LookAt(Target, Vector3.Up);
		MeshInstance.RotateObjectLocal(Vector3.Up, (float)Math.PI / 2);
	}

	public override void _Process(double delta)
	{
		var color = LightningMaterial.AlbedoColor;
		var alpha = Math.Max(0f, color.A - (float)delta / FadeDuration);
		LightningMaterial.AlbedoColor = new Color(color.R, color.G, color.B, alpha);
		if (alpha <= 0.00f)
		{
			QueueFree();
		}
	}

	public void SetTarget(Vector3 position)
	{
		Target = position;
		Draw();
	}
}
