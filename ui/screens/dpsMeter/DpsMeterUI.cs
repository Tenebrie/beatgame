using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project;

public partial class DpsMeterUI : Control
{
	[Export] VBoxContainer castContainer;
	[Export] Label totalDamageLabel;
	[Export] Label combatTimeLabel;
	[Export] Label damagePerSecondLabel;
	[Export] Button clearButton;

	bool isStarted = false;
	float currentTime = 0;
	float totalDamageDealt = 0;
	readonly Dictionary<BaseCast, DpsMeterCast> damageDealtPerCast = new();

	public override void _Ready()
	{
		Visible = false;
		for (var i = 0; i < 3; i++)
			castContainer.GetChild(i).QueueFree();

		SignalBus.Singleton.DamageTaken += OnDamageTaken;
		Music.Singleton.BeatTick += OnBeatTick;
		SkillTreeManager.Singleton.SkillUp += (BaseSkill skill) => Reset();
		SkillTreeManager.Singleton.SkillDown += (BaseSkill skill) => Reset();
		clearButton.Pressed += Reset;
	}

	void OnBeatTick(BeatTime time)
	{
		if (!isStarted || time.HasNot(BeatTime.EveryFullBeat))
			return;

		currentTime += 1;
		combatTimeLabel.Text = currentTime.ToString();
		damagePerSecondLabel.Text = Math.Round(totalDamageDealt / currentTime).ToString();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleDpsMeter"))
		{
			Visible = !Visible;
			if (!Visible)
				Reset();
		}
	}

	void OnDamageTaken(BuffIncomingDamageVisitor data)
	{
		if (data.Target is PlayerController || data.SourceUnit is not PlayerController || data.SourceCast == null)
			return;

		isStarted = true;
		var meterExists = damageDealtPerCast.TryGetValue(data.SourceCast, out var affectedMeter);
		if (!meterExists)
		{
			affectedMeter = Lib.LoadScene(Lib.UI.DpsMeterCast).Instantiate<DpsMeterCast>();
			castContainer.AddChild(affectedMeter);
			affectedMeter.SetCast(data.SourceCast);
			damageDealtPerCast.Add(data.SourceCast, affectedMeter);
		}
		affectedMeter.DamageDealt += data.Value;
		totalDamageDealt += data.Value;
		totalDamageLabel.Text = totalDamageDealt.ToString();

		var highestDamageDealt = damageDealtPerCast.Values.Select(val => val.DamageDealt).OrderByDescending(val => val).First();
		foreach (var meter in damageDealtPerCast.Values)
		{
			meter.SetFractionOfHighestDamageCast(meter.DamageDealt / highestDamageDealt);
			meter.SetFractionOfTotalDamage(meter.DamageDealt / totalDamageDealt);
			castContainer.RemoveChild(meter);
		}

		var sortedMeters = damageDealtPerCast.Values.OrderByDescending(val => val.FractionOfHighestDamage);
		foreach (var meter in sortedMeters)
		{
			castContainer.AddChild(meter);
		}
	}

	void Reset()
	{
		isStarted = false;
		totalDamageDealt = 0;
		foreach (var meter in damageDealtPerCast.Values)
			meter.QueueFree();
		damageDealtPerCast.Clear();

		combatTimeLabel.Text = "0";
		totalDamageLabel.Text = "0";
		damagePerSecondLabel.Text = "0";
	}
}
