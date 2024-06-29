namespace Project;

public enum PlayableScene : int
{
	MainMenu = 1,
	TrainingRoom = 2,
	BossArenaAeriel = 3,
}

public static class PlayableSceneExtensions
{
	public static int ToVariant(this PlayableScene scene)
	{
		return (int)scene;
	}
}