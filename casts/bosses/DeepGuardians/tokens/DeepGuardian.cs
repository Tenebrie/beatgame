namespace Project;

public partial class DeepGuardian : BasicEnemyController
{
	public DeepGuardian()
	{
		FriendlyName = "Deep Guardian";
		Gravity = 0;
	}

	public void Activate()
	{
		QueueFree();
	}
}
