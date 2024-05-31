using System;
using System.Diagnostics;

public static class ObjectExtensions
{
	public static void Log(this Object _, float message)
	{
		Debug.WriteLine(message);
	}
	public static void Log(this Object _, double message)
	{
		Debug.WriteLine(message);
	}
	public static void Log(this Object _, string message)
	{
		Debug.WriteLine(message);
	}
	public static void Log(this Object _, object message)
	{
		if (message == null)
			Debug.WriteLine("null");
		else
			Debug.WriteLine(message.ToString());
	}
}