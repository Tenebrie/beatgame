using Project;

public partial class TestBossTimeline : BaseTimeline<TestBoss>
{
	public TestBossTimeline(TestBoss parent) : base(parent)
	{
		for (var i = 1; i < 20; i++)
			Add(i * 4, parent.AutoAttack);
	}
}