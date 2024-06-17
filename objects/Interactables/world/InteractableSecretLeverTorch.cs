using System.Linq;
using Godot;

namespace Project;

public partial class InteractableSecretLeverTorch : InteractableObject
{
	protected override void OnInteract()
	{
		var light = GetParent().GetChildren().Where(child => child is ControllableOmniLight).Cast<ControllableOmniLight>().First();
		if (light == null)
			return;

		light.Toggle();
	}
}