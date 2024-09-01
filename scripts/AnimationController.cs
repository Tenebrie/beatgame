using Godot;
using System;
using System.Collections.Generic;

namespace Project;

public partial class AnimationController : AnimationTree
{
	readonly Dictionary<StringName, StringName> states = new();
	readonly Dictionary<StringName, (StringName, Func<string, bool>)[]> stateMachines = new();

	public override void _Ready()
	{
		InjectSpeedControls();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (TreeRoot is not AnimationNodeBlendTree blendTree)
		{
			GD.PushWarning($"TreeRoot is not an AnimationNodeBlendTree");
			return;
		}

		foreach (var entry in stateMachines)
		{
			var node = blendTree.GetNode(entry.Key);
			if (node is not AnimationNodeStateMachine)
			{
				GD.PushWarning($"Node {entry.Key} is not an AnimationNodeStateMachine");
				continue;
			}

			RunStateMachine(states.GetValueOrDefault(entry.Key), entry.Key, entry.Value);
		}
	}

	StringName RunStateMachine(StringName state, string name, (StringName, Func<string, bool>)[] values)
	{
		foreach (var tuple in values)
		{
			if (tuple.Item2(state))
			{
				var playback = Get($"parameters/{name}/playback").As<AnimationNodeStateMachinePlayback>();
				playback.Travel(tuple.Item1);
				return tuple.Item1;
			}
		}
		return state;
	}

	void InjectSpeedControls()
	{
		var blendTree = new AnimationNodeBlendTree();
		blendTree.AddNode("__userContent", TreeRoot);
		blendTree.AddNode("timeScale", new AnimationNodeTimeScale());
		blendTree.ConnectNode("timeScale", 0, "__userContent");
		blendTree.ConnectNode("output", 0, "timeScale");

		TreeRoot = blendTree;
	}

	public void RegisterStateTransitions(params (StringName, Func<string, bool>)[] transitions)
	{
		stateMachines["__userContent"] = transitions;
	}

	public void RegisterStateTransitions(StringName stateMachineName, params (StringName, Func<string, bool>)[] transitions)
	{
		stateMachines[stateMachineName] = transitions;
	}
}
