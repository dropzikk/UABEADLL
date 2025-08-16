using System;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionGraphicsDevice2 : IInspectable, IUnknown, IDisposable
{
	ICompositionDrawingSurface CreateDrawingSurface2(UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode);
}
