using System;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Rendering;

[PrivateApi]
public interface IRenderer : IDisposable
{
	RendererDiagnostics Diagnostics { get; }

	event EventHandler<SceneInvalidatedEventArgs>? SceneInvalidated;

	void AddDirty(Visual visual);

	void RecalculateChildren(Visual visual);

	void Resized(Size size);

	void Paint(Rect rect);

	void Start();

	void Stop();

	ValueTask<object?> TryGetRenderInterfaceFeature(Type featureType);
}
