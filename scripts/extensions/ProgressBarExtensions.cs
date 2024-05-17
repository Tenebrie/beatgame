using System;
using Godot;

public static class ProgressBarExtensions
{
	public static float GetFillOpacity(this ProgressBar bar)
	{
		return GetStyleOpacity(bar, "fill");
	}

	public static float GetBackgroundOpacity(this ProgressBar bar)
	{
		return GetStyleOpacity(bar, "background");
	}

	public static void SetFillOpacity(this ProgressBar bar, double value)
	{
		SetStyleOpacity(bar, "fill", (float)value);
	}

	public static void SetBackgroundOpacity(this ProgressBar bar, double value)
	{
		SetStyleOpacity(bar, "background", (float)value);
	}

	public static float GetStyleOpacity(this ProgressBar bar, string styleName)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		return style.BgColor.A;
	}

	public static void SetStyleOpacity(this ProgressBar bar, string styleName, float value)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		style.BgColor = new Color(style.BgColor.R, style.BgColor.G, style.BgColor.B, Math.Max(0, Math.Min(1, value)));
	}

	public static Color GetFillColor(this ProgressBar bar)
	{
		return GetStyleColor(bar, "fill");
	}

	public static Color GetBackgroundColor(this ProgressBar bar)
	{
		return GetStyleColor(bar, "background");
	}

	public static void SetFillColor(this ProgressBar bar, Color value)
	{
		SetStyleColor(bar, "fill", value);
	}

	public static void SetBackgroundColor(this ProgressBar bar, Color value)
	{
		SetStyleColor(bar, "background", value);
	}

	private static Color GetStyleColor(this ProgressBar bar, string styleName)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		return style.BgColor;
	}

	private static void SetStyleColor(this ProgressBar bar, string styleName, Color color)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		style.BgColor = color;
	}
}