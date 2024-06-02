using System.Collections.Generic;
using Godot;

namespace Project;

public partial class EnvironmentController : Node
{
	public List<ShaderMaterial> ControlledMaterials = new()
	{
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/Tree_Mat_01.tres"),
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/Tree_Mat_02.tres"),
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/Tree_Mat_03.tres"),
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/SM_Env_Tree_Forest_02_Sturdy.tres"),
	};
	readonly Dictionary<string, List<IControllableLight>> ControllableLights = new();

	public override void _EnterTree()
	{
		instance = this;
	}

	public void RegisterControllableLight(IControllableLight light, string groupName)
	{
		List<IControllableLight> list;
		if (ControllableLights.TryGetValue(groupName, out var existingList))
		{
			list = existingList;
		}
		else
		{
			list = new();
			ControllableLights.Add(groupName, list);
		}
		list.Add(light);
	}

	public void UpdateLights(string groupName, bool enabled)
	{
		if (!ControllableLights.TryGetValue(groupName, out var list))
			return;

		foreach (var light in list)
		{
			if (enabled)
				light.TurnOn();
			else
				light.TurnOff();
		}
	}

	public void SetBiolumenescence(float value)
	{
		foreach (var mat in ControlledMaterials)
		{
			mat.SetShaderParameter("Leaf_Emmisive_Color", new Color(0, 1, 1));
			mat.SetShaderParameter("Leaf_Emissive_Str", value);
		}
	}

	private static EnvironmentController instance = null;
	public static EnvironmentController Singleton
	{
		get => instance;
	}
}