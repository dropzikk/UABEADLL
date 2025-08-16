using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGISurface : IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	DXGI_SURFACE_DESC Desc { get; }

	unsafe void Map(DXGI_MAPPED_RECT* pLockedRect, ushort MapFlags);

	void Unmap();
}
