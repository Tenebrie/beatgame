using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class BossCelestiosTimeline : BaseTimeline
{
	new BossCelestios Parent;
	RectangularTelegraph puddleRect;

	public BossCelestiosTimeline(BossCelestios parent) : base(parent)
	{
		Parent = parent;
		GotoAbleton(1);

		Mark("Start");

		// RegisterAutoAttacks();
		// RegisterRaidwides();

		// ===================================================
		// Intro
		// ===================================================
		Goto(0);

		Act(() =>
		{
			Parent.Composables.Find<FadingObjectBehaviour>().SetFade(1.05f);
			Parent.Composables.Find<FadingObjectBehaviour>().FadeIn(5);
		});
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
			8.3, 12.3, 24.3, 32.3, 57.4, 99.3, 103.3, 107.3, 128.4,
		};
		foreach (var autoIndex in locations)
		{
			GotoAbleton(autoIndex);
			// Wait(-Parent.Raidwide.Settings.PrepareTime);
			// Cast(Parent.Raidwide);
		}
	}

	protected override void HandleBeatTick(BeatTime time)
	{
		if (puddleRect == null)
			return;

		foreach (var target in puddleRect.GetTargets())
		{
			target.Health.Damage(5f, Parent);
			var effect = Lib.LoadScene(Lib.Effect.ShieldBashImpact).Instantiate<SimpleParticleEffect>();
			GetTree().CurrentScene.AddChild(effect);
			effect.SetLifetime(0.01f);
			effect.Position = target.GlobalCastAimPosition;
		}
	}
}