using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal readonly struct XYZWShuffle4Slice3 : IShuffle4Slice3, IComponentShuffle
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShuffleReduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest)
	{
		SimdUtils.HwIntrinsics.Shuffle4Slice3Reduce(ref source, ref dest, 228);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RunFallbackShuffle(ReadOnlySpan<byte> source, Span<byte> dest)
	{
		ref uint reference = ref Unsafe.As<byte, uint>(ref MemoryMarshal.GetReference(source));
		ref Byte3 source2 = ref Unsafe.As<byte, Byte3>(ref MemoryMarshal.GetReference(dest));
		nint num = (nint)(uint)source.Length / (nint)4;
		nint num2 = Numerics.Modulo4(num);
		nint elementOffset = num - num2;
		ref uint right = ref Unsafe.Add(ref reference, elementOffset);
		ref uint right2 = ref Unsafe.Add(ref reference, num);
		while (Unsafe.IsAddressLessThan(ref reference, ref right))
		{
			Unsafe.Add(ref source2, 0) = Unsafe.As<uint, Byte3>(ref Unsafe.Add(ref reference, 0));
			Unsafe.Add(ref source2, 1) = Unsafe.As<uint, Byte3>(ref Unsafe.Add(ref reference, 1));
			Unsafe.Add(ref source2, 2) = Unsafe.As<uint, Byte3>(ref Unsafe.Add(ref reference, 2));
			Unsafe.Add(ref source2, 3) = Unsafe.As<uint, Byte3>(ref Unsafe.Add(ref reference, 3));
			reference = ref Unsafe.Add(ref reference, 4);
			source2 = ref Unsafe.Add(ref source2, 4);
		}
		while (Unsafe.IsAddressLessThan(ref reference, ref right2))
		{
			Unsafe.Add(ref source2, 0) = Unsafe.As<uint, Byte3>(ref Unsafe.Add(ref reference, 0));
			reference = ref Unsafe.Add(ref reference, 1);
			source2 = ref Unsafe.Add(ref source2, 1);
		}
	}
}
