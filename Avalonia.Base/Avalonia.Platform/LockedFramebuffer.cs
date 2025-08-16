using System;

namespace Avalonia.Platform;

public class LockedFramebuffer : ILockedFramebuffer, IDisposable
{
	private readonly Action? _onDispose;

	public IntPtr Address { get; }

	public PixelSize Size { get; }

	public int RowBytes { get; }

	public Vector Dpi { get; }

	public PixelFormat Format { get; }

	public LockedFramebuffer(IntPtr address, PixelSize size, int rowBytes, Vector dpi, PixelFormat format, Action? onDispose)
	{
		_onDispose = onDispose;
		Address = address;
		Size = size;
		RowBytes = rowBytes;
		Dpi = dpi;
		Format = format;
	}

	public void Dispose()
	{
		_onDispose?.Invoke();
	}
}
