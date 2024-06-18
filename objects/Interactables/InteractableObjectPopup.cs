using Godot;
using System;
using System.Collections.Generic;

namespace Project;

public partial class InteractableObjectPopup : BasePopup
{
	RichTextLabel label;
	readonly List<InteractableObject> objects = new();

	public override void _Ready()
	{
		base._Ready();
		Visible = false;
		instance = this;
		FollowMouse();
		label = GetNode<RichTextLabel>("Label");
	}

	public void AddObject(InteractableObject interactable)
	{
		if (!objects.Contains(interactable))
			objects.Add(interactable);

		var box = label.GetThemeDefaultFont().GetStringSize(interactable.labelText, width: 200);
		label.Size = box;
		SetBody(interactable.labelText);
		MakeVisible();
	}

	public void RemoveObject(InteractableObject interactable)
	{
		objects.Remove(interactable);
		Visible = objects.Count > 0;
	}

	private static InteractableObjectPopup instance = null;
	public static InteractableObjectPopup Singleton
	{
		get => instance;
	}
}
