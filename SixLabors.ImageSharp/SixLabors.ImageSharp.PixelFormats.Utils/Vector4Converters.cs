using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace SixLabors.ImageSharp.PixelFormats.Utils;

internal static class Vector4Converters
{
	public static class Default
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FromVector4<TPixel>(Span<Vector4> sourceVectors, Span<TPixel> destPixels, PixelConversionModifiers modifiers) where TPixel : unmanaged, IPixel<TPixel>
		{
			Guard.DestinationShouldNotBeTooShort(sourceVectors, destPixels, "destPixels");
			UnsafeFromVector4(sourceVectors, destPixels, modifiers);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToVector4<TPixel>(ReadOnlySpan<TPixel> sourcePixels, Span<Vector4> destVectors, PixelConversionModifiers modifiers) where TPixel : unmanaged, IPixel<TPixel>
		{
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destVectors, "destVectors");
			UnsafeToVector4(sourcePixels, destVectors, modifiers);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UnsafeFromVector4<TPixel>(Span<Vector4> sourceVectors, Span<TPixel> destPixels, PixelConversionModifiers modifiers) where TPixel : unmanaged, IPixel<TPixel>
		{
			ApplyBackwardConversionModifiers(sourceVectors, modifiers);
			if (PixelConversionModifiersExtensions.IsDefined(modifiers, PixelConversionModifiers.Scale))
			{
				UnsafeFromScaledVector4Core(sourceVectors, destPixels);
			}
			else
			{
				UnsafeFromVector4Core(sourceVectors, destPixels);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UnsafeToVector4<TPixel>(ReadOnlySpan<TPixel> sourcePixels, Span<Vector4> destVectors, PixelConversionModifiers modifiers) where TPixel : unmanaged, IPixel<TPixel>
		{
			if (PixelConversionModifiersExtensions.IsDefined(modifiers, PixelConversionModifiers.Scale))
			{
				UnsafeToScaledVector4Core(sourcePixels, destVectors);
			}
			else
			{
				UnsafeToVector4Core(sourcePixels, destVectors);
			}
			ApplyForwardConversionModifiers(destVectors, modifiers);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UnsafeFromVector4Core<TPixel>(ReadOnlySpan<Vector4> sourceVectors, Span<TPixel> destPixels) where TPixel : unmanaged, IPixel<TPixel>
		{
			ref Vector4 reference = ref MemoryMarshal.GetReference(sourceVectors);
			ref Vector4 right = ref Unsafe.Add(ref reference, (uint)sourceVectors.Length);
			ref TPixel reference2 = ref MemoryMarshal.GetReference(destPixels);
			while (Unsafe.IsAddressLessThan(ref reference, ref right))
			{
				reference2.FromVector4(reference);
				reference = ref Unsafe.Add(ref reference, 1);
				reference2 = ref Unsafe.Add(ref reference2, 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UnsafeToVector4Core<TPixel>(ReadOnlySpan<TPixel> sourcePixels, Span<Vector4> destVectors) where TPixel : unmanaged, IPixel<TPixel>
		{
			ref TPixel reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref TPixel right = ref Unsafe.Add(ref reference, (uint)sourcePixels.Length);
			ref Vector4 reference2 = ref MemoryMarshal.GetReference(destVectors);
			while (Unsafe.IsAddressLessThan(ref reference, ref right))
			{
				reference2 = reference.ToVector4();
				reference = ref Unsafe.Add(ref reference, 1);
				reference2 = ref Unsafe.Add(ref reference2, 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UnsafeFromScaledVector4Core<TPixel>(ReadOnlySpan<Vector4> sourceVectors, Span<TPixel> destinationColors) where TPixel : unmanaged, IPixel<TPixel>
		{
			ref Vector4 reference = ref MemoryMarshal.GetReference(sourceVectors);
			ref Vector4 right = ref Unsafe.Add(ref reference, (uint)sourceVectors.Length);
			ref TPixel reference2 = ref MemoryMarshal.GetReference(destinationColors);
			while (Unsafe.IsAddressLessThan(ref reference, ref right))
			{
				reference2.FromScaledVector4(reference);
				reference = ref Unsafe.Add(ref reference, 1);
				reference2 = ref Unsafe.Add(ref reference2, 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UnsafeToScaledVector4Core<TPixel>(ReadOnlySpan<TPixel> sourceColors, Span<Vector4> destinationVectors) where TPixel : unmanaged, IPixel<TPixel>
		{
			ref TPixel reference = ref MemoryMarshal.GetReference(sourceColors);
			ref TPixel right = ref Unsafe.Add(ref reference, (uint)sourceColors.Length);
			ref Vector4 reference2 = ref MemoryMarshal.GetReference(destinationVectors);
			while (Unsafe.IsAddressLessThan(ref reference, ref right))
			{
				reference2 = reference.ToScaledVector4();
				reference = ref Unsafe.Add(ref reference, 1);
				reference2 = ref Unsafe.Add(ref reference2, 1);
			}
		}
	}

	public static class RgbaCompatible
	{
		private static readonly int Vector4ConversionThreshold = CalculateVector4ConversionThreshold();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ToVector4<TPixel>(Configuration configuration, PixelOperations<TPixel> pixelOperations, ReadOnlySpan<TPixel> sourcePixels, Span<Vector4> destVectors, PixelConversionModifiers modifiers) where TPixel : unmanaged, IPixel<TPixel>
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destVectors, "destVectors");
			int length = sourcePixels.Length;
			if (length < Vector4ConversionThreshold)
			{
				Default.UnsafeToVector4(sourcePixels, destVectors, modifiers);
				return;
			}
			int num = length - 1;
			ReadOnlySpan<TPixel> sourcePixels2 = sourcePixels.Slice(0, num);
			Span<Rgba32> span = MemoryMarshal.Cast<Vector4, Rgba32>(destVectors).Slice(3 * length + 1, num);
			pixelOperations.ToRgba32(configuration, sourcePixels2, span);
			SimdUtils.ByteToNormalizedFloat(MemoryMarshal.Cast<Rgba32, byte>(span), MemoryMarshal.Cast<Vector4, float>(destVectors.Slice(0, num)));
			destVectors[num] = sourcePixels[num].ToVector4();
			ApplyForwardConversionModifiers(destVectors, modifiers);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FromVector4<TPixel>(Configuration configuration, PixelOperations<TPixel> pixelOperations, Span<Vector4> sourceVectors, Span<TPixel> destPixels, PixelConversionModifiers modifiers) where TPixel : unmanaged, IPixel<TPixel>
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourceVectors, destPixels, "destPixels");
			int length = sourceVectors.Length;
			if (length < Vector4ConversionThreshold)
			{
				Default.UnsafeFromVector4(sourceVectors, destPixels, modifiers);
				return;
			}
			ApplyBackwardConversionModifiers(sourceVectors, modifiers);
			using IMemoryOwner<Rgba32> memoryOwner = configuration.MemoryAllocator.Allocate<Rgba32>(length);
			Span<Rgba32> span = memoryOwner.Memory.Span;
			SimdUtils.NormalizedFloatToByteSaturate(MemoryMarshal.Cast<Vector4, float>(sourceVectors), MemoryMarshal.Cast<Rgba32, byte>(span));
			pixelOperations.FromRgba32(configuration, span, destPixels);
		}

		private static int CalculateVector4ConversionThreshold()
		{
			if (!Vector.IsHardwareAccelerated)
			{
				return int.MaxValue;
			}
			if (!SimdUtils.ExtendedIntrinsics.IsAvailable || !SimdUtils.HasVector8)
			{
				return 128;
			}
			return 256;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ApplyForwardConversionModifiers(Span<Vector4> vectors, PixelConversionModifiers modifiers)
	{
		if (PixelConversionModifiersExtensions.IsDefined(modifiers, PixelConversionModifiers.SRgbCompand))
		{
			SRgbCompanding.Expand(vectors);
		}
		if (PixelConversionModifiersExtensions.IsDefined(modifiers, PixelConversionModifiers.Premultiply))
		{
			Numerics.Premultiply(vectors);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ApplyBackwardConversionModifiers(Span<Vector4> vectors, PixelConversionModifiers modifiers)
	{
		if (PixelConversionModifiersExtensions.IsDefined(modifiers, PixelConversionModifiers.Premultiply))
		{
			Numerics.UnPremultiply(vectors);
		}
		if (PixelConversionModifiersExtensions.IsDefined(modifiers, PixelConversionModifiers.SRgbCompand))
		{
			SRgbCompanding.Compress(vectors);
		}
	}
}
