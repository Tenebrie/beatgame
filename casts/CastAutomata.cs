using System;
using System.Collections.Generic;
using System.Linq;

namespace Project;

public class CastAutomata<StateT> where StateT : Enum
{
	bool IsStarted = false;
	public StateT State;

	readonly StateT StartingState;
	readonly List<(float atBeatIndex, StateT fromState, StateT toState)> Transitions;
	readonly Action<StateT> StateTransitionCallback;
	double StartingBeatIndex;

	public CastAutomata(StateT startingState, Action<StateT> onStateTransition, List<(float atBeatIndex, StateT toState)> transitions)
	{
		StartingState = startingState;
		StateTransitionCallback = onStateTransition;

		Transitions = new();
		StateT fromState = startingState;
		foreach (var (atBeatIndex, toState) in transitions)
		{
			Transitions.Add((atBeatIndex, fromState, toState));
			fromState = toState;
		}
	}

	public void Tick(BeatTime _)
	{
		if (!IsStarted)
			return;

		var beatIndex = Music.Singleton.BeatIndex - StartingBeatIndex;
		foreach (var (atBeatIndex, fromState, toState) in Transitions)
		{
			if (State.Equals(fromState) && beatIndex >= atBeatIndex - 1)
			{
				StateTransitionCallback(toState);
				State = toState;
			}
		}
	}

	public void Start()
	{
		IsStarted = true;
		State = StartingState;
		StartingBeatIndex = Music.Singleton.BeatIndex;
	}
}