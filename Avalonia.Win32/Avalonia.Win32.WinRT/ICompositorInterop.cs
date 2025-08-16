using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositorInterop : IUnknown, IDisposable
{
	ICompositionSurface CreateCompositionSurfaceForHandle(IntPtr swapChain);

	ICompositionSurface CreateCompositionSurfaceForSwapChain(IUnknown swapChain);

	ICompositionGraphicsDevice CreateGraphicsDevice(IUnknown renderingDevice);
}
