using System;

namespace Avalonia.Input.GestureRecognizers;

internal class VelocityTracker
{
	private const int AssumePointerMoveStoppedMilliseconds = 40;

	private const int HistorySize = 20;

	private const int HorizonMilliseconds = 100;

	private const int MinSampleSize = 3;

	private const double MinFlingVelocity = 50.0;

	private const double MaxFlingVelocity = 8000.0;

	private readonly PointAtTime[] _samples = new PointAtTime[20];

	private int _index;

	public void AddPosition(TimeSpan time, Vector position)
	{
		_index++;
		if (_index == 20)
		{
			_index = 0;
		}
		_samples[_index] = new PointAtTime(Valid: true, position, time);
	}

	protected virtual VelocityEstimate? GetVelocityEstimate()
	{
		Span<double> span = stackalloc double[20];
		Span<double> span2 = stackalloc double[20];
		Span<double> span3 = stackalloc double[20];
		Span<double> span4 = stackalloc double[20];
		int num = 0;
		int num2 = _index;
		PointAtTime pointAtTime = _samples[num2];
		if (!pointAtTime.Valid)
		{
			return null;
		}
		PointAtTime pointAtTime2 = pointAtTime;
		PointAtTime pointAtTime3 = pointAtTime;
		do
		{
			PointAtTime pointAtTime4 = _samples[num2];
			if (!pointAtTime4.Valid)
			{
				break;
			}
			double totalMilliseconds = (pointAtTime.Time - pointAtTime4.Time).TotalMilliseconds;
			double num3 = Math.Abs((pointAtTime4.Time - pointAtTime2.Time).TotalMilliseconds);
			pointAtTime2 = pointAtTime4;
			if (totalMilliseconds > 100.0 || num3 > 40.0)
			{
				break;
			}
			pointAtTime3 = pointAtTime4;
			Vector point = pointAtTime4.Point;
			span[num] = point.X;
			span2[num] = point.Y;
			span3[num] = 1.0;
			span4[num] = 0.0 - totalMilliseconds;
			num2 = ((num2 == 0) ? 20 : num2) - 1;
			num++;
		}
		while (num < 20);
		if (num >= 3)
		{
			PolynomialFit polynomialFit = LeastSquaresSolver.Solve(2, span4.Slice(0, num), span.Slice(0, num), span3.Slice(0, num));
			if (polynomialFit != null)
			{
				PolynomialFit polynomialFit2 = LeastSquaresSolver.Solve(2, span4.Slice(0, num), span2.Slice(0, num), span3.Slice(0, num));
				if (polynomialFit2 != null)
				{
					return new VelocityEstimate(new Vector(polynomialFit.Coefficients[1] * 1000.0, polynomialFit2.Coefficients[1] * 1000.0), polynomialFit.Confidence * polynomialFit2.Confidence, pointAtTime.Time - pointAtTime3.Time, pointAtTime.Point - pointAtTime3.Point);
				}
			}
		}
		return new VelocityEstimate(Vector.Zero, 1.0, pointAtTime.Time - pointAtTime3.Time, pointAtTime.Point - pointAtTime3.Point);
	}

	internal Velocity GetVelocity()
	{
		VelocityEstimate velocityEstimate = GetVelocityEstimate();
		if (velocityEstimate == null || velocityEstimate.PixelsPerSecond == default(Vector))
		{
			return new Velocity(Vector.Zero);
		}
		return new Velocity(velocityEstimate.PixelsPerSecond);
	}

	internal virtual Velocity GetFlingVelocity()
	{
		return GetVelocity().ClampMagnitude(50.0, 8000.0);
	}
}
