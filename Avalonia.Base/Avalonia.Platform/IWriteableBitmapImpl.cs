using System;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IWriteableBitmapImpl : IBitmapImpl, IDisposable, IReadableBitmapImpl
{
}
