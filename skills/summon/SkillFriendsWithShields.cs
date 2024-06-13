namespace Project;

public partial class SkillFriendsWithShields : BaseSkill
{
	public SkillFriendsWithShields()
	{
		Settings = new()
		{
			FriendlyName = "Friends With Shields",
			Description = MakeDescription(
				$"Who needs to be tanky when you have allies to protect you? Your summons have extra health and generate increased threat.",
				"\nIn other words, the enemies will always prioritize your summons before attacking you."
			),
			IconPath = "res://assets/icons/SpellBook06_79.png",
			PassiveBuff = BuffFactory.Of<BuffExtraTankySummons>(),
		};
	}
}