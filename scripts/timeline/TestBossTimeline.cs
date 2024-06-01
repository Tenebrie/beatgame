using System;
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

		// Act(() => parent.DeepGuardians.Reset());
		// Cast(parent.DeepGuardians, advance: false);

		Mark(84 * 4 + 1, "Debug");

		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);

		Act(() => parent.EliteGuardian.RandomizeOrientation());
		Cast(parent.EliteGuardian, advance: false);
		Wait(4);

		Act(() => parent.Geysers.Variation = BossCastGeysers.VariationType.UnderBoss | BossCastGeysers.VariationType.FourSides);
		Cast(parent.Geysers);
		Wait(4);

		Cast(parent.RavagingWinds);

		Cast(parent.AnimatedTridents);

		Act(() => parent.AreaAttack.Settings.HoldTime = 4);
		Target(new Vector3(0, 0, 0), allowMultitarget: true);
		Target(new Vector3(+16, 0, 0), allowMultitarget: true);
		Target(new Vector3(-16, 0, 0), allowMultitarget: true);
		Target(new Vector3(0, 0, +16), allowMultitarget: true);
		Target(new Vector3(0, 0, -16), allowMultitarget: true);
		Cast(parent.AreaAttack);

		Act(() => parent.DeepGuardians.Reset());
		Cast(parent.DeepGuardians, advance: false);

		Wait(4);

		Act(() => parent.DeepGuardiansTwo.Orientation = parent.DeepGuardians.Orientation);
		Act(() => parent.DeepGuardiansTwo.AdvanceOrientation());
		Cast(parent.DeepGuardiansTwo);

		Cast(parent.ConsumingWinds);

		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Wait(1);

		Cast(parent.TorrentialRain);

		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Wait(1);


		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Wait(1);

		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Wait(1);

		Cast(parent.ConsumingWinds);

		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Wait(1);

		Act(() => parent.DeepGuardians.Reset());
		Act(() => parent.DeepGuardiansTwo.Reset());
		Cast(parent.DeepGuardians, advance: false);

		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);

		Act(() => parent.DeepGuardiansTwo.Orientation = parent.DeepGuardians.Orientation);
		Act(() => parent.DeepGuardiansTwo.AdvanceOrientation());
		Cast(parent.DeepGuardiansTwo, advance: false);

		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Wait(1);

		Wait(1);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Cast(parent.AutoAttack);
		Wait(1);

		Cast(parent.TorrentialRain);
		Cast(parent.TorrentialRain);
		Cast(parent.TorrentialRain);

		Cast(parent.HardEnrage);
	}

	public override void _Ready()
	{
		Start();
		Music.Singleton.SeekTo(GetMarkBeatIndex("Debug"));
	}
}