using System;
using System.Buffers;
using System.Linq;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal abstract class SpectralConverter
{
}
internal class SpectralConverter<TPixel> : SpectralConverter, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly ComponentProcessor[] componentProcessors;

	private readonly int pixelRowsPerStep;

	private int pixelRowCounter;

	private readonly Buffer2D<TPixel> pixelBuffer;

	private readonly IMemoryOwner<float> redLane;

	private readonly IMemoryOwner<float> greenLane;

	private readonly IMemoryOwner<float> blueLane;

	private readonly int alignedPixelWidth;

	private readonly JpegColorConverterBase colorConverter;

	public SpectralConverter(JpegFrame frame, Image<TPixel> image, Block8x8F[] dequantTables)
	{
		MemoryAllocator memoryAllocator = image.Configuration.MemoryAllocator;
		int num = frame.Components.Max((Component component) => component.SizeInBlocks.Width);
		int num2 = frame.Components.Max((Component component) => component.SamplingFactors.Height);
		pixelRowsPerStep = num2 * 8;
		pixelBuffer = image.GetRootFramePixelBuffer();
		alignedPixelWidth = num * 8;
		Size postProcessorBufferSize = new Size(alignedPixelWidth, pixelRowsPerStep);
		componentProcessors = new ComponentProcessor[frame.Components.Length];
		for (int i = 0; i < componentProcessors.Length; i++)
		{
			Component component2 = frame.Components[i];
			componentProcessors[i] = new ComponentProcessor(memoryAllocator, component2, postProcessorBufferSize, dequantTables[component2.QuantizationTableIndex]);
		}
		redLane = memoryAllocator.Allocate<float>(alignedPixelWidth, AllocationOptions.Clean);
		greenLane = memoryAllocator.Allocate<float>(alignedPixelWidth, AllocationOptions.Clean);
		blueLane = memoryAllocator.Allocate<float>(alignedPixelWidth, AllocationOptions.Clean);
		colorConverter = JpegColorConverterBase.GetConverter(frame.ColorSpace, 8);
	}

	public void ConvertStrideBaseline()
	{
		ConvertStride(0);
	}

	public void ConvertFull()
	{
		int num = (int)Numerics.DivideCeil((uint)pixelBuffer.Height, (uint)pixelRowsPerStep);
		for (int i = 0; i < num; i++)
		{
			ConvertStride(i);
		}
	}

	private void ConvertStride(int spectralStep)
	{
		int num = pixelRowCounter;
		int num2 = num + pixelRowsPerStep;
		int val = pixelBuffer.Height - 1;
		int width = pixelBuffer.Width;
		_ = alignedPixelWidth;
		_ = pixelBuffer.Width;
		Span<float> span = redLane.GetSpan();
		Span<float> span2 = greenLane.GetSpan();
		Span<float> span3 = blueLane.GetSpan();
		for (int i = num; i < num2; i++)
		{
			int row = i - pixelRowCounter;
			int y = Math.Min(i, val);
			Span<TPixel> span4 = pixelBuffer.DangerousGetRowSpan(y);
			PixelOperations<TPixel>.Instance.UnpackIntoRgbPlanes(span, span2, span3, span4);
			span.Slice(width).Fill(span[width - 1]);
			span2.Slice(width).Fill(span2[width - 1]);
			span3.Slice(width).Fill(span3[width - 1]);
			JpegColorConverterBase.ComponentValues values = new JpegColorConverterBase.ComponentValues(componentProcessors, row);
			colorConverter.ConvertFromRgb(in values, span, span2, span3);
		}
		for (int j = 0; j < componentProcessors.Length; j++)
		{
			componentProcessors[j].CopyColorBufferToBlocks(spectralStep);
		}
		pixelRowCounter = num2;
	}

	public void Dispose()
	{
		ComponentProcessor[] array = componentProcessors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Dispose();
		}
		redLane.Dispose();
		greenLane.Dispose();
		blueLane.Dispose();
	}
}
