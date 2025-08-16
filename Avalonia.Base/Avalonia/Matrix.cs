using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct Matrix : IEquatable<Matrix>
{
	public record struct Decomposed
	{
		public Vector Translate;

		public Vector Scale;

		public Vector Skew;

		public double Angle;
	}

	private readonly double _m11;

	private readonly double _m12;

	private readonly double _m13;

	private readonly double _m21;

	private readonly double _m22;

	private readonly double _m23;

	private readonly double _m31;

	private readonly double _m32;

	private readonly double _m33;

	public static Matrix Identity { get; } = new Matrix(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0);

	public bool IsIdentity => Equals(Identity);

	public bool HasInverse => !MathUtilities.IsZero(GetDeterminant());

	public double M11 => _m11;

	public double M12 => _m12;

	public double M13 => _m13;

	public double M21 => _m21;

	public double M22 => _m22;

	public double M23 => _m23;

	public double M31 => _m31;

	public double M32 => _m32;

	public double M33 => _m33;

	public Matrix(double scaleX, double skewY, double skewX, double scaleY, double offsetX, double offsetY)
		: this(scaleX, skewY, 0.0, skewX, scaleY, 0.0, offsetX, offsetY, 1.0)
	{
	}

	public Matrix(double scaleX, double skewY, double perspX, double skewX, double scaleY, double perspY, double offsetX, double offsetY, double perspZ)
	{
		_m11 = scaleX;
		_m12 = skewY;
		_m13 = perspX;
		_m21 = skewX;
		_m22 = scaleY;
		_m23 = perspY;
		_m31 = offsetX;
		_m32 = offsetY;
		_m33 = perspZ;
	}

	public static Matrix operator *(Matrix value1, Matrix value2)
	{
		return new Matrix(value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31, value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32, value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33, value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31, value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32, value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33, value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31, value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32, value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33);
	}

	public static Matrix operator -(Matrix value)
	{
		return value.Invert();
	}

	public static bool operator ==(Matrix value1, Matrix value2)
	{
		return value1.Equals(value2);
	}

	public static bool operator !=(Matrix value1, Matrix value2)
	{
		return !value1.Equals(value2);
	}

	public static Matrix CreateRotation(double radians)
	{
		double num = Math.Cos(radians);
		double num2 = Math.Sin(radians);
		return new Matrix(num, num2, 0.0 - num2, num, 0.0, 0.0);
	}

	public static Matrix CreateSkew(double xAngle, double yAngle)
	{
		double skewX = Math.Tan(xAngle);
		double skewY = Math.Tan(yAngle);
		return new Matrix(1.0, skewY, skewX, 1.0, 0.0, 0.0);
	}

	public static Matrix CreateScale(double xScale, double yScale)
	{
		return new Matrix(xScale, 0.0, 0.0, yScale, 0.0, 0.0);
	}

	public static Matrix CreateScale(Vector scales)
	{
		return CreateScale(scales.X, scales.Y);
	}

	public static Matrix CreateTranslation(Vector position)
	{
		return CreateTranslation(position.X, position.Y);
	}

	public static Matrix CreateTranslation(double xPosition, double yPosition)
	{
		return new Matrix(1.0, 0.0, 0.0, 1.0, xPosition, yPosition);
	}

	public static double ToRadians(double angle)
	{
		return angle * 0.0174532925;
	}

	public Matrix Append(Matrix value)
	{
		return this * value;
	}

	public Matrix Prepend(Matrix value)
	{
		return value * this;
	}

	public double GetDeterminant()
	{
		return _m11 * (_m22 * _m33 - _m23 * _m32) - _m12 * (_m21 * _m33 - _m23 * _m31) + _m13 * (_m21 * _m32 - _m22 * _m31);
	}

	public Point Transform(Point p)
	{
		if (ContainsPerspective())
		{
			Vector3 vector = Vector3.Transform(matrix: new Matrix4x4((float)M11, (float)M12, (float)M13, 0f, (float)M21, (float)M22, (float)M23, 0f, (float)M31, (float)M32, (float)M33, 0f, 0f, 0f, 0f, 1f), position: new Vector3((float)p.X, (float)p.Y, 1f));
			float num = 1f / vector.Z;
			return new Point(vector.X * num, vector.Y * num);
		}
		return new Point(p.X * M11 + p.Y * M21 + M31, p.X * M12 + p.Y * M22 + M32);
	}

	public bool Equals(Matrix other)
	{
		if (_m11 == other.M11 && _m12 == other.M12 && _m13 == other.M13 && _m21 == other.M21 && _m22 == other.M22 && _m23 == other.M23 && _m31 == other.M31 && _m32 == other.M32)
		{
			return _m33 == other.M33;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Matrix other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (_m11, _m12, _m13, _m21, _m22, _m23, _m31, _m32, _m33).GetHashCode();
	}

	public bool ContainsPerspective()
	{
		if (_m13 == 0.0 && _m23 == 0.0)
		{
			return _m33 != 1.0;
		}
		return true;
	}

	public override string ToString()
	{
		CultureInfo ci = CultureInfo.CurrentCulture;
		string text;
		double[] source;
		if (ContainsPerspective())
		{
			text = "{{ {{M11:{0} M12:{1} M13:{2}}} {{M21:{3} M22:{4} M23:{5}}} {{M31:{6} M32:{7} M33:{8}}} }}";
			source = new double[9] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };
		}
		else
		{
			text = "{{ {{M11:{0} M12:{1}}} {{M21:{2} M22:{3}}} {{M31:{4} M32:{5}}} }}";
			source = new double[6] { M11, M12, M21, M22, M31, M32 };
		}
		CultureInfo provider = ci;
		string format = text;
		object[] args = source.Select((double v) => v.ToString(ci)).ToArray();
		return string.Format(provider, format, args);
	}

	public bool TryInvert(out Matrix inverted)
	{
		double determinant = GetDeterminant();
		if (MathUtilities.IsZero(determinant))
		{
			inverted = default(Matrix);
			return false;
		}
		double num = 1.0 / determinant;
		inverted = new Matrix((_m22 * _m33 - _m32 * _m23) * num, (_m13 * _m32 - _m12 * _m33) * num, (_m12 * _m23 - _m13 * _m22) * num, (_m23 * _m31 - _m21 * _m33) * num, (_m11 * _m33 - _m13 * _m31) * num, (_m21 * _m13 - _m11 * _m23) * num, (_m21 * _m32 - _m31 * _m22) * num, (_m31 * _m12 - _m11 * _m32) * num, (_m11 * _m22 - _m21 * _m12) * num);
		return true;
	}

	public Matrix Invert()
	{
		if (!TryInvert(out var inverted))
		{
			throw new InvalidOperationException("Transform is not invertible.");
		}
		return inverted;
	}

	public static Matrix Parse(string s)
	{
		double result = 0.0;
		double result2 = 0.0;
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid Matrix.");
		double scaleX = stringTokenizer.ReadDouble(null);
		double skewY = stringTokenizer.ReadDouble(null);
		double skewX = stringTokenizer.ReadDouble(null);
		double scaleY = stringTokenizer.ReadDouble(null);
		double offsetX = stringTokenizer.ReadDouble(null);
		double offsetY = stringTokenizer.ReadDouble(null);
		if (stringTokenizer.TryReadDouble(out var result3, null) && stringTokenizer.TryReadDouble(out result, null) && stringTokenizer.TryReadDouble(out result2, null))
		{
			return new Matrix(scaleX, skewY, result3, skewX, scaleY, result, offsetX, offsetY, result2);
		}
		return new Matrix(scaleX, skewY, skewX, scaleY, offsetX, offsetY);
	}

	public static bool TryDecomposeTransform(Matrix matrix, out Decomposed decomposed)
	{
		decomposed = default(Decomposed);
		double determinant = matrix.GetDeterminant();
		if (MathUtilities.IsZero(determinant) || matrix.ContainsPerspective())
		{
			return false;
		}
		double m = matrix.M11;
		double m2 = matrix.M21;
		double m3 = matrix.M12;
		double m4 = matrix.M22;
		decomposed.Translate = new Vector(matrix.M31, matrix.M32);
		double num = 1.0;
		double num2 = 1.0;
		if (determinant < 0.0)
		{
			if (m < m4)
			{
				num *= -1.0;
			}
			else
			{
				num2 *= -1.0;
			}
		}
		num *= Math.Sqrt(m * m + m3 * m3);
		m /= num;
		m3 /= num;
		double num3 = m * m2 + m3 * m4;
		m2 -= m * num3;
		m4 -= m3 * num3;
		num2 *= Math.Sqrt(m2 * m2 + m4 * m4);
		decomposed.Scale = new Vector(num, num2);
		decomposed.Skew = new Vector(num3 / num2, 0.0);
		decomposed.Angle = Math.Atan2(m3, m);
		return true;
	}
}
