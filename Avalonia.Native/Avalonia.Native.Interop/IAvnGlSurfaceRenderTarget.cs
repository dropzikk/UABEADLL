using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnGlSurfaceRenderTarget : IUnknown, IDisposable
{
	IAvnGlSurfaceRenderingSession BeginDrawing();
}
