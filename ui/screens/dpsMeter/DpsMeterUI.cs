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
	readonly Dictionary<string, DpsMeterCast> damageDealtPerCast = new();

	readonly Dictionary<SilentDamageReason, VirtualCastRetaliation> virtualCasts = new();

	public override void _Ready()
	{
		Visible = false;
		for (var i = 0; i < 3; i++)
			castContainer.GetChild(i).QueueFree();

		SignalBus.Singleton.DamageTaken += OnDamageTaken;
		SignalBus.Singleton.SilentDamageTaken += OnSilentDamageTaken;
		Music.Singleton.BeatTick += OnBeatTick;
		clearButton.Pressed += Reset;

		virtualCasts.Add(SilentDamageReason.Retaliate, new VirtualCastRetaliation(null));
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
		if (@event.IsActionPressed("ToggleDpsMeter".ToStringName()))
		{
			Visible = !Visible;
		}
	}

	void OnSilentDamageTaken(BuffIncomingDamageVisitor data, SilentDamageReason reason)
	{
		if (data.ResourceType != ObjectResourceType.Health || data.SourceUnit == null || data.SourceUnit.Alliance != UnitAlliance.Player)
			return;

		if (reason == SilentDamageReason.ResourceCost)
			return;

		var castId = data.SourceUnit.GetType().ToString() + "-silent-" + reason;
		BaseCast virtualSourceCast = virtualCasts.GetValueOrDefault(reason);
		ProcessDamage(data.Value, virtualSourceCast, castId);
	}

	void OnDamageTaken(BuffIncomingDamageVisitor data)
	{
		if (data.ResourceType != ObjectResourceType.Health || data.SourceUnit == null || data.SourceUnit.Alliance != UnitAlliance.Player || data.SourceCast == null)
			return;

		var castId = data.SourceUnit.GetType().ToString() + data.SourceCast.GetType();
		ProcessDamage(data.Value, data.SourceCast, castId);
	}

	void ProcessDamage(float value, BaseCast sourceCast, string castId)
	{
		if (!isStarted)
			currentTime = 0;
		isStarted = true;
		var meterExists = damageDealtPerCast.TryGetValue(castId, out var affectedMeter);
		if (!meterExists)
		{
			affectedMeter = Lib.LoadScene(Lib.UI.DpsMeterCast).Instantiate<DpsMeterCast>();
			castContainer.AddChild(affectedMeter);
			affectedMeter.SetCast(sourceCast);
			damageDealtPerCast.Add(castId, affectedMeter);
		}
		affectedMeter.DamageDealt += value;
		totalDamageDealt += value;
		totalDamageLabel.Text = Math.Round(totalDamageDealt).ToString();

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
