using System;
using System.Runtime.InteropServices;
using Avalonia.Platform;

namespace Avalonia.Media.Imaging;

internal class BitmapMemory : IDisposable
{
	private readonly int _memorySize;

	public IntPtr Address { get; private set; }

	public PixelSize Size { get; }

	public int RowBytes { get; }

	public PixelFormat Format { get; }

	public BitmapMemory(PixelFormat format, PixelSize size)
	{
		Format = format;
		Size = size;
		RowBytes = (size.Width * format.BitsPerPixel + 7) / 8;
		_memorySize = RowBytes * size.Height;
		Address = Marshal.AllocHGlobal(_memorySize);
		GC.AddMemoryPressure(_memorySize);
	}

	private void ReleaseUnmanagedResources()
	{
		if (Address != IntPtr.Zero)
		{
			GC.RemoveMemoryPressure(_memorySize);
			Marshal.FreeHGlobal(Address);
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~BitmapMemory()
	{
		ReleaseUnmanagedResources();
	}

	public void CopyToRgba(IntPtr buffer, int rowBytes)
	{
		PixelFormatReader.Transcode(buffer, Address, Size, RowBytes, rowBytes, Format);
	}
}
