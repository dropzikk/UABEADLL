using System;

namespace SixLabors.ImageSharp.Advanced;

public interface IRowOperation
{
	void Invoke(int y);
}
public interface IRowOperation<TBuffer> where TBuffer : unmanaged
{
	int GetRequiredBufferLength(Rectangle bounds);

	void Invoke(int y, Span<TBuffer> span);
}
