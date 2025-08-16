using System;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Metal;

[PrivateApi]
public interface IMetalDevice : IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	IntPtr Device { get; }

	IntPtr CommandQueue { get; }
}
