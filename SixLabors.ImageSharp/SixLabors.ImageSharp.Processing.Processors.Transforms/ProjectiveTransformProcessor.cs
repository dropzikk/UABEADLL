using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class ProjectiveTransformProcessor : CloningImageProcessor
{
	public IResampler Sampler { get; }

	public Matrix4x4 TransformMatrix { get; }

	public Size DestinationSize { get; }

	public ProjectiveTransformProcessor(Matrix4x4 matrix, IResampler sampler, Size targetDimensions)
	{
		Guard.NotNull(sampler, "sampler");
		Guard.MustBeValueType(sampler, "sampler");
		if (TransformUtils.IsDegenerate(matrix))
		{
			throw new DegenerateTransformException("Matrix is degenerate. Check input values.");
		}
		Sampler = sampler;
		TransformMatrix = matrix;
		DestinationSize = targetDimensions;
	}

	public override ICloningImageProcessor<TPixel> CreatePixelSpecificCloningProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new ProjectiveTransformProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class ProjectiveTransformProcessor<TPixel> : TransformProcessor<TPixel>, IResamplingTransformImageProcessor<TPixel>, IImageProcessor<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct NNProjectiveOperation : IRowOperation
	{
		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<TPixel> destination;

		private readonly Rectangle bounds;

		private readonly Matrix4x4 matrix;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NNProjectiveOperation(Buffer2D<TPixel> source, Rectangle bounds, Buffer2D<TPixel> destination, Matrix4x4 matrix)
		{
			this.source = source;
			this.bounds = bounds;
			this.destination = destination;
			this.matrix = matrix;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<TPixel> span = destination.DangerousGetRowSpan(y);
			for (int i = 0; i < span.Length; i++)
			{
				Vector2 vector = TransformUtils.ProjectiveTransform2D(i, y, matrix);
				int x = (int)MathF.Round(vector.X);
				int y2 = (int)MathF.Round(vector.Y);
				if (bounds.Contains(x, y2))
				{
					span[i] = source.GetElementUnsafe(x, y2);
				}
			}
		}
	}

	private readonly struct ProjectiveOperation<TResampler> : IRowIntervalOperation<Vector4> where TResampler : struct, IResampler
	{
		private readonly Configuration configuration;

		private readonly Buffer2D<TPixel> source;

		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> destination;

		private readonly TResampler sampler;

		private readonly Matrix4x4 matrix;

		private readonly float yRadius;

		private readonly float xRadius;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ProjectiveOperation(Configuration configuration, Buffer2D<TPixel> source, Rectangle bounds, Buffer2D<TPixel> destination, in TResampler sampler, Matrix4x4 matrix)
		{
			this.configuration = configuration;
			this.source = source;
			this.bounds = bounds;
			this.destination = destination;
			this.sampler = sampler;
			this.matrix = matrix;
			yRadius = LinearTransformUtility.GetSamplingRadius(in sampler, bounds.Height, destination.Height);
			xRadius = LinearTransformUtility.GetSamplingRadius(in sampler, bounds.Width, destination.Width);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(in RowInterval rows, Span<Vector4> span)
		{
			Matrix4x4 matrix4x = matrix;
			TResampler val = sampler;
			float radius = yRadius;
			float radius2 = xRadius;
			int y = bounds.Y;
			int max = bounds.Bottom - 1;
			int x = bounds.X;
			int max2 = bounds.Right - 1;
			for (int i = rows.Min; i < rows.Max; i++)
			{
				Span<TPixel> span2 = destination.DangerousGetRowSpan(i);
				PixelOperations<TPixel>.Instance.ToVector4(configuration, span2, span, PixelConversionModifiers.Scale);
				for (int j = 0; j < span.Length; j++)
				{
					Vector2 vector = TransformUtils.ProjectiveTransform2D(j, i, matrix4x);
					float y2 = vector.Y;
					float x2 = vector.X;
					int rangeStart = LinearTransformUtility.GetRangeStart(radius, y2, y, max);
					int rangeEnd = LinearTransformUtility.GetRangeEnd(radius, y2, y, max);
					int rangeStart2 = LinearTransformUtility.GetRangeStart(radius2, x2, x, max2);
					int rangeEnd2 = LinearTransformUtility.GetRangeEnd(radius2, x2, x, max2);
					if (rangeEnd <= rangeStart || rangeEnd2 <= rangeStart2)
					{
						continue;
					}
					Vector4 zero = Vector4.Zero;
					for (int k = rangeStart; k <= rangeEnd; k++)
					{
						float value = val.GetValue((float)k - y2);
						for (int l = rangeStart2; l <= rangeEnd2; l++)
						{
							float value2 = val.GetValue((float)l - x2);
							Vector4 vector2 = source.GetElementUnsafe(l, k).ToScaledVector4();
							Numerics.Premultiply(ref vector2);
							zero += vector2 * value2 * value;
						}
					}
					span[j] = zero;
				}
				Numerics.UnPremultiply(span);
				PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, span2, PixelConversionModifiers.Scale);
			}
		}

		void IRowIntervalOperation<Vector4>.Invoke(in RowInterval rows, Span<Vector4> span)
		{
			Invoke(in rows, span);
		}
	}

	private readonly Size destinationSize;

	private readonly IResampler resampler;

	private readonly Matrix4x4 transformMatrix;

	private ImageFrame<TPixel>? source;

	private ImageFrame<TPixel>? destination;

	public ProjectiveTransformProcessor(Configuration configuration, ProjectiveTransformProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		destinationSize = definition.DestinationSize;
		transformMatrix = definition.TransformMatrix;
		resampler = definition.Sampler;
	}

	protected override Size GetDestinationSize()
	{
		return destinationSize;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
	{
		this.source = source;
		this.destination = destination;
		resampler.ApplyTransform(this);
	}

	public void ApplyTransform<TResampler>(in TResampler sampler) where TResampler : struct, IResampler
	{
		Configuration configuration = base.Configuration;
		ImageFrame<TPixel> imageFrame = source;
		ImageFrame<TPixel> imageFrame2 = destination;
		Matrix4x4 result = transformMatrix;
		if (result.Equals(Matrix4x4.Identity))
		{
			Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, imageFrame2.Bounds());
			Buffer2DRegion<TPixel> region = imageFrame.PixelBuffer.GetRegion(rectangle);
			Buffer2DRegion<TPixel> region2 = imageFrame2.PixelBuffer.GetRegion(rectangle);
			for (int i = 0; i < region.Height; i++)
			{
				region.DangerousGetRowSpan(i).CopyTo(region2.DangerousGetRowSpan(i));
			}
		}
		else
		{
			Matrix4x4.Invert(result, out result);
			if (sampler is NearestNeighborResampler)
			{
				NNProjectiveOperation operation = new NNProjectiveOperation(imageFrame.PixelBuffer, Rectangle.Intersect(base.SourceRectangle, imageFrame.Bounds()), imageFrame2.PixelBuffer, result);
				ParallelRowIterator.IterateRows(configuration, imageFrame2.Bounds(), in operation);
			}
			else
			{
				ProjectiveOperation<TResampler> operation2 = new ProjectiveOperation<TResampler>(configuration, imageFrame.PixelBuffer, Rectangle.Intersect(base.SourceRectangle, imageFrame.Bounds()), imageFrame2.PixelBuffer, in sampler, result);
				ParallelRowIterator.IterateRowIntervals<ProjectiveOperation<TResampler>, Vector4>(configuration, imageFrame2.Bounds(), in operation2);
			}
		}
	}

	void IResamplingTransformImageProcessor<TPixel>.ApplyTransform<TResampler>(in TResampler sampler)
	{
		ApplyTransform(in sampler);
	}
}
