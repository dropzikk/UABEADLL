using System;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Controls.Platform.Surfaces;

[Unstable]
public interface IFramebufferRenderTarget : IDisposable
{
	ILockedFramebuffer Lock();
}
