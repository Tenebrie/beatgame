using System;
using System.Collections.Generic;
using BeatGame.scripts.music;
using Godot;

namespace Project;

public partial class Music : Node
{
	public float PredictiveBeatTime = 0;

	readonly Dictionary<BeatTime, List<(Action<BeatTime> callback, float timeBefore)>> PredictiveBeatCallbacks = new();

	public void InitPredictiveBeatCallbacks()
	{
		PredictiveBeatCallbacks[BeatTime.Whole] = new();
		PredictiveBeatCallbacks[BeatTime.Half] = new();
		PredictiveBeatCallbacks[BeatTime.Quarter] = new();
		PredictiveBeatCallbacks[BeatTime.Eighth] = new();
		PredictiveBeatCallbacks[BeatTime.Sixteenth] = new();
	}

	private void ProcessPredictiveBeatCallbacks()
	{
		var upcomingBeats = GetUpcomingBeats();
		var engineTime = CastUtils.GetEngineTime();
		foreach (var upcomingBeat in upcomingBeats)
		{
			if (PredictiveBeatTime >= engineTime + upcomingBeat.timeUntil)
				continue;

			PredictiveBeatTime = engineTime + upcomingBeat.timeUntil;
			foreach (var callback in PredictiveBeatCallbacks[upcomingBeat.beatTime])
			{
				QueuePredictiveBeat(upcomingBeat, callback);
			}
		}
	}

	private void QueuePredictiveBeat((BeatTime beatTime, float timeUntil) upcomingBeat, (Action<BeatTime> callback, float timeBefore) callback)
	{
		new Action(async () =>
		{
			await ToSignal(GetTree().CreateTimer(upcomingBeat.timeUntil - callback.timeBefore), "timeout".ToStringName());
			// Check if the callback was removed before the timeout
			if (PredictiveBeatCallbacks[upcomingBeat.beatTime].Contains(callback))
				callback.callback(upcomingBeat.beatTime);
		}).Invoke();
	}

	public List<(BeatTime beatTime, float timeUntil)> GetUpcomingBeats()
	{
		var beats = new List<(BeatTime, float)>();
		var engineTime = CastUtils.GetEngineTime();
		for (var i = 1; i < 10; i++)
		{
			var nextBeatTime = Utils.TickIndexToBeatTime(PreciseBeatIndex + i);
			var nextBeatTimingMs = BeatTimer.LastTickedAt + MinBeatSize / GameSpeed * i - engineTime;
			beats.Add((nextBeatTime, nextBeatTimingMs));
		}

		return beats;
	}

	public void OnBeforeBeatTick(BeatTime beatTime, Action<BeatTime> callback, float timeBefore)
	{
		var callbackTuple = (callback, timeBefore);
		if (beatTime.Has(BeatTime.Whole))
			PredictiveBeatCallbacks[BeatTime.Whole].Add(callbackTuple);
		if (beatTime.Has(BeatTime.Half))
			PredictiveBeatCallbacks[BeatTime.Half].Add(callbackTuple);
		if (beatTime.Has(BeatTime.Quarter))
			PredictiveBeatCallbacks[BeatTime.Quarter].Add(callbackTuple);
		if (beatTime.Has(BeatTime.Eighth))
			PredictiveBeatCallbacks[BeatTime.Eighth].Add(callbackTuple);
		if (beatTime.Has(BeatTime.Sixteenth))
			PredictiveBeatCallbacks[BeatTime.Sixteenth].Add(callbackTuple);

		// If processing time is in the future, process new callback immediately
		var upcomingBeats = GetUpcomingBeats();
		var engineTime = CastUtils.GetEngineTime();
		foreach (var upcomingBeat in upcomingBeats)
		{
			if (PredictiveBeatTime < engineTime + upcomingBeat.timeUntil || beatTime.HasNot(upcomingBeat.beatTime))
				continue;

			QueuePredictiveBeat(upcomingBeat, callbackTuple);
		}
	}

	public void OffBeforeBeatTick(Action<BeatTime> callback)
	{
		foreach (var beatTime in PredictiveBeatCallbacks.Keys)
		{
			PredictiveBeatCallbacks[beatTime].RemoveAll(tuple => tuple.callback == callback);
		}
	}
}