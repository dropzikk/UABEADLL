using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public struct ColorMatrix : IEquatable<ColorMatrix>
{
	internal struct Impl : IEquatable<Impl>
	{
		public Vector4 X;

		public Vector4 Y;

		public Vector4 Z;

		public Vector4 W;

		public Vector4 V;

		public static Impl Identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Impl result = default(Impl);
				result.X = Vector4.UnitX;
				result.Y = Vector4.UnitY;
				result.Z = Vector4.UnitZ;
				result.W = Vector4.UnitW;
				result.V = Vector4.Zero;
				return result;
			}
		}

		public readonly bool IsIdentity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (X == Vector4.UnitX && Y == Vector4.UnitY && Z == Vector4.UnitZ && W == Vector4.UnitW)
				{
					return V == Vector4.Zero;
				}
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator +(in Impl left, in Impl right)
		{
			Impl result = default(Impl);
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
			result.W = left.W + right.W;
			result.V = left.V + right.V;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator -(in Impl left, in Impl right)
		{
			Impl result = default(Impl);
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
			result.W = left.W - right.W;
			result.V = left.V - right.V;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator -(in Impl value)
		{
			Impl result = default(Impl);
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			result.W = -value.W;
			result.V = -value.V;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator *(in Impl left, in Impl right)
		{
			Impl result = default(Impl);
			result.X = right.X * left.X.X;
			result.X += right.Y * left.X.Y;
			result.X += right.Z * left.X.Z;
			result.X += right.W * left.X.W;
			result.Y = right.X * left.Y.X;
			result.Y += right.Y * left.Y.Y;
			result.Y += right.Z * left.Y.Z;
			result.Y += right.W * left.Y.W;
			result.Z = right.X * left.Z.X;
			result.Z += right.Y * left.Z.Y;
			result.Z += right.Z * left.Z.Z;
			result.Z += right.W * left.Z.W;
			result.W = right.X * left.W.X;
			result.W += right.Y * left.W.Y;
			result.W += right.Z * left.W.Z;
			result.W += right.W * left.W.W;
			result.V = right.X * left.V.X;
			result.V += right.Y * left.V.Y;
			result.V += right.Z * left.V.Z;
			result.V += right.W * left.V.W;
			result.V += right.V;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator *(in Impl left, float right)
		{
			Impl result = default(Impl);
			result.X = left.X * right;
			result.Y = left.Y * right;
			result.Z = left.Z * right;
			result.W = left.W * right;
			result.V = left.V * right;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Impl left, in Impl right)
		{
			if (left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W)
			{
				return left.V == right.V;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Impl left, in Impl right)
		{
			if (left.X != right.X && left.Y != right.Y && left.Z != right.Z && left.W != right.W)
			{
				return left.V != right.V;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[UnscopedRef]
		public ref ColorMatrix AsColorMatrix()
		{
			return ref Unsafe.As<Impl, ColorMatrix>(ref this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Init(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44, float m51, float m52, float m53, float m54)
		{
			X = new Vector4(m11, m12, m13, m14);
			Y = new Vector4(m21, m22, m23, m24);
			Z = new Vector4(m31, m32, m33, m34);
			W = new Vector4(m41, m42, m43, m44);
			V = new Vector4(m51, m52, m53, m54);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals([NotNullWhen(true)] object? obj)
		{
			if (obj is ColorMatrix colorMatrix)
			{
				return Equals(in colorMatrix.AsImpl());
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Impl other)
		{
			if (X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W))
			{
				return V.Equals(other.V);
			}
			return false;
		}

		bool IEquatable<Impl>.Equals(Impl other)
		{
			return Equals(in other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(X, Y, Z, W, V);
		}
	}

	public float M11;

	public float M12;

	public float M13;

	public float M14;

	public float M21;

	public float M22;

	public float M23;

	public float M24;

	public float M31;

	public float M32;

	public float M33;

	public float M34;

	public float M41;

	public float M42;

	public float M43;

	public float M44;

	public float M51;

	public float M52;

	public float M53;

	public float M54;

	public static ColorMatrix Identity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Impl.Identity.AsColorMatrix();
		}
	}

	public bool IsIdentity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return AsROImpl().IsIdentity;
		}
	}

	public ColorMatrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44, float m51, float m52, float m53, float m54)
	{
		Unsafe.SkipInit<ColorMatrix>(out this);
		AsImpl().Init(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44, m51, m52, m53, m54);
	}

	public static ColorMatrix operator +(ColorMatrix value1, ColorMatrix value2)
	{
		return (value1.AsImpl() + value2.AsImpl()).AsColorMatrix();
	}

	public static ColorMatrix operator -(ColorMatrix value1, ColorMatrix value2)
	{
		return (value1.AsImpl() - value2.AsImpl()).AsColorMatrix();
	}

	public static ColorMatrix operator -(ColorMatrix value)
	{
		return (-value.AsImpl()).AsColorMatrix();
	}

	public static ColorMatrix operator *(ColorMatrix value1, ColorMatrix value2)
	{
		return (value1.AsImpl() * value2.AsImpl()).AsColorMatrix();
	}

	public static ColorMatrix operator *(ColorMatrix value1, float value2)
	{
		return (value1.AsImpl() * value2).AsColorMatrix();
	}

	public static bool operator ==(ColorMatrix value1, ColorMatrix value2)
	{
		return value1.AsImpl() == value2.AsImpl();
	}

	public static bool operator !=(ColorMatrix value1, ColorMatrix value2)
	{
		return value1.AsImpl() != value2.AsImpl();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		return AsROImpl().Equals(obj);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ColorMatrix other)
	{
		return AsROImpl().Equals(in other.AsImpl());
	}

	public override int GetHashCode()
	{
		return AsROImpl().GetHashCode();
	}

	public override string ToString()
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		return string.Format(currentCulture, "{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} {{M51:{16} M52:{17} M53:{18} M54:{19}}} }}", M11.ToString(currentCulture), M12.ToString(currentCulture), M13.ToString(currentCulture), M14.ToString(currentCulture), M21.ToString(currentCulture), M22.ToString(currentCulture), M23.ToString(currentCulture), M24.ToString(currentCulture), M31.ToString(currentCulture), M32.ToString(currentCulture), M33.ToString(currentCulture), M34.ToString(currentCulture), M41.ToString(currentCulture), M42.ToString(currentCulture), M43.ToString(currentCulture), M44.ToString(currentCulture), M51.ToString(currentCulture), M52.ToString(currentCulture), M53.ToString(currentCulture), M54.ToString(currentCulture));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[UnscopedRef]
	internal ref Impl AsImpl()
	{
		return ref Unsafe.As<ColorMatrix, Impl>(ref this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[UnscopedRef]
	internal readonly ref readonly Impl AsROImpl()
	{
		return ref Unsafe.As<ColorMatrix, Impl>(ref Unsafe.AsRef(in this));
	}
}
