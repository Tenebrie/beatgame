namespace Project;

public partial class ObjectReactions : ComposableScript
{
	public ObjectReactions(BaseUnit parent) : base(parent) { }

	public override void _EnterTree()
	{
		base._EnterTree();
		SignalBus.Singleton.DamageTaken += OnLifeLeechDamageDealt;
		SignalBus.Singleton.DamageTaken += OnRetaliateDamageDealt;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		SignalBus.Singleton.DamageTaken -= OnLifeLeechDamageDealt;
		SignalBus.Singleton.DamageTaken -= OnRetaliateDamageDealt;
	}

	void OnLifeLeechDamageDealt(BuffIncomingDamageVisitor data)
	{
		if (data.SourceUnit != Parent || data.ResourceType != ObjectResourceType.Health || data.Target == Parent)
			return;

		Parent.Health.Restore(data.Value * Parent.Buffs.State.LifeLeechFraction, Parent);
	}

	void OnRetaliateDamageDealt(BuffIncomingDamageVisitor data)
	{
		var retaliateDamage = Parent.Buffs.State.RetaliateDamageFraction;
		if (data.SourceUnit == Parent || data.ResourceType != ObjectResourceType.Health || data.Target != Parent || retaliateDamage == 0)
			return;

		data.SourceUnit.Health.DamageSilently(data.BaseValue * retaliateDamage, Parent, null, SilentDamageReason.Retaliate);
	}
}
