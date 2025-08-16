using System;
using System.Buffers;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors;

namespace SixLabors.ImageSharp.Processing;

public static class ProcessingExtensions
{
	private class ProcessingVisitor : IImageVisitor
	{
		private readonly Configuration configuration;

		private readonly Action<IImageProcessingContext> operation;

		private readonly bool mutate;

		private Image? resultImage;

		public ProcessingVisitor(Configuration configuration, Action<IImageProcessingContext> operation, bool mutate)
		{
			this.configuration = configuration;
			this.operation = operation;
			this.mutate = mutate;
		}

		public Image GetResultImage()
		{
			return resultImage;
		}

		public void Visit<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
		{
			IInternalImageProcessingContext<TPixel> internalImageProcessingContext = configuration.ImageOperationsProvider.CreateImageProcessingContext(configuration, image, mutate);
			operation(internalImageProcessingContext);
			resultImage = internalImageProcessingContext.GetResultImage();
		}
	}

	public static void Mutate(this Image source, Action<IImageProcessingContext> operation)
	{
		source.Mutate(source.Configuration, operation);
	}

	public static void Mutate(this Image source, Configuration configuration, Action<IImageProcessingContext> operation)
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(source, "source");
		Guard.NotNull(operation, "operation");
		source.EnsureNotDisposed();
		source.AcceptVisitor(new ProcessingVisitor(configuration, operation, mutate: true));
	}

	public static void Mutate<TPixel>(this Image<TPixel> source, Action<IImageProcessingContext> operation) where TPixel : unmanaged, IPixel<TPixel>
	{
		source.Mutate(source.Configuration, operation);
	}

	public static void Mutate<TPixel>(this Image<TPixel> source, Configuration configuration, Action<IImageProcessingContext> operation) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(source, "source");
		Guard.NotNull(operation, "operation");
		source.EnsureNotDisposed();
		IInternalImageProcessingContext<TPixel> obj = configuration.ImageOperationsProvider.CreateImageProcessingContext(configuration, source, mutate: true);
		operation(obj);
	}

	public static void Mutate<TPixel>(this Image<TPixel> source, params IImageProcessor[] operations) where TPixel : unmanaged, IPixel<TPixel>
	{
		source.Mutate(source.Configuration, operations);
	}

	public static void Mutate<TPixel>(this Image<TPixel> source, Configuration configuration, params IImageProcessor[] operations) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(source, "source");
		Guard.NotNull(operations, "operations");
		source.EnsureNotDisposed();
		configuration.ImageOperationsProvider.CreateImageProcessingContext(configuration, source, mutate: true).ApplyProcessors(operations);
	}

	public static Image Clone(this Image source, Action<IImageProcessingContext> operation)
	{
		return source.Clone(source.Configuration, operation);
	}

	public static Image Clone(this Image source, Configuration configuration, Action<IImageProcessingContext> operation)
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(source, "source");
		Guard.NotNull(operation, "operation");
		source.EnsureNotDisposed();
		ProcessingVisitor processingVisitor = new ProcessingVisitor(configuration, operation, mutate: false);
		source.AcceptVisitor(processingVisitor);
		return processingVisitor.GetResultImage();
	}

	public static Image<TPixel> Clone<TPixel>(this Image<TPixel> source, Action<IImageProcessingContext> operation) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source.Clone(source.Configuration, operation);
	}

	public static Image<TPixel> Clone<TPixel>(this Image<TPixel> source, Configuration configuration, Action<IImageProcessingContext> operation) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(source, "source");
		Guard.NotNull(operation, "operation");
		source.EnsureNotDisposed();
		IInternalImageProcessingContext<TPixel> internalImageProcessingContext = configuration.ImageOperationsProvider.CreateImageProcessingContext(configuration, source, mutate: false);
		operation(internalImageProcessingContext);
		return internalImageProcessingContext.GetResultImage();
	}

	public static Image<TPixel> Clone<TPixel>(this Image<TPixel> source, params IImageProcessor[] operations) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source.Clone(source.Configuration, operations);
	}

	public static Image<TPixel> Clone<TPixel>(this Image<TPixel> source, Configuration configuration, params IImageProcessor[] operations) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(source, "source");
		Guard.NotNull(operations, "operations");
		source.EnsureNotDisposed();
		IInternalImageProcessingContext<TPixel> internalImageProcessingContext = configuration.ImageOperationsProvider.CreateImageProcessingContext(configuration, source, mutate: false);
		internalImageProcessingContext.ApplyProcessors(operations);
		return internalImageProcessingContext.GetResultImage();
	}

	public static IImageProcessingContext ApplyProcessors(this IImageProcessingContext source, params IImageProcessor[] operations)
	{
		foreach (IImageProcessor processor in operations)
		{
			source = source.ApplyProcessor(processor);
		}
		return source;
	}

	public static Buffer2D<ulong> CalculateIntegralImage<TPixel>(this Image<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source.Frames.RootFrame.CalculateIntegralImage();
	}

	public static Buffer2D<ulong> CalculateIntegralImage<TPixel>(this Image<TPixel> source, Rectangle bounds) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source.Frames.RootFrame.CalculateIntegralImage(bounds);
	}

	public static Buffer2D<ulong> CalculateIntegralImage<TPixel>(this ImageFrame<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
	{
		return source.CalculateIntegralImage(source.Bounds());
	}

	public static Buffer2D<ulong> CalculateIntegralImage<TPixel>(this ImageFrame<TPixel> source, Rectangle bounds) where TPixel : unmanaged, IPixel<TPixel>
	{
		Configuration configuration = source.Configuration;
		Rectangle rectangle = Rectangle.Intersect(bounds, source.Bounds());
		int y = rectangle.Y;
		int x = rectangle.X;
		int height = rectangle.Height;
		Buffer2D<ulong> buffer2D = configuration.MemoryAllocator.Allocate2D<ulong>(rectangle.Width, rectangle.Height);
		ulong num = 0uL;
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		using IMemoryOwner<L8> buffer = configuration.MemoryAllocator.Allocate<L8>(rectangle.Width);
		Span<L8> span = buffer.GetSpan();
		Span<TPixel> span2 = pixelBuffer.DangerousGetRowSpan(y);
		Span<TPixel> span3 = span2.Slice(x, span.Length);
		Span<ulong> span4 = buffer2D.DangerousGetRowSpan(0);
		PixelOperations<TPixel>.Instance.ToL8(configuration, span3, span);
		for (int i = 0; i < span.Length; i++)
		{
			num += span[i].PackedValue;
			span4[i] = num;
		}
		Span<ulong> span5 = span4;
		for (int j = 1; j < height; j++)
		{
			span2 = pixelBuffer.DangerousGetRowSpan(j + y);
			span3 = span2.Slice(x, span.Length);
			span4 = buffer2D.DangerousGetRowSpan(j);
			PixelOperations<TPixel>.Instance.ToL8(configuration, span3, span);
			num = span[0].PackedValue;
			span4[0] = num + span5[0];
			for (int k = 1; k < span.Length; k++)
			{
				num += span[k].PackedValue;
				span4[k] = num + span5[k];
			}
			span5 = span4;
		}
		return buffer2D;
	}
}
