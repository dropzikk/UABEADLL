using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory;

internal sealed class ByteMemoryManager<T> : MemoryManager<T> where T : unmanaged
{
	private readonly Memory<byte> memory;

	public ByteMemoryManager(Memory<byte> memory)
	{
		this.memory = memory;
	}

	protected override void Dispose(bool disposing)
	{
	}

	public override Span<T> GetSpan()
	{
		return MemoryMarshal.Cast<byte, T>(memory.Span);
	}

	public override MemoryHandle Pin(int elementIndex = 0)
	{
		Memory<byte> memory = this.memory;
		int num = elementIndex * Unsafe.SizeOf<T>();
		return memory.Slice(num, memory.Length - num).Pin();
	}

	public override void Unpin()
	{
	}
}
