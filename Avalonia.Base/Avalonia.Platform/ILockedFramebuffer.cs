using System;

namespace Avalonia.Platform;

public interface ILockedFramebuffer : IDisposable
{
	IntPtr Address { get; }

	PixelSize Size { get; }

	int RowBytes { get; }

	Vector Dpi { get; }

	PixelFormat Format { get; }
}
