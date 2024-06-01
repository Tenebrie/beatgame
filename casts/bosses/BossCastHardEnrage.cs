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

	protected override void OnCastCompleted(CastTargetData target)
	{
		var targets = BaseUnit.AllUnits.Where(unit => unit.IsAlive && unit.HostileTo(Parent));

		foreach (var t in targets)
		{
			t.Instakill();
		}
	}
}