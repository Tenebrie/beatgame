using System.Diagnostics;

namespace Project;

public static class ObjectExtensions
{
	public static void Log(this object _, float message)
	{
		Debug.WriteLine(message);
	}
	public static void Log(this object _, double message)
	{
		Debug.WriteLine(message);
	}
	public static void Log(this object _, string message)
	{
		Debug.WriteLine(message);
	}
	public static void Log(this object _, object message)
	{
		if (message == null)
			Debug.WriteLine("null");
		else
			Debug.WriteLine(message.ToString());
	}
}

public static class Colors
{
	public const string Value = "#FF9900";
	public const string Active = "#FF9900";
	public const string Passive = "#7777AA";
	public const string Health = "#CCCC50";
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