using System;
using System.Buffers;
using System.Numerics;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.PixelFormats;

public abstract class PixelBlender<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public abstract TPixel Blend(TPixel background, TPixel source, float amount);

	public void Blend<TPixelSrc>(Configuration configuration, Span<TPixel> destination, ReadOnlySpan<TPixel> background, ReadOnlySpan<TPixelSrc> source, float amount) where TPixelSrc : unmanaged, IPixel<TPixelSrc>
	{
		int length = destination.Length;
		Guard.MustBeGreaterThanOrEqualTo(background.Length, length, "Length");
		Guard.MustBeGreaterThanOrEqualTo(source.Length, length, "Length");
		Guard.MustBeBetweenOrEqualTo(amount, 0f, 1f, "amount");
		using IMemoryOwner<Vector4> buffer = configuration.MemoryAllocator.Allocate<Vector4>(length * 3);
		Span<Vector4> destination2 = buffer.Slice(0, length);
		Span<Vector4> span = buffer.Slice(length, length);
		Span<Vector4> span2 = buffer.Slice(length * 2, length);
		PixelOperations<TPixel>.Instance.ToVector4(configuration, background.Slice(0, length), span, PixelConversionModifiers.Scale);
		PixelOperations<TPixelSrc>.Instance.ToVector4(configuration, source.Slice(0, length), span2, PixelConversionModifiers.Scale);
		BlendFunction(destination2, span, span2, amount);
		PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, destination2.Slice(0, length), destination, PixelConversionModifiers.Scale);
	}

	public void Blend(Configuration configuration, Span<TPixel> destination, ReadOnlySpan<TPixel> background, ReadOnlySpan<TPixel> source, ReadOnlySpan<float> amount)
	{
		this.Blend<TPixel>(configuration, destination, background, source, amount);
	}

	public void Blend<TPixelSrc>(Configuration configuration, Span<TPixel> destination, ReadOnlySpan<TPixel> background, ReadOnlySpan<TPixelSrc> source, ReadOnlySpan<float> amount) where TPixelSrc : unmanaged, IPixel<TPixelSrc>
	{
		int length = destination.Length;
		Guard.MustBeGreaterThanOrEqualTo(background.Length, length, "Length");
		Guard.MustBeGreaterThanOrEqualTo(source.Length, length, "Length");
		Guard.MustBeGreaterThanOrEqualTo(amount.Length, length, "Length");
		using IMemoryOwner<Vector4> buffer = configuration.MemoryAllocator.Allocate<Vector4>(length * 3);
		Span<Vector4> destination2 = buffer.Slice(0, length);
		Span<Vector4> span = buffer.Slice(length, length);
		Span<Vector4> span2 = buffer.Slice(length * 2, length);
		PixelOperations<TPixel>.Instance.ToVector4(configuration, background.Slice(0, length), span, PixelConversionModifiers.Scale);
		PixelOperations<TPixelSrc>.Instance.ToVector4(configuration, source.Slice(0, length), span2, PixelConversionModifiers.Scale);
		BlendFunction(destination2, span, span2, amount);
		PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, destination2.Slice(0, length), destination, PixelConversionModifiers.Scale);
	}

	protected abstract void BlendFunction(Span<Vector4> destination, ReadOnlySpan<Vector4> background, ReadOnlySpan<Vector4> source, float amount);

	protected abstract void BlendFunction(Span<Vector4> destination, ReadOnlySpan<Vector4> background, ReadOnlySpan<Vector4> source, ReadOnlySpan<float> amount);
}
