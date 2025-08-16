using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionGeometricClip : IInspectable, IUnknown, IDisposable
{
	ICompositionGeometry Geometry { get; }

	void SetGeometry(ICompositionGeometry value);
}
