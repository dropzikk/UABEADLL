using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp;

internal readonly struct DefaultShuffle4 : IShuffle4, IComponentShuffle
{
	public byte Control { get; }

	public DefaultShuffle4(byte control)
	{
		Control = control;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShuffleReduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest)
	{
		SimdUtils.HwIntrinsics.Shuffle4Reduce(ref source, ref dest, Control);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RunFallbackShuffle(ReadOnlySpan<byte> source, Span<byte> dest)
	{
		ref byte reference = ref MemoryMarshal.GetReference(source);
		ref byte reference2 = ref MemoryMarshal.GetReference(dest);
		SimdUtils.Shuffle.InverseMMShuffle(Control, out var p, out var p2, out var p3, out var p4);
		for (nuint num = 0u; num < (uint)source.Length; num += 4)
		{
			Unsafe.Add(ref reference2, num + 0) = Unsafe.Add(ref reference, p4 + num);
			Unsafe.Add(ref reference2, num + 1) = Unsafe.Add(ref reference, p3 + num);
			Unsafe.Add(ref reference2, num + 2) = Unsafe.Add(ref reference, p2 + num);
			Unsafe.Add(ref reference2, num + 3) = Unsafe.Add(ref reference, p + num);
		}
	}
}
