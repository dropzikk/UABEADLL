using System;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionGraphicsDevice : IInspectable, IUnknown, IDisposable
{
	ICompositionDrawingSurface CreateDrawingSurface(UnmanagedMethods.SIZE_F sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode);

	unsafe void AddRenderingDeviceReplaced(void* handler, void* token);

	void RemoveRenderingDeviceReplaced(int token);
}
