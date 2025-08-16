namespace Avalonia.Animation.Easings;

public class SplineEasing : Easing
{
	private readonly KeySpline _internalKeySpline;

	public double X1
	{
		get
		{
			return _internalKeySpline.ControlPointX1;
		}
		set
		{
			_internalKeySpline.ControlPointX1 = value;
		}
	}

	public double Y1
	{
		get
		{
			return _internalKeySpline.ControlPointY1;
		}
		set
		{
			_internalKeySpline.ControlPointY1 = value;
		}
	}

	public double X2
	{
		get
		{
			return _internalKeySpline.ControlPointX2;
		}
		set
		{
			_internalKeySpline.ControlPointX2 = value;
		}
	}

	public double Y2
	{
		get
		{
			return _internalKeySpline.ControlPointY2;
		}
		set
		{
			_internalKeySpline.ControlPointY2 = value;
		}
	}

	public SplineEasing(double x1 = 0.0, double y1 = 0.0, double x2 = 1.0, double y2 = 1.0)
	{
		_internalKeySpline = new KeySpline();
		X1 = x1;
		Y1 = y1;
		X2 = x2;
		Y1 = y2;
	}

	public SplineEasing(KeySpline keySpline)
	{
		_internalKeySpline = keySpline;
	}

	public SplineEasing()
	{
		_internalKeySpline = new KeySpline();
	}

	public override double Ease(double progress)
	{
		return _internalKeySpline.GetSplineProgress(progress);
	}
}
