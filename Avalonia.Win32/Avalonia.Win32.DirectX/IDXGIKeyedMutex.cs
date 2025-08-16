using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIKeyedMutex : IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	void AcquireSync(ulong Key, uint dwMilliseconds);

	void ReleaseSync(ulong Key);
}
