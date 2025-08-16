using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Memory;

public sealed class Buffer2D<T> : IDisposable where T : struct
{
	public int Width { get; private set; }

	public int Height { get; private set; }

	public IMemoryGroup<T> MemoryGroup => FastMemoryGroup.View;

	internal MemoryGroup<T> FastMemoryGroup { get; private set; }

	internal bool IsDisposed { get; private set; }

	public ref T this[int x, int y]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref DangerousGetRowSpan(y)[x];
		}
	}

	internal Buffer2D(MemoryGroup<T> memoryGroup, int width, int height)
	{
		FastMemoryGroup = memoryGroup;
		Width = width;
		Height = height;
	}

	public void Dispose()
	{
		FastMemoryGroup.Dispose();
		IsDisposed = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T> DangerousGetRowSpan(int y)
	{
		if ((uint)y >= (uint)Height)
		{
			ThrowYOutOfRangeException(y);
		}
		return FastMemoryGroup.GetRowSpanCoreUnsafe(y, Width);
	}

	internal bool DangerousTryGetPaddedRowSpan(int y, int padding, out Span<T> paddedSpan)
	{
		int num = Width + padding;
		Span<T> remainingSliceOfBuffer = FastMemoryGroup.GetRemainingSliceOfBuffer((long)y * (long)Width);
		if (remainingSliceOfBuffer.Length < num)
		{
			paddedSpan = default(Span<T>);
			return false;
		}
		paddedSpan = remainingSliceOfBuffer.Slice(0, num);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref T GetElementUnsafe(int x, int y)
	{
		return ref FastMemoryGroup.GetRowSpanCoreUnsafe(y, Width)[x];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Memory<T> GetSafeRowMemory(int y)
	{
		return FastMemoryGroup.View.GetBoundedMemorySlice((long)y * (long)Width, Width);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Span<T> DangerousGetSingleSpan()
	{
		return FastMemoryGroup.Single().Span;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Memory<T> DangerousGetSingleMemory()
	{
		return FastMemoryGroup.Single();
	}

	internal static bool SwapOrCopyContent(Buffer2D<T> destination, Buffer2D<T> source)
	{
		bool result = false;
		Buffer2D<T> buffer2D;
		if (MemoryGroup<T>.CanSwapContent(destination.FastMemoryGroup, source.FastMemoryGroup))
		{
			buffer2D = source;
			MemoryGroup<T> fastMemoryGroup = source.FastMemoryGroup;
			MemoryGroup<T> fastMemoryGroup2 = destination.FastMemoryGroup;
			destination.FastMemoryGroup = fastMemoryGroup;
			buffer2D.FastMemoryGroup = fastMemoryGroup2;
			destination.FastMemoryGroup.RecreateViewAfterSwap();
			source.FastMemoryGroup.RecreateViewAfterSwap();
			result = true;
		}
		else
		{
			if (destination.FastMemoryGroup.TotalLength != source.FastMemoryGroup.TotalLength)
			{
				throw new InvalidMemoryOperationException("Trying to copy/swap incompatible buffers. This is most likely caused by applying an unsupported processor to wrapped-memory images.");
			}
			source.FastMemoryGroup.CopyTo(destination.MemoryGroup);
		}
		buffer2D = source;
		int width = source.Width;
		int width2 = destination.Width;
		destination.Width = width;
		buffer2D.Width = width2;
		buffer2D = source;
		width2 = source.Height;
		width = destination.Height;
		destination.Height = width2;
		buffer2D.Height = width;
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ThrowYOutOfRangeException(int y)
	{
		throw new ArgumentOutOfRangeException($"DangerousGetRowSpan({y}). Y was out of range. Height={Height}");
	}
}
