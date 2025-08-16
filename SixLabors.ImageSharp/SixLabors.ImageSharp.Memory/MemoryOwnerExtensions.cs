using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory;

internal static class MemoryOwnerExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> GetSpan<T>(this IMemoryOwner<T> buffer)
	{
		return buffer.Memory.Span;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Length<T>(this IMemoryOwner<T> buffer)
	{
		return buffer.Memory.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> Slice<T>(this IMemoryOwner<T> buffer, int start)
	{
		Span<T> span = buffer.GetSpan();
		return span.Slice(start, span.Length - start);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> Slice<T>(this IMemoryOwner<T> buffer, int start, int length)
	{
		return buffer.GetSpan().Slice(start, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Clear<T>(this IMemoryOwner<T> buffer)
	{
		buffer.GetSpan().Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T GetReference<T>(this IMemoryOwner<T> buffer) where T : struct
	{
		return ref MemoryMarshal.GetReference(buffer.GetSpan());
	}
}
