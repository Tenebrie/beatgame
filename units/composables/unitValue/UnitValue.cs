
using System;
using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

namespace Project;

public partial class UnitValue
{
	public int BaseValue;
	public (UnitStat stat, float multiplier)[] DependsOn;

	public UnitValue(int baseValue, params (UnitStat stat, float multiplier)[] dependsOn)
	{
		BaseValue = baseValue;
		DependsOn = dependsOn;
	}

	public float GetValue(BaseCast cast) => GetValue(cast.Parent);

	public float GetValue(BaseUnit parent)
	{
		float value = BaseValue;
		if (parent == null)
			return value;

		foreach (var (stat, multiplier) in DependsOn)
		{
			value += parent.Stats.Get(stat) * multiplier;
		}
		return value;
	}

	public string GetExplanation()
	{
		StringBuilder builder = new();
		foreach (var (stat, multiplier) in DependsOn)
		{
			if (builder.Length > 0)
				builder.Append(", ");
			builder.Append($"{Math.Round(multiplier * 100)}% {stat}");
		}
		return builder.ToString();
	}

	public readonly static JsonSerializerOptions serializerOptions = new() { IncludeFields = true };
	public override string ToString()
	{
		return $"||{JsonSerializer.Serialize(this, serializerOptions)}||";
	}

	public static UnitValue FromString(string json)
	{
		return JsonSerializer.Deserialize<UnitValue>(json, serializerOptions);
	}
}