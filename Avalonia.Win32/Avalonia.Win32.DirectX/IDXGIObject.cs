using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIObject : IUnknown, IDisposable
{
	unsafe void SetPrivateData(Guid* Name, ushort DataSize, void** pData);

	unsafe void SetPrivateDataInterface(Guid* Name, IUnknown pUnknown);

	unsafe void* GetPrivateData(Guid* Name, ushort* pDataSize);

	unsafe void* GetParent(Guid* riid);
}
