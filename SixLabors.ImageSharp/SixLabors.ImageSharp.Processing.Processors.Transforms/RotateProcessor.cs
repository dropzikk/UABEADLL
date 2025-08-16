using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class RotateProcessor : AffineTransformProcessor
{
	public float Degrees { get; }

	public RotateProcessor(float degrees, Size sourceSize)
		: this(degrees, KnownResamplers.Bicubic, sourceSize)
	{
	}

	public RotateProcessor(float degrees, IResampler sampler, Size sourceSize)
		: this(TransformUtils.CreateRotationMatrixDegrees(degrees, sourceSize), sampler, sourceSize)
	{
		Degrees = degrees;
	}

	private RotateProcessor(Matrix3x2 rotationMatrix, IResampler sampler, Size sourceSize)
		: base(rotationMatrix, sampler, TransformUtils.GetTransformedSize(sourceSize, rotationMatrix))
	{
	}

	public override ICloningImageProcessor<TPixel> CreatePixelSpecificCloningProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new RotateProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class RotateProcessor<TPixel> : AffineTransformProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct Rotate180RowOperation : IRowOperation
	{
		private readonly int width;

		private readonly int height;

		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<TPixel> destination;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rotate180RowOperation(int width, int height, Buffer2D<TPixel> source, Buffer2D<TPixel> destination)
		{
			this.width = width;
			this.height = height;
			this.source = source;
			this.destination = destination;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<TPixel> span = source.DangerousGetRowSpan(y);
			Span<TPixel> span2 = destination.DangerousGetRowSpan(height - y - 1);
			for (int i = 0; i < width; i++)
			{
				span2[width - i - 1] = span[i];
			}
		}
	}

	private readonly struct Rotate270RowIntervalOperation : IRowIntervalOperation
	{
		private readonly Rectangle bounds;

		private readonly int width;

		private readonly int height;

		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<TPixel> destination;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rotate270RowIntervalOperation(Rectangle bounds, int width, int height, Buffer2D<TPixel> source, Buffer2D<TPixel> destination)
		{
			this.bounds = bounds;
			this.width = width;
			this.height = height;
			this.source = source;
			this.destination = destination;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(in RowInterval rows)
		{
			for (int i = rows.Min; i < rows.Max; i++)
			{
				Span<TPixel> span = source.DangerousGetRowSpan(i);
				for (int j = 0; j < width; j++)
				{
					int num = height - i - 1;
					num = height - num - 1;
					int y = width - j - 1;
					if (bounds.Contains(num, y))
					{
						destination[num, y] = span[j];
					}
				}
			}
		}

		void IRowIntervalOperation.Invoke(in RowInterval rows)
		{
			Invoke(in rows);
		}
	}

	private readonly struct Rotate90RowOperation : IRowOperation
	{
		private readonly Rectangle bounds;

		private readonly int width;

		private readonly int height;

		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<TPixel> destination;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rotate90RowOperation(Rectangle bounds, int width, int height, Buffer2D<TPixel> source, Buffer2D<TPixel> destination)
		{
			this.bounds = bounds;
			this.width = width;
			this.height = height;
			this.source = source;
			this.destination = destination;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<TPixel> span = source.DangerousGetRowSpan(y);
			int x = height - y - 1;
			for (int i = 0; i < width; i++)
			{
				if (bounds.Contains(x, i))
				{
					destination[x, i] = span[i];
				}
			}
		}
	}

	private readonly float degrees;

	public RotateProcessor(Configuration configuration, RotateProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, (AffineTransformProcessor)definition, source, sourceRectangle)
	{
		degrees = definition.Degrees;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
	{
		if (!OptimizedApply(source, destination, base.Configuration))
		{
			base.OnFrameApply(source, destination);
		}
	}

	protected override void AfterImageApply(Image<TPixel> destination)
	{
		ExifProfile exifProfile = destination.Metadata.ExifProfile;
		if (exifProfile != null && !(MathF.Abs(WrapDegrees(degrees)) < Constants.Epsilon))
		{
			exifProfile.RemoveValue(ExifTag.Orientation);
			base.AfterImageApply(destination);
		}
	}

	private static float WrapDegrees(float degrees)
	{
		for (degrees %= 360f; degrees < 0f; degrees += 360f)
		{
		}
		return degrees;
	}

	private bool OptimizedApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination, Configuration configuration)
	{
		float num = WrapDegrees(degrees);
		if (MathF.Abs(num) < Constants.Epsilon)
		{
			source.GetPixelMemoryGroup().CopyTo(destination.GetPixelMemoryGroup());
			return true;
		}
		if (MathF.Abs(num - 90f) < Constants.Epsilon)
		{
			Rotate90(source, destination, configuration);
			return true;
		}
		if (MathF.Abs(num - 180f) < Constants.Epsilon)
		{
			Rotate180(source, destination, configuration);
			return true;
		}
		if (MathF.Abs(num - 270f) < Constants.Epsilon)
		{
			Rotate270(source, destination, configuration);
			return true;
		}
		return false;
	}

	private static void Rotate180(ImageFrame<TPixel> source, ImageFrame<TPixel> destination, Configuration configuration)
	{
		Rotate180RowOperation operation = new Rotate180RowOperation(source.Width, source.Height, source.PixelBuffer, destination.PixelBuffer);
		ParallelRowIterator.IterateRows(configuration, source.Bounds(), in operation);
	}

	private static void Rotate270(ImageFrame<TPixel> source, ImageFrame<TPixel> destination, Configuration configuration)
	{
		Rotate270RowIntervalOperation operation = new Rotate270RowIntervalOperation(destination.Bounds(), source.Width, source.Height, source.PixelBuffer, destination.PixelBuffer);
		ParallelRowIterator.IterateRowIntervals(configuration, source.Bounds(), in operation);
	}

	private static void Rotate90(ImageFrame<TPixel> source, ImageFrame<TPixel> destination, Configuration configuration)
	{
		Rotate90RowOperation operation = new Rotate90RowOperation(destination.Bounds(), source.Width, source.Height, source.PixelBuffer, destination.PixelBuffer);
		ParallelRowIterator.IterateRows(configuration, source.Bounds(), in operation);
	}
}
