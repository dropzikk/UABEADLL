using System;
using Avalonia.Utilities;

namespace Avalonia.Animation.Easings;

internal struct CubicBezier
{
	private const int CUBIC_BEZIER_SPLINE_SAMPLES = 11;

	private double ax_;

	private double bx_;

	private double cx_;

	private double ay_;

	private double by_;

	private double cy_;

	private double start_gradient_;

	private double end_gradient_;

	private double range_min_;

	private double range_max_;

	private bool monotonically_increasing_;

	private unsafe fixed double spline_samples_[11];

	private const double kBezierEpsilon = 1E-07;

	private const int kMaxNewtonIterations = 4;

	public readonly double RangeMin => range_min_;

	public readonly double RangeMax => range_max_;

	public CubicBezier(double p1x, double p1y, double p2x, double p2y)
	{
		this = default(CubicBezier);
		InitCoefficients(p1x, p1y, p2x, p2y);
		InitGradients(p1x, p1y, p2x, p2y);
		InitRange(p1y, p2y);
		InitSpline();
	}

	public readonly double SampleCurveX(double t)
	{
		return ((ax_ * t + bx_) * t + cx_) * t;
	}

	private readonly double SampleCurveY(double t)
	{
		return ((ay_ * t + by_) * t + cy_) * t;
	}

	private readonly double SampleCurveDerivativeX(double t)
	{
		return (3.0 * ax_ * t + 2.0 * bx_) * t + cx_;
	}

	private readonly double SampleCurveDerivativeY(double t)
	{
		return (3.0 * ay_ * t + 2.0 * by_) * t + cy_;
	}

	public readonly double SolveWithEpsilon(double x, double epsilon)
	{
		if (x < 0.0)
		{
			return 0.0 + start_gradient_ * x;
		}
		if (x > 1.0)
		{
			return 1.0 + end_gradient_ * (x - 1.0);
		}
		return SampleCurveY(SolveCurveX(x, epsilon));
	}

	private void InitCoefficients(double p1x, double p1y, double p2x, double p2y)
	{
		cx_ = 3.0 * p1x;
		bx_ = 3.0 * (p2x - p1x) - cx_;
		ax_ = 1.0 - cx_ - bx_;
		cy_ = 3.0 * p1y;
		by_ = 3.0 * (p2y - p1y) - cy_;
		ay_ = 1.0 - cy_ - by_;
	}

	private void InitGradients(double p1x, double p1y, double p2x, double p2y)
	{
		if (p1x > 0.0)
		{
			start_gradient_ = p1y / p1x;
		}
		else if (p1y == 0.0 && p2x > 0.0)
		{
			start_gradient_ = p2y / p2x;
		}
		else if (p1y == 0.0 && p2y == 0.0)
		{
			start_gradient_ = 1.0;
		}
		else
		{
			start_gradient_ = 0.0;
		}
		if (p2x < 1.0)
		{
			end_gradient_ = (p2y - 1.0) / (p2x - 1.0);
		}
		else if (p2y == 1.0 && p1x < 1.0)
		{
			end_gradient_ = (p1y - 1.0) / (p1x - 1.0);
		}
		else if (p2y == 1.0 && p1y == 1.0)
		{
			end_gradient_ = 1.0;
		}
		else
		{
			end_gradient_ = 0.0;
		}
	}

	private void InitRange(double p1y, double p2y)
	{
		range_min_ = 0.0;
		range_max_ = 1.0;
		if (0.0 <= p1y && p1y < 1.0 && 0.0 <= p2y && p2y <= 1.0)
		{
			return;
		}
		double num = 1E-07;
		double num2 = 3.0 * ay_;
		double num3 = 2.0 * by_;
		double num4 = cy_;
		if (Math.Abs(num2) < num && Math.Abs(num3) < num)
		{
			return;
		}
		double num5 = 0.0;
		double num6;
		if (Math.Abs(num2) < num)
		{
			num6 = (0.0 - num4) / num3;
		}
		else
		{
			double num7 = num3 * num3 - 4.0 * num2 * num4;
			if (num7 < 0.0)
			{
				return;
			}
			double num8 = Math.Sqrt(num7);
			num6 = (0.0 - num3 + num8) / (2.0 * num2);
			num5 = (0.0 - num3 - num8) / (2.0 * num2);
		}
		double val = 0.0;
		double val2 = 0.0;
		if (0.0 < num6 && num6 < 1.0)
		{
			val = SampleCurveY(num6);
		}
		if (0.0 < num5 && num5 < 1.0)
		{
			val2 = SampleCurveY(num5);
		}
		range_min_ = Math.Min(Math.Min(range_min_, val), val2);
		range_max_ = Math.Max(Math.Max(range_max_, val), val2);
	}

	private unsafe void InitSpline()
	{
		double num = 0.1;
		for (int i = 0; i < 11; i++)
		{
			spline_samples_[i] = SampleCurveX((double)i * num);
		}
	}

	public unsafe readonly double SolveCurveX(double x, double epsilon)
	{
		if (x < 0.0 || x > 1.0)
		{
			throw new ArgumentException();
		}
		double num = 0.0;
		double num2 = 0.0;
		double num3 = x;
		double num4 = 0.0;
		double num5 = 0.1;
		for (int i = 1; i < 11; i++)
		{
			if (x <= spline_samples_[i])
			{
				num2 = num5 * (double)i;
				num = num2 - num5;
				num3 = num + (num2 - num) * (x - spline_samples_[i - 1]) / (spline_samples_[i] - spline_samples_[i - 1]);
				break;
			}
		}
		double num6 = Math.Min(1E-07, epsilon);
		for (int i = 0; i < 4; i++)
		{
			num4 = SampleCurveX(num3) - x;
			if (Math.Abs(num4) < num6)
			{
				return num3;
			}
			double num7 = SampleCurveDerivativeX(num3);
			if (Math.Abs(num7) < 1E-07)
			{
				break;
			}
			num3 -= num4 / num7;
		}
		if (Math.Abs(num4) < epsilon)
		{
			return num3;
		}
		while (num < num2)
		{
			num4 = SampleCurveX(num3);
			if (Math.Abs(num4 - x) < epsilon)
			{
				return num3;
			}
			if (x > num4)
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
			num3 = (num2 + num) * 0.5;
		}
		return num3;
	}

	public readonly double Solve(double x)
	{
		return SolveWithEpsilon(x, 1E-07);
	}

	public readonly double SlopeWithEpsilon(double x, double epsilon)
	{
		x = MathUtilities.Clamp(x, 0.0, 1.0);
		double t = SolveCurveX(x, epsilon);
		double num = SampleCurveDerivativeX(t);
		return SampleCurveDerivativeY(t) / num;
	}

	public readonly double Slope(double x)
	{
		return SlopeWithEpsilon(x, 1E-07);
	}
}
