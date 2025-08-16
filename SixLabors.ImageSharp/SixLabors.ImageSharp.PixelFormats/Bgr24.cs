using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats.Utils;

namespace SixLabors.ImageSharp.PixelFormats;

[StructLayout(LayoutKind.Explicit)]
public struct Bgr24 : IPixel<Bgr24>, IPixel, IEquatable<Bgr24>
{
	internal class PixelOperations : PixelOperations<Bgr24>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<Bgr24>(PixelAlphaRepresentation.None), isThreadSafe: true);

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}

		public override void FromBgr24(Configuration configuration, ReadOnlySpan<Bgr24> source, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
			source.CopyTo(destinationPixels.Slice(0, source.Length));
		}

		public override void ToBgr24(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			sourcePixels.CopyTo(destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override void FromVector4Destructive(Configuration configuration, Span<Vector4> sourceVectors, Span<Bgr24> destinationPixels, PixelConversionModifiers modifiers)
		{
			Vector4Converters.RgbaCompatible.FromVector4(configuration, this, sourceVectors, destinationPixels, modifiers.Remove(PixelConversionModifiers.Scale | PixelConversionModifiers.Premultiply));
		}

		public override void ToVector4(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Vector4> destVectors, PixelConversionModifiers modifiers)
		{
			Vector4Converters.RgbaCompatible.ToVector4(configuration, this, sourcePixels, destVectors, modifiers.Remove(PixelConversionModifiers.Scale | PixelConversionModifiers.Premultiply));
		}

		public override void ToRgba32(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgr24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgba32, byte>(destinationPixels);
			PixelConverter.FromBgr24.ToRgba32(source, dest);
		}

		public override void FromRgba32(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgba32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgr24, byte>(destinationPixels);
			PixelConverter.FromRgba32.ToBgr24(source, dest);
		}

		public override void ToArgb32(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Argb32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgr24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Argb32, byte>(destinationPixels);
			PixelConverter.FromBgr24.ToArgb32(source, dest);
		}

		public override void FromArgb32(Configuration configuration, ReadOnlySpan<Argb32> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Argb32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgr24, byte>(destinationPixels);
			PixelConverter.FromArgb32.ToBgr24(source, dest);
		}

		public override void ToAbgr32(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Abgr32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgr24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Abgr32, byte>(destinationPixels);
			PixelConverter.FromBgr24.ToAbgr32(source, dest);
		}

		public override void FromAbgr32(Configuration configuration, ReadOnlySpan<Abgr32> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Abgr32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgr24, byte>(destinationPixels);
			PixelConverter.FromAbgr32.ToBgr24(source, dest);
		}

		public override void ToBgra32(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Bgra32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgr24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgra32, byte>(destinationPixels);
			PixelConverter.FromBgr24.ToBgra32(source, dest);
		}

		public override void FromBgra32(Configuration configuration, ReadOnlySpan<Bgra32> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgra32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgr24, byte>(destinationPixels);
			PixelConverter.FromBgra32.ToBgr24(source, dest);
		}

		public override void ToRgb24(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Rgb24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgr24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgb24, byte>(destinationPixels);
			PixelConverter.FromBgr24.ToRgb24(source, dest);
		}

		public override void FromRgb24(Configuration configuration, ReadOnlySpan<Rgb24> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgb24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgr24, byte>(destinationPixels);
			PixelConverter.FromRgb24.ToBgr24(source, dest);
		}

		public override void ToL8(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<L8> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Bgr24 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L8 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromBgr24(reference3);
			}
		}

		public override void ToL16(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<L16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Bgr24 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromBgr24(reference3);
			}
		}

		public override void ToLa16(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<La16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Bgr24 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromBgr24(reference3);
			}
		}

		public override void ToLa32(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<La32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Bgr24 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromBgr24(reference3);
			}
		}

		public override void ToRgb48(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Rgb48> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Bgr24 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb48 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromBgr24(reference3);
			}
		}

		public override void ToRgba64(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Rgba64> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Bgr24 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba64 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromBgr24(reference3);
			}
		}

		public override void ToBgra5551(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Bgra5551> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Bgr24 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra5551 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromBgr24(reference3);
			}
		}

		public override void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<Bgr24> destinationPixels)
		{
			PixelOperations<TSourcePixel>.Instance.ToBgr24(configuration, sourcePixels, destinationPixels.Slice(0, sourcePixels.Length));
		}
	}

	[FieldOffset(0)]
	public byte B;

	[FieldOffset(1)]
	public byte G;

	[FieldOffset(2)]
	public byte R;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bgr24(byte r, byte g, byte b)
	{
		R = r;
		G = g;
		B = b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Color(Bgr24 source)
	{
		return new Color(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Bgr24(Color color)
	{
		return color.ToBgr24();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Bgr24 left, Bgr24 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Bgr24 left, Bgr24 right)
	{
		return !left.Equals(right);
	}

	public readonly PixelOperations<Bgr24> CreatePixelOperations()
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
		Rgba32 source = default(Rgba32);
		source.FromVector4(vector);
		FromRgba32(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Rgba32(R, G, B, byte.MaxValue).ToVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		R = source.R;
		G = source.G;
		B = source.B;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		this = source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		R = source.R;
		G = source.G;
		B = source.B;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8(L8 source)
	{
		R = source.PackedValue;
		G = source.PackedValue;
		B = source.PackedValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		B = (G = (R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.PackedValue)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		R = source.L;
		G = source.L;
		B = source.L;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		B = (G = (R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.L)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		R = source.R;
		G = source.G;
		B = source.B;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		this = Unsafe.As<byte, Bgr24>(ref Unsafe.AddByteOffset(ref Unsafe.As<Abgr32, byte>(ref source), 1u));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		this = source.Bgr;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.R = R;
		dest.G = G;
		dest.B = B;
		dest.A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.R);
		G = ColorNumerics.DownScaleFrom16BitTo8Bit(source.G);
		B = ColorNumerics.DownScaleFrom16BitTo8Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64(Rgba64 source)
	{
		R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.R);
		G = ColorNumerics.DownScaleFrom16BitTo8Bit(source.G);
		B = ColorNumerics.DownScaleFrom16BitTo8Bit(source.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Bgr24 other)
	{
		if (R.Equals(other.R) && G.Equals(other.G))
		{
			return B.Equals(other.B);
		}
		return false;
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is Bgr24 other)
		{
			return Equals(other);
		}
		return false;
	}

	public override readonly string ToString()
	{
		return $"Bgr24({B}, {G}, {R})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return HashCode.Combine(R, B, G);
	}
}
