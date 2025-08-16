using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.X11;

internal class X11ScreensUserSettings
{
	public double GlobalScaleFactor { get; set; } = 1.0;

	public Dictionary<string, double> NamedScaleFactors { get; set; }

	private static double? TryParse(string s)
	{
		if (s == null)
		{
			return null;
		}
		if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return null;
	}

	public static X11ScreensUserSettings DetectEnvironment()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("AVALONIA_GLOBAL_SCALE_FACTOR");
		string environmentVariable2 = Environment.GetEnvironmentVariable("AVALONIA_SCREEN_SCALE_FACTORS");
		if (environmentVariable == null && environmentVariable2 == null)
		{
			return null;
		}
		X11ScreensUserSettings x11ScreensUserSettings = new X11ScreensUserSettings
		{
			GlobalScaleFactor = (TryParse(environmentVariable) ?? 1.0)
		};
		try
		{
			if (!string.IsNullOrWhiteSpace(environmentVariable2))
			{
				x11ScreensUserSettings.NamedScaleFactors = (from x in environmentVariable2.Split(';')
					where !string.IsNullOrWhiteSpace(x)
					select x.Split('=')).ToDictionary((string[] x) => x[0], (string[] x) => double.Parse(x[1], CultureInfo.InvariantCulture));
			}
		}
		catch
		{
		}
		return x11ScreensUserSettings;
	}

	public static X11ScreensUserSettings Detect()
	{
		return DetectEnvironment() ?? new X11ScreensUserSettings();
	}
}
