using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class EnvironmentController : Node
{
	public WorldEnvironment WorldEnvironment;
	float TargetFogDensity;
	float TargetBiolumenescence;
	public List<ShaderMaterial> ControlledMaterials = new()
	{
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/Tree_Mat_01.tres"),
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/Tree_Mat_02.tres"),
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/Tree_Mat_03.tres"),
		GD.Load<ShaderMaterial>("res://assets/PolygonTropicalJungle/Materials/Plants/SM_Env_Tree_Forest_02_Sturdy.tres"),
	};
	readonly Dictionary<string, List<IControllableEnvironment>> Controllables = new();

	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		WorldEnvironment = GetNode<WorldEnvironment>("WorldEnvironment");
		TargetFogDensity = WorldEnvironment.Environment.VolumetricFogDensity;
	}

	public void RegisterControllable(IControllableEnvironment light, string groupName)
	{
		List<IControllableEnvironment> list;
		if (Controllables.TryGetValue(groupName, out var existingList))
		{
			list = existingList;
		}
		else
		{
			list = new();
			Controllables.Add(groupName, list);
		}
		list.Add(light);
	}

	public void SetEnabled(string groupName, bool enabled)
	{
		if (!Controllables.TryGetValue(groupName, out var list))
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
		TargetBiolumenescence = value;
		var materials = ControlledMaterials.Where(mat => mat != null);
		foreach (var mat in materials)
		{
			mat.SetShaderParameter("Leaf_Emmisive_Color".ToStringName(), new Color(0, 1, 1));
		}
	}

	public void SetFogDensity(float value)
	{
		TargetFogDensity = value;
	}

	public override void _Process(double delta)
	{
		var env = WorldEnvironment.Environment;
		if (env.VolumetricFogDensity != TargetFogDensity)
		{
			env.VolumetricFogDensity += (TargetFogDensity - env.VolumetricFogDensity) * 5.0f * (float)delta;
		}

		for (var i = 0; i < ControlledMaterials.Count; i++)
		{
			var mat = ControlledMaterials[i];
			if (mat == null)
				continue;

			var str = (float)mat.GetShaderParameter("Leaf_Emissive_Str".ToStringName());
			if (str == TargetBiolumenescence)
				continue;

			str += (TargetBiolumenescence - str) * 5.0f * (float)delta;
			mat.SetShaderParameter("Leaf_Emissive_Str".ToStringName(), str);
		}
	}

	private static EnvironmentController instance = null;
	public static EnvironmentController Singleton => instance;
}