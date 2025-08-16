using System;
using System.Runtime.CompilerServices;

namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorRGB9e5 : IColor<double>
{
	private uint bits;

	public double R
	{
		get
		{
			return (double)RBits * Scale;
		}
		set
		{
			GetChannels(out var _, out var g, out var b, out var _);
			SetChannels(value, g, b, 0.0);
		}
	}

	public double G
	{
		get
		{
			return (double)GBits * Scale;
		}
		set
		{
			GetChannels(out var r, out var _, out var b, out var _);
			SetChannels(r, value, b, 0.0);
		}
	}

	public double B
	{
		get
		{
			return (double)BBits * Scale;
		}
		set
		{
			GetChannels(out var r, out var g, out var _, out var _);
			SetChannels(r, g, value, 0.0);
		}
	}

	public double A
	{
		get
		{
			return 1.0;
		}
		set
		{
		}
	}

	private int Exponent => (int)((bits >> 27) - 24);

	private double Scale => Math.Pow(2.0, Exponent);

	private uint RBits => bits & 0x1FF;

	private uint GBits => (bits >> 9) & 0x1FF;

	private uint BBits => (bits >> 18) & 0x1FF;

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void GetChannels(out double r, out double g, out double b, out double a)
	{
		double scale = Scale;
		r = (double)RBits * scale;
		g = (double)GBits * scale;
		b = (double)BBits * scale;
		a = 1.0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public void SetChannels(double r, double g, double b, double a)
	{
		int num = CalculateExponent(r, g, b);
		double num2 = Math.Pow(2.0, num);
		uint num3 = (uint)(r / num2) & 0x1FF;
		uint num4 = (uint)(g / num2) & 0x1FF;
		uint num5 = (uint)(b / num2) & 0x1FF;
		uint num6 = (uint)(num + 24);
		bits = (num6 << 27) | (num5 << 18) | (num4 << 9) | num3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static int CalculateExponent(double r, double g, double b)
	{
		return (int)Math.Ceiling(Math.Log2(Math.Max(r, Math.Max(g, b)) / 511.0));
	}
}
