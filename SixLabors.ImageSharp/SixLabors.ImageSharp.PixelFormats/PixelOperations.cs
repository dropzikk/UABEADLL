using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats.PixelBlenders;
using SixLabors.ImageSharp.PixelFormats.Utils;

namespace SixLabors.ImageSharp.PixelFormats;

public class PixelOperations<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<TPixel>(), isThreadSafe: true);

	private static readonly Lazy<PixelOperations<TPixel>> LazyInstance = new Lazy<PixelOperations<TPixel>>(() => default(TPixel).CreatePixelOperations(), isThreadSafe: true);

	public static PixelOperations<TPixel> Instance => LazyInstance.Value;

	public virtual PixelTypeInfo GetPixelTypeInfo()
	{
		return LazyInfo.Value;
	}

	public virtual void FromVector4Destructive(Configuration configuration, Span<Vector4> sourceVectors, Span<TPixel> destinationPixels, PixelConversionModifiers modifiers)
	{
		Guard.NotNull(configuration, "configuration");
		Vector4Converters.Default.FromVector4(sourceVectors, destinationPixels, modifiers);
	}

	public void FromVector4Destructive(Configuration configuration, Span<Vector4> sourceVectors, Span<TPixel> destinationPixels)
	{
		FromVector4Destructive(configuration, sourceVectors, destinationPixels, PixelConversionModifiers.None);
	}

	public virtual void ToVector4(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Vector4> destinationVectors, PixelConversionModifiers modifiers)
	{
		Guard.NotNull(configuration, "configuration");
		Vector4Converters.Default.ToVector4(sourcePixels, destinationVectors, modifiers);
	}

	public void ToVector4(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Vector4> destinationVectors)
	{
		ToVector4(configuration, sourcePixels, destinationVectors, PixelConversionModifiers.None);
	}

	public virtual void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<TPixel> destinationPixels) where TSourcePixel : unmanaged, IPixel<TSourcePixel>
	{
		int num = sourcePixels.Length / 1024;
		using IMemoryOwner<Vector4> buffer = configuration.MemoryAllocator.Allocate<Vector4>(1024);
		Span<Vector4> span = buffer.GetSpan();
		for (int i = 0; i < num; i++)
		{
			int start = i * 1024;
			ReadOnlySpan<TSourcePixel> sourcePixels2 = sourcePixels.Slice(start, 1024);
			Span<TPixel> destinationPixels2 = destinationPixels.Slice(start, 1024);
			PixelOperations<TSourcePixel>.Instance.ToVector4(configuration, sourcePixels2, span, PixelConversionModifiers.Scale);
			FromVector4Destructive(configuration, span, destinationPixels2, PixelConversionModifiers.Scale);
		}
		int num2 = num * 1024;
		int num3 = sourcePixels.Length - num2;
		if (num3 > 0)
		{
			int num4 = num2;
			ReadOnlySpan<TSourcePixel> sourcePixels3 = sourcePixels.Slice(num4, sourcePixels.Length - num4);
			num4 = num2;
			Span<TPixel> destinationPixels3 = destinationPixels.Slice(num4, destinationPixels.Length - num4);
			span = span.Slice(0, num3);
			PixelOperations<TSourcePixel>.Instance.ToVector4(configuration, sourcePixels3, span, PixelConversionModifiers.Scale);
			FromVector4Destructive(configuration, span, destinationPixels3, PixelConversionModifiers.Scale);
		}
	}

	public virtual void To<TDestinationPixel>(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<TDestinationPixel> destinationPixels) where TDestinationPixel : unmanaged, IPixel<TDestinationPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		PixelOperations<TDestinationPixel>.Instance.From(configuration, sourcePixels, destinationPixels);
	}

	internal virtual void PackFromRgbPlanes(ReadOnlySpan<byte> redChannel, ReadOnlySpan<byte> greenChannel, ReadOnlySpan<byte> blueChannel, Span<TPixel> destination)
	{
		int length = redChannel.Length;
		GuardPackFromRgbPlanes(greenChannel, blueChannel, destination, length);
		Rgb24 source = default(Rgb24);
		ref byte reference = ref MemoryMarshal.GetReference(redChannel);
		ref byte reference2 = ref MemoryMarshal.GetReference(greenChannel);
		ref byte reference3 = ref MemoryMarshal.GetReference(blueChannel);
		ref TPixel reference4 = ref MemoryMarshal.GetReference(destination);
		for (nuint num = 0u; num < (uint)length; num++)
		{
			source.R = Unsafe.Add(ref reference, num);
			source.G = Unsafe.Add(ref reference2, num);
			source.B = Unsafe.Add(ref reference3, num);
			Unsafe.Add(ref reference4, num).FromRgb24(source);
		}
	}

	internal virtual void UnpackIntoRgbPlanes(Span<float> redChannel, Span<float> greenChannel, Span<float> blueChannel, ReadOnlySpan<TPixel> source)
	{
		GuardUnpackIntoRgbPlanes(redChannel, greenChannel, blueChannel, source);
		int length = source.Length;
		Rgba32 dest = default(Rgba32);
		ref float reference = ref MemoryMarshal.GetReference(redChannel);
		ref float reference2 = ref MemoryMarshal.GetReference(greenChannel);
		ref float reference3 = ref MemoryMarshal.GetReference(blueChannel);
		ref TPixel reference4 = ref MemoryMarshal.GetReference(source);
		for (nuint num = 0u; num < (uint)length; num++)
		{
			Unsafe.Add(ref reference4, num).ToRgba32(ref dest);
			Unsafe.Add(ref reference, num) = (int)dest.R;
			Unsafe.Add(ref reference2, num) = (int)dest.G;
			Unsafe.Add(ref reference3, num) = (int)dest.B;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void GuardUnpackIntoRgbPlanes(Span<float> redChannel, Span<float> greenChannel, Span<float> blueChannel, ReadOnlySpan<TPixel> source)
	{
		Guard.IsTrue(greenChannel.Length == redChannel.Length, "greenChannel", "Channels must be of same size!");
		Guard.IsTrue(blueChannel.Length == redChannel.Length, "blueChannel", "Channels must be of same size!");
		Guard.IsTrue(source.Length <= redChannel.Length, "source", "'source' span should not be bigger than the destination channels!");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void GuardPackFromRgbPlanes(ReadOnlySpan<byte> greenChannel, ReadOnlySpan<byte> blueChannel, Span<TPixel> destination, int count)
	{
		Guard.IsTrue(greenChannel.Length == count, "greenChannel", "Channels must be of same size!");
		Guard.IsTrue(blueChannel.Length == count, "blueChannel", "Channels must be of same size!");
		Guard.IsTrue(destination.Length > count + 2, "destination", "'destination' must contain a padding of 3 elements!");
	}

	public virtual void FromArgb32(Configuration configuration, ReadOnlySpan<Argb32> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Argb32 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Argb32 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromArgb32(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromArgb32(configuration, MemoryMarshal.Cast<byte, Argb32>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToArgb32(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Argb32> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Argb32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToArgb32Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToArgb32(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Argb32>(destBytes));
	}

	public virtual void FromAbgr32(Configuration configuration, ReadOnlySpan<Abgr32> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Abgr32 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Abgr32 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromAbgr32(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromAbgr32(configuration, MemoryMarshal.Cast<byte, Abgr32>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToAbgr32(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Abgr32> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Abgr32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToAbgr32Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToAbgr32(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Abgr32>(destBytes));
	}

	public virtual void FromBgr24(Configuration configuration, ReadOnlySpan<Bgr24> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Bgr24 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Bgr24 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromBgr24(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromBgr24(configuration, MemoryMarshal.Cast<byte, Bgr24>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToBgr24(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Bgr24> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Bgr24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToBgr24Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToBgr24(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Bgr24>(destBytes));
	}

	public virtual void FromBgra32(Configuration configuration, ReadOnlySpan<Bgra32> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Bgra32 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Bgra32 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromBgra32(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromBgra32(configuration, MemoryMarshal.Cast<byte, Bgra32>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToBgra32(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Bgra32> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Bgra32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToBgra32Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToBgra32(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Bgra32>(destBytes));
	}

	public virtual void FromL8(Configuration configuration, ReadOnlySpan<L8> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref L8 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref L8 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromL8(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromL8(configuration, MemoryMarshal.Cast<byte, L8>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToL8(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<L8> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref L8 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToL8Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToL8(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, L8>(destBytes));
	}

	public virtual void FromL16(Configuration configuration, ReadOnlySpan<L16> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref L16 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref L16 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromL16(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromL16(configuration, MemoryMarshal.Cast<byte, L16>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToL16(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<L16> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref L16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToL16Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToL16(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, L16>(destBytes));
	}

	public virtual void FromLa16(Configuration configuration, ReadOnlySpan<La16> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref La16 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref La16 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromLa16(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromLa16(configuration, MemoryMarshal.Cast<byte, La16>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToLa16(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<La16> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref La16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToLa16Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToLa16(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, La16>(destBytes));
	}

	public virtual void FromLa32(Configuration configuration, ReadOnlySpan<La32> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref La32 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref La32 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromLa32(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromLa32(configuration, MemoryMarshal.Cast<byte, La32>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToLa32(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<La32> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref La32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToLa32Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToLa32(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, La32>(destBytes));
	}

	public virtual void FromRgb24(Configuration configuration, ReadOnlySpan<Rgb24> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Rgb24 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Rgb24 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromRgb24(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromRgb24(configuration, MemoryMarshal.Cast<byte, Rgb24>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToRgb24(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Rgb24> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Rgb24 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgb24Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToRgb24(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Rgb24>(destBytes));
	}

	public virtual void FromRgba32(Configuration configuration, ReadOnlySpan<Rgba32> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Rgba32 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromRgba32(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromRgba32(configuration, MemoryMarshal.Cast<byte, Rgba32>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToRgba32(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Rgba32> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Rgba32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToRgba32(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Rgba32>(destBytes));
	}

	public virtual void FromRgb48(Configuration configuration, ReadOnlySpan<Rgb48> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Rgb48 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Rgb48 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromRgb48(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromRgb48(configuration, MemoryMarshal.Cast<byte, Rgb48>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToRgb48(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Rgb48> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Rgb48 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgb48Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToRgb48(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Rgb48>(destBytes));
	}

	public virtual void FromRgba64(Configuration configuration, ReadOnlySpan<Rgba64> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Rgba64 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Rgba64 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromRgba64(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromRgba64(configuration, MemoryMarshal.Cast<byte, Rgba64>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToRgba64(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Rgba64> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Rgba64 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba64Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToRgba64(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Rgba64>(destBytes));
	}

	public virtual void FromBgra5551(Configuration configuration, ReadOnlySpan<Bgra5551> source, Span<TPixel> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
		ref Bgra5551 reference = ref MemoryMarshal.GetReference(source);
		ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)source.Length; num++)
		{
			ref Bgra5551 reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromBgra5551(reference3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551Bytes(Configuration configuration, ReadOnlySpan<byte> sourceBytes, Span<TPixel> destinationPixels, int count)
	{
		FromBgra5551(configuration, MemoryMarshal.Cast<byte, Bgra5551>(sourceBytes).Slice(0, count), destinationPixels);
	}

	public virtual void ToBgra5551(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<Bgra5551> destinationPixels)
	{
		Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
		ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
		ref Bgra5551 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
		for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
		{
			ref TPixel reference3 = ref Unsafe.Add(ref reference, num);
			Unsafe.Add(ref reference2, num).FromScaledVector4(reference3.ToScaledVector4());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToBgra5551Bytes(Configuration configuration, ReadOnlySpan<TPixel> sourcePixels, Span<byte> destBytes, int count)
	{
		ToBgra5551(configuration, sourcePixels.Slice(0, count), MemoryMarshal.Cast<byte, Bgra5551>(destBytes));
	}

	public PixelBlender<TPixel> GetPixelBlender(GraphicsOptions options)
	{
		return GetPixelBlender(options.ColorBlendingMode, options.AlphaCompositionMode);
	}

	public virtual PixelBlender<TPixel> GetPixelBlender(PixelColorBlendingMode colorMode, PixelAlphaCompositionMode alphaMode)
	{
		return alphaMode switch
		{
			PixelAlphaCompositionMode.Clear => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplyClear.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddClear.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractClear.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenClear.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenClear.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenClear.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlayClear.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightClear.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalClear.Instance, 
			}, 
			PixelAlphaCompositionMode.Xor => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplyXor.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddXor.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractXor.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenXor.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenXor.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenXor.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlayXor.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightXor.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalXor.Instance, 
			}, 
			PixelAlphaCompositionMode.Src => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplySrc.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddSrc.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractSrc.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenSrc.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenSrc.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenSrc.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlaySrc.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightSrc.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalSrc.Instance, 
			}, 
			PixelAlphaCompositionMode.SrcAtop => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplySrcAtop.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddSrcAtop.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractSrcAtop.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenSrcAtop.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenSrcAtop.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenSrcAtop.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlaySrcAtop.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightSrcAtop.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalSrcAtop.Instance, 
			}, 
			PixelAlphaCompositionMode.SrcIn => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplySrcIn.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddSrcIn.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractSrcIn.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenSrcIn.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenSrcIn.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenSrcIn.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlaySrcIn.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightSrcIn.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalSrcIn.Instance, 
			}, 
			PixelAlphaCompositionMode.SrcOut => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplySrcOut.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddSrcOut.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractSrcOut.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenSrcOut.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenSrcOut.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenSrcOut.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlaySrcOut.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightSrcOut.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalSrcOut.Instance, 
			}, 
			PixelAlphaCompositionMode.Dest => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplyDest.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddDest.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractDest.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenDest.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenDest.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenDest.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlayDest.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightDest.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalDest.Instance, 
			}, 
			PixelAlphaCompositionMode.DestAtop => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplyDestAtop.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddDestAtop.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractDestAtop.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenDestAtop.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenDestAtop.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenDestAtop.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlayDestAtop.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightDestAtop.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalDestAtop.Instance, 
			}, 
			PixelAlphaCompositionMode.DestIn => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplyDestIn.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddDestIn.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractDestIn.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenDestIn.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenDestIn.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenDestIn.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlayDestIn.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightDestIn.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalDestIn.Instance, 
			}, 
			PixelAlphaCompositionMode.DestOut => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplyDestOut.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddDestOut.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractDestOut.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenDestOut.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenDestOut.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenDestOut.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlayDestOut.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightDestOut.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalDestOut.Instance, 
			}, 
			PixelAlphaCompositionMode.DestOver => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplyDestOver.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddDestOver.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractDestOver.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenDestOver.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenDestOver.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenDestOver.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlayDestOver.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightDestOver.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalDestOver.Instance, 
			}, 
			_ => colorMode switch
			{
				PixelColorBlendingMode.Multiply => DefaultPixelBlenders<TPixel>.MultiplySrcOver.Instance, 
				PixelColorBlendingMode.Add => DefaultPixelBlenders<TPixel>.AddSrcOver.Instance, 
				PixelColorBlendingMode.Subtract => DefaultPixelBlenders<TPixel>.SubtractSrcOver.Instance, 
				PixelColorBlendingMode.Screen => DefaultPixelBlenders<TPixel>.ScreenSrcOver.Instance, 
				PixelColorBlendingMode.Darken => DefaultPixelBlenders<TPixel>.DarkenSrcOver.Instance, 
				PixelColorBlendingMode.Lighten => DefaultPixelBlenders<TPixel>.LightenSrcOver.Instance, 
				PixelColorBlendingMode.Overlay => DefaultPixelBlenders<TPixel>.OverlaySrcOver.Instance, 
				PixelColorBlendingMode.HardLight => DefaultPixelBlenders<TPixel>.HardLightSrcOver.Instance, 
				_ => DefaultPixelBlenders<TPixel>.NormalSrcOver.Instance, 
			}, 
		};
	}
}
