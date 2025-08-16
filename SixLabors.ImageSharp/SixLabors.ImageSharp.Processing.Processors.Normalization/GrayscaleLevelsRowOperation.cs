using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

internal readonly struct GrayscaleLevelsRowOperation<TPixel> : IRowOperation<Vector4> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly Configuration configuration;

	private readonly Rectangle bounds;

	private readonly IMemoryOwner<int> histogramBuffer;

	private readonly Buffer2D<TPixel> source;

	private readonly int luminanceLevels;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GrayscaleLevelsRowOperation(Configuration configuration, Rectangle bounds, IMemoryOwner<int> histogramBuffer, Buffer2D<TPixel> source, int luminanceLevels)
	{
		this.configuration = configuration;
		this.bounds = bounds;
		this.histogramBuffer = histogramBuffer;
		this.source = source;
		this.luminanceLevels = luminanceLevels;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetRequiredBufferLength(Rectangle bounds)
	{
		return bounds.Width;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Invoke(int y, Span<Vector4> span)
	{
		Span<Vector4> span2 = span.Slice(0, bounds.Width);
		ref Vector4 reference = ref MemoryMarshal.GetReference(span2);
		ref int reference2 = ref MemoryMarshal.GetReference(histogramBuffer.GetSpan());
		int num = luminanceLevels;
		Span<TPixel> span3 = source.DangerousGetRowSpan(y);
		PixelOperations<TPixel>.Instance.ToVector4(configuration, span3, span2);
		for (int i = 0; i < bounds.Width; i++)
		{
			Vector4 vector = Unsafe.Add(ref reference, (uint)i);
			int bT709Luminance = ColorNumerics.GetBT709Luminance(ref vector, num);
			Interlocked.Increment(ref Unsafe.Add(ref reference2, (uint)bT709Luminance));
		}
	}
}
