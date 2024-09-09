using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
namespace Project;

public enum UnitStat
{
	Power,
	CastSpeed, // quantized
	CritChance, // all abilities can crit
	CritDamage,
	DotSomething, // how do dots work? How to make dots scale fairly?
	SummonPower,
	SummonEfficiency,
	CooldownReduction, // quantized????
}

public static class UnitStatExtensions
{
	public static Dictionary<UnitStat, T> MakeDictionary<T>(T defaultValue)
	{
		var dict = new Dictionary<UnitStat, T>();
		foreach (UnitStat resource in (UnitStat[])Enum.GetValues(typeof(UnitStat)))
		{
			dict[resource] = defaultValue;
		}
		return dict;
	}
}

public partial class ObjectStats : ComposableScript
{
	readonly Dictionary<UnitStat, int> stats = UnitStatExtensions.MakeDictionary<int>(0);

	public ObjectStats(BaseUnit parent) : base(parent) { }

	public int Get(UnitStat stat)
	{
		return stats.GetValueOrDefault(stat);
	}

	public void Set(UnitStat stat, int value)
	{
		stats[stat] = value;
	}
}
