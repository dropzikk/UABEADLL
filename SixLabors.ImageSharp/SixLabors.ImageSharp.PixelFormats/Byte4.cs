using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct Byte4 : IPixel<Byte4>, IPixel, IEquatable<Byte4>, IPackedVector<uint>
{
	internal class PixelOperations : PixelOperations<Byte4>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<Byte4>(PixelAlphaRepresentation.Unassociated), isThreadSafe: true);

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	public uint PackedValue { get; set; }

	public Byte4(Vector4 vector)
	{
		PackedValue = Pack(ref vector);
	}

	public Byte4(float x, float y, float z, float w)
	{
		Vector4 vector = new Vector4(x, y, z, w);
		PackedValue = Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Byte4 left, Byte4 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Byte4 left, Byte4 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<Byte4> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		FromVector4(vector * 255f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return ToVector4() / 255f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		PackedValue = Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4(PackedValue & 0xFF, (PackedValue >> 8) & 0xFF, (PackedValue >> 16) & 0xFF, (PackedValue >> 24) & 0xFF);
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
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
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
		if (obj is Byte4 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Byte4 other)
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
		return FormattableString.Invariant($"Byte4({vector.X:#0.##}, {vector.Y:#0.##}, {vector.Z:#0.##}, {vector.W:#0.##})");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Pack(ref Vector4 vector)
	{
		vector = Numerics.Clamp(vector, Vector4.Zero, new Vector4(255f));
		uint num = (uint)Math.Round(vector.X) & 0xFF;
		uint num2 = ((uint)Math.Round(vector.Y) & 0xFF) << 8;
		uint num3 = ((uint)Math.Round(vector.Z) & 0xFF) << 16;
		uint num4 = ((uint)Math.Round(vector.W) & 0xFF) << 24;
		return num | num2 | num3 | num4;
	}
}
