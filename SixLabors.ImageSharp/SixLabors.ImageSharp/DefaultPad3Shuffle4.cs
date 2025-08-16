using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp;

internal readonly struct DefaultPad3Shuffle4 : IPad3Shuffle4, IComponentShuffle
{
	public byte Control { get; }

	public DefaultPad3Shuffle4(byte control)
	{
		Control = control;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ShuffleReduce(ref ReadOnlySpan<byte> source, ref Span<byte> dest)
	{
		SimdUtils.HwIntrinsics.Pad3Shuffle4Reduce(ref source, ref dest, Control);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RunFallbackShuffle(ReadOnlySpan<byte> source, Span<byte> dest)
	{
		ref byte reference = ref MemoryMarshal.GetReference(source);
		ref byte reference2 = ref MemoryMarshal.GetReference(dest);
		SimdUtils.Shuffle.InverseMMShuffle(Control, out var p, out var p2, out var p3, out var p4);
		ref byte reference3 = ref MemoryMarshal.GetReference(stackalloc byte[4]);
		ref uint reference4 = ref Unsafe.As<byte, uint>(ref reference3);
		nuint num = 0u;
		nuint num2 = 0u;
		while (num < (uint)source.Length)
		{
			reference4 = Unsafe.As<byte, uint>(ref Unsafe.Add(ref reference, num)) | 0xFF000000u;
			Unsafe.Add(ref reference2, num2 + 0) = Unsafe.Add(ref reference3, p4);
			Unsafe.Add(ref reference2, num2 + 1) = Unsafe.Add(ref reference3, p3);
			Unsafe.Add(ref reference2, num2 + 2) = Unsafe.Add(ref reference3, p2);
			Unsafe.Add(ref reference2, num2 + 3) = Unsafe.Add(ref reference3, p);
			num += 3;
			num2 += 4;
		}
	}
}
