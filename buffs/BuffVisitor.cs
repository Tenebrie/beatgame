using System;
using System.Collections.Generic;

namespace Project;

public class BuffVisitor
{
	public Dictionary<ObjectResourceType, float> PercentageDamageReduction = new();
	public float PercentageCCReduction = 0;
	public float GravityModifier = 1;

	public BuffVisitor()
	{
		foreach (ObjectResourceType resource in (ObjectResourceType[])Enum.GetValues(typeof(ObjectResourceType)))
		{
			PercentageDamageReduction[resource] = 0.00f;
		}
	}
}