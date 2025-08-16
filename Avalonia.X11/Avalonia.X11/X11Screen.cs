using System;
using System.Linq;

namespace Avalonia.X11;

internal class X11Screen
{
	private const int FullHDWidth = 1920;

	private const int FullHDHeight = 1080;

	public bool IsPrimary { get; }

	public string Name { get; set; }

	public PixelRect Bounds { get; set; }

	public Size? PhysicalSize { get; set; }

	public double Scaling { get; set; }

	public PixelRect WorkingArea { get; set; }

	public X11Screen(PixelRect bounds, bool isPrimary, string name, Size? physicalSize, double? scaling)
	{
		IsPrimary = isPrimary;
		Name = name;
		Bounds = bounds;
		if (!physicalSize.HasValue && !scaling.HasValue)
		{
			Scaling = 1.0;
			return;
		}
		if (!scaling.HasValue)
		{
			Scaling = GuessPixelDensity(bounds, physicalSize.Value);
			return;
		}
		Scaling = scaling.Value;
		PhysicalSize = physicalSize;
	}

	public static double GuessPixelDensity(PixelRect pixel, Size physical)
	{
		double num = 1.0;
		if (physical.Width > 0.0)
		{
			num = ((pixel.Width <= 1920) ? 1.0 : Math.Max(1.0, (double)pixel.Width / physical.Width * 25.4 / 96.0));
		}
		else if (physical.Height > 0.0)
		{
			num = ((pixel.Height <= 1080) ? 1.0 : Math.Max(1.0, (double)pixel.Height / physical.Height * 25.4 / 96.0));
		}
		if (num > 3.0)
		{
			return 1.0;
		}
		double[] array = new double[5] { 1.0, 1.25, 1.5, 1.75, 2.0 };
		double[] array2 = array;
		foreach (double num2 in array2)
		{
			if (num <= num2 + 0.2)
			{
				return num2;
			}
		}
		return array.Last();
	}
}
