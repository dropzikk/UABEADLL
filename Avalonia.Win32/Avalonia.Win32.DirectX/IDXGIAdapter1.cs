using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIAdapter1 : IDXGIAdapter, IDXGIObject, IUnknown, IDisposable
{
	DXGI_ADAPTER_DESC1 Desc1 { get; }
}
