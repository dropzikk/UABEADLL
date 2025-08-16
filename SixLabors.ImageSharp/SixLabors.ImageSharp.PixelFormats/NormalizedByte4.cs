using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct NormalizedByte4 : IPixel<NormalizedByte4>, IPixel, IEquatable<NormalizedByte4>, IPackedVector<uint>
{
	internal class PixelOperations : PixelOperations<NormalizedByte4>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<NormalizedByte4>(PixelAlphaRepresentation.Unassociated), isThreadSafe: true);

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	private const float MaxPos = 127f;

	private static readonly Vector4 Half = new Vector4(127f);

	private static readonly Vector4 MinusOne = new Vector4(-1f);

	public uint PackedValue { get; set; }

	public NormalizedByte4(float x, float y, float z, float w)
		: this(new Vector4(x, y, z, w))
	{
	}

	public NormalizedByte4(Vector4 vector)
	{
		PackedValue = Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(NormalizedByte4 left, NormalizedByte4 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(NormalizedByte4 left, NormalizedByte4 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<NormalizedByte4> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		vector *= 2f;
		vector -= Vector4.One;
		FromVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return (ToVector4() + Vector4.One) / 2f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		PackedValue = Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4((float)(sbyte)(PackedValue & 0xFF) / 127f, (float)(sbyte)((PackedValue >> 8) & 0xFF) / 127f, (float)(sbyte)((PackedValue >> 16) & 0xFF) / 127f, (float)(sbyte)((PackedValue >> 24) & 0xFF) / 127f);
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
		if (obj is NormalizedByte4 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(NormalizedByte4 other)
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
		return FormattableString.Invariant($"NormalizedByte4({vector.X:#0.##}, {vector.Y:#0.##}, {vector.Z:#0.##}, {vector.W:#0.##})");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Pack(ref Vector4 vector)
	{
		vector = Numerics.Clamp(vector, MinusOne, Vector4.One) * Half;
		int num = Convert.ToInt16(MathF.Round(vector.X)) & 0xFF;
		uint num2 = (uint)((Convert.ToInt16(MathF.Round(vector.Y)) & 0xFF) << 8);
		uint num3 = (uint)((Convert.ToInt16(MathF.Round(vector.Z)) & 0xFF) << 16);
		uint num4 = (uint)((Convert.ToInt16(MathF.Round(vector.W)) & 0xFF) << 24);
		return (uint)num | num2 | num3 | num4;
	}
}
