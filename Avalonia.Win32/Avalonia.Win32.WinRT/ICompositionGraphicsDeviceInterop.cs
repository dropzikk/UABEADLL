using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionGraphicsDeviceInterop : IUnknown, IDisposable
{
	IUnknown RenderingDevice { get; }

	void SetRenderingDevice(IUnknown value);
}
