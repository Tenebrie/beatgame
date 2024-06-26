using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastHardEnrage : BaseCast
{
	public BossCastHardEnrage(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Destruction of the Universe",
			TargetType = CastTargetType.None,
			InputType = CastInputType.AutoRelease,
			HoldTime = 32,
			RecastTime = 0,
		};
	}

	public override void _Process(double delta)
	{
		if (!IsCasting)
			return;

		if (Parent.IsDead)
			CastInterrupt();
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		Parent.Buffs.Add(new InvulnerabilityBuff());
		RestartAfterDelay();
	}

	async void RestartAfterDelay()
	{
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		TimelineManager.Singleton.ResetFight();
	}

	public partial class InvulnerabilityBuff : BaseBuff
	{
		public InvulnerabilityBuff()
		{
			Settings = new()
			{
				FriendlyName = "Destruction of the Universe",
				Description = MakeDescription(
					"It was nice while it lasted..."
				),
				IconPath = "res://assets/icons/SpellBook08_89.png"
			};
		}

		public override void ModifyUnit(BuffUnitStatsVisitor visitor)
		{
			visitor.PercentageDamageTaken[ObjectResourceType.Health] = 0;
			visitor.PercentageCCReduction = 1;
		}
	}
}