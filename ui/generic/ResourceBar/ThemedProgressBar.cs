using System.Diagnostics;
using Godot;

namespace Project;

[Tool]
public partial class ThemedProgressBar : Control
{
	[Export] public ProgressBar Bar;
	[Export] public ProgressBar PositiveGhost;
	[Export] public ProgressBar NegativeGhost;
	[Export] public Label PositiveComboLabel;
	[Export] public Label NegativeComboLabel;

	public override void _Draw()
	{
		base._Draw();

		if (Engine.IsEditorHint())
		{
			// LoadStyles();
		}
	}
	public override void _Ready()
	{
		LoadStyles();
	}

	private void LoadStyles()
	{
		if (!HasThemeStylebox("background", "GhostBar"))
			return;

		var backgroundStyle = GetThemeStylebox("background", "GhostBar");
		var fillStyle = GetThemeStylebox("fill", "GhostBar");
		var positiveGhostStyle = GetThemeStylebox("ghost_positive", "GhostBar");
		var negativeGhostStyle = GetThemeStylebox("ghost_negative", "GhostBar");
		if (Engine.IsEditorHint() && fillStyle.ResourcePath.StartsWith("res://ui/generic/ResourceBar/ThemedProgressBar.tscn"))
		{
			var defaultTheme = GD.Load<Theme>("res://ui/generic/ResourceBar/themes/HealthBar.tres");
			backgroundStyle = defaultTheme.GetStylebox("background", "GhostBar");
			fillStyle = defaultTheme.GetStylebox("fill", "GhostBar");
			positiveGhostStyle = defaultTheme.GetStylebox("ghost_positive", "GhostBar");
			negativeGhostStyle = defaultTheme.GetStylebox("ghost_negative", "GhostBar");
		}

		Bar.AddThemeStyleboxOverride("fill", fillStyle);
		PositiveGhost.AddThemeStyleboxOverride("fill", positiveGhostStyle);
		NegativeGhost.AddThemeStyleboxOverride("fill", negativeGhostStyle);
		NegativeGhost.AddThemeStyleboxOverride("background", backgroundStyle);

	}
}
