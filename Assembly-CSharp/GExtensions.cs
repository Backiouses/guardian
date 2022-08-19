using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public static class GExtensions
{
	private static readonly Regex HexColorPattern = new Regex("\\[([a-f0-9]{6}|-)\\]", RegexOptions.IgnoreCase);

	private static readonly Regex ColorTagPattern = new Regex("<\\/?color(=#?\\w+)?>", RegexOptions.IgnoreCase);

	public static T[] CopyOfRange<T>(this T[] arrIn, int startIndex, int endIndex)
	{
		while (endIndex >= arrIn.Length)
		{
			endIndex--;
		}
		int num = endIndex - startIndex + 1;
		T[] array = new T[num];
		Array.Copy(arrIn, startIndex, array, 0, num);
		return array;
	}

	public static T[] Sorted<T>(this T[] arrIn, Comparison<T> comparator)
	{
		T[] array = new T[arrIn.Length];
		arrIn.CopyTo(array, 0);
		Array.Sort(array, comparator);
		return array;
	}

	public static string NGUIToUnity(this string str)
	{
		string text = string.Empty;
		Stack<string> stack = new Stack<string>();
		bool flag = false;
		for (int i = 0; i < str.Length; i++)
		{
			char c = str[i];
			if (c.Equals('[') && i + 2 < str.Length)
			{
				if (str[i + 1].Equals('-') && str[i + 2].Equals(']'))
				{
					if (stack.Count > 0)
					{
						stack.Pop();
					}
					if (stack.Count < 1)
					{
						stack.Push("FFFFFF");
					}
					text += (flag ? ("</color><color=#" + stack.Peek() + ">") : ("<color=#" + stack.Peek() + ">"));
					flag = true;
					i += 2;
					continue;
				}
				if (i + 7 < str.Length && str[i + 7].Equals(']') && str.Substring(i + 1, 6).IsHex())
				{
					string text2 = str.Substring(i + 1, 6).ToUpper();
					stack.Push(text2);
					text += (flag ? ("</color><color=#" + text2 + ">") : ("<color=#" + text2 + ">"));
					flag = true;
					i += 7;
					continue;
				}
			}
			text += c;
		}
		return text + (flag ? "</color>" : string.Empty);
	}

	public static string StripNGUI(this string str)
	{
		return HexColorPattern.Replace(str, string.Empty);
	}

	public static string StripUnityColors(this string str)
	{
		return ColorTagPattern.Replace(str, string.Empty);
	}

	public static bool IsHex(this string str)
	{
		int result;
		if (str.Length == 6 || str.Length == 8)
		{
			return int.TryParse(str, NumberStyles.AllowHexSpecifier, null, out result);
		}
		return false;
	}

	public static string ToHex(this Color color)
	{
		int num = Mathf.RoundToInt(color.r * 255f);
		int num2 = Mathf.RoundToInt(color.g * 255f);
		int num3 = Mathf.RoundToInt(color.b * 255f);
		return string.Concat(str3: Mathf.RoundToInt(color.a * 255f).ToString("X2"), str0: num.ToString("X2"), str1: num2.ToString("X2"), str2: num3.ToString("X2"));
	}

	public static Color ToColor(this string str)
	{
		float r = 0f;
		float g = 0f;
		float b = 0f;
		float a = 1f;
		if (int.TryParse(str.Substr(0, 1), NumberStyles.AllowHexSpecifier, null, out var result))
		{
			r = (float)result / 255f;
		}
		if (int.TryParse(str.Substr(2, 3), NumberStyles.AllowHexSpecifier, null, out var result2))
		{
			g = (float)result2 / 255f;
		}
		if (int.TryParse(str.Substr(4, 5), NumberStyles.AllowHexSpecifier, null, out var result3))
		{
			b = (float)result3 / 255f;
		}
		if (str.Length == 8 && int.TryParse(str.Substr(6, 7), NumberStyles.AllowHexSpecifier, null, out var result4))
		{
			a = (float)result4 / 255f;
		}
		return new Color(r, g, b, a);
	}

	public static string AsBold(this string str)
	{
		return "<b>" + str + "</b>";
	}

	public static string AsItalic(this string str)
	{
		return "<i>" + str + "</i>";
	}

	public static string AsColor(this string str, string hex)
	{
		if (hex.IsHex())
		{
			return "<color=#" + hex + ">" + str + "</color>";
		}
		return "<color=" + hex + ">" + str + "</color>";
	}

	public static string AsString(object obj)
	{
		if (obj != null && obj is string)
		{
			return (string)obj;
		}
		return string.Empty;
	}

	public static int AsInt(object obj)
	{
		if (obj != null && obj is int)
		{
			return (int)obj;
		}
		return 0;
	}

	public static float AsFloat(object obj)
	{
		if (obj != null && obj is float)
		{
			return (float)obj;
		}
		return 0f;
	}

	public static bool AsBool(object obj)
	{
		if (obj != null && obj is bool)
		{
			return (bool)obj;
		}
		return false;
	}

	public static bool TryParseEnum<T>(string input, out T value) where T : Enum
	{
		value = default(T);
		try
		{
			Type typeFromHandle = typeof(T);
			value = (T)Enum.Parse(typeFromHandle, input, ignoreCase: true);
			if (Enum.IsDefined(typeFromHandle, value))
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	public static string Substr(this string str, int startIndex, int endIndex)
	{
		while (endIndex >= str.Length)
		{
			endIndex--;
		}
		int length = endIndex - startIndex + 1;
		return str.Substring(startIndex, length);
	}

	public static bool IsKeyDown(this KeyCode keyCode)
	{
		if (Event.current != null && Event.current.type == EventType.KeyDown)
		{
			return Event.current.keyCode == keyCode;
		}
		return false;
	}

	public static bool IsKeyUp(this KeyCode keyCode)
	{
		if (Event.current != null && Event.current.type == EventType.KeyUp)
		{
			return Event.current.keyCode == keyCode;
		}
		return false;
	}

	public static Vector2 GetScaleVector(this Texture image, int originalWidth, int originalHeight)
	{
		return new Vector2((float)image.width / (float)originalWidth, (float)image.height / (float)originalHeight);
	}
}
