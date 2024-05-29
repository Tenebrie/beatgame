using Godot;
using Project;

public partial class TestBossTimeline : BaseTimeline<TestBoss>
{
	public TestBossTimeline(TestBoss parent) : base(parent)
	{
		// return;
		// for (var i = 0; i < 20; i++)
		// 	Add(i * 4, parent.AutoAttack);


		Add(1, () =>
		{
			var rect = this.CreateGroundRectangularArea(Vector3.Zero);
			rect.GrowTime = 1;
			rect.Length = 8;
			rect.Width = 8;
			rect.Periodic = true;
		});

		Add(1, parent.TorrentialRain);
		// Add(1, parent.AnimatedTridents);


		// Add(9, parent.MidGuardians);

		// Target(15, new Vector3(+16, 0, 0));
		// Add(15, parent.LargeCornerAttack);
		// Target(15, new Vector3(-16, 0, 0));
		// Add(15, parent.LargeCornerAttack);
		// Target(15, new Vector3(0, 0, +16));
		// Add(15, parent.LargeCornerAttack);
		// Target(15, new Vector3(0, 0, -16));
		// Add(15, parent.LargeCornerAttack);

		// Add(1, () => parent.DeepGuardians.RandomizeOrientation());
		// Add(1, parent.DeepGuardians);
		// Add(5, () => parent.DeepGuardiansTwo.Orientation = parent.DeepGuardians.Orientation);
		// Add(5, () => parent.DeepGuardiansTwo.AdvanceOrientation());
		// Add(5, parent.DeepGuardiansTwo);

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

	public override void _Ready()
	{
		Start();
	}
}