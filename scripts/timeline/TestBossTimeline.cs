using System;
using System.Collections.Generic;
using Godot;

namespace Project;

public partial class TestBossTimeline : BaseTimeline<BossAeriel>
{
	public TestBossTimeline(BossAeriel parent) : base(parent)
	{
		// GotoAbleton(42);
		GotoAbleton(86);

		Mark("Debug");
		Wait(2);

		RegisterAutoAttacks();
		RegisterRaidwides();

		// ===================================================
		// Intro
		// ===================================================
		Goto(0);

		Act(() =>
		{
			var rect = this.CreateGroundRectangularArea(Vector3.Zero);
			rect.GrowTime = 1;
			rect.Length = 8;
			rect.Width = 8;
			rect.Periodic = true;
		});
		Cast(parent.OpeningTorrentialRain);

		// ===================================================
		// Phase 1
		// ===================================================
		GotoAbleton(9);
		Cast(parent.ConsumingWinds);

		GotoAbleton(17);
		Cast(parent.RavagingWinds);

		// ===================================================
		// Deep Guardians 1
		// ===================================================
		GotoAbleton(25);
		Act(() => parent.DeepGuardians.Settings.PrepareTime = 8);
		Act(() => parent.DeepGuardians.Reset());
		Cast(parent.DeepGuardians);

		GotoAbleton(29);
		Act(() => parent.DeepGuardians.AdvanceOrientation());
		Cast(parent.DeepGuardians, advance: false);

		// ===================================================
		// Phase 2
		// ===================================================
		GotoAbleton(33);
		Cast(parent.TwiceConsumingWinds);

		GotoAbleton(37);
		Act(() => parent.AreaAttack.AreaRadius = 12);
		Act(() => parent.AreaAttack.Settings.HoldTime = 12);
		Target(new Vector3(+16, 0, +16), allowMultitarget: true);
		Target(new Vector3(-16, 0, +16), allowMultitarget: true);
		Target(new Vector3(+16, 0, -16), allowMultitarget: true);
		Target(new Vector3(-16, 0, -16), allowMultitarget: true);
		Cast(parent.AreaAttack);

		GotoAbleton(39);
		Cast(parent.Buster);

		// ===================================================
		// Lightning Orbs 1
		// ===================================================
		GotoAbleton(42);
		Cast(parent.LightningOrbs);

		// ===================================================
		// Thrice Consuming Winds
		// ===================================================
		GotoAbleton(61);
		Cast(parent.ThriceConsumingWinds);

		GotoAbleton(65);
		Act(() => parent.AreaAttack.AreaRadius = 12);
		Act(() => parent.AreaAttack.Settings.HoldTime = 8);
		Target(new Vector3(+16, 0, 0), allowMultitarget: true);
		Target(new Vector3(-16, 0, 0), allowMultitarget: true);
		Target(new Vector3(0, 0, +16), allowMultitarget: true);
		Target(new Vector3(0, 0, -16), allowMultitarget: true);
		Cast(parent.AreaAttack);

		GotoAbleton(69);
		Cast(parent.MiniBuster);

		GotoAbleton(71);
		Cast(parent.MiniBuster);

		GotoAbleton(73);
		Cast(parent.MiniBuster);

		GotoAbleton(75);
		Cast(parent.MiniBuster);

		GotoAbleton(77);
		Cast(parent.MiniBuster);

		// ===================================================
		// Geysers
		// ===================================================
		GotoAbleton(79);

		Act(() => parent.Geysers.Settings.HoldTime = 14);
		Act(() => parent.Geysers.Variation = BossCastGeysers.VariationType.UnderBoss);
		Cast(parent.Geysers, advance: false);

		Act(() => parent.DeepGuardians.Settings.PrepareTime = 16);
		Act(() => parent.DeepGuardians.Reset());
		Cast(parent.DeepGuardians, advance: false);
		Wait(8);
		Act(() => parent.DeepGuardiansTwo.Orientation = parent.DeepGuardians.Orientation);
		Act(() => parent.DeepGuardiansTwo.AdvanceOrientation());
		Cast(parent.DeepGuardiansTwo, advance: false);

		GotoAbleton(86);

		Target(new Vector3(0, 0, 0));
		Act(() => parent.AreaAttack.AreaRadius = 8f);
		Act(() => parent.AreaAttack.Settings.HoldTime = 4f);
		Cast(parent.AreaAttack);

		// ===================================================
		// Skyreaching Guardian + Geysers
		// ===================================================
		GotoAbleton(87);

		Wait(6);
		Act(() => parent.EliteGuardian.RandomizeOrientation());
		Cast(parent.EliteGuardian, advance: false);
		Wait(9);

		Act(() => parent.Geysers.Settings.HoldTime = 5);
		Act(() => parent.Geysers.Variation = BossCastGeysers.VariationType.AllPositions);
		Cast(parent.Geysers);

		GotoAbleton(96);
		Act(() =>
		{
			Parent.ReleaseDarkness();
			EnvironmentController.Singleton.UpdateLights("sun", false);
			EnvironmentController.Singleton.UpdateLights("arena", true);
			EnvironmentController.Singleton.SetBiolumenescence(1);
			EnvironmentController.Singleton.SetFogDensity(0.005f);
		});

		// Wait(2);

		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);

		// Act(() => parent.EliteGuardian.RandomizeOrientation());
		// Cast(parent.EliteGuardian, advance: false);
		// Wait(4);

		// Act(() => parent.Geysers.Variation = BossCastGeysers.VariationType.UnderBoss | BossCastGeysers.VariationType.FourSides);
		// Cast(parent.Geysers);
		// Wait(4);

		// Cast(parent.RavagingWinds);

		// Cast(parent.AnimatedTridents);

		// Act(() => parent.AreaAttack.Settings.HoldTime = 4);
		// Target(new Vector3(0, 0, 0), allowMultitarget: true);
		// Target(new Vector3(+16, 0, 0), allowMultitarget: true);
		// Target(new Vector3(-16, 0, 0), allowMultitarget: true);
		// Target(new Vector3(0, 0, +16), allowMultitarget: true);
		// Target(new Vector3(0, 0, -16), allowMultitarget: true);
		// Cast(parent.AreaAttack);

		// Act(() => parent.DeepGuardians.Reset());
		// Cast(parent.DeepGuardians, advance: false);

		// Wait(4);

		// Act(() => parent.DeepGuardiansTwo.Orientation = parent.DeepGuardians.Orientation);
		// Act(() => parent.DeepGuardiansTwo.AdvanceOrientation());
		// Cast(parent.DeepGuardiansTwo);

		// Cast(parent.ConsumingWinds);

		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Wait(1);

		// Cast(parent.TorrentialRain);

		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Wait(1);


		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Wait(1);

		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Wait(1);

		// Cast(parent.ConsumingWinds);

		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Wait(1);

		// Act(() => parent.DeepGuardians.Reset());
		// Act(() => parent.DeepGuardiansTwo.Reset());
		// Cast(parent.DeepGuardians, advance: false);

		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);

		// Act(() => parent.DeepGuardiansTwo.Orientation = parent.DeepGuardians.Orientation);
		// Act(() => parent.DeepGuardiansTwo.AdvanceOrientation());
		// Cast(parent.DeepGuardiansTwo, advance: false);

		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Wait(1);

		// Wait(1);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Cast(parent.AutoAttack);
		// Wait(1);

		// Cast(parent.TorrentialRain);
		// Cast(parent.TorrentialRain);
		// Cast(parent.TorrentialRain);

		// ===================================================
		// Final Sequence
		// ===================================================

		GotoAbleton(163);
		Cast(parent.HardEnrage);

		// GotoAbleton(169);
		// Act(() => 
		// {
		// 	EnvironmentController.Singleton.UpdateLights("sun", false);
		// 	EnvironmentController.Singleton.UpdateLights("arena", true);
		// 	EnvironmentController.Singleton.SetBiolumenescence(1);
		// 	EnvironmentController.Singleton.SetFogDensity(0.005f);
		// });

		// Wait(1);

		// Act(() => 
		// {
		// 	EnvironmentController.Singleton.UpdateLights("sun", false);
		// 	EnvironmentController.Singleton.UpdateLights("arena", true);
		// 	EnvironmentController.Singleton.SetBiolumenescence(1);
		// 	EnvironmentController.Singleton.SetFogDensity(0.005f);
		// });
	}

	void RegisterAutoAttacks()
	{
		// ================================
		// Auto attacks
		// ================================
		for (var i = 5; i < 171; i++)
		{
			if ((i >= 39 && i < 59) || (i >= 79 && i < 96) || (i >= 113 && i < 139))
				continue;
			GotoAbleton(i);
			Wait(-Parent.AutoAttack.Settings.HoldTime);
			Cast(Parent.AutoAttack);
		}

		List<double> extraAutos = new()
		{
			8.3, 12.3, 16.3, 20.3, 24.3, 28.3, 32.3, 36.3,
			62.3, 66.3, 70.3, 74.3, 78.3,
			99.3, 103.3, 107.3, 111.3
		};
		for (var i = 139; i < 171; i++)
		{
			double index = i + 0.3;
			extraAutos.Add(index);
		}
		foreach (var autoIndex in extraAutos)
		{
			GotoAbleton(autoIndex);
			Wait(-Parent.AutoAttack.Settings.HoldTime);
			Cast(Parent.AutoAttack);
		}
	}

	void RegisterRaidwides()
	{
		List<double> locations = new()
		{
			8.3, 12.3, 24.3, 32.3, 99.3, 103.3, 107.3, 128.3,
		};
		foreach (var autoIndex in locations)
		{
			GotoAbleton(autoIndex);
			Wait(-Parent.Raidwide.Settings.PrepareTime);
			Cast(Parent.Raidwide);
		}
	}

	public override void _Ready()
	{
		Start();
		var targetIndex = GetMarkBeatIndex("Debug");
		Music.Singleton.SeekTo(targetIndex);
	}
}