using System;

namespace Avalonia.Utilities;

public static class MathUtilities
{
	internal const double DoubleEpsilon = 2.220446049250313E-16;

	private const float FloatEpsilon = 1.1920929E-07f;

	public static bool AreClose(double value1, double value2)
	{
		if (value1 == value2)
		{
			return true;
		}
		double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.220446049250313E-16;
		double num2 = value1 - value2;
		if (0.0 - num < num2)
		{
			return num > num2;
		}
		return false;
	}

	public static bool AreClose(double value1, double value2, double eps)
	{
		if (value1 == value2)
		{
			return true;
		}
		double num = value1 - value2;
		if (0.0 - eps < num)
		{
			return eps > num;
		}
		return false;
	}

	public static bool AreClose(float value1, float value2)
	{
		if (value1 == value2)
		{
			return true;
		}
		float num = (Math.Abs(value1) + Math.Abs(value2) + 10f) * 1.1920929E-07f;
		float num2 = value1 - value2;
		if (0f - num < num2)
		{
			return num > num2;
		}
		return false;
	}

	public static bool LessThan(double value1, double value2)
	{
		if (value1 < value2)
		{
			return !AreClose(value1, value2);
		}
		return false;
	}

	public static bool LessThan(float value1, float value2)
	{
		if (value1 < value2)
		{
			return !AreClose(value1, value2);
		}
		return false;
	}

	public static bool GreaterThan(double value1, double value2)
	{
		if (value1 > value2)
		{
			return !AreClose(value1, value2);
		}
		return false;
	}

	public static bool GreaterThan(float value1, float value2)
	{
		if (value1 > value2)
		{
			return !AreClose(value1, value2);
		}
		return false;
	}

	public static bool LessThanOrClose(double value1, double value2)
	{
		if (!(value1 < value2))
		{
			return AreClose(value1, value2);
		}
		return true;
	}

	public static bool LessThanOrClose(float value1, float value2)
	{
		if (!(value1 < value2))
		{
			return AreClose(value1, value2);
		}
		return true;
	}

	public static bool GreaterThanOrClose(double value1, double value2)
	{
		if (!(value1 > value2))
		{
			return AreClose(value1, value2);
		}
		return true;
	}

	public static bool GreaterThanOrClose(float value1, float value2)
	{
		if (!(value1 > value2))
		{
			return AreClose(value1, value2);
		}
		return true;
	}

	public static bool IsOne(double value)
	{
		return Math.Abs(value - 1.0) < 2.220446049250313E-15;
	}

	public static bool IsOne(float value)
	{
		return Math.Abs(value - 1f) < 1.1920929E-06f;
	}

	public static bool IsZero(double value)
	{
		return Math.Abs(value) < 2.220446049250313E-15;
	}

	public static bool IsZero(float value)
	{
		return Math.Abs(value) < 1.1920929E-06f;
	}

	public static double Clamp(double val, double min, double max)
	{
		if (min > max)
		{
			ThrowCannotBeGreaterThanException(min, max);
		}
		if (val < min)
		{
			return min;
		}
		if (val > max)
		{
			return max;
		}
		return val;
	}

	public static decimal Clamp(decimal val, decimal min, decimal max)
	{
		if (min > max)
		{
			ThrowCannotBeGreaterThanException(min, max);
		}
		if (val < min)
		{
			return min;
		}
		if (val > max)
		{
			return max;
		}
		return val;
	}

	public static float Clamp(float value, float min, float max)
	{
		float val = Math.Max(min, max);
		float val2 = Math.Min(min, max);
		return Math.Min(Math.Max(value, val2), val);
	}

	public static int Clamp(int val, int min, int max)
	{
		if (min > max)
		{
			ThrowCannotBeGreaterThanException(min, max);
		}
		if (val < min)
		{
			return min;
		}
		if (val > max)
		{
			return max;
		}
		return val;
	}

	public static double Deg2Rad(double angle)
	{
		return angle * (Math.PI / 180.0);
	}

	public static double Grad2Rad(double angle)
	{
		return angle * (Math.PI / 200.0);
	}

	public static double Turn2Rad(double angle)
	{
		return angle * 2.0 * Math.PI;
	}

	public static Point GetEllipsePoint(Point centre, double radiusX, double radiusY, double angle)
	{
		return new Point(radiusX * Math.Cos(angle) + centre.X, radiusY * Math.Sin(angle) + centre.Y);
	}

	public static (double min, double max) GetMinMax(double a, double b)
	{
		if (!(a < b))
		{
			return (min: b, max: a);
		}
		return (min: a, max: b);
	}

	public static (double min, double max) GetMinMaxFromDelta(double initialValue, double delta)
	{
		return GetMinMax(initialValue, initialValue + delta);
	}

	private static void ThrowCannotBeGreaterThanException<T>(T min, T max)
	{
		throw new ArgumentException($"{min} cannot be greater than {max}.");
	}
}
