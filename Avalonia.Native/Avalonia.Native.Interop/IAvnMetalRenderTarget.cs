using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMetalRenderTarget : IUnknown, IDisposable
{
	IAvnMetalRenderingSession BeginDrawing();
}
