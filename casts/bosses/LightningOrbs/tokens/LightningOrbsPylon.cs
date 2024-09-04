using Godot;
using Project;
using System;
using System.Linq;

namespace Project;

public partial class LightningOrbsPylon : BasicEnemyController
{
	bool IsActive = false;
	BossAuto BossAuto;
	EnergyExpulsion AreaAttack;

	public override void _Ready()
	{
		FriendlyName = "Power Pylon";
		Alliance = UnitAlliance.Hostile;

		base._Ready();

		Health.SetBaseMaxValue(5);
		Targetable.SelectionRadius = 1.00f;

		Buffs.Add(new BuffLightningOrbsPylonInvuln());
		BossAuto = CastLibrary.Register(new BossAuto(this));
		BossAuto.Damage = 3;
		AreaAttack = CastLibrary.Register(new EnergyExpulsion(this));
		Music.Singleton.BeatTick += OnBeatTick;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Music.Singleton.BeatTick -= OnBeatTick;
	}

	public void Activate()
	{
		IsActive = true;
	}

	public void OnBeatTick(BeatTime time)
	{
		if (!IsActive)
			return;

		var index = Music.Singleton.BeatIndex;
		if (index % 4 != 1 && index % 4 != 3)
			return;

		var targets = AllUnits.Where(unit => unit.HostileTo(this)).ToList();
		var target = targets[(int)(GD.Randi() % targets.Count)];
		BossAuto.CastBegin(new CastTargetData()
		{
			HostileUnit = target,
		});
	}

	public void PerformFinalCast()
	{
		AreaAttack.CastBegin(new CastTargetData());
	}

	partial class EnergyExpulsion : BossCastRaidwide
	{
		public EnergyExpulsion(BaseUnit parent) : base(parent)
		{
			Settings = new()
			{
				FriendlyName = "Energy Expulsion",
				InputType = CastInputType.AutoRelease,
				TargetType = CastTargetType.None,
				HoldTime = 5,
				PrepareTime = 0,
				RecastTime = 0,
			};
		}

		protected override void OnCastStarted(CastTargetData targets)
		{
			base.OnCastStarted(targets);
			DamageTotal = (float)Math.Round(Parent.Health.Current) * 15.0f;
		}

		protected override void OnCastCompleted(CastTargetData _)
		{
			Parent.Instakill();
		}
	}
}
