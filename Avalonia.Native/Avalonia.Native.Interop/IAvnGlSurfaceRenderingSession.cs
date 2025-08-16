using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnGlSurfaceRenderingSession : IUnknown, IDisposable
{
	AvnPixelSize PixelSize { get; }

	double Scaling { get; }
}
