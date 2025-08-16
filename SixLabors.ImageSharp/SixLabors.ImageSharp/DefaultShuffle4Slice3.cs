using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp;

internal readonly struct DefaultShuffle4Slice3 : IShuffle4Slice3, IComponentShuffle
{
	public byte Control { get; }

	public DefaultShuffle4Slice3(byte control)
	{
		Control = control;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShuffleReduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest)
	{
		SimdUtils.HwIntrinsics.Shuffle4Slice3Reduce(ref source, ref dest, Control);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RunFallbackShuffle(ReadOnlySpan<byte> source, Span<byte> dest)
	{
		ref byte reference = ref MemoryMarshal.GetReference(source);
		ref byte reference2 = ref MemoryMarshal.GetReference(dest);
		SimdUtils.Shuffle.InverseMMShuffle(Control, out var _, out var p2, out var p3, out var p4);
		nuint num = 0u;
		nuint num2 = 0u;
		while (num < (uint)dest.Length)
		{
			Unsafe.Add(ref reference2, num + 0) = Unsafe.Add(ref reference, p4 + num2);
			Unsafe.Add(ref reference2, num + 1) = Unsafe.Add(ref reference, p3 + num2);
			Unsafe.Add(ref reference2, num + 2) = Unsafe.Add(ref reference, p2 + num2);
			num += 3;
			num2 += 4;
		}
	}
}
