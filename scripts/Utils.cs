using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public static class ObjectExtensions
{
	public static void Log(this object _, float message)
	{
		Debug.WriteLine(message);
		SignalBus.SendMessage(message.ToString());
	}
	public static void Log(this object _, double message)
	{
		Debug.WriteLine(message);
		SignalBus.SendMessage(message.ToString());
	}
	public static void Log(this object _, string message)
	{
		Debug.WriteLine(message);
		SignalBus.SendMessage(message);
	}
	public static void Log(this object _, object message)
	{
		if (message == null)
		{
			Debug.WriteLine("null");
			SignalBus.SendMessage("null");
		}
		else
		{
			Debug.WriteLine(message.ToString());
			SignalBus.SendMessage(message.ToString());
		}
	}
}

public static class NodeExtensions
{
	public static T GetParent<T>(this Node node) where T : Node
	{
		return node.GetParent<T>() ?? throw new Exception("Parent not found");
	}

	public static T GetParentOrDefault<T>(this Node node) where T : Node
	{
		var parent = node.GetParent();
		if (parent is T t)
			return t;
		if (parent == null)
			return default;
		return parent.GetParentOrDefault<T>();
	}

	public static T GetComponent<T>(this Node parent) where T : Node
	{
		return GetComponentOrDefault<T>(parent, 0) ?? throw new Exception("Component not found");
	}

	public static T GetComponentOrDefault<T>(this Node parent, int depth = 0) where T : Node
	{
		if (parent is BaseUnit unit && unit.Components.GetCachedComponent<T>(out var cachedComponent))
		{
			return cachedComponent;
		}

		var children = parent.GetChildren();

		var component = children.Where(child => child is T).Cast<T>().FirstOrDefault();
		if (component != null)
		{
			if (parent is BaseUnit parentUnit)
				parentUnit.Components.CacheComponent(component);
			return component;
		}

		foreach (var child in children)
		{
			var comp = GetComponentOrDefault<T>(child, depth + 1);
			if (comp != null)
			{
				if (parent is BaseUnit parentUnit)
					parentUnit.Components.CacheComponent(comp);
				return comp;
			}
		}

		return default;
	}

	public static List<T> GetComponentsUncached<T>(this Node parent, int depth = 0) where T : Node
	{
		var children = parent.GetChildren();

		var components = children.Where(child => child is T).Cast<T>();

		foreach (var child in children)
		{
			var extraComponents = GetComponentsUncached<T>(child, depth + 1);
			components = components.Concat(extraComponents);
		}

		return components.ToList();
	}
}

public static class Colors
{
	public const string Value = "#FF9900";
	public const string Active = "#FF9900";
	public const string Passive = "#7777AA";
	public const string Health = "#CC3020";
	public const string Mana = "#0099FF";
	public const string Forbidden = "#BB2525";

	public static string Tag(object text, string color = Value)
	{
		return $"[color={color}]{text}[/color]";
	}

	public static string Lore(object text)
	{
		return Tag(text, "#595959");
	}
}