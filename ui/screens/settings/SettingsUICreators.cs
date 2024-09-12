using BeatGame.scripts.preferences;
using Godot;
using System;
using System.Drawing;
using System.Linq;

namespace Project;

public partial class SettingsUI : Control
{
	void InitUI(SettingsContainer container)
	{
		foreach (var child in TabPanelContainer.GetChildren())
			TabPanelContainer.RemoveChild(child);
		var tabBar = this.GetComponent<TabBar>();
		tabBar.ClearTabs();
		tabBar.TabChanged += (tab) =>
		{
			TabPanelContainer.GetChildren().Cast<Control>().ToList().ForEach(c => c.Visible = false);
			TabPanelContainer.GetChild<Control>((int)tab).Visible = true;
		};

		int tabIndex = 0;
		foreach (var tabId in container.Tabs)
		{
			var tabPanel = CreateTabPanel(container, tabIndex++);
			tabBar.AddTab(tabId);
		}
		TabPanelContainer.GetChild<Control>(0).Visible = true;
	}

	Control CreateTabPanel(SettingsContainer container, int tabIndex)
	{
		var wrapper = new MarginContainer();
		wrapper.SetAnchorsPreset(LayoutPreset.FullRect);
		wrapper.Visible = false;
		wrapper.AddThemeConstantOverride("margin_top", 8);
		wrapper.AddThemeConstantOverride("margin_left", 8);
		wrapper.AddThemeConstantOverride("margin_right", 8);
		wrapper.AddThemeConstantOverride("margin_bottom", 8);
		TabPanelContainer.AddChild(wrapper);

		var tabPanel = new VBoxContainer();
		tabPanel.SetAnchorsPreset(LayoutPreset.FullRect);
		wrapper.AddChild(tabPanel);

		foreach (var entry in container.Entries.Where(e => e.Tab == tabIndex && e.Category == -1))
		{
			CreateEntryControls(entry, tabPanel);
		}

		int categoryIndex = 0;
		foreach (var (_, name) in container.Categories.Where(c => c.tabIndex == tabIndex))
		{
			CreateCategoryPanel(container, tabPanel, tabIndex, categoryIndex, name);
			categoryIndex++;
		}

		return tabPanel;
	}

	Control CreateCategoryPanel(SettingsContainer container, Control tab, int tabIndex, int categoryIndex, string name)
	{
		var categoryPanel = new VBoxContainer();
		categoryPanel.SetAnchorsPreset(LayoutPreset.TopWide);
		tab.AddChild(categoryPanel);

		var categoryLabel = new Label()
		{
			Text = name,
		};
		categoryLabel.AddThemeFontSizeOverride("font_size".ToStringName(), 22);
		categoryPanel.AddChild(categoryLabel);
		categoryPanel.AddChild(new HSeparator());

		foreach (var entry in container.Entries.Where(e => e.Tab == tabIndex && e.Category == categoryIndex))
		{
			CreateEntryControls(entry, categoryPanel);
		}

		return categoryPanel;
	}

	void CreateEntryControls(SettingsEntry entry, Control parent)
	{
		var container = new HBoxContainer
		{
			CustomMinimumSize = new Vector2(0, 30)
		};
		parent.AddChild(container);

		container.AddChild(new Label
		{
			Text = entry.Name,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		});

		if (entry is DropdownSettingsEntry dropdown)
		{
			var dropdownControl = new OptionButton
			{
				Text = dropdown.Name,
				Selected = dropdown.Options.ToList().IndexOf(dropdown.Selected),
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
			};
			dropdown.Options
				.ToList()
				.ForEach((option) =>
				{
					dropdownControl.AddItem(option.Label);
				});

			dropdown.OnUpdateUI += () => dropdownControl.Selected = dropdown.Options.ToList().IndexOf(dropdown.Selected);
			dropdownControl.ItemSelected += (index) => dropdown.SetValue(dropdown.Options[index].Value);
			container.AddChild(dropdownControl);
			dropdownControl.Selected = dropdown.Options.ToList().IndexOf(dropdown.Selected);
		}
		else if (entry is SliderSettingsEntry slider)
		{
			var subcontainer = new HBoxContainer()
			{
				SizeFlagsVertical = SizeFlags.ExpandFill,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
			};
			var sliderControl = new HSlider
			{
				Name = slider.Name,
				MinValue = slider.Min,
				MaxValue = slider.Max,
				Value = slider.Value,
				Step = slider.Step,
				SizeFlagsVertical = SizeFlags.ExpandFill,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
			};
			var sliderValue = new Label
			{
				Text = slider.TextValue,
				HorizontalAlignment = HorizontalAlignment.Right,
				CustomMinimumSize = new Vector2(50, 0),
				SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
			};

			slider.OnUpdateUI += () => sliderControl.Value = slider.Value;
			sliderControl.ValueChanged += (value) =>
			{
				slider.SetValue(value);
				sliderValue.Text = slider.TextValue;
			};
			subcontainer.AddChild(sliderControl);
			subcontainer.AddChild(sliderValue);
			container.AddChild(subcontainer);
			// Godot bug, requires force updating the slider value again for it to render properly
			sliderControl.Value = slider.Value;
		}
		else if (entry is InputSettingsEntry input)
		{
			var inputControl = new LineEdit
			{
				PlaceholderText = input.Name,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
			};
			container.AddChild(inputControl);
		}
	}
}