using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{

    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
    public static Color HexToColorWithOpacity(string str)
    {
        float r, g, b, a = 1.0f;

        r = (float)byte.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
        g = (float)byte.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
        b = (float)byte.Parse(str.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;

        if (str.Length == 8)
        {
            a = (float)byte.Parse(str.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
        }

        var ret = new Color(r, g, b, a);
        return ret;
    }

    public static Color HexToColorWithOpacity(string hex, float opacityPercentage)
    {
        if (hex.StartsWith("#"))
        {
            hex = hex.Substring(1);
        }

        var r = (float)byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
        var g = (float)byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
        var b = (float)byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;

        // 불투명도 퍼센트를 0-255 범위의 값으로 변환
        var a = opacityPercentage / 100f;

        var ret = new Color(r, g, b, a);
        return ret;
    } 

    public static Vector2 Add(this Vector2 origin, float angle, float distance)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance + origin;
    }

    public static void AddMany<T>(this List<T> list, params T[] values)
    {
        list.AddRange(values);
    }

    public static float ToDeg(this float angle)
    {
        return angle * 57.29578f;
    }

    public static int WithinArray(this int value, ICollection array)
    {
        return Mathf.Clamp(value, 0, array.Count - 1);
    }

    public static int WithinArray(this int value, Array array)
    {
        return Mathf.Clamp(value, 0, array.Length - 1);
    }

    public static int Conn(this Vector3 v)
    {
        return (int)v.z;
    }

    public static void Add<T>(this List<T> list, params T[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            list.Add(items[i]);
        }
    }

}
