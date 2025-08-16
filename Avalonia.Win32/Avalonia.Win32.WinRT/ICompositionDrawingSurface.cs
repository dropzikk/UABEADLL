using System;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionDrawingSurface : IInspectable, IUnknown, IDisposable
{
	DirectXAlphaMode AlphaMode { get; }

	DirectXPixelFormat PixelFormat { get; }

	UnmanagedMethods.SIZE_F Size { get; }
}
