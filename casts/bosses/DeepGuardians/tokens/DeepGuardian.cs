using System.Linq;

namespace Project;

public partial class DeepGuardian : BasicEnemyController
{
	GroundAreaRect castRect;
	public DeepGuardian()
	{
		FriendlyName = "Deep Guardian";
		Gravity = 0;
		Alliance = UnitAlliance.Hostile;
	}
}
