using Godot;
using System;
using Project;
using System.Collections.Generic;

namespace BeatGame.scripts.navigation;

public partial class NavMeshSettings : Resource
{
	public string Name;
	public float AgentRadius;
	public float AgentHeight;
	public uint Layer;
}

public enum NavigationLayer
{
	SmallAgent = 0,
	MediumAgent = 1,
	LargeAgent = 2,
}

public static class NavigationLayerExtensions
{
	public static uint ToMaskValue(this NavigationLayer layer)
	{
		return (uint)(1 << (int)layer);
	}
};

[Tool]
public partial class NavMeshManager : Node3D
{
	[Export]
	public bool GenerateNavMesh
	{
		get => false;
		set
		{
			if (value)
				DoGenerateNavMesh();
		}
	}

	[Export]
	public bool UpdateLayerNames
	{
		get => false;
		set
		{
			if (value)
				DoUpdateLayerNames();
		}
	}

	void DoUpdateLayerNames()
	{
		foreach (NavigationLayer layer in (NavigationLayer[])Enum.GetValues(typeof(NavigationLayer)))
		{
			ProjectSettings.SetSetting($"layer_names/3d_navigation/layer_{(int)(layer + 1)}", layer.ToString());
		}
	}

	void DoGenerateNavMesh()
	{
		var smallAgentSize = 0.15f;
		var mediumAgentSize = 0.30f;
		var largeAgentSize = 0.60f;
		var cellSize = 0.20f;
		NavMeshSettings[] settings = new[]
		{
			new NavMeshSettings { Name = "SmallAgent", AgentRadius = smallAgentSize, AgentHeight = smallAgentSize, Layer = NavigationLayer.SmallAgent.ToMaskValue(), },
			new NavMeshSettings { Name = "MediumAgent", AgentRadius = mediumAgentSize, AgentHeight = mediumAgentSize, Layer = NavigationLayer.MediumAgent.ToMaskValue() },
			new NavMeshSettings { Name = "LargeAgent", AgentRadius = largeAgentSize, AgentHeight = largeAgentSize, Layer = NavigationLayer.LargeAgent.ToMaskValue() },
		};

		var oldRegions = this.GetComponentsUncached<NavigationRegion3D>();
		foreach (var region in oldRegions)
		{
			region.QueueFree();
			RemoveChild(region);
		}

		GD.Print("Grabbing mesh data...");
		var navigationData = new NavigationMeshSourceGeometryData3D();
		NavigationServer3D.ParseSourceGeometryData(new NavigationMesh()
		{
			GeometryParsedGeometryType = NavigationMesh.ParsedGeometryType.StaticColliders,
		}, navigationData, this);

		foreach (var setting in settings)
		{
			GD.Print($"Creating region {setting.Name}...");
			var region = new NavigationRegion3D();

			AddChild(region);
			region.Name = setting.Name;
			region.Owner = GetTree().EditedSceneRoot;

			Rid navigationMap = NavigationServer3D.MapCreate();
			NavigationServer3D.MapSetActive(navigationMap, true);
			NavigationServer3D.MapSetCellSize(navigationMap, cellSize);
			NavigationServer3D.MapSetCellHeight(navigationMap, cellSize);
			region.SetNavigationMap(navigationMap);

			region.NavigationLayers = setting.Layer;
			region.NavigationMesh = new NavigationMesh
			{
				CellSize = cellSize,
				CellHeight = cellSize,
				BorderSize = 0.4f,
				AgentRadius = setting.AgentRadius,
				AgentHeight = setting.AgentHeight,
				GeometryParsedGeometryType = NavigationMesh.ParsedGeometryType.StaticColliders,
			};

			GD.Print($"Generating navmesh for region {setting.Name}");
			NavigationServer3D.BakeFromSourceGeometryData(region.NavigationMesh, navigationData);
		}

	}
}
