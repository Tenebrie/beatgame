using Project;

public partial class TestBossTimeline : BaseTimeline<TestBoss>
{
	public TestBossTimeline(TestBoss parent) : base(parent)
	{
		// return;
		// for (var i = 0; i < 20; i++)
		// 	Add(i * 4, parent.AutoAttack);

		Add(0, parent.DeepGuardians);

		// Add(0, parent.AutoAttack);
		// Add(0.5, parent.GroundAttack);
		// Add(1, parent.AutoAttack);
		// Add(1.5, parent.GroundAttack);
		// Add(2, parent.AutoAttack);
		// Add(2.5, parent.GroundAttack);
		// Add(3, parent.AutoAttack);
		// Add(3.5, parent.GroundAttack);
		// Add(4, parent.AutoAttack);
		// Add(4.5, parent.GroundAttack);
		// Add(5, parent.AutoAttack);
		// Add(5.5, parent.GroundAttack);
		// Add(6, parent.AutoAttack);
		// Add(6.5, parent.GroundAttack);
		// Add(7, parent.AutoAttack);
		// Add(7.25, parent.AutoAttack);
		// Add(7.5, parent.AutoAttack);
	}
}