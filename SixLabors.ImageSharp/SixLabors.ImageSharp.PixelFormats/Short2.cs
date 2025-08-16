using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct Short2 : IPixel<Short2>, IPixel, IEquatable<Short2>, IPackedVector<uint>
{
	internal class PixelOperations : PixelOperations<Short2>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<Short2>(PixelAlphaRepresentation.None), isThreadSafe: true);

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	private const float MaxPos = 32767f;

	private const float MinNeg = -32768f;

	private static readonly Vector2 Max = new Vector2(32767f);

	private static readonly Vector2 Min = new Vector2(-32768f);

	public uint PackedValue { get; set; }

	public Short2(float x, float y)
		: this(new Vector2(x, y))
	{
	}

	public Short2(Vector2 vector)
	{
		PackedValue = Pack(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Short2 left, Short2 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Short2 left, Short2 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<Short2> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		Vector2 vector2 = new Vector2(vector.X, vector.Y) * 65534f;
		vector2 -= new Vector2(32767f);
		PackedValue = Pack(vector2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return new Vector4((ToVector2() + new Vector2(32767f)) / 65534f, 0f, 1f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		Vector2 vector2 = new Vector2(vector.X, vector.Y);
		PackedValue = Pack(vector2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4((short)(PackedValue & 0xFFFF), (short)(PackedValue >> 16), 0f, 1f);
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector2 ToVector2()
	{
		return new Vector2((short)(PackedValue & 0xFFFF), (short)(PackedValue >> 16));
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is Short2 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Short2 other)
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
		Vector2 vector = ToVector2();
		return FormattableString.Invariant($"Short2({vector.X:#0.##}, {vector.Y:#0.##})");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Pack(Vector2 vector)
	{
		vector = Vector2.Clamp(vector, Min, Max);
		int num = Convert.ToInt32(Math.Round(vector.X)) & 0xFFFF;
		uint num2 = (uint)((Convert.ToInt32(Math.Round(vector.Y)) & 0xFFFF) << 16);
		return (uint)num | num2;
	}
}
