using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct NormalizedByte2 : IPixel<NormalizedByte2>, IPixel, IEquatable<NormalizedByte2>, IPackedVector<ushort>
{
	internal class PixelOperations : PixelOperations<NormalizedByte2>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<NormalizedByte2>(PixelAlphaRepresentation.None), isThreadSafe: true);

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	private const float MaxPos = 127f;

	private static readonly Vector2 Half = new Vector2(127f);

	private static readonly Vector2 MinusOne = new Vector2(-1f);

	public ushort PackedValue { get; set; }

	public NormalizedByte2(float x, float y)
		: this(new Vector2(x, y))
	{
	}

	public NormalizedByte2(Vector2 vector)
	{
		PackedValue = Pack(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(NormalizedByte2 left, NormalizedByte2 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(NormalizedByte2 left, NormalizedByte2 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<NormalizedByte2> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		Vector2 vector2 = new Vector2(vector.X, vector.Y) * 2f;
		vector2 -= Vector2.One;
		PackedValue = Pack(vector2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return new Vector4((ToVector2() + Vector2.One) / 2f, 0f, 1f);
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
		return new Vector4(ToVector2(), 0f, 1f);
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
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.FromScaledVector4(ToScaledVector4());
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
		return new Vector2((float)(sbyte)(PackedValue & 0xFF) / 127f, (float)(sbyte)((PackedValue >> 8) & 0xFF) / 127f);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is NormalizedByte2 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(NormalizedByte2 other)
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
		return FormattableString.Invariant($"NormalizedByte2({vector.X:#0.##}, {vector.Y:#0.##})");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort Pack(Vector2 vector)
	{
		vector = Vector2.Clamp(vector, MinusOne, Vector2.One) * Half;
		int num = (ushort)Convert.ToInt16(Math.Round(vector.X)) & 0xFF;
		int num2 = ((ushort)Convert.ToInt16(Math.Round(vector.Y)) & 0xFF) << 8;
		return (ushort)(num | num2);
	}
}
