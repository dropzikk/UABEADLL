namespace Avalonia.Native.Interop;

internal struct AvnFramebuffer
{
	public unsafe void* Data;

	public int Width;

	public int Height;

	public int Stride;

	public AvnVector Dpi;

	public AvnPixelFormat PixelFormat;
}
