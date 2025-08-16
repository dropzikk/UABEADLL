using System;
using System.Buffers;
using System.Linq;
using System.Threading;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

internal abstract class SpectralConverter
{
	private static readonly int[] ScaledBlockSizes = new int[3] { 1, 2, 4 };

	protected bool Converted { get; private set; }

	public abstract void InjectFrameData(JpegFrame frame, IRawJpegData jpegData);

	public abstract void PrepareForDecoding();

	public abstract void ConvertStrideBaseline();

	public void CommitConversion()
	{
		Converted = true;
	}

	protected virtual JpegColorConverterBase GetColorConverter(JpegFrame frame, IRawJpegData jpegData)
	{
		return JpegColorConverterBase.GetConverter(jpegData.ColorSpace, frame.Precision);
	}

	public static Size CalculateResultingImageSize(Size size, Size? targetSize, out int blockPixelSize)
	{
		blockPixelSize = 8;
		if (targetSize.HasValue)
		{
			Size value = targetSize.Value;
			int num = (int)((uint)size.Width / 8u);
			int num2 = (int)((uint)size.Height / 8u);
			int num3 = size.Width & 7;
			int num4 = size.Height & 7;
			for (int i = 0; i < ScaledBlockSizes.Length; i++)
			{
				int num5 = ScaledBlockSizes[i];
				int num6 = num * num5 + (int)Numerics.DivideCeil((uint)(num3 * num5), 8u);
				int num7 = num2 * num5 + (int)Numerics.DivideCeil((uint)(num4 * num5), 8u);
				if (num6 >= value.Width && num7 >= value.Height)
				{
					blockPixelSize = num5;
					return new Size(num6, num7);
				}
			}
		}
		return size;
	}

	public abstract bool HasPixelBuffer();
}
internal class SpectralConverter<TPixel> : SpectralConverter, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private JpegFrame frame;

	private IRawJpegData jpegData;

	private ComponentProcessor[] componentProcessors;

	private JpegColorConverterBase colorConverter;

	private IMemoryOwner<byte> rgbBuffer;

	private IMemoryOwner<TPixel> paddedProxyPixelRow;

	private Buffer2D<TPixel> pixelBuffer;

	private int pixelRowsPerStep;

	private int pixelRowCounter;

	private Size? targetSize;

	public Configuration Configuration { get; }

	public SpectralConverter(Configuration configuration, Size? targetSize = null)
	{
		Configuration = configuration;
		this.targetSize = targetSize;
	}

	public override bool HasPixelBuffer()
	{
		return pixelBuffer != null;
	}

	public Buffer2D<TPixel> GetPixelBuffer(CancellationToken cancellationToken)
	{
		if (!base.Converted)
		{
			PrepareForDecoding();
			int num = (int)Math.Ceiling((float)pixelBuffer.Height / (float)pixelRowsPerStep);
			for (int i = 0; i < num; i++)
			{
				cancellationToken.ThrowIfCancellationRequested();
				ConvertStride(i);
			}
		}
		Buffer2D<TPixel> result = pixelBuffer;
		pixelBuffer = null;
		return result;
	}

	private void ConvertStride(int spectralStep)
	{
		int num = Math.Min(pixelBuffer.Height, pixelRowCounter + pixelRowsPerStep);
		for (int i = 0; i < componentProcessors.Length; i++)
		{
			componentProcessors[i].CopyBlocksToColorBuffer(spectralStep);
		}
		int width = pixelBuffer.Width;
		for (int j = pixelRowCounter; j < num; j++)
		{
			int row = j - pixelRowCounter;
			JpegColorConverterBase.ComponentValues values = new JpegColorConverterBase.ComponentValues(componentProcessors, row);
			colorConverter.ConvertToRgbInplace(in values);
			values = values.Slice(0, width);
			Span<byte> span = rgbBuffer.Slice(0, width);
			Span<byte> span2 = rgbBuffer.Slice(width, width);
			Span<byte> span3 = rgbBuffer.Slice(width * 2, width);
			SimdUtils.NormalizedFloatToByteSaturate(values.Component0, span);
			SimdUtils.NormalizedFloatToByteSaturate(values.Component1, span2);
			SimdUtils.NormalizedFloatToByteSaturate(values.Component2, span3);
			if (pixelBuffer.DangerousTryGetPaddedRowSpan(j, 3, out var paddedSpan))
			{
				PixelOperations<TPixel>.Instance.PackFromRgbPlanes(span, span2, span3, paddedSpan);
				continue;
			}
			Span<TPixel> span4 = paddedProxyPixelRow.GetSpan();
			PixelOperations<TPixel>.Instance.PackFromRgbPlanes(span, span2, span3, span4);
			span4.Slice(0, width).CopyTo(pixelBuffer.DangerousGetRowSpan(j));
		}
		pixelRowCounter += pixelRowsPerStep;
	}

	public override void InjectFrameData(JpegFrame frame, IRawJpegData jpegData)
	{
		this.frame = frame;
		this.jpegData = jpegData;
	}

	public override void PrepareForDecoding()
	{
		MemoryAllocator memoryAllocator = Configuration.MemoryAllocator;
		JpegColorConverterBase jpegColorConverterBase = (colorConverter = GetColorConverter(frame, jpegData));
		int blockPixelSize;
		Size size = SpectralConverter.CalculateResultingImageSize(frame.PixelSize, targetSize, out blockPixelSize);
		int num = frame.Components.Max((JpegComponent component) => component.SizeInBlocks.Width);
		int num2 = frame.Components.Max((JpegComponent component) => component.SamplingFactors.Height);
		pixelRowsPerStep = num2 * blockPixelSize;
		pixelBuffer = memoryAllocator.Allocate2D<TPixel>(size.Width, size.Height, Configuration.PreferContiguousImageBuffers, AllocationOptions.Clean);
		paddedProxyPixelRow = memoryAllocator.Allocate<TPixel>(size.Width + 3);
		int num3 = num * blockPixelSize;
		int elementsPerBatch = jpegColorConverterBase.ElementsPerBatch;
		int num4 = num3 & (elementsPerBatch - 1);
		int num5 = ((num4 != 0) ? (elementsPerBatch - num4) : 0);
		componentProcessors = CreateComponentProcessors(processorBufferSize: new Size(num3 + num5, pixelRowsPerStep), frame: frame, jpegData: jpegData, blockPixelSize: blockPixelSize);
		rgbBuffer = memoryAllocator.Allocate<byte>(size.Width * 3);
	}

	public override void ConvertStrideBaseline()
	{
		ConvertStride(0);
		ComponentProcessor[] array = componentProcessors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClearSpectralBuffers();
		}
	}

	protected ComponentProcessor[] CreateComponentProcessors(JpegFrame frame, IRawJpegData jpegData, int blockPixelSize, Size processorBufferSize)
	{
		MemoryAllocator memoryAllocator = Configuration.MemoryAllocator;
		ComponentProcessor[] array = new ComponentProcessor[frame.Components.Length];
		for (int i = 0; i < array.Length; i++)
		{
			ComponentProcessor[] array2 = array;
			int num = i;
			array2[num] = blockPixelSize switch
			{
				4 => new DownScalingComponentProcessor2(memoryAllocator, frame, jpegData, processorBufferSize, frame.Components[i]), 
				2 => new DownScalingComponentProcessor4(memoryAllocator, frame, jpegData, processorBufferSize, frame.Components[i]), 
				1 => new DownScalingComponentProcessor8(memoryAllocator, frame, jpegData, processorBufferSize, frame.Components[i]), 
				_ => new DirectComponentProcessor(memoryAllocator, frame, jpegData, processorBufferSize, frame.Components[i]), 
			};
		}
		return array;
	}

	public void Dispose()
	{
		if (componentProcessors != null)
		{
			ComponentProcessor[] array = componentProcessors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Dispose();
			}
		}
		rgbBuffer?.Dispose();
		paddedProxyPixelRow?.Dispose();
		pixelBuffer?.Dispose();
	}
}
