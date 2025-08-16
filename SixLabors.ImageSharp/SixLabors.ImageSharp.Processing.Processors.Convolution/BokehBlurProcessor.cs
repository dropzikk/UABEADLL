using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Convolution.Parameters;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public sealed class BokehBlurProcessor : IImageProcessor
{
	internal readonly struct SecondPassConvolutionRowOperation : IRowOperation
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<Vector4> targetValues;

		private readonly Buffer2D<ComplexVector4> sourceValues;

		private readonly KernelSamplingMap map;

		private readonly Complex64[] kernel;

		private readonly float z;

		private readonly float w;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SecondPassConvolutionRowOperation(Rectangle bounds, Buffer2D<Vector4> targetValues, Buffer2D<ComplexVector4> sourceValues, KernelSamplingMap map, Complex64[] kernel, float z, float w)
		{
			this.bounds = bounds;
			this.targetValues = targetValues;
			this.sourceValues = sourceValues;
			this.map = map;
			this.kernel = kernel;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			int x = bounds.X;
			int width = bounds.Width;
			int num = kernel.Length;
			ref int reference = ref Unsafe.Add(ref MemoryMarshal.GetReference(map.GetRowOffsetSpan()), (uint)((y - bounds.Y) * num));
			ref Vector4 elementUnsafe = ref targetValues.GetElementUnsafe(x, y);
			ref Complex64 reference2 = ref MemoryMarshal.GetArrayDataReference(kernel);
			ref Complex64 right = ref Unsafe.Add(ref reference2, (uint)num);
			while (Unsafe.IsAddressLessThan(ref reference2, ref right))
			{
				ref ComplexVector4 reference3 = ref sourceValues.GetElementUnsafe(0, reference);
				ref ComplexVector4 right2 = ref Unsafe.Add(ref reference3, (uint)width);
				ref Vector4 reference4 = ref elementUnsafe;
				Complex64 complex = reference2;
				while (Unsafe.IsAddressLessThan(ref reference3, ref right2))
				{
					reference4 += (complex * reference3).WeightedSum(z, w);
					reference3 = ref Unsafe.Add(ref reference3, 1);
					reference4 = ref Unsafe.Add(ref reference4, 1);
				}
				reference2 = ref Unsafe.Add(ref reference2, 1);
				reference = ref Unsafe.Add(ref reference, 1);
			}
		}
	}

	public const int DefaultRadius = 32;

	public const int DefaultComponents = 2;

	public const float DefaultGamma = 3f;

	public int Radius { get; }

	public int Components { get; }

	public float Gamma { get; }

	public BokehBlurProcessor()
	{
		Radius = 32;
		Components = 2;
		Gamma = 3f;
	}

	public BokehBlurProcessor(int radius, int components, float gamma)
	{
		Guard.MustBeGreaterThan(radius, 0, "radius");
		Guard.MustBeBetweenOrEqualTo(components, 1, 6, "components");
		Guard.MustBeGreaterThanOrEqualTo(gamma, 1f, "gamma");
		Radius = radius;
		Components = components;
		Gamma = gamma;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new BokehBlurProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class BokehBlurProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct FirstPassConvolutionRowOperation : IRowOperation<Vector4>
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<ComplexVector4> targetValues;

		private readonly Buffer2D<TPixel> sourcePixels;

		private readonly KernelSamplingMap map;

		private readonly Complex64[] kernel;

		private readonly Configuration configuration;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FirstPassConvolutionRowOperation(Rectangle bounds, Buffer2D<ComplexVector4> targetValues, Buffer2D<TPixel> sourcePixels, KernelSamplingMap map, Complex64[] kernel, Configuration configuration)
		{
			this.bounds = bounds;
			this.targetValues = targetValues;
			this.sourcePixels = sourcePixels;
			this.map = map;
			this.kernel = kernel;
			this.configuration = configuration;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Vector4> span)
		{
			int x = bounds.X;
			int width = bounds.Width;
			int num = kernel.Length;
			Span<ComplexVector4> span2 = targetValues.DangerousGetRowSpan(y);
			span2.Clear();
			Span<TPixel> span3 = sourcePixels.DangerousGetRowSpan(y).Slice(x, width);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, span3, span);
			ref Vector4 reference = ref MemoryMarshal.GetReference(span);
			ref ComplexVector4 reference2 = ref MemoryMarshal.GetReference(span2);
			ref ComplexVector4 right = ref Unsafe.Add(ref reference2, (uint)span.Length);
			ref Complex64 arrayDataReference = ref MemoryMarshal.GetArrayDataReference(kernel);
			ref Complex64 right2 = ref Unsafe.Add(ref arrayDataReference, (uint)num);
			ref int reference3 = ref MemoryMarshal.GetReference(map.GetColumnOffsetSpan());
			while (Unsafe.IsAddressLessThan(ref reference2, ref right))
			{
				ref Complex64 reference4 = ref arrayDataReference;
				ref int reference5 = ref reference3;
				while (Unsafe.IsAddressLessThan(ref reference4, ref right2))
				{
					Vector4 vector = Unsafe.Add(ref reference, (uint)(reference5 - x));
					reference2.Sum(reference4 * vector);
					reference4 = ref Unsafe.Add(ref reference4, 1);
					reference5 = ref Unsafe.Add(ref reference5, 1);
				}
				reference3 = ref Unsafe.Add(ref reference3, (uint)num);
				reference2 = ref Unsafe.Add(ref reference2, 1);
			}
		}
	}

	private readonly struct ApplyGammaExposureRowOperation : IRowOperation<Vector4>
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Configuration configuration;

		private readonly float gamma;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ApplyGammaExposureRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Configuration configuration, float gamma)
		{
			this.bounds = bounds;
			this.targetPixels = targetPixels;
			this.configuration = configuration;
			this.gamma = gamma;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Vector4> span)
		{
			Span<TPixel> span2 = targetPixels.DangerousGetRowSpan(y);
			int x = bounds.X;
			Span<TPixel> destinationPixels = span2.Slice(x, span2.Length - x);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, destinationPixels.Slice(0, span.Length), span, PixelConversionModifiers.Premultiply);
			ref Vector4 reference = ref MemoryMarshal.GetReference(span);
			for (int i = 0; i < bounds.Width; i++)
			{
				ref Vector4 reference2 = ref Unsafe.Add(ref reference, (uint)i);
				reference2.X = MathF.Pow(reference2.X, gamma);
				reference2.Y = MathF.Pow(reference2.Y, gamma);
				reference2.Z = MathF.Pow(reference2.Z, gamma);
			}
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, destinationPixels);
		}
	}

	private readonly struct ApplyGamma3ExposureRowOperation : IRowOperation<Vector4>
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Configuration configuration;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ApplyGamma3ExposureRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Configuration configuration)
		{
			this.bounds = bounds;
			this.targetPixels = targetPixels;
			this.configuration = configuration;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetRequiredBufferLength(Rectangle bounds)
		{
			return bounds.Width;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y, Span<Vector4> span)
		{
			Span<TPixel> span2 = targetPixels.DangerousGetRowSpan(y);
			int x = bounds.X;
			Span<TPixel> destinationPixels = span2.Slice(x, span2.Length - x);
			PixelOperations<TPixel>.Instance.ToVector4(configuration, destinationPixels.Slice(0, span.Length), span, PixelConversionModifiers.Premultiply);
			Numerics.CubePowOnXYZ(span);
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, destinationPixels);
		}
	}

	private readonly struct ApplyInverseGammaExposureRowOperation : IRowOperation
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Buffer2D<Vector4> sourceValues;

		private readonly Configuration configuration;

		private readonly float inverseGamma;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ApplyInverseGammaExposureRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<Vector4> sourceValues, Configuration configuration, float inverseGamma)
		{
			this.bounds = bounds;
			this.targetPixels = targetPixels;
			this.sourceValues = sourceValues;
			this.configuration = configuration;
			this.inverseGamma = inverseGamma;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Vector4 zero = Vector4.Zero;
			Vector4 max = new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			Span<TPixel> span = targetPixels.DangerousGetRowSpan(y);
			int x = bounds.X;
			Span<TPixel> destinationPixels = span.Slice(x, span.Length - x);
			Span<Vector4> span2 = sourceValues.DangerousGetRowSpan(y);
			x = bounds.X;
			Span<Vector4> span3 = span2.Slice(x, span2.Length - x);
			ref Vector4 reference = ref MemoryMarshal.GetReference(span3);
			for (int i = 0; i < bounds.Width; i++)
			{
				ref Vector4 reference2 = ref Unsafe.Add(ref reference, (uint)i);
				Vector4 vector = Numerics.Clamp(reference2, zero, max);
				reference2.X = MathF.Pow(vector.X, inverseGamma);
				reference2.Y = MathF.Pow(vector.Y, inverseGamma);
				reference2.Z = MathF.Pow(vector.Z, inverseGamma);
			}
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span3.Slice(0, bounds.Width), destinationPixels, PixelConversionModifiers.Premultiply);
		}
	}

	private readonly struct ApplyInverseGamma3ExposureRowOperation : IRowOperation
	{
		private readonly Rectangle bounds;

		private readonly Buffer2D<TPixel> targetPixels;

		private readonly Buffer2D<Vector4> sourceValues;

		private readonly Configuration configuration;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ApplyInverseGamma3ExposureRowOperation(Rectangle bounds, Buffer2D<TPixel> targetPixels, Buffer2D<Vector4> sourceValues, Configuration configuration)
		{
			this.bounds = bounds;
			this.targetPixels = targetPixels;
			this.sourceValues = sourceValues;
			this.configuration = configuration;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<Vector4> span = sourceValues.DangerousGetRowSpan(y).Slice(bounds.X, bounds.Width);
			MemoryMarshal.GetReference(span);
			Numerics.Clamp(MemoryMarshal.Cast<Vector4, float>(span), 0f, float.PositiveInfinity);
			Numerics.CubeRootOnXYZ(span);
			Span<TPixel> span2 = targetPixels.DangerousGetRowSpan(y);
			int x = bounds.X;
			Span<TPixel> destinationPixels = span2.Slice(x, span2.Length - x);
			PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span.Slice(0, bounds.Width), destinationPixels, PixelConversionModifiers.Premultiply);
		}
	}

	private readonly float gamma;

	private readonly int kernelSize;

	private readonly Vector4[] kernelParameters;

	private readonly Complex64[][] kernels;

	public IReadOnlyList<Complex64[]> Kernels => kernels;

	public IReadOnlyList<Vector4> KernelParameters => kernelParameters;

	public BokehBlurProcessor(Configuration configuration, BokehBlurProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		gamma = definition.Gamma;
		kernelSize = definition.Radius * 2 + 1;
		BokehBlurKernelData bokehBlurKernelData = BokehBlurKernelDataProvider.GetBokehBlurKernelData(definition.Radius, kernelSize, definition.Components);
		kernelParameters = bokehBlurKernelData.Parameters;
		kernels = bokehBlurKernelData.Kernels;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		if (gamma == 3f)
		{
			ApplyGamma3ExposureRowOperation operation = new ApplyGamma3ExposureRowOperation(rectangle, source.PixelBuffer, base.Configuration);
			ParallelRowIterator.IterateRows<ApplyGamma3ExposureRowOperation, Vector4>(base.Configuration, rectangle, in operation);
		}
		else
		{
			ApplyGammaExposureRowOperation operation2 = new ApplyGammaExposureRowOperation(rectangle, source.PixelBuffer, base.Configuration, gamma);
			ParallelRowIterator.IterateRows<ApplyGammaExposureRowOperation, Vector4>(base.Configuration, rectangle, in operation2);
		}
		using Buffer2D<Vector4> buffer2D = base.Configuration.MemoryAllocator.Allocate2D<Vector4>(source.Size(), AllocationOptions.Clean);
		OnFrameApplyCore(source, rectangle, base.Configuration, buffer2D);
		if (gamma == 3f)
		{
			ApplyInverseGamma3ExposureRowOperation operation3 = new ApplyInverseGamma3ExposureRowOperation(rectangle, source.PixelBuffer, buffer2D, base.Configuration);
			ParallelRowIterator.IterateRows(base.Configuration, rectangle, in operation3);
		}
		else
		{
			ApplyInverseGammaExposureRowOperation operation4 = new ApplyInverseGammaExposureRowOperation(rectangle, source.PixelBuffer, buffer2D, base.Configuration, 1f / gamma);
			ParallelRowIterator.IterateRows(base.Configuration, rectangle, in operation4);
		}
	}

	private void OnFrameApplyCore(ImageFrame<TPixel> source, Rectangle sourceRectangle, Configuration configuration, Buffer2D<Vector4> processingBuffer)
	{
		using Buffer2D<ComplexVector4> buffer2D = configuration.MemoryAllocator.Allocate2D<ComplexVector4>(source.Size());
		using KernelSamplingMap kernelSamplingMap = new KernelSamplingMap(configuration.MemoryAllocator);
		kernelSamplingMap.BuildSamplingOffsetMap(kernelSize, kernelSize, sourceRectangle);
		ref Complex64[] reference = ref MemoryMarshal.GetReference(kernels.AsSpan());
		ref Vector4 reference2 = ref MemoryMarshal.GetReference(kernelParameters.AsSpan());
		for (int i = 0; i < kernels.Length; i++)
		{
			Complex64[] kernel = Unsafe.Add(ref reference, (uint)i);
			Vector4 vector = Unsafe.Add(ref reference2, (uint)i);
			ParallelRowIterator.IterateRows<FirstPassConvolutionRowOperation, Vector4>(configuration, sourceRectangle, new FirstPassConvolutionRowOperation(sourceRectangle, buffer2D, source.PixelBuffer, kernelSamplingMap, kernel, configuration));
			ParallelRowIterator.IterateRows<BokehBlurProcessor.SecondPassConvolutionRowOperation>(configuration, sourceRectangle, new BokehBlurProcessor.SecondPassConvolutionRowOperation(sourceRectangle, processingBuffer, buffer2D, kernelSamplingMap, kernel, vector.Z, vector.W));
		}
	}
}
