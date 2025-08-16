using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public class ResizeProcessor : CloningImageProcessor
{
	public int DestinationWidth { get; }

	public int DestinationHeight { get; }

	public Rectangle DestinationRectangle { get; }

	public ResizeOptions Options { get; }

	public ResizeProcessor(ResizeOptions options, Size sourceSize)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(options.Sampler, "Sampler");
		Guard.MustBeValueType(options.Sampler, "options.Sampler");
		(Size Size, Rectangle Rectangle) tuple = ResizeHelper.CalculateTargetLocationAndBounds(sourceSize, options);
		Size item = tuple.Size;
		Rectangle item2 = tuple.Rectangle;
		Options = options;
		DestinationWidth = item.Width;
		DestinationHeight = item.Height;
		DestinationRectangle = item2;
	}

	public override ICloningImageProcessor<TPixel> CreatePixelSpecificCloningProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new ResizeProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class ResizeProcessor<TPixel> : TransformProcessor<TPixel>, IResamplingTransformImageProcessor<TPixel>, IImageProcessor<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct NNRowOperation : IRowOperation
	{
		private readonly Rectangle sourceBounds;

		private readonly Rectangle destinationBounds;

		private readonly Rectangle interest;

		private readonly float widthFactor;

		private readonly float heightFactor;

		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<TPixel> destination;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NNRowOperation(Rectangle sourceBounds, Rectangle destinationBounds, Rectangle interest, float widthFactor, float heightFactor, Buffer2D<TPixel> source, Buffer2D<TPixel> destination)
		{
			this.sourceBounds = sourceBounds;
			this.destinationBounds = destinationBounds;
			this.interest = interest;
			this.widthFactor = widthFactor;
			this.heightFactor = heightFactor;
			this.source = source;
			this.destination = destination;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			int x = sourceBounds.X;
			int y2 = sourceBounds.Y;
			int x2 = destinationBounds.X;
			int y3 = destinationBounds.Y;
			int left = interest.Left;
			int right = interest.Right;
			Span<TPixel> span = source.DangerousGetRowSpan((int)((float)(y - y3) * heightFactor + (float)y2));
			Span<TPixel> span2 = destination.DangerousGetRowSpan(y);
			for (int i = left; i < right; i++)
			{
				span2[i] = span[(int)((float)(i - x2) * widthFactor + (float)x)];
			}
		}
	}

	private readonly ResizeOptions options;

	private readonly int destinationWidth;

	private readonly int destinationHeight;

	private readonly IResampler resampler;

	private readonly Rectangle destinationRectangle;

	private Image<TPixel>? destination;

	public ResizeProcessor(Configuration configuration, ResizeProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		destinationWidth = definition.DestinationWidth;
		destinationHeight = definition.DestinationHeight;
		destinationRectangle = definition.DestinationRectangle;
		options = definition.Options;
		resampler = definition.Options.Sampler;
	}

	protected override Size GetDestinationSize()
	{
		return new Size(destinationWidth, destinationHeight);
	}

	protected override void BeforeImageApply(Image<TPixel> destination)
	{
		this.destination = destination;
		resampler.ApplyTransform(this);
		base.BeforeImageApply(destination);
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
	{
	}

	public void ApplyTransform<TResampler>(in TResampler sampler) where TResampler : struct, IResampler
	{
		Configuration configuration = base.Configuration;
		Image<TPixel> source = base.Source;
		Image<TPixel> image = destination;
		Rectangle sourceRectangle = base.SourceRectangle;
		Rectangle rectangle = destinationRectangle;
		bool compand = options.Compand;
		bool premultiplyAlpha = options.PremultiplyAlpha;
		TPixel value = options.PadColor.ToPixel<TPixel>();
		bool flag = (options.Mode == ResizeMode.BoxPad || options.Mode == ResizeMode.Pad) && options.PadColor != default(Color);
		if (source.Width == image.Width && source.Height == image.Height && sourceRectangle == rectangle)
		{
			for (int i = 0; i < source.Frames.Count; i++)
			{
				ImageFrame<TPixel> source2 = source.Frames[i];
				MemoryGroupExtensions.CopyTo(target: image.Frames[i].GetPixelMemoryGroup(), source: source2.GetPixelMemoryGroup());
			}
			return;
		}
		Rectangle interest = Rectangle.Intersect(rectangle, image.Bounds);
		if (sampler is NearestNeighborResampler)
		{
			for (int j = 0; j < source.Frames.Count; j++)
			{
				ImageFrame<TPixel> source3 = source.Frames[j];
				ImageFrame<TPixel> imageFrame = image.Frames[j];
				if (flag)
				{
					imageFrame.Clear(value);
				}
				ApplyNNResizeFrameTransform(configuration, source3, imageFrame, sourceRectangle, rectangle, interest);
			}
			return;
		}
		MemoryAllocator memoryAllocator = configuration.MemoryAllocator;
		using ResizeKernelMap horizontalKernelMap = ResizeKernelMap.Calculate(in sampler, rectangle.Width, sourceRectangle.Width, memoryAllocator);
		using ResizeKernelMap verticalKernelMap = ResizeKernelMap.Calculate(in sampler, rectangle.Height, sourceRectangle.Height, memoryAllocator);
		for (int k = 0; k < source.Frames.Count; k++)
		{
			ImageFrame<TPixel> source4 = source.Frames[k];
			ImageFrame<TPixel> imageFrame2 = image.Frames[k];
			if (flag)
			{
				imageFrame2.Clear(value);
			}
			ApplyResizeFrameTransform(configuration, source4, imageFrame2, horizontalKernelMap, verticalKernelMap, sourceRectangle, rectangle, interest, compand, premultiplyAlpha);
		}
	}

	private static void ApplyNNResizeFrameTransform(Configuration configuration, ImageFrame<TPixel> source, ImageFrame<TPixel> destination, Rectangle sourceRectangle, Rectangle destinationRectangle, Rectangle interest)
	{
		float widthFactor = (float)sourceRectangle.Width / (float)destinationRectangle.Width;
		float heightFactor = (float)sourceRectangle.Height / (float)destinationRectangle.Height;
		ParallelRowIterator.IterateRows<NNRowOperation>(configuration, interest, new NNRowOperation(sourceRectangle, destinationRectangle, interest, widthFactor, heightFactor, source.PixelBuffer, destination.PixelBuffer));
	}

	private static PixelConversionModifiers GetModifiers(bool compand, bool premultiplyAlpha)
	{
		if (premultiplyAlpha)
		{
			return PixelConversionModifiers.Premultiply.ApplyCompanding(compand);
		}
		return PixelConversionModifiers.None.ApplyCompanding(compand);
	}

	private static void ApplyResizeFrameTransform(Configuration configuration, ImageFrame<TPixel> source, ImageFrame<TPixel> destination, ResizeKernelMap horizontalKernelMap, ResizeKernelMap verticalKernelMap, Rectangle sourceRectangle, Rectangle destinationRectangle, Rectangle interest, bool compand, bool premultiplyAlpha)
	{
		PixelAlphaRepresentation? pixelAlphaRepresentation = PixelOperations<TPixel>.Instance.GetPixelTypeInfo()?.AlphaRepresentation;
		bool flag = !pixelAlphaRepresentation.HasValue || pixelAlphaRepresentation.Value == PixelAlphaRepresentation.Unassociated;
		premultiplyAlpha = premultiplyAlpha && flag;
		PixelConversionModifiers modifiers = GetModifiers(compand, premultiplyAlpha);
		Buffer2DRegion<TPixel> region = source.PixelBuffer.GetRegion(sourceRectangle);
		using ResizeWorker<TPixel> resizeWorker = new ResizeWorker<TPixel>(configuration, region, modifiers, horizontalKernelMap, verticalKernelMap, interest, destinationRectangle.Location);
		resizeWorker.Initialize();
		RowInterval rowInterval = new RowInterval(interest.Top, interest.Bottom);
		resizeWorker.FillDestinationPixels(rowInterval, destination.PixelBuffer);
	}

	void IResamplingTransformImageProcessor<TPixel>.ApplyTransform<TResampler>(in TResampler sampler)
	{
		ApplyTransform(in sampler);
	}
}
