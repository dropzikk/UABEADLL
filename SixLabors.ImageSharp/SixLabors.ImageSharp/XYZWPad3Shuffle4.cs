using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal readonly struct XYZWPad3Shuffle4 : IPad3Shuffle4, IComponentShuffle
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShuffleReduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest)
	{
		SimdUtils.HwIntrinsics.Pad3Shuffle4Reduce(ref source, ref dest, 228);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RunFallbackShuffle(ReadOnlySpan<byte> source, Span<byte> dest)
	{
		ref byte reference = ref MemoryMarshal.GetReference(source);
		ref byte source2 = ref MemoryMarshal.GetReference(dest);
		ref byte reference2 = ref Unsafe.Add(ref reference, (uint)source.Length);
		ref byte right = ref Unsafe.Subtract(ref reference2, 4);
		while (Unsafe.IsAddressLessThan(ref reference, ref right))
		{
			Unsafe.As<byte, uint>(ref source2) = Unsafe.As<byte, uint>(ref reference) | 0xFF000000u;
			reference = ref Unsafe.Add(ref reference, 3);
			source2 = ref Unsafe.Add(ref source2, 4);
		}
		while (Unsafe.IsAddressLessThan(ref reference, ref reference2))
		{
			Unsafe.Add(ref source2, 0) = Unsafe.Add(ref reference, 0);
			Unsafe.Add(ref source2, 1) = Unsafe.Add(ref reference, 1);
			Unsafe.Add(ref source2, 2) = Unsafe.Add(ref reference, 2);
			Unsafe.Add(ref source2, 3) = byte.MaxValue;
			reference = ref Unsafe.Add(ref reference, 3);
			source2 = ref Unsafe.Add(ref source2, 4);
		}
	}
}
