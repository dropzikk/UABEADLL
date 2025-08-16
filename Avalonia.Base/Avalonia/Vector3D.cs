using System;
using System.Globalization;
using System.Numerics;
using Avalonia.Utilities;

namespace Avalonia;

public readonly record struct Vector3D(double X, double Y, double Z)
{
	public double Length => Math.Sqrt(Dot(this, this));

	public Vector3D(double X, double Y, double Z)
	{
		this.X = X;
		this.Y = Y;
		this.Z = Z;
	}

	public static Vector3D Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid Vector.");
		return new Vector3D(stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null));
	}

	internal Vector3 ToVector3()
	{
		return new Vector3((float)X, (float)Y, (float)Z);
	}

	internal Vector3D(Vector3 v)
		: this(v.X, v.Y, v.Z)
	{
	}

	public static implicit operator Vector3D(Vector3 vector)
	{
		return new Vector3D(vector);
	}

	public static double Dot(Vector3D vector1, Vector3D vector2)
	{
		return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
	}

	public static Vector3D Add(Vector3D left, Vector3D right)
	{
		return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
	}

	public static Vector3D operator +(Vector3D left, Vector3D right)
	{
		return Add(left, right);
	}

	public static Vector3D Substract(Vector3D left, Vector3D right)
	{
		return new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
	}

	public static Vector3D operator -(Vector3D left, Vector3D right)
	{
		return Substract(left, right);
	}

	public static Vector3D operator -(Vector3D v)
	{
		return new Vector3D(0.0 - v.X, 0.0 - v.Y, 0.0 - v.Z);
	}

	public static Vector3D Multiply(Vector3D left, Vector3D right)
	{
		return new Vector3D(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
	}

	public static Vector3D Multiply(Vector3D left, double right)
	{
		return new Vector3D(left.X * right, left.Y * right, left.Z * right);
	}

	public static Vector3D operator *(Vector3D left, double right)
	{
		return Multiply(left, right);
	}

	public static Vector3D Divide(Vector3D left, Vector3D right)
	{
		return new Vector3D(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
	}

	public static Vector3D Divide(Vector3D left, double right)
	{
		return new Vector3D(left.X / right, left.Y / right, left.Z / right);
	}

	public Vector3D Abs()
	{
		return new Vector3D(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
	}

	public static Vector3D Clamp(Vector3D value, Vector3D min, Vector3D max)
	{
		return Min(Max(value, min), max);
	}

	public static Vector3D Max(Vector3D left, Vector3D right)
	{
		return new Vector3D(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y), Math.Max(left.Z, right.Z));
	}

	public static Vector3D Min(Vector3D left, Vector3D right)
	{
		return new Vector3D(Math.Min(left.X, right.X), Math.Min(left.Y, right.Y), Math.Min(left.Z, right.Z));
	}

	public static Vector3D Normalize(Vector3D value)
	{
		return Divide(value, value.Length);
	}

	public static double DistanceSquared(Vector3D value1, Vector3D value2)
	{
		Vector3D vector3D = Substract(value1, value2);
		return Dot(vector3D, vector3D);
	}

	public static double Distance(Vector3D value1, Vector3D value2)
	{
		return Math.Sqrt(DistanceSquared(value1, value2));
	}
}
