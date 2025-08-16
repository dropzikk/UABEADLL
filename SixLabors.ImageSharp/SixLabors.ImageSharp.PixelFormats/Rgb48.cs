using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct Rgb48 : IPixel<Rgb48>, IPixel, IEquatable<Rgb48>
{
	internal class PixelOperations : PixelOperations<Rgb48>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<Rgb48>(PixelAlphaRepresentation.None), isThreadSafe: true);

		public override void FromRgb48(Configuration configuration, ReadOnlySpan<Rgb48> source, Span<Rgb48> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
			source.CopyTo(destinationPixels.Slice(0, source.Length));
		}

		public override void ToRgb48(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Rgb48> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			sourcePixels.CopyTo(destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override void ToArgb32(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Argb32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Argb32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToAbgr32(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Abgr32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Abgr32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToBgr24(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgr24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToBgra32(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Bgra32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToL8(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<L8> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L8 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToL16(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<L16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToLa16(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<La16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToLa32(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<La32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToRgb24(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Rgb24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToRgba32(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToRgba64(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Rgba64> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba64 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void ToBgra5551(Configuration configuration, ReadOnlySpan<Rgb48> sourcePixels, Span<Bgra5551> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgb48 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra5551 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgb48(reference3);
			}
		}

		public override void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<Rgb48> destinationPixels)
		{
			PixelOperations<TSourcePixel>.Instance.ToRgb48(configuration, sourcePixels, destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}
	}

	private const float Max = 65535f;

	public ushort R;

	public ushort G;

	public ushort B;

	public Rgb48(ushort r, ushort g, ushort b)
	{
		this = default(Rgb48);
		R = r;
		G = g;
		B = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Rgb48 left, Rgb48 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Rgb48 left, Rgb48 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<Rgb48> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		FromVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return ToVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		vector = Numerics.Clamp(vector, Vector4.Zero, Vector4.One) * 65535f;
		R = (ushort)MathF.Round(vector.X);
		G = (ushort)MathF.Round(vector.Y);
		B = (ushort)MathF.Round(vector.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4((float)(int)R / 65535f, (float)(int)G / 65535f, (float)(int)B / 65535f, 1f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64(Rgba64 source)
	{
		this = source.Rgb;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8(L8 source)
	{
		B = (G = (R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.PackedValue)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		R = source.PackedValue;
		G = source.PackedValue;
		B = source.PackedValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		B = (G = (R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.L)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		R = source.L;
		G = source.L;
		B = source.L;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.R = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		dest.G = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		dest.B = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		dest.A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		this = source;
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is Rgb48 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Rgb48 other)
	{
		if (R.Equals(other.R) && G.Equals(other.G))
		{
			return B.Equals(other.B);
		}
		return false;
	}

	public override readonly string ToString()
	{
		return $"Rgb48({R}, {G}, {B})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return HashCode.Combine(R, G, B);
	}
}
