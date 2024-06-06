using Godot;
using System;
using System.Collections.Generic;

namespace Project;

public partial class FightEditorUI : Control
{
	private TextEdit textEdit;
	public bool EditingMode = false;

	public override void _Ready()
	{
		instance = this;
		Visible = EditingMode;
		textEdit = GetNode<TextEdit>("TextEdit");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleEditor"))
		{
			EditingMode = !EditingMode;
			Visible = EditingMode;
		}

		var beatIndex = Music.Singleton.GetNearestBeatIndex(BeatTime.Quarter);
		List<string> castBindings = new() { "AutoAttack", "GroundAttack", "MARK_ONE", "MARK_TWO" };

		if (@event.IsActionPressed("EditorCast1"))
		{
			Add(beatIndex, castBindings[0]);
		}
		if (@event.IsActionPressed("EditorCast2"))
		{
			Add(beatIndex, castBindings[1]);
		}
		if (@event.IsActionPressed("EditorCast3"))
		{
			Add(beatIndex, castBindings[2]);
		}
		if (@event.IsActionPressed("EditorCast4"))
		{
			Add(beatIndex, castBindings[3]);
		}
	}

	private void Add(double beatIndex, string cast)
	{
		var bossName = "parent";
		var script = $"Add({beatIndex + 1}, {bossName}.{cast});";

		textEdit.Text += script + "\n";
		textEdit.ScrollVertical = 999999;
	}

	private static FightEditorUI instance = null;
	public static FightEditorUI Singleton
	{
		get => instance;
	}
}
