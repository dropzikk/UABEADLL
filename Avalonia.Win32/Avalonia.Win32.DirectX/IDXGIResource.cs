using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIResource : IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	IntPtr SharedHandle { get; }

	uint Usage { get; }

	ushort EvictionPriority { get; }

	void SetEvictionPriority(ushort EvictionPriority);
}
