using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIDevice : IDXGIObject, IUnknown, IDisposable
{
	IDXGIAdapter Adapter { get; }

	int GPUThreadPriority { get; }

	unsafe IDXGISurface CreateSurface(DXGI_SURFACE_DESC* pDesc, ushort NumSurfaces, uint Usage, void** pSharedResource);

	unsafe void QueryResourceResidency(IUnknown ppResources, DXGI_RESIDENCY* pResidencyStatus, ushort NumResources);

	void SetGPUThreadPriority(int Priority);
}
