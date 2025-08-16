using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct L8 : IPixel<L8>, IPixel, IEquatable<L8>, IPackedVector<byte>
{
	internal class PixelOperations : PixelOperations<L8>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<L8>(PixelAlphaRepresentation.None), isThreadSafe: true);

		public override void FromL8(Configuration configuration, ReadOnlySpan<L8> source, Span<L8> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
			source.CopyTo(destinationPixels.Slice(0, source.Length));
		}

		public override void ToL8(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<L8> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			sourcePixels.CopyTo(destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override void ToArgb32(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Argb32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Argb32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToAbgr32(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Abgr32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Abgr32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToBgr24(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgr24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToBgra32(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Bgra32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToL16(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<L16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToLa16(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<La16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToLa32(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<La32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToRgb24(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Rgb24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToRgba32(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToRgb48(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Rgb48> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb48 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToRgba64(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Rgba64> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba64 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void ToBgra5551(Configuration configuration, ReadOnlySpan<L8> sourcePixels, Span<Bgra5551> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref L8 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra5551 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref L8 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromL8(reference3);
			}
		}

		public override void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<L8> destinationPixels)
		{
			PixelOperations<TSourcePixel>.Instance.ToL8(configuration, sourcePixels, destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	private static readonly Vector4 MaxBytes = new Vector4(255f);

	private static readonly Vector4 Half = new Vector4(0.5f);

	public byte PackedValue { get; set; }

	public L8(byte luminance)
	{
		PackedValue = luminance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(L8 left, L8 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(L8 left, L8 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<L8> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		ConvertFromRgbaScaledVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return ToVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		ConvertFromRgbaScaledVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		float num = (float)(int)PackedValue / 255f;
		return new Vector4(num, num, num, 1f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8(L8 source)
	{
		this = source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		PackedValue = ColorNumerics.DownScaleFrom16BitTo8Bit(source.PackedValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		PackedValue = source.L;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		PackedValue = ColorNumerics.DownScaleFrom16BitTo8Bit(source.L);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(source.R, source.G, source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.R = PackedValue;
		dest.G = PackedValue;
		dest.B = PackedValue;
		dest.A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(ColorNumerics.DownScaleFrom16BitTo8Bit(source.R), ColorNumerics.DownScaleFrom16BitTo8Bit(source.G), ColorNumerics.DownScaleFrom16BitTo8Bit(source.B));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64(Rgba64 source)
	{
		PackedValue = ColorNumerics.Get8BitBT709Luminance(ColorNumerics.DownScaleFrom16BitTo8Bit(source.R), ColorNumerics.DownScaleFrom16BitTo8Bit(source.G), ColorNumerics.DownScaleFrom16BitTo8Bit(source.B));
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is L8 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(L8 other)
	{
		return PackedValue.Equals(other.PackedValue);
	}

	public override readonly string ToString()
	{
		return $"L8({PackedValue})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return PackedValue.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ConvertFromRgbaScaledVector4(Vector4 vector)
	{
		vector *= MaxBytes;
		vector += Half;
		vector = Numerics.Clamp(vector, Vector4.Zero, MaxBytes);
		PackedValue = ColorNumerics.Get8BitBT709Luminance((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
	}
}
