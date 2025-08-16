using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

[StructLayout(LayoutKind.Explicit)]
public struct La16 : IPixel<La16>, IPixel, IEquatable<La16>, IPackedVector<ushort>
{
	internal class PixelOperations : PixelOperations<La16>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<La16>(PixelAlphaRepresentation.Unassociated), isThreadSafe: true);

		public override void FromLa16(Configuration configuration, ReadOnlySpan<La16> source, Span<La16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
			source.CopyTo(destinationPixels.Slice(0, source.Length));
		}

		public override void ToLa16(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<La16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			sourcePixels.CopyTo(destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override void ToArgb32(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Argb32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Argb32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToAbgr32(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Abgr32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Abgr32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToBgr24(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgr24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToBgra32(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Bgra32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToL8(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<L8> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L8 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToL16(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<L16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToLa32(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<La32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToRgb24(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Rgb24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToRgba32(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToRgb48(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Rgb48> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb48 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToRgba64(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Rgba64> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba64 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void ToBgra5551(Configuration configuration, ReadOnlySpan<La16> sourcePixels, Span<Bgra5551> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref La16 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra5551 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref La16 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromLa16(reference3);
			}
		}

		public override void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<La16> destinationPixels)
		{
			PixelOperations<TSourcePixel>.Instance.ToLa16(configuration, sourcePixels, destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	private static readonly Vector4 MaxBytes = new Vector4(255f);

	private static readonly Vector4 Half = new Vector4(0.5f);

	[FieldOffset(0)]
	public byte L;

	[FieldOffset(1)]
	public byte A;

	public ushort PackedValue
	{
		readonly get
		{
			return Unsafe.As<La16, ushort>(ref Unsafe.AsRef(in this));
		}
		set
		{
			Unsafe.As<La16, ushort>(ref this) = value;
		}
	}

	public La16(byte l, byte a)
	{
		L = l;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(La16 left, La16 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(La16 left, La16 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<La16> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(La16 other)
	{
		return PackedValue.Equals(other.PackedValue);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is La16 other)
		{
			return Equals(other);
		}
		return false;
	}

	public override readonly string ToString()
	{
		return $"La16({L}, {A})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return PackedValue.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		L = ColorNumerics.DownScaleFrom16BitTo8Bit(source.PackedValue);
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8(L8 source)
	{
		L = source.PackedValue;
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		this = source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		L = ColorNumerics.DownScaleFrom16BitTo8Bit(source.L);
		A = ColorNumerics.DownScaleFrom16BitTo8Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(ColorNumerics.DownScaleFrom16BitTo8Bit(source.R), ColorNumerics.DownScaleFrom16BitTo8Bit(source.G), ColorNumerics.DownScaleFrom16BitTo8Bit(source.B));
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
		A = source.A;
	}

	public void FromRgba64(Rgba64 source)
	{
		L = ColorNumerics.Get8BitBT709Luminance(ColorNumerics.DownScaleFrom16BitTo8Bit(source.R), ColorNumerics.DownScaleFrom16BitTo8Bit(source.G), ColorNumerics.DownScaleFrom16BitTo8Bit(source.B));
		A = ColorNumerics.DownScaleFrom16BitTo8Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		ConvertFromRgbaScaledVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		ConvertFromRgbaScaledVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.R = L;
		dest.G = L;
		dest.B = L;
		dest.A = A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return ToVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		float num = (float)(int)L / 255f;
		return new Vector4(num, num, num, (float)(int)A / 255f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ConvertFromRgbaScaledVector4(Vector4 vector)
	{
		vector *= MaxBytes;
		vector += Half;
		vector = Numerics.Clamp(vector, Vector4.Zero, MaxBytes);
		L = ColorNumerics.Get8BitBT709Luminance((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
		A = (byte)vector.W;
	}
}
