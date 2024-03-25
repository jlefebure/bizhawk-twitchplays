using System;
using System.Collections.Generic;
using System.Reflection;

namespace TwitchPlays;

public class BtnConst
{
	public static string Start = "Start";
	public static string Select = "Select";

	public static string Up = "Up";
	public static string Down = "Down";
	public static string Left = "Left";
	public static string Right = "Right";

	public static string A = "A";
	public static string B = "B";
	public static string Y = "Y";
	public static string X = "X";

	public static string L = "L";
	public static string R = "R";
	public static string ZL = "ZL";
	public static string ZR = "ZR";

	public static readonly Dictionary<string, bool> UnpressKeys = GetUnpressKeys();

	private static Dictionary<string, bool> GetUnpressKeys()
	{
		Dictionary<string, bool> output = new();
		Type type = typeof(BtnConst);
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
		
		foreach (var propertyInfo in properties)
		{
			output.Add(propertyInfo.GetValue(null).ToString(), false);
		}

		return output;
	}
}