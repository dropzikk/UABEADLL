using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMetalRenderingSession : IUnknown, IDisposable
{
	AvnPixelSize PixelSize { get; }

	double Scaling { get; }

	IntPtr Texture { get; }
}
