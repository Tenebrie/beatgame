using Godot;
using System;

namespace Project;

public partial class SecretTreeChest : InteractableObject
{
	protected override void OnInteract()
	{
		MakeNonInteractable();
		UnlockSecretTree();
	}

	void UnlockSecretTree()
	{
		var secretTree = new SkillTree(
			group: SkillGroup.Secret,
			roots: new() { new SecretSkillNukeOfTheGreatTree(), new SecretSkillUnlimitedPower0() },

			links: new()
			{
				SkillTreeManager.Link<SecretSkillUnlimitedPower0, SecretSkillUnlimitedPower1>(),
				SkillTreeManager.Link<SecretSkillUnlimitedPower1, SecretSkillUnlimitedPower2>(),
				SkillTreeManager.Link<SecretSkillUnlimitedPower2, SecretSkillUnlimitedPower3>(),
			}
		);
		SkillTreeManager.Singleton.RegisterSkillTree(secretTree);
		SignalBus.SendMessage("Secret skill tree unlocked!");
	}
}
