using Project;

public partial class TestBossTimeline : BaseTimeline<TestBoss>
{
	public TestBossTimeline(TestBoss parent) : base(parent)
	{
		// return;
		for (var i = 0; i < 20; i++)
			Add(i * 4, parent.AutoAttack);
	}
}