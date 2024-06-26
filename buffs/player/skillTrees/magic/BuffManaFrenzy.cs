using System;

namespace Project;

public partial class BuffManaFrenzy : BaseBuff
{
	public const float DamageIncrease = 0.01f;
	public const float FrenzyDuration = 16;
	public const int MaximumStacks = 100;
	public BuffManaFrenzy()
	{
		Settings = new()
		{
			FriendlyName = "Mana Frenzy",
			DynamicDesc = () => MakeDescription($"Increases damage by {Colors.Tag(Math.Round(DamageIncrease * Stacks * 100) + "%")}."),
			IconPath = "res://assets/icons/SpellBook06_53.PNG",
			RefreshOthersWhenAdded = true,
			MaximumStacks = MaximumStacks,
		};
		Duration = FrenzyDuration;
	}

	public override void ModifyOutgoingDamage(BuffOutgoingDamageVisitor damage)
	{
		if (damage.Target == Parent || damage.ResourceType != ObjectResourceType.Health)
			return;

		damage.Value += damage.BaseValue * DamageIncrease;
	}
}