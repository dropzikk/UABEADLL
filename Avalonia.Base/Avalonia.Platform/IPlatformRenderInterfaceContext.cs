using System;
using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
[PrivateApi]
public interface IPlatformRenderInterfaceContext : IOptionalFeatureProvider, IDisposable
{
	bool IsLost { get; }

	IRenderTarget CreateRenderTarget(IEnumerable<object> surfaces);
}
