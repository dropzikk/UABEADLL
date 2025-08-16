using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIAdapter : IDXGIObject, IUnknown, IDisposable
{
	DXGI_ADAPTER_DESC Desc { get; }

	unsafe int EnumOutputs(ushort Output, void* ppOutput);

	unsafe ulong CheckInterfaceSupport(Guid* InterfaceName);
}
