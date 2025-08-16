using Avalonia.Win32.Interop;

namespace Avalonia.Win32.DirectX;

internal struct DXGI_OUTPUT_DESC
{
	internal unsafe fixed ushort DeviceName[32];

	internal UnmanagedMethods.RECT DesktopCoordinates;

	internal int AttachedToDesktop;

	internal DXGI_MODE_ROTATION Rotation;

	internal HANDLE Monitor;
}
