using System;
using System.Collections.Generic;
using Godot;

namespace Project;

public partial class BuffUnitStatsVisitor : RefCounted
{
	public Dictionary<ObjectResourceType, float> PercentageDamageReduction = ObjectResource.MakeDictionary<float>(0f);
	public float CastManaEfficiency = 0;
	public float PercentageCCReduction = 0;
	public float GravityModifier = 1;
	public float MoveSpeedPercentage = 1;
	public float MaximumHealth = 0;
	public float MaximumMana = 0;
	public float SummonHealth = 0;
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