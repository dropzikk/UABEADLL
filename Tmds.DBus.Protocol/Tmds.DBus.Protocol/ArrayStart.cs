using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tmds.DBus.Protocol;

public ref struct ArrayStart
{
	private Span<byte> _span;

	private int _offset;

	internal ArrayStart(Span<byte> lengthSpan, int offset)
	{
		_span = lengthSpan;
		_offset = offset;
	}

	internal void WriteLength(int offset)
	{
		uint value = (uint)(offset - _offset);
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(_span), value);
	}
}
