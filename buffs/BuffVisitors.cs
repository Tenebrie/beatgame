using System;
using System.Collections.Generic;
using Godot;

namespace Project;

public partial class BuffUnitStatsVisitor : RefCounted
{
	// TODO: Make the dictionaries lists of multipliers instead of one multiplier
	public Dictionary<UnitStat, int> Stats = UnitStatExtensions.MakeDictionary(0);
	public Dictionary<ObjectResourceType, float> PercentageDamageTaken = ObjectResource.MakeDictionary<float>(1f);
	public Dictionary<ObjectResourceType, float> FlatResourceRegen = ObjectResource.MakeDictionary<float>(0f);
	public Dictionary<ObjectResourceType, float> PercentageResourceRegen = ObjectResource.MakeDictionary<float>(1f);
	public float CastManaEfficiency = 0;
	public float PercentageCCReduction = 0;
	public float GravityModifier = 1;
	public float MoveSpeedPercentage = 1;
	public float MaximumHealth = 0;
	public float MaximumMana = 0;
	public float SummonHealth = 0;
	public float LifeLeechFraction = 0;
	public float RetaliateDamageFraction = 0;
}

public partial class BuffIncomingDamageVisitor : RefCounted
{
	public float Value;
	public float BaseValue;
	public BaseUnit SourceUnit;
	public BaseCast SourceCast;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}

public partial class BuffOutgoingDamageVisitor : RefCounted
{
	public float Value;
	public float BaseValue;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}

public partial class BuffIncomingRestorationVisitor : RefCounted
{
	public float Value;
	public float BaseValue;
	public BaseUnit SourceUnit;
	public BaseCast SourceCast;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}

public partial class BuffOutgoingRestorationVisitor : RefCounted
{
	public float Value;
	public float BaseValue;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}