using System;
using Avalonia.Metadata;

namespace Avalonia.Rendering.Composition;

[NotClientImplementable]
public interface ICompositionImportedGpuSemaphore : ICompositionGpuImportedObject, IAsyncDisposable
{
}
