using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal readonly struct YZWXShuffle4 : IShuffle4, IComponentShuffle
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShuffleReduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest)
	{
		SimdUtils.HwIntrinsics.Shuffle4Reduce(ref source, ref dest, 57);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RunFallbackShuffle(ReadOnlySpan<byte> source, Span<byte> dest)
	{
		ref uint source2 = ref Unsafe.As<byte, uint>(ref MemoryMarshal.GetReference(source));
		ref uint source3 = ref Unsafe.As<byte, uint>(ref MemoryMarshal.GetReference(dest));
		uint num = (uint)source.Length / 4u;
		for (nuint num2 = 0u; num2 < num; num2++)
		{
			uint value = Unsafe.Add(ref source2, num2);
			Unsafe.Add(ref source3, num2) = BitOperations.RotateRight(value, 8);
		}
	}
}
