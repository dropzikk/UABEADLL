using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct Short4 : IPixel<Short4>, IPixel, IEquatable<Short4>, IPackedVector<ulong>
{
	internal class PixelOperations : PixelOperations<Short4>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<Short4>(PixelAlphaRepresentation.Unassociated), isThreadSafe: true);

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	private const float MaxPos = 32767f;

	private const float MinNeg = -32768f;

	private static readonly Vector4 Max = new Vector4(32767f);

	private static readonly Vector4 Min = new Vector4(-32768f);

	public ulong PackedValue { get; set; }

	public Short4(float x, float y, float z, float w)
		: this(new Vector4(x, y, z, w))
	{
	}

	public Short4(Vector4 vector)
	{
		PackedValue = Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Short4 left, Short4 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Short4 left, Short4 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<Short4> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		vector *= 65534f;
		vector -= new Vector4(32767f);
		FromVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return (ToVector4() + new Vector4(32767f)) / 65534f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		PackedValue = Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4((short)(PackedValue & 0xFFFF), (short)((PackedValue >> 16) & 0xFFFF), (short)((PackedValue >> 32) & 0xFFFF), (short)((PackedValue >> 48) & 0xFFFF));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8(L8 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.FromScaledVector4(ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64(Rgba64 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is Short4 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Short4 other)
	{
		return PackedValue.Equals(other.PackedValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return PackedValue.GetHashCode();
	}

	public override readonly string ToString()
	{
		Vector4 vector = ToVector4();
		return FormattableString.Invariant($"Short4({vector.X:#0.##}, {vector.Y:#0.##}, {vector.Z:#0.##}, {vector.W:#0.##})");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Pack(ref Vector4 vector)
	{
		vector = Numerics.Clamp(vector, Min, Max);
		long num = (long)Convert.ToInt32(Math.Round(vector.X)) & 0xFFFFL;
		ulong num2 = ((ulong)Convert.ToInt32(Math.Round(vector.Y)) & 0xFFFFuL) << 16;
		ulong num3 = ((ulong)Convert.ToInt32(Math.Round(vector.Z)) & 0xFFFFuL) << 32;
		ulong num4 = ((ulong)Convert.ToInt32(Math.Round(vector.W)) & 0xFFFFuL) << 48;
		return (ulong)num | num2 | num3 | num4;
	}
}
