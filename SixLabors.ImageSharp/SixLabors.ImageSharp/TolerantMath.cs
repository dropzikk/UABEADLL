using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

internal readonly struct TolerantMath
{
	private readonly double epsilon;

	private readonly double negEpsilon;

	public static readonly TolerantMath Default = new TolerantMath(1E-08);

	public TolerantMath(double epsilon)
	{
		this.epsilon = epsilon;
		negEpsilon = 0.0 - epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsZero(double a)
	{
		if (a > negEpsilon)
		{
			return a < epsilon;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsPositive(double a)
	{
		return a > epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsNegative(double a)
	{
		return a < negEpsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool AreEqual(double a, double b)
	{
		return IsZero(a - b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsGreater(double a, double b)
	{
		return a > b + epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsLess(double a, double b)
	{
		return a < b - epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsGreaterOrEqual(double a, double b)
	{
		return a >= b - epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsLessOrEqual(double a, double b)
	{
		return b >= a - epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Ceiling(double a)
	{
		double a2 = Math.IEEERemainder(a, 1.0);
		if (IsZero(a2))
		{
			return Math.Round(a);
		}
		return Math.Ceiling(a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Floor(double a)
	{
		double a2 = Math.IEEERemainder(a, 1.0);
		if (IsZero(a2))
		{
			return Math.Round(a);
		}
		return Math.Floor(a);
	}
}
