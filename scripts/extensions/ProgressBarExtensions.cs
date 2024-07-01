using System;
using Godot;

namespace Project;

public static class ProgressBarExtensions
{
	public static float GetFillOpacity(this ProgressBar bar)
	{
		return GetStyleOpacity(bar, "fill".ToStringName());
	}

	public static float GetBackgroundOpacity(this ProgressBar bar)
	{
		return GetStyleOpacity(bar, "background".ToStringName());
	}

	public static void SetFillOpacity(this ProgressBar bar, double value)
	{
		SetStyleOpacity(bar, "fill".ToStringName(), (float)value);
	}

	public static void SetBackgroundOpacity(this ProgressBar bar, double value)
	{
		SetStyleOpacity(bar, "background".ToStringName(), (float)value);
	}

	public static float GetStyleOpacity(this ProgressBar bar, StringName styleName)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		return style.BgColor.A;
	}

	public static void SetStyleOpacity(this ProgressBar bar, StringName styleName, float value)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		if (style.BgColor.A == value)
			return;
		style.BgColor = new Color(style.BgColor.R, style.BgColor.G, style.BgColor.B, Math.Max(0, Math.Min(1, value)));
	}

	public static Color GetFillColor(this ProgressBar bar)
	{
		return GetStyleColor(bar, "fill".ToStringName());
	}

	public static Color GetBackgroundColor(this ProgressBar bar)
	{
		return GetStyleColor(bar, "background".ToStringName());
	}

	public static void SetFillColor(this ProgressBar bar, Color value)
	{
		SetStyleColor(bar, "fill".ToStringName(), value);
	}

	public static void SetBackgroundColor(this ProgressBar bar, Color value)
	{
		SetStyleColor(bar, "background".ToStringName(), value);
	}

	private static Color GetStyleColor(this ProgressBar bar, StringName styleName)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		return style.BgColor;
	}

	private static void SetStyleColor(this ProgressBar bar, StringName styleName, Color color)
	{
		var fillStyle = bar.GetThemeStylebox(styleName);
		if (fillStyle is not StyleBoxFlat)
			throw new Exception("Fill style is not StyleBoxFlat");

		var style = (StyleBoxFlat)fillStyle;
		style.BgColor = color;
	}
}