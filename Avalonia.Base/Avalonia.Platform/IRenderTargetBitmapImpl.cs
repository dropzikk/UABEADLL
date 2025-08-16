using System;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IRenderTargetBitmapImpl : IBitmapImpl, IDisposable, IRenderTarget
{
}
