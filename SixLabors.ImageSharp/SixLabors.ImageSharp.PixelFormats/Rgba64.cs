using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats;

namespace SixLabors.ImageSharp.PixelFormats;

public struct Rgba64 : IPixel<Rgba64>, IPixel, IEquatable<Rgba64>, IPackedVector<ulong>
{
	internal class PixelOperations : PixelOperations<Rgba64>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<Rgba64>(PixelAlphaRepresentation.Unassociated), isThreadSafe: true);

		public override void FromRgba64(Configuration configuration, ReadOnlySpan<Rgba64> source, Span<Rgba64> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
			source.CopyTo(destinationPixels.Slice(0, source.Length));
		}

		public override void ToRgba64(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Rgba64> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			sourcePixels.CopyTo(destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override void ToArgb32(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Argb32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Argb32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToAbgr32(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Abgr32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Abgr32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToBgr24(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgr24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToBgra32(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Bgra32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToL8(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<L8> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L8 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToL16(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<L16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToLa16(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<La16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToLa32(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<La32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToRgb24(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Rgb24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToRgba32(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToRgb48(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Rgb48> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb48 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void ToBgra5551(Configuration configuration, ReadOnlySpan<Rgba64> sourcePixels, Span<Bgra5551> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba64 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra5551 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba64(reference3);
			}
		}

		public override void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<Rgba64> destinationPixels)
		{
			PixelOperations<TSourcePixel>.Instance.ToRgba64(configuration, sourcePixels, destinationPixels.Slice(0, sourcePixels.Length));
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

	public ushort A;

	public Rgb48 Rgb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Unsafe.As<Rgba64, Rgb48>(ref Unsafe.AsRef(in this));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Unsafe.As<Rgba64, Rgb48>(ref this) = value;
		}
	}

	public ulong PackedValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Unsafe.As<Rgba64, ulong>(ref Unsafe.AsRef(in this));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Unsafe.As<Rgba64, ulong>(ref this) = value;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(ushort r, ushort g, ushort b, ushort a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(Rgba32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(Bgra32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(Argb32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(Abgr32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(Rgb24 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(Bgr24 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba64(Vector4 vector)
	{
		vector = Numerics.Clamp(vector, Vector4.Zero, Vector4.One) * 65535f;
		R = (ushort)MathF.Round(vector.X);
		G = (ushort)MathF.Round(vector.Y);
		B = (ushort)MathF.Round(vector.Z);
		A = (ushort)MathF.Round(vector.W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Color(Rgba64 source)
	{
		return new Color(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Rgba64(Color color)
	{
		return color.ToPixel<Rgba64>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Rgba64 left, Rgba64 right)
	{
		return left.PackedValue == right.PackedValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Rgba64 left, Rgba64 right)
	{
		return left.PackedValue != right.PackedValue;
	}

	public readonly PixelOperations<Rgba64> CreatePixelOperations()
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
		A = (ushort)MathF.Round(vector.W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4((int)R, (int)G, (int)B, (int)A) / 65535f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
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
		A = ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		R = source.PackedValue;
		G = source.PackedValue;
		B = source.PackedValue;
		A = ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		B = (G = (R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.L)));
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		R = source.L;
		G = source.L;
		B = source.L;
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		R = ColorNumerics.UpscaleFrom8BitTo16Bit(source.R);
		G = ColorNumerics.UpscaleFrom8BitTo16Bit(source.G);
		B = ColorNumerics.UpscaleFrom8BitTo16Bit(source.B);
		A = ColorNumerics.UpscaleFrom8BitTo16Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.R = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		dest.G = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		dest.B = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		dest.A = ColorNumerics.DownScaleFrom16BitTo8Bit(A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		Rgb = source;
		A = ushort.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64(Rgba64 source)
	{
		this = source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Rgba32 ToRgba32()
	{
		byte r = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		byte g = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		byte b = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		byte a = ColorNumerics.DownScaleFrom16BitTo8Bit(A);
		return new Rgba32(r, g, b, a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Bgra32 ToBgra32()
	{
		byte r = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		byte g = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		byte b = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		byte a = ColorNumerics.DownScaleFrom16BitTo8Bit(A);
		return new Bgra32(r, g, b, a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Argb32 ToArgb32()
	{
		byte r = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		byte g = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		byte b = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		byte a = ColorNumerics.DownScaleFrom16BitTo8Bit(A);
		return new Argb32(r, g, b, a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Abgr32 ToAbgr32()
	{
		byte r = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		byte g = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		byte b = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		byte a = ColorNumerics.DownScaleFrom16BitTo8Bit(A);
		return new Abgr32(r, g, b, a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Rgb24 ToRgb24()
	{
		byte r = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		byte g = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		byte b = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		return new Rgb24(r, g, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Bgr24 ToBgr24()
	{
		byte r = ColorNumerics.DownScaleFrom16BitTo8Bit(R);
		byte g = ColorNumerics.DownScaleFrom16BitTo8Bit(G);
		byte b = ColorNumerics.DownScaleFrom16BitTo8Bit(B);
		return new Bgr24(r, g, b);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is Rgba64 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Rgba64 other)
	{
		return PackedValue.Equals(other.PackedValue);
	}

	public override readonly string ToString()
	{
		return $"Rgba64({R}, {G}, {B}, {A})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return PackedValue.GetHashCode();
	}
}
