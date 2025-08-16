using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIDeviceSubObject : IDXGIObject, IUnknown, IDisposable
{
	unsafe void* GetDevice(Guid* riid);
}
