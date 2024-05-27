using System;
using System.Diagnostics;

public static class ObjectExtensions
{
	public static void Log(this Object _, string message)
	{
		Debug.WriteLine(message);
	}
}