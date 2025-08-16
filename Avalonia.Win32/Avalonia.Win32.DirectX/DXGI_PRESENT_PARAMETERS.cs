using Avalonia.Win32.Interop;

namespace Avalonia.Win32.DirectX;

internal struct DXGI_PRESENT_PARAMETERS
{
	public uint DirtyRectsCount;

	public unsafe UnmanagedMethods.RECT* pDirtyRects;

	public unsafe UnmanagedMethods.RECT* pScrollRect;

	public unsafe UnmanagedMethods.POINT* pScrollOffset;
}
