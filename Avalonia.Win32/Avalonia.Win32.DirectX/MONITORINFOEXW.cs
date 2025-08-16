using Avalonia.Win32.Interop;

namespace Avalonia.Win32.DirectX;

internal struct MONITORINFOEXW
{
	internal UnmanagedMethods.MONITORINFO Base;

	internal unsafe fixed ushort szDevice[32];
}
