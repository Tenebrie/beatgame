using System.Linq;

namespace Project;

public partial class DeepGuardian : BasicEnemyController
{
	GroundAreaRect castRect;
	public DeepGuardian()
	{
		FriendlyName = "Deep Guardian";
		BaseGravity = 0;
		Alliance = UnitAlliance.Hostile;
	}
}
