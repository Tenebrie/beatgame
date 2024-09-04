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

	readonly List<StringName> castBindings = new()
	{
		"AutoAttack".ToStringName(),
		"GroundAttack".ToStringName(),
		"MARK_ONE".ToStringName(),
		"MARK_TWO".ToStringName()
	};
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
			return;

		if (@event.IsActionPressed("ToggleEditor".ToStringName()))
		{
			EditingMode = !EditingMode;
			Visible = EditingMode;
		}

		// var beatIndex = Music.Singleton.GetNearestBeatIndex(BeatTime.Quarter);

		// if (@event.IsActionPressed("EditorCast1".ToStringName()))
		// {
		// 	Add(beatIndex, castBindings[0]);
		// }
		// if (@event.IsActionPressed("EditorCast2".ToStringName()))
		// {
		// 	Add(beatIndex, castBindings[1]);
		// }
		// if (@event.IsActionPressed("EditorCast3".ToStringName()))
		// {
		// 	Add(beatIndex, castBindings[2]);
		// }
		// if (@event.IsActionPressed("EditorCast4".ToStringName()))
		// {
		// 	Add(beatIndex, castBindings[3]);
		// }
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
