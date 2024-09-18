using System;
using System.Linq;

namespace BeatGame.scripts.preferences;

public abstract class SettingsEntry
{
	public int Tab;
	public int Category;
	public SettingsKey Key;
	public string Name;

	public Action OnApplySideEffects;
	public void ApplySideEffects()
	{
		OnApplySideEffects?.Invoke();
	}

	public Action OnUpdateUI;
	public void UpdateUI()
	{
		OnUpdateUI?.Invoke();
	}
}

public class ToggleSettingsEntry : SettingsEntry
{
	public bool Value;

	public string TextValue => Value ? "On" : "Off";

	public void SetValue(bool value)
	{
		Value = value;
		ApplySideEffects();
	}
}

public class SliderSettingsEntry : SettingsEntry
{
	public float Min;
	public float Max;
	public float Value;
	public float Step;
	public bool Percentage;
	public Func<float, string> TextConverter;

	public string TextValue
	{
		get
		{
			if (TextConverter != null)
				return TextConverter(Value);
			if (Percentage)
				return Math.Round(Value * 100) + "%";
			return Value.ToString();
		}
	}

	public void SetValue(double value)
	{
		Value = (float)value;
		ApplySideEffects();
	}
}

public class DropdownSettingsEntry : SettingsEntry
{
	public (string Label, int Value) Selected;
	public (string Label, int Value)[] Options;

	public void SetValue(int value)
	{
		Selected = Options.First((o) => o.Value.Equals(value));
		ApplySideEffects();
	}
}

public class InputSettingsEntry : SettingsEntry
{

}