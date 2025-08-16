using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia.Animation;

[TypeConverter(typeof(KeySplineTypeConverter))]
public sealed class KeySpline : AvaloniaObject
{
	private double _controlPointX1;

	private double _controlPointY1;

	private double _controlPointX2;

	private double _controlPointY2;

	private bool _isSpecified;

	private bool _isDirty;

	private double _parameter;

	private double _Bx;

	private double _Cx;

	private double _Cx_Bx;

	private double _three_Cx;

	private double _By;

	private double _Cy;

	private const double _accuracy = 0.001;

	private const double _fuzz = 1E-06;

	public double ControlPointX1
	{
		get
		{
			return _controlPointX1;
		}
		set
		{
			if (IsValidXValue(value))
			{
				_controlPointX1 = value;
				_isDirty = true;
				return;
			}
			throw new ArgumentException("Invalid KeySpline X1 value. Must be >= 0.0 and <= 1.0.");
		}
	}

	public double ControlPointY1
	{
		get
		{
			return _controlPointY1;
		}
		set
		{
			_controlPointY1 = value;
			_isDirty = true;
		}
	}

	public double ControlPointX2
	{
		get
		{
			return _controlPointX2;
		}
		set
		{
			if (IsValidXValue(value))
			{
				_controlPointX2 = value;
				_isDirty = true;
				return;
			}
			throw new ArgumentException("Invalid KeySpline X2 value. Must be >= 0.0 and <= 1.0.");
		}
	}

	public double ControlPointY2
	{
		get
		{
			return _controlPointY2;
		}
		set
		{
			_controlPointY2 = value;
			_isDirty = true;
		}
	}

	public KeySpline()
	{
		_controlPointX1 = 0.0;
		_controlPointY1 = 0.0;
		_controlPointX2 = 1.0;
		_controlPointY2 = 1.0;
		_isDirty = true;
	}

	public KeySpline(double x1, double y1, double x2, double y2)
	{
		_controlPointX1 = x1;
		_controlPointY1 = y1;
		_controlPointX2 = x2;
		_controlPointY2 = y2;
		_isDirty = true;
	}

	public static KeySpline Parse(string value, CultureInfo? culture)
	{
		if (culture == null)
		{
			culture = CultureInfo.InvariantCulture;
		}
		using StringTokenizer stringTokenizer = new StringTokenizer(value, culture, "Invalid KeySpline string: \"" + value + "\".");
		return new KeySpline(stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null));
	}

	public double GetSplineProgress(double linearProgress)
	{
		if (_isDirty)
		{
			Build();
		}
		if (!_isSpecified)
		{
			return linearProgress;
		}
		SetParameterFromX(linearProgress);
		return GetBezierValue(_By, _Cy, _parameter);
	}

	public bool IsValid()
	{
		if (IsValidXValue(_controlPointX1))
		{
			return IsValidXValue(_controlPointX2);
		}
		return false;
	}

	private static bool IsValidXValue(double value)
	{
		if (value >= 0.0)
		{
			return value <= 1.0;
		}
		return false;
	}

	private void Build()
	{
		if (_controlPointX1 == 0.0 && _controlPointY1 == 0.0 && _controlPointX2 == 1.0 && _controlPointY2 == 1.0)
		{
			_isSpecified = false;
		}
		else
		{
			_isSpecified = true;
			_parameter = 0.0;
			_Bx = 3.0 * _controlPointX1;
			_Cx = 3.0 * _controlPointX2;
			_Cx_Bx = 2.0 * (_Cx - _Bx);
			_three_Cx = 3.0 - _Cx;
			_By = 3.0 * _controlPointY1;
			_Cy = 3.0 * _controlPointY2;
		}
		_isDirty = false;
	}

	private static double GetBezierValue(double b, double c, double t)
	{
		double num = 1.0 - t;
		double num2 = t * t;
		return b * t * num * num + c * num2 * num + num2 * t;
	}

	private void GetXAndDx(double t, out double x, out double dx)
	{
		double num = 1.0 - t;
		double num2 = t * t;
		double num3 = num * num;
		x = _Bx * t * num3 + _Cx * num2 * num + num2 * t;
		dx = _Bx * num3 + _Cx_Bx * num * t + _three_Cx * num2;
	}

	private void SetParameterFromX(double time)
	{
		double num = 0.0;
		double num2 = 1.0;
		if (time == 0.0)
		{
			_parameter = 0.0;
			return;
		}
		if (time == 1.0)
		{
			_parameter = 1.0;
			return;
		}
		while (num2 - num > 1E-06)
		{
			GetXAndDx(_parameter, out var x, out var dx);
			double num3 = Math.Abs(dx);
			if (x > time)
			{
				num2 = _parameter;
			}
			else
			{
				num = _parameter;
			}
			if (Math.Abs(x - time) < 0.001 * num3)
			{
				break;
			}
			if (num3 > 1E-06)
			{
				double num4 = _parameter - (x - time) / dx;
				if (num4 >= num2)
				{
					_parameter = (_parameter + num2) / 2.0;
				}
				else if (num4 <= num)
				{
					_parameter = (_parameter + num) / 2.0;
				}
				else
				{
					_parameter = num4;
				}
			}
			else
			{
				_parameter = (num + num2) / 2.0;
			}
		}
	}
}
