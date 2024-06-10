using System;
using System.Collections.Generic;

namespace Project;

public class BuffUnitStatsVisitor
{
	public Dictionary<ObjectResourceType, float> PercentageDamageReduction = ObjectResource.MakeDictionary<float>(0f);
	public float PercentageCCReduction = 0;
	public float GravityModifier = 1;
	public float MoveSpeedPercentage = 1;
	public float MaximumHealth = 0;
	public float MaximumMana = 0;
}

public class BuffIncomingDamageVisitor
{
	public float Value;
	public BaseUnit SourceUnit;
	public BaseCast SourceCast;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}

public class BuffOutgoingDamageVisitor
{
	public float Value;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}

public class BuffIncomingRestorationVisitor
{
	public float Value;
	public BaseUnit SourceUnit;
	public BaseCast SourceCast;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}

public class BuffOutgoingRestorationVisitor
{
	public float Value;
	public BaseUnit Target;
	public ObjectResourceType ResourceType;
}