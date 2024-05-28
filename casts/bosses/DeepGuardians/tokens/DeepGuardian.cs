using System.Linq;

namespace Project;

public partial class DeepGuardian : BasicEnemyController
{
	GroundAreaRect castRect;
	public DeepGuardian()
	{
		FriendlyName = "Deep Guardian";
		Gravity = 0;
	}

	public void AttachRect(GroundAreaRect rect)
	{
		castRect = rect;
	}

	public void Activate()
	{
		var targets = castRect.GetUnitsInside().Where(unit => unit.Alliance.HostileTo(Alliance));
		foreach (var target in targets)
		{
			target.Health.Damage(50);
			target.ForcefulMovement.Push(8, ForwardVector, 1);
		}

		QueueFree();
	}
}
