using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

public static class ColorHelper
{
	private static readonly Dictionary<HsvColor, string> _cachedDisplayNames = new Dictionary<HsvColor, string>();

	private static readonly Dictionary<KnownColor, string> _cachedKnownColorNames = new Dictionary<KnownColor, string>();

	private static readonly object _displayNameCacheMutex = new object();

	private static readonly object _knownColorCacheMutex = new object();

	private static readonly KnownColor[] _knownColors = (KnownColor[])Enum.GetValues(typeof(KnownColor));

	public static bool ToDisplayNameExists => CultureInfo.CurrentUICulture.Name.StartsWith("EN", StringComparison.OrdinalIgnoreCase);

	public static double GetRelativeLuminance(Color color)
	{
		double num = ((color.R <= 10) ? ((double)(int)color.R / 3294.0) : Math.Pow((double)(int)color.R / 269.0 + 0.0513, 2.4));
		double num2 = ((color.G <= 10) ? ((double)(int)color.G / 3294.0) : Math.Pow((double)(int)color.G / 269.0 + 0.0513, 2.4));
		double num3 = ((color.B <= 10) ? ((double)(int)color.B / 3294.0) : Math.Pow((double)(int)color.B / 269.0 + 0.0513, 2.4));
		return 0.2126 * num + 0.7152 * num2 + 0.0722 * num3;
	}

	public static string ToDisplayName(Color color)
	{
		HsvColor hsvColor = color.ToHsv();
		if (color.A == 0)
		{
			return GetDisplayName(KnownColor.Transparent);
		}
		HsvColor key = new HsvColor(1.0, Math.Round(hsvColor.H, 0), Math.Round(hsvColor.S, 1), Math.Round(hsvColor.V, 1));
		lock (_displayNameCacheMutex)
		{
			if (_cachedDisplayNames.TryGetValue(key, out string value))
			{
				return value;
			}
		}
		lock (_knownColorCacheMutex)
		{
			if (_cachedKnownColorNames.Count == 0)
			{
				for (int i = 1; i < _knownColors.Length; i++)
				{
					KnownColor knownColor = _knownColors[i];
					if (!_cachedKnownColorNames.ContainsKey(knownColor))
					{
						_cachedKnownColorNames.Add(knownColor, GetDisplayName(knownColor));
					}
				}
			}
		}
		KnownColor knownColor2 = KnownColor.None;
		double num = double.PositiveInfinity;
		for (int j = 1; j < _knownColors.Length; j++)
		{
			KnownColor knownColor3 = _knownColors[j];
			if (knownColor3 != KnownColor.Transparent)
			{
				HsvColor hsvColor2 = knownColor3.ToColor().ToHsv();
				double num2 = Math.Sqrt(Math.Pow(key.H - hsvColor2.H, 2.0) + Math.Pow(key.S - hsvColor2.S, 2.0) + Math.Pow(key.V - hsvColor2.V, 2.0));
				if (num2 < num)
				{
					knownColor2 = knownColor3;
					num = num2;
				}
			}
		}
		if (knownColor2 != 0)
		{
			string value2;
			lock (_knownColorCacheMutex)
			{
				if (!_cachedKnownColorNames.TryGetValue(knownColor2, out value2))
				{
					value2 = GetDisplayName(knownColor2);
				}
			}
			lock (_displayNameCacheMutex)
			{
				_cachedDisplayNames.Add(key, value2);
				return value2;
			}
		}
		return string.Empty;
	}

	private static string GetDisplayName(KnownColor knownColor)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		string text = knownColor.ToString();
		for (int i = 0; i < text.Length; i++)
		{
			if (i != 0 && char.IsUpper(text[i]))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(text[i]);
		}
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}
}
