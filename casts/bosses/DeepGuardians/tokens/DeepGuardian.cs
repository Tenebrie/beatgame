using System.Linq;

namespace Project;

public partial class DeepGuardian : BasicEnemyController
{
	RectangularTelegraph castRect;
	public DeepGuardian()
	{
		FriendlyName = "Deep Guardian";
		BaseGravity = 0;
		Alliance = UnitAlliance.Hostile;
	}
}
