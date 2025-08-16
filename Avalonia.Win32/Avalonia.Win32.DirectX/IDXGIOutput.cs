using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIOutput : IDXGIObject, IUnknown, IDisposable
{
	DXGI_OUTPUT_DESC Desc { get; }

	DXGI_FRAME_STATISTICS FrameStatistics { get; }

	unsafe DXGI_MODE_DESC GetDisplayModeList(DXGI_FORMAT EnumFormat, ushort Flags, ushort* pNumModes);

	unsafe void FindClosestMatchingMode(DXGI_MODE_DESC* pModeToMatch, DXGI_MODE_DESC* pClosestMatch, IUnknown pConcernedDevice);

	void WaitForVBlank();

	void TakeOwnership(IUnknown pDevice, int Exclusive);

	void ReleaseOwnership();

	void GetGammaControlCapabilities(IntPtr pGammaCaps);

	unsafe void SetGammaControl(void* pArray);

	void GetGammaControl(IntPtr pArray);

	void SetDisplaySurface(IDXGISurface pScanoutSurface);

	void GetDisplaySurfaceData(IDXGISurface pDestination);
}
