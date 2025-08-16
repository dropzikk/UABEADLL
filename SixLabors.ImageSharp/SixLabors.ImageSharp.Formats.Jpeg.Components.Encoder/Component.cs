using System;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal class Component : IDisposable
{
	private readonly MemoryAllocator memoryAllocator;

	public int DcPredictor { get; set; }

	public int HorizontalSamplingFactor { get; }

	public int VerticalSamplingFactor { get; }

	public Buffer2D<Block8x8> SpectralBlocks { get; private set; }

	public Size SubSamplingDivisors { get; private set; }

	public int QuantizationTableIndex { get; }

	public Size SizeInBlocks { get; private set; }

	public Size SamplingFactors { get; set; }

	public int WidthInBlocks { get; private set; }

	public int HeightInBlocks { get; private set; }

	public int DcTableId { get; set; }

	public int AcTableId { get; set; }

	public Component(MemoryAllocator memoryAllocator, int horizontalFactor, int verticalFactor, int quantizationTableIndex)
	{
		this.memoryAllocator = memoryAllocator;
		HorizontalSamplingFactor = horizontalFactor;
		VerticalSamplingFactor = verticalFactor;
		SamplingFactors = new Size(horizontalFactor, verticalFactor);
		QuantizationTableIndex = quantizationTableIndex;
	}

	public void Dispose()
	{
		SpectralBlocks?.Dispose();
		SpectralBlocks = null;
	}

	public void Init(JpegFrame frame, int maxSubFactorH, int maxSubFactorV)
	{
		uint num = (uint)(frame.PixelWidth + 7) / 8u;
		uint num2 = (uint)(frame.PixelHeight + 7) / 8u;
		WidthInBlocks = (int)MathF.Ceiling((float)num * (float)HorizontalSamplingFactor / (float)maxSubFactorH);
		HeightInBlocks = (int)MathF.Ceiling((float)num2 * (float)VerticalSamplingFactor / (float)maxSubFactorV);
		int width = frame.McusPerLine * HorizontalSamplingFactor;
		int height = frame.McusPerColumn * VerticalSamplingFactor;
		SizeInBlocks = new Size(width, height);
		SubSamplingDivisors = new Size(maxSubFactorH, maxSubFactorV).DivideBy(SamplingFactors);
		if (SubSamplingDivisors.Width == 0 || SubSamplingDivisors.Height == 0)
		{
			JpegThrowHelper.ThrowBadSampling();
		}
	}

	public void AllocateSpectral(bool fullScan)
	{
		int width = SizeInBlocks.Width;
		int height = (fullScan ? SizeInBlocks.Height : VerticalSamplingFactor);
		SpectralBlocks = memoryAllocator.Allocate2D<Block8x8>(width, height);
	}
}
