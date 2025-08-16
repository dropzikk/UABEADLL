using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositor6 : IInspectable, IUnknown, IDisposable
{
	ICompositionGeometricClip CreateGeometricClip();

	ICompositionGeometricClip CreateGeometricClipWithGeometry(ICompositionGeometry geometry);
}
