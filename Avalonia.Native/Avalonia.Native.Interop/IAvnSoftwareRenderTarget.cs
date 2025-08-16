using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnSoftwareRenderTarget : IUnknown, IDisposable
{
	unsafe void SetFrame(AvnFramebuffer* fb);
}
