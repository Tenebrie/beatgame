using Godot;
using System;
using System.Collections.Generic;

namespace Project;

// [Tool]
public partial class LightningZapEffect : Node3D
{
	[Signal]
	public delegate void AnimationFinishedEventHandler();

	[Export]
	ShaderMaterial LightningMaterial;
	MeshInstance3D MeshInstance;
	Vector3 Target = new(10.0f, 10.0f, 10.0f);

	public float FadeDuration = 1.0f;
	public float ProgressDuration = 0.25f;

	bool signalEmitted = false;
	float fadeValue = 1.25f;
	float progressValue = 0.0f;

	public override void _Ready()
	{
		MeshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
	}

	void Draw()
	{
		var totalDistance = (Target - GlobalPosition).Length();
		var segmentSize = 0.2f;
		var randomness = 0.12f;
		var branchRandomness = 1.4f;

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

				var effectiveSegmentSize = depth == 0 ? segmentSize : segmentSize;
				var straightVector = currentPosition + (targetPosition - currentPosition).Normalized() * Math.Min(effectiveSegmentSize, distanceToTarget);
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
				if (stepsSinceLastBranch >= 1 && GD.Randf() <= 0.03f * stepsSinceLastBranch && depth == 0)
				{
					stepsSinceLastBranch = 0;
					float getRandom() => (GD.Randf() - 0.5f) * 2;
					var r2 = new Vector3(Math.Max(0.75f, GD.Randf() * 1.5f), getRandom(), getRandom()).Normalized() * Math.Max(0.9f, GD.Randf()) * branchRandomness * Math.Min(distanceToTarget / 10, 1);
					var t = currentPosition + r2;
					GenerateBranch(currentPosition, t, depth + 1);
				}

				st.AddVertex(newVertex);
			}
			return;
		}

		var targetPosition = new Vector3(totalDistance, 0, 0);
		GenerateBranch(new Vector3(0, 0, 0), targetPosition, 0);

		var mesh = st.Commit();

		MeshInstance.Mesh = mesh;

		MeshInstance.LookAt(Target, Vector3.Up);
		MeshInstance.RotateObjectLocal(Vector3.Up, (float)Math.PI / 2);

		MeshInstance.SetInstanceShaderParameter("LENGTH", totalDistance);

		ProgressDuration = 0.25f;
		FadeDuration = 1.0f;
		fadeValue = FadeDuration + ProgressDuration;
	}

	public override void _Process(double delta)
	{
		fadeValue = Math.Max(0.0f, fadeValue - (float)delta / FadeDuration);
		progressValue += (float)delta / ProgressDuration;
		if (progressValue >= 0.5f && !signalEmitted)
		{
			signalEmitted = true;
			EmitSignal(SignalName.AnimationFinished);
		}

		MeshInstance.SetInstanceShaderParameter("FADE", Math.Min(1.0, fadeValue));
		MeshInstance.SetInstanceShaderParameter("PROGRESS", progressValue);
		if (fadeValue <= 0.00f)
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
