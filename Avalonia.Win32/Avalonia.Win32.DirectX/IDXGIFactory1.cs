using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIFactory1 : IDXGIFactory, IDXGIObject, IUnknown, IDisposable
{
	unsafe int EnumAdapters1(ushort Adapter, void** ppAdapter);

	int IsCurrent();
}
