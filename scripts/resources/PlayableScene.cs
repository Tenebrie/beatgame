namespace Project;

public enum PlayableScene : int
{
	Unknown = 0,
	MainMenu = 1,
	TrainingRoom = 2,
	BossArenaAeriel = 3,
	BossArenaCelestios = 31,
	DungeonRatBasement = 401,
}

public static class PlayableSceneExtensions
{
	public static int ToVariant(this PlayableScene scene)
	{
		return (int)scene;
	}
}