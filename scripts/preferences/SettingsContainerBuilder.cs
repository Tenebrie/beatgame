using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Project;

namespace BeatGame.scripts.preferences;

public class SettingsContainerBuilder
{
	readonly SettingsContainer container = new();

	int editedTab = -1;
	int editedCategory = -1;
	readonly Dictionary<SettingsKey, SettingsEntry> entryMap = new();

	public SettingsContainerBuilder SetTab(string tab)
	{
		container.Tabs = container.Tabs.Append(tab).ToArray();
		editedTab++;
		editedCategory = -1;
		return this;
	}

	public SettingsContainerBuilder SetCategory(string category)
	{
		container.Categories = container.Categories.Append((editedTab, category)).ToArray();
		editedCategory++;
		return this;
	}

	private SettingsContainerBuilder AddEntry(SettingsKey id, SettingsEntry entry)
	{
		entry.Key = id;
		entry.Tab = editedTab;
		entry.Category = editedCategory;
		container.AddEntry(entry);
		entryMap[entry.Key] = entry;
		return this;
	}

	public SettingsContainerBuilder AddToggle(SettingsKey id, string name, bool value)
	{
		var entry = new ToggleSettingsEntry
		{
			Name = name,
			Value = value,
		};
		return AddEntry(id, entry);
	}

	public SettingsContainerBuilder AddSlider(SettingsKey id, string name, float min, float value, float max, float step, bool percentage = false, Func<float, string> textConverter = null)
	{
		var entry = new SliderSettingsEntry
		{
			Name = name,
			Min = min,
			Max = max,
			Value = value,
			Step = step,
			Percentage = percentage,
			TextConverter = textConverter,
		};
		return AddEntry(id, entry);
	}

	public SettingsContainerBuilder AddDropdown<T>(SettingsKey id, string name, (string Label, T Value)[] options, T selected)
	{
		var plainOptions = options.Select(o => (o.Label, Value: Convert.ToInt32(o.Value))).ToArray();
		var entry = new DropdownSettingsEntry
		{
			Name = name,
			Options = plainOptions,
			Selected = plainOptions.First(o => o.Value.Equals(Convert.ToInt32(selected))),
		};
		return AddEntry(id, entry);
	}

	public SettingsContainer Build()
	{
		return container;
	}
}