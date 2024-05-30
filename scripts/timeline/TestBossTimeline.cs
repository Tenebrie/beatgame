using Godot;
using Project;

public partial class TestBossTimeline : BaseTimeline<TestBoss>
{
	public TestBossTimeline(TestBoss parent) : base(parent)
	{
		// return;
		// for (var i = 0; i < 20; i++)
		// 	Add(i * 4, parent.AutoAttack);


		Act(1, () =>
		{
			var rect = this.CreateGroundRectangularArea(Vector3.Zero);
			rect.GrowTime = 1;
			rect.Length = 8;
			rect.Width = 8;
			rect.Periodic = true;
		});

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Cast(parent.TorrentialRain);

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Cast(parent.ConsumingWinds);

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Cast(parent.AnimatedTridents);

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Target(new Vector3(0, 0, 0), allowMultitarget: true);
		Target(new Vector3(+16, 0, 0), allowMultitarget: true);
		Target(new Vector3(-16, 0, 0), allowMultitarget: true);
		Target(new Vector3(0, 0, +16), allowMultitarget: true);
		Target(new Vector3(0, 0, -16), allowMultitarget: true);
		Cast(parent.AreaAttack);

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Cast(parent.ConsumingWinds);

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Act(() => parent.DeepGuardians.RandomizeOrientation());
		Cast(parent.DeepGuardians, advance: false);
		AdvanceTime(4);
		Act(5, () => parent.DeepGuardiansTwo.Orientation = parent.DeepGuardians.Orientation);
		Act(5, () => parent.DeepGuardiansTwo.AdvanceOrientation());
		Cast(5, parent.DeepGuardiansTwo, advance: false);
		AdvanceTime(4);

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Cast(parent.TorrentialRain);
		Cast(parent.TorrentialRain);
		Cast(parent.TorrentialRain);

		Cast(parent.HardEnrage);
	}

	public override void _Ready()
	{
		Start();
	}
}