using System;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Advanced;

public interface IRowIntervalOperation
{
	void Invoke(in RowInterval rows);
}
public interface IRowIntervalOperation<TBuffer> where TBuffer : unmanaged
{
	int GetRequiredBufferLength(Rectangle bounds);

	void Invoke(in RowInterval rows, Span<TBuffer> span);
}
