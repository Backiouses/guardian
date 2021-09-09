﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class GExtensions
{
    private static readonly Regex s_colorPattern = new Regex("\\[([a-fA-F0-9]{6}|-)\\]", RegexOptions.IgnoreCase);

    public static T[] CopyOfRange<T>(this T[] arrIn, int startIndex, int endIndex)
    {
        // Decrement endIndex until it is arrIn.Length - 1
        while (endIndex >= arrIn.Length)
        {
            endIndex--;
        }

        int len = endIndex - startIndex + 1;
        T[] arrOut = new T[len];

        Array.Copy(arrIn, startIndex, arrOut, 0, len);

        return arrOut;
    }

    public static T[] Sorted<T>(this T[] arrIn, Comparison<T> comparator)
    {
        T[] sorted = new T[arrIn.Length];
        arrIn.CopyTo(sorted, 0);
        Array.Sort(sorted, comparator);

        return sorted;
    }

    // My implementation of NGUI color parsing
    public static string Colored(this string str)
    {
        string output = string.Empty;
        Stack<string> colors = new Stack<string>(); // Thank you to Kevin for telling me to use a Stack
        bool coloring = false;

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (c == '[' && i + 2 < str.Length)
            {
                if (str[i + 1] == '-' && str[i + 2] == ']') // [-], aka return to previous color in the stack
                {
                    string previous = "FFFFFF"; // Default to white

                    if (colors.Count > 0)
                    {
                        colors.Pop();

                        // Is there any color left to use?
                        if (colors.Count > 0)
                        {
                            previous = colors.Peek();
                        }
                    }

                    output += coloring ? $"</color><color=#{previous}>" : $"<color=#{previous}>";
                    coloring = true;
                    i += 2;
                    continue;
                }
                else if (i + 7 < str.Length && str[i + 7] == ']' && str.Substring(i + 1, 6).IsHex()) // [RRGGBB], aka use the color supplied by RRGGBB
                {
                    string color = str.Substring(i + 1, 6).ToUpper();
                    colors.Push(color);
                    output += coloring ? $"</color><color=#{color}>" : $"<color=#{color}>";
                    coloring = true;
                    i += 7;
                    continue;
                }
            }

            output += c;
        }

        return output + (coloring ? "</color>" : string.Empty);
    }

    public static string Uncolored(this string str)
    {
        return s_colorPattern.Replace(str, string.Empty);
    }

    public static bool IsHex(this string str)
    {
        return (str.Length == 6 || str.Length == 8) && int.TryParse(str, System.Globalization.NumberStyles.AllowHexSpecifier, null, out int result);
    }

    public static string ToHex(this Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        int a = Mathf.RoundToInt(color.a * 255f);

        return r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + a.ToString("X2");
    }

    public static Color ToColor(this string str)
    {
        float r = 0;
        float g = 0;
        float b = 0;
        float a = 1f;

        // Red
        if (int.TryParse(str.Substr(0, 1), System.Globalization.NumberStyles.AllowHexSpecifier, null, out int ri))
        {
            r = ri / 255F;
        }

        // Green
        if (int.TryParse(str.Substr(2, 3), System.Globalization.NumberStyles.AllowHexSpecifier, null, out int gi))
        {
            g = gi / 255F;
        }

        // Blue
        if (int.TryParse(str.Substr(4, 5), System.Globalization.NumberStyles.AllowHexSpecifier, null, out int bi))
        {
            b = bi / 255F;
        }

        // Alpha
        if (str.Length == 8 && int.TryParse(str.Substr(6, 7), System.Globalization.NumberStyles.AllowHexSpecifier, null, out int ai))
        {
            a = ai / 255F;
        }

        return new Color(r, g, b, a);
    }

    public static Color ToColor(this int color)
    {
        int r = color >> 24 & 0xFF;
        int g = color >> 16 & 0xFF;
        int b = color >> 8 & 0xFF;
        int a = color & 0xFF;

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color ToColor(this uint color)
    {
        int r = unchecked((int)color) >> 24 & 0xFF;
        int g = unchecked((int)color) >> 16 & 0xFF;
        int b = unchecked((int)color) >> 8 & 0xFF;
        int a = unchecked((int)color) & 0xFF;

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static string AsBold(this string str)
    {
        return $"<b>{str}</b>";
    }

    public static string AsItalic(this string str)
    {
        return $"<i>{str}</i>";
    }

    public static string WithColor(this string str, string hex)
    {
        if (hex.IsHex())
        {
            return $"<color=#{hex}>{str}</color>";
        }

        return $"<color={hex}>{str}</color>";
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
        value = default;
        try
        {
            Type enumType = typeof(T);
            value = (T)Enum.Parse(enumType, input, true);

            if (Enum.IsDefined(enumType, value))
            {
                return true;
            }
        }
        catch { }

        return false;
    }

    public static string Substr(this string str, int startIndex, int endIndex)
    {
        // Decrement endIndex until it is str.Length - 1
        while (endIndex >= str.Length)
        {
            endIndex--;
        }

        int len = endIndex - startIndex + 1;

        return str.Substring(startIndex, len);
    }

    public static bool WasPressedInGUI(this KeyCode keyCode)
    {
        if (Event.current != null && Event.current.type == EventType.KeyUp)
        {
            return Event.current.keyCode == keyCode;
        }

        return false;
    }
}