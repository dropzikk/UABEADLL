using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal class ResizeKernelMap : IDisposable
{
	private sealed class PeriodicKernelMap : ResizeKernelMap
	{
		private readonly int period;

		private readonly int cornerInterval;

		internal override string Info => base.Info + $"|period:{period}|cornerInterval:{cornerInterval}";

		public PeriodicKernelMap(MemoryAllocator memoryAllocator, int sourceLength, int destinationLength, double ratio, double scale, int radius, int period, int cornerInterval)
			: base(memoryAllocator, sourceLength, destinationLength, cornerInterval * 2 + period, ratio, scale, radius)
		{
			this.cornerInterval = cornerInterval;
			this.period = period;
		}

		protected internal override void Initialize<TResampler>(in TResampler sampler)
		{
			int num = cornerInterval + period;
			for (int i = 0; i < num; i++)
			{
				kernels[i] = BuildKernel(in sampler, i, i);
			}
			int num2 = base.DestinationLength - cornerInterval;
			for (int j = num; j < num2; j++)
			{
				double num3 = ((double)j + 0.5) * ratio - 0.5;
				int left = (int)TolerantMath.Ceiling(num3 - (double)radius);
				ResizeKernel resizeKernel = kernels[j - period];
				kernels[j] = resizeKernel.AlterLeftValue(left);
			}
			int num4 = cornerInterval + period;
			for (int k = 0; k < cornerInterval; k++)
			{
				kernels[num2 + k] = BuildKernel(in sampler, num2 + k, num4 + k);
			}
		}
	}

	private static readonly TolerantMath TolerantMath = TolerantMath.Default;

	private readonly int sourceLength;

	private readonly double ratio;

	private readonly double scale;

	private readonly int radius;

	private readonly MemoryHandle pinHandle;

	private readonly Buffer2D<float> data;

	private readonly ResizeKernel[] kernels;

	private bool isDisposed;

	private readonly double[] tempValues;

	public int DestinationLength { get; }

	public int MaxDiameter { get; }

	internal virtual string Info => $"radius:{radius}|sourceSize:{sourceLength}|destinationSize:{DestinationLength}|ratio:{ratio}|scale:{scale}";

	private ResizeKernelMap(MemoryAllocator memoryAllocator, int sourceLength, int destinationLength, int bufferHeight, double ratio, double scale, int radius)
	{
		this.ratio = ratio;
		this.scale = scale;
		this.radius = radius;
		this.sourceLength = sourceLength;
		DestinationLength = destinationLength;
		MaxDiameter = radius * 2 + 1;
		data = memoryAllocator.Allocate2D<float>(MaxDiameter, bufferHeight, preferContiguosImageBuffers: true, AllocationOptions.Clean);
		pinHandle = data.DangerousGetSingleMemory().Pin();
		kernels = new ResizeKernel[destinationLength];
		tempValues = new double[MaxDiameter];
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			isDisposed = true;
			if (disposing)
			{
				pinHandle.Dispose();
				data.Dispose();
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref ResizeKernel GetKernel(nuint destIdx)
	{
		return ref kernels[(uint)destIdx];
	}

	public static ResizeKernelMap Calculate<TResampler>(in TResampler sampler, int destinationSize, int sourceSize, MemoryAllocator memoryAllocator) where TResampler : struct, IResampler
	{
		double num = (double)sourceSize / (double)destinationSize;
		double num2 = num;
		if (num2 < 1.0)
		{
			num2 = 1.0;
		}
		int num3 = (int)TolerantMath.Ceiling(num2 * (double)sampler.Radius);
		int num4 = destinationSize / Numerics.GreatestCommonDivisor(sourceSize, destinationSize);
		double num5 = (num - 1.0) * 0.5;
		double a = ((double)num3 - num5 - 1.0) / num;
		int num6 = (int)TolerantMath.Ceiling(a);
		if (TolerantMath.AreEqual(a, num6))
		{
			num6++;
		}
		ResizeKernelMap obj = ((2 * (num6 + num4) < destinationSize) ? new PeriodicKernelMap(memoryAllocator, sourceSize, destinationSize, num, num2, num3, num4, num6) : new ResizeKernelMap(memoryAllocator, sourceSize, destinationSize, destinationSize, num, num2, num3));
		obj.Initialize(in sampler);
		return obj;
	}

	protected internal virtual void Initialize<TResampler>(in TResampler sampler) where TResampler : struct, IResampler
	{
		for (int i = 0; i < DestinationLength; i++)
		{
			kernels[i] = BuildKernel(in sampler, i, i);
		}
	}

	private ResizeKernel BuildKernel<TResampler>(in TResampler sampler, int destRowIndex, int dataRowIndex) where TResampler : struct, IResampler
	{
		double num = ((double)destRowIndex + 0.5) * ratio - 0.5;
		int num2 = (int)TolerantMath.Ceiling(num - (double)radius);
		if (num2 < 0)
		{
			num2 = 0;
		}
		int num3 = (int)TolerantMath.Floor(num + (double)radius);
		if (num3 > sourceLength - 1)
		{
			num3 = sourceLength - 1;
		}
		ResizeKernel result = CreateKernel(dataRowIndex, num2, num3);
		Span<double> values = tempValues.AsSpan(0, result.Length);
		double num4 = 0.0;
		for (int i = num2; i <= num3; i++)
		{
			double num5 = sampler.GetValue((float)(((double)i - num) / scale));
			num4 += num5;
			values[i - num2] = num5;
		}
		if (num4 > 0.0)
		{
			for (int j = 0; j < result.Length; j++)
			{
				values[j] /= num4;
			}
		}
		result.Fill(values);
		return result;
	}

	private unsafe ResizeKernel CreateKernel(int dataRowIndex, int left, int right)
	{
		int length = right - left + 1;
		float* bufferPtr = (float*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(data.DangerousGetRowSpan(dataRowIndex)));
		return new ResizeKernel(left, bufferPtr, length);
	}

	[Conditional("DEBUG")]
	private void ValidateSizesForCreateKernel(int length, int dataRowIndex, int left, int right)
	{
		if (length > data.Width)
		{
			throw new InvalidOperationException($"Error in KernelMap.CreateKernel({dataRowIndex},{left},{right}): left > this.data.Width");
		}
	}
}
